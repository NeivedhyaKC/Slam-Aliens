using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WanderState : MovementState
{
    [SerializeField] private float wanderTimeLimit = 0;
    [SerializeField] private float wanderAnimationTime = 0;
    [SerializeField] private float distance = 0;
    [SerializeField] private bool isIdle = false;
    [SerializeField] private LeanTweenType easeType = LeanTweenType.linear;
    [SerializeField] private bool chartPathAtStart = false;
    private Vector3 randomPosition = Vector3.zero;
    bool isWandering = false;
    private float wanderTimer = 0;
    PathScript pathScript;
    
    public override void OnStateEnter()
    {
        base.OnStateEnter();
        wanderTimer = Random.Range(0.0f, 8.0f);
        if (!isIdle)
        {
            pathScript = GetComponent<PathScript>();
        }
        randomPosition.x = EventManager.Instance.camTopRightPos.x + 1.0f;
        while (randomPosition.x <= EventManager.Instance.camBottomLeftPos.x || randomPosition.x >= EventManager.Instance.camTopRightPos.x)
        {
            randomPosition.x = Random.Range(-1.0f, 1.0f);
            randomPosition.y = Random.Range(-1.0f, 1.0f);
            randomPosition = transform.position + randomPosition.normalized * Random.Range(1.0f, distance);
        }
        if (!isIdle && chartPathAtStart)
        {
            Vector3[] points = new Vector3[] { transform.position, randomPosition };
            pathScript.ChartPath(points);
        }

    }
    public override EnemyStateMachine.EnemyStateType OnUpdate()
    {
        if (!isWandering)
        {
            wanderTimer += Time.deltaTime;
        }
        if (base.OnUpdate() == EnemyStateMachine.EnemyStateType.Bouncing)
        {
            return EnemyStateMachine.EnemyStateType.Bouncing;
        }
        if (wanderTimer >= wanderTimeLimit && !isIdle)
        {
            wanderTimer = 0;
            isWandering = true;
            if (!chartPathAtStart)
            {
                Vector3[] points = new Vector3[] { transform.position, randomPosition };
                pathScript.ChartPath(points);
            }
            LeanTween.move(gameObject,randomPosition, wanderAnimationTime).setEase(easeType)
                .setOnComplete(delegate()
                {
                    isWandering = false;
                    pathScript.RemoveAll();
                    randomPosition.x = EventManager.Instance.camTopRightPos.x + 1.0f;
                    while (randomPosition.x <= EventManager.Instance.camBottomLeftPos.x || randomPosition.x >= EventManager.Instance.camTopRightPos.x)
                    {
                        randomPosition.x = Random.Range(-1.0f, 1.0f);
                        randomPosition.y = Random.Range(-1.0f, 1.0f);
                        randomPosition = transform.position + randomPosition.normalized * Random.Range(1.0f, distance);
                    }
                    if (chartPathAtStart)
                    {
                        Vector3[] points = new Vector3[] { transform.position, randomPosition };
                        pathScript.ChartPath(points);
                    }
                });
        }
        return EnemyStateMachine.EnemyStateType.Moving;
    }
    public override void OnStateExit()
    {
        base.OnStateExit();
        if (!isIdle)
        {
            GetComponent<PathScript>().RemoveAll();
        }
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
