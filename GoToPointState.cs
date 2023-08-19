using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToPointState : MovementState
{
    [SerializeField] Vector3 point1;
    [SerializeField] LeanTweenType easeType1;
    [SerializeField] float moveTime1 = 1.0f;
    [SerializeField] Vector3 point2;
    [SerializeField] LeanTweenType easeType2;
    [SerializeField] float moveTime2 = 1.0f;
    [SerializeField] bool isZigZag;
    [SerializeField] int moveLimit = 10;
    [SerializeField] float startDelay = -1.0f;
    int moveLimitNum = 0;
    PathScript pathScript;
    public override void OnStateEnter()
    {
        base.OnStateEnter();
        pathScript = GetComponent<PathScript>();
        pathScript.RemoveAll();
        if (!isZigZag)
        {
            Vector3[] pathPoints = new Vector3[] { transform.position, transform.position + point1 };
            pathScript.ChartPath(pathPoints);
        }
        else
        {
            transform.position = new Vector3(-2.4f, transform.position.y, transform.position.z);
            Vector3[] pathPoints = new Vector3[] { transform.position,transform.position + point1,transform.position + point1 + point2,
            transform.position+point1 * 2 + point2,transform.position + point1 * 2 + point2 * 2 };
            pathScript.ChartPath(pathPoints);
        }
        LeanTween.move(gameObject, transform.position, 0).setDelay(startDelay>=0?startDelay:Random.Range(0, 1.0f))
            .setOnComplete(MoveToPoint1);
    }
    void MoveToPoint1()
    {
        if(moveLimitNum < moveLimit)
        {
            LeanTween.move(gameObject, transform.position + point1, moveTime1).setEase(easeType1).setOnComplete(MoveToPoint2);
            moveLimitNum++;
        }
    }
    void MoveToPoint2()
    {
        if(moveLimitNum < moveLimit)
        {
            LeanTween.move(gameObject, transform.position + point2, moveTime2).setEase(easeType2).setOnComplete(MoveToPoint1);
            moveLimitNum++;
        }
    }
    public override EnemyStateMachine.EnemyStateType OnUpdate()
    {
        return base.OnUpdate();
    }
    public void OnDestroy()
    {
        //OnStateExit();
    }
    public override void OnStateExit()
    {
        base.OnStateExit();
        pathScript.RemoveAll();
        LeanTween.cancel(gameObject);
    }
}
