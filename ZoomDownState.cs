using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomDownState : MovementState
{
    [SerializeField] private float stayInPositionTime;
    [SerializeField] private float YBelowTop = 1.5f;
    [SerializeField] private float localYAwayFromCamTime;
    [SerializeField] private float localYAfterLaunch;
    [SerializeField] private float localYAfterLaunchTime;
    private float stayInPositionTimer;
    private bool inPosition;
    PathScript pathScript;

    private void Start()
    {
        transform.parent = Camera.main.transform;
    }
    public override void OnStateEnter()
    {
        base.OnStateEnter();
        pathScript = GetComponent<PathScript>();
        LeanTween.moveLocalY(gameObject, (EventManager.Instance.camTopRightPos.y - YBelowTop) - Camera.main.transform.position.y, localYAwayFromCamTime)
            .setEaseOutQuad().setLoopOnce().setOnComplete(FirstAnimationComplete);
    }
    private void FirstAnimationComplete()
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
            stayInPositionTimer += Time.deltaTime;
            if(stayInPositionTimer >= stayInPositionTime)
            {
                inPosition = false;
                Vector3[] points = new Vector3[] { transform.position, transform.position + new Vector3(0, localYAfterLaunch,0)};
                pathScript.ChartPath(points);
                foreach(GameObject point in pathScript.GetPoints())
                {
                    point.transform.parent = Camera.main.transform;
                }
                LeanTween.moveLocalY(gameObject, localYAfterLaunch, localYAfterLaunchTime).setEaseInCirc().setLoopOnce();
            }
        }
        return EnemyStateMachine.EnemyStateType.Moving;
    }
    public override void OnStateExit()
    {
        base.OnStateExit();
        pathScript.RemoveAll();
        if (LeanTween.isTweening(gameObject))
        {
            LeanTween.cancel(gameObject);
        }
    }
    private void OnDestroy()
    {
        //OnStateExit();
    }
}
