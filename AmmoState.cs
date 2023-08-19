using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoState : MovementState
{
    [SerializeField] private LeanTweenType easeType = LeanTweenType.linear;
    [SerializeField] private float waitTime = 0.3f;
    [SerializeField] private float moveTime = 1.0f;
    private PathScript pathScript;
    private float waitTimer = 0.0f;
    private Vector3 endPosition = new Vector3(0,-8.5f,0);

    public override void OnStateEnter()
    {
        base.OnStateEnter();
        gameObject.transform.parent = Camera.main.transform;
    }
    public void SetEaseType( LeanTweenType ease)
    {
        easeType = ease;
    }
    public void SetEndPosition(Vector3 endPos)
    {
        endPosition = endPos;
    }
    public override EnemyStateMachine.EnemyStateType OnUpdate()
    {
        waitTimer += Time.deltaTime;
        if(base.OnUpdate()==EnemyStateMachine.EnemyStateType.Bouncing) return EnemyStateMachine.EnemyStateType.Bouncing;

        if (waitTimer >= waitTime)
        {
            pathScript = GetComponent<PathScript>();
            if (pathScript != null)
            {
                pathScript.ChartPath(new Vector3[]
                {
                transform.localPosition, endPosition
                });
                List<GameObject> pathScriptPoints = pathScript.GetPoints();
                foreach (var pointObj in pathScriptPoints)
                {
                    pointObj.transform.parent = Camera.main.transform;
                    pointObj.transform.localPosition = pointObj.transform.position;
                }
            }
            LeanTween.moveLocalY(gameObject, endPosition.y, moveTime).setEase(easeType).setOnComplete(delegate ()
            {
                pathScript.RemoveAll();
            });
            waitTime = 1000.0f;
            waitTimer = 0.0f;
        }
        return EnemyStateMachine.EnemyStateType.Moving;
    }
    public override void OnStateExit()
    {
        base.OnStateExit();
        pathScript.RemoveAll();
    }
    private void OnDestroy()
    {
        //OnStateExit();
    }

}
