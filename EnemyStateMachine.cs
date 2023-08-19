using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine : MonoBehaviour
{
    public enum EnemyStateType { Moving ,Bouncing};
    public bool isConfigured;
    private int id;
    [SerializeField] private EnemyStateType startStateType;
    [SerializeField] private GameObject textPrefab;
    private EnemyState currentState;
    private Vector3 previousPos;
    private Vector3 velocity = Vector3.zero;


    private bool stateChanged = true;
    private EnemyStateType OnUpdateReturnType;
    private Dictionary<EnemyStateType, EnemyState> enemyStates;
    public void Init(EnemyFactory enemyFactory)
    {
        id = enemyFactory.GetID();
    }
    public int GetId()
    {
        return id;
    }
    public Vector3 GetVelocity()
    {
        return velocity;
    }
    // Start is called before the first frame update
    void Start()
    {
        enemyStates = new Dictionary<EnemyStateType, EnemyState>();
        EnemyState[] enemyStateComponents = GetComponents<EnemyState>();
        foreach(EnemyState enemyState in enemyStateComponents)
        {
            enemyStates.Add(enemyState.type, enemyState);
        }
        if(!enemyStates.TryGetValue(startStateType,out currentState))
        {
            Debug.Assert(false, "Couldnt find enemyState in dictionary");
        }
        previousPos = transform.position;
        EventManager.Instance.enemyKilled += InstantiateText;
    }

    // Update is called once per frame
    void Update()
    {
        velocity = (transform.position - previousPos) / Time.deltaTime;
        if (stateChanged)
        {
            currentState.OnStateEnter();
            stateChanged = false;
        }
        OnUpdateReturnType = currentState.OnUpdate();
        if (OnUpdateReturnType == EnemyStateType.Moving)
        {
            if (EventManager.Instance.camBottomLeftPos.y - 1 >= transform.position.y)
            {
                LeanTween.cancel(gameObject);
                currentState.OnStateExit();
                Destroy(gameObject);
            }
        }
        if ( OnUpdateReturnType != currentState.type)
        {
            currentState.OnStateExit();
            if(!enemyStates.TryGetValue(OnUpdateReturnType,out currentState))
            {
                Debug.Assert(false, "Couldnt find enemyState in dictionary");
            }
            stateChanged = true;
        }
        if(Camera.main.transform == transform.parent)
        {
            previousPos = transform.position - new Vector3(0, -GameManager.Instance.GetCamUpSpeed() * Time.deltaTime, 0);
        }
        else
        {
            previousPos = transform.position;
        }
    }
    void InstantiateText(int enemyId, int score, Vector3 bouncingDirection)
    {
        if (id == enemyId)
        {
            GameObject text = Instantiate(textPrefab);
            RectTransform canvasTransform = EventManager.Instance.GetCanvas().transform.GetComponent<RectTransform>();
            text.GetComponent<RectTransform>().SetParent(canvasTransform);
            Vector3 screenPoints = Camera.main.WorldToScreenPoint(transform.position);
            Vector2 localPoint = Vector2.zero;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasTransform, screenPoints, null, out localPoint);
            text.GetComponent<RectTransform>().anchoredPosition = localPoint;
            text.GetComponent<TextScript>().Init(score);
        }
    }
    private void OnDestroy()
    {
        if (EventManager.Instance)
        {
            EventManager.Instance.enemyDestroyed(GetId());
            EventManager.Instance.enemyKilled -= InstantiateText;

        }
    }
}
