using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class InverseScript : MovementState
{
    [SerializeField] private float YBelowTop = 2.25f;
    [SerializeField] private float localYAwayFromCamTime = 1.0f;
    [SerializeField] private float horizontalSensitivity = -1.0f;
    [SerializeField] private float verticalSensitivity = -1.0f;
    [SerializeField] private float delayTime = 0.0f;
    private float timer = 0.0f;
    private Queue<Vector2> queue = new Queue<Vector2>(200);
    private bool inPosition = false;
    PlayerClasses.PlayerController playerController;
    Vector3 playerPrevPos = Vector3.zero;
    private void Start()
    {
        transform.parent = Camera.main.transform;
        playerController = EventManager.Instance.GetPlayerTransform().GetComponent<PlayerClasses.PlayerController>();
    }
    public override void OnStateEnter()
    {
        base.OnStateEnter();
        LeanTween.moveLocalY(gameObject, (EventManager.Instance.camTopRightPos.y - YBelowTop) - Camera.main.transform.position.y, localYAwayFromCamTime)
            .setEaseOutQuad().setLoopOnce().setOnComplete(FirstAnimationComplete);
    }
    void FirstAnimationComplete()
    {
        inPosition = true;
    }
    public override EnemyStateMachine.EnemyStateType OnUpdate()
    {
        if (base.OnUpdate() == EnemyStateMachine.EnemyStateType.Bouncing)
        {
            return EnemyStateMachine.EnemyStateType.Bouncing;
        }
        if (inPosition)
        {
            timer += Time.deltaTime;
            Vector2 deltaPlayerPos = EventManager.Instance.GetPlayerTransform().position - playerPrevPos;
            deltaPlayerPos.y -= GameManager.Instance.GetCamUpSpeed() * Time.deltaTime;
            queue.Enqueue(deltaPlayerPos);
            if(timer > delayTime && queue.Count > 0)
            {
                Vector2 deltaPosition = queue.Dequeue();
                transform.localPosition += new Vector3(deltaPosition.x * horizontalSensitivity,
                    deltaPosition.y * verticalSensitivity, 0);
                Vector3 clampedPosition = Vector3.zero;
                clampedPosition.x = Mathf.Clamp(transform.position.x, EventManager.Instance.camBottomLeftPos.x, EventManager.Instance.camTopRightPos.x);
                clampedPosition.y = Mathf.Clamp(transform.position.y, EventManager.Instance.camBottomLeftPos.y, EventManager.Instance.camTopRightPos.y);
                transform.position = clampedPosition;
            }
        }
        playerPrevPos = EventManager.Instance.GetPlayerTransform().position;
        return EnemyStateMachine.EnemyStateType.Moving;
    }
    public override void OnStateExit()
    {
        base.OnStateExit();
        if (LeanTween.isTweening(gameObject))
        {
            LeanTween.cancel(gameObject);
        }
    }
    public void OnDestroy()
    {
        //OnStateExit();
    }
}
