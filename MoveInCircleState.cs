using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveInCircleState : MovementState
{
    [SerializeField] private bool clockwise;
    [SerializeField] private float circleAnimationTime = 0;
    [SerializeField] private bool clockwiseIsConfigured = false;
    [SerializeField] private float circleRadius = 1.0f;
    [SerializeField] private bool chartPathEnable= true;
    [SerializeField] private AnimationCurve circleRadiusCurve;
    [SerializeField] private AnimationCurve circleRadiusCurve2;
    [SerializeField] private float startRotation = 0.0f;
    [SerializeField] private AnimationCurve circleSpeedAnimation;
    [SerializeField] private bool isEliptical = false;
    [SerializeField] private bool isShrinking = false;
    [SerializeField] private bool isStartingFromMiddle = false;
    private Vector3 initialPosition; 
    PathScript pathScript;
    private void Start()
    {
        if (!clockwiseIsConfigured)
        {
            clockwise = transform.position.x >= 0 ? false : true;
        }
        for(int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (child.CompareTag("Enemy Weapon"))
            {
                JointMotor2D motor2D = new JointMotor2D
                {
                    motorSpeed = Random.Range(0, 10) > 5 ? 360.0f : -360.0f,
                    maxMotorTorque = 10000.0f
                };
                child.GetComponent<HingeJoint2D>().motor= motor2D;
                child.GetComponent<HingeJoint2D>().useMotor = true;
                break;
            }
        }
    }
    public override void OnStateEnter()
    {
        base.OnStateEnter();
        int noOfPoints = 15;
        if (isEliptical)
        {
            noOfPoints = 30;
            startRotation = transform.position.x >= 0 ? 315.0f : 225.0f;
            Vector3 clampedPosition = new Vector3(transform.position.x >= 0 ? Mathf.Max(0.45f, transform.position.x) : 
                Mathf.Min(-0.45f, transform.position.x),transform.position.y, transform.position.x);
            transform.position = clampedPosition;
        }
        if (isShrinking)
        {
            startRotation = Random.Range(0, 360.0f);
            noOfPoints = 18;
        }
        initialPosition = transform.position;
        
        Vector3[] points = new Vector3[noOfPoints];
        for(int i = 0; i < noOfPoints; i++)
        {
            float circleRadiusMultiplier = isEliptical ? circleRadiusCurve.Evaluate((i * 360/noOfPoints) / 360.0f) : 1;
            points[i] = initialPosition + Quaternion.Euler(0, 0, startRotation + i * 360/noOfPoints) * Vector3.right * circleRadius 
            * circleRadiusMultiplier;
        }
        pathScript = GetComponent<PathScript>();
        if(pathScript!=null && chartPathEnable)
        {
            if (isEliptical)
            {
                pathScript.ChartPath(points);
            }
            else
            {
                pathScript.ChartPathExactly(points);
            }
        }
        circleSpeedAnimation.preWrapMode = WrapMode.Loop;
        circleSpeedAnimation.postWrapMode = WrapMode.Loop;
        LTDescr lTDescr = LeanTween.value(gameObject, 0, clockwise ? -360 : 360, circleAnimationTime).setOnUpdate(MoveInCircle).setLoopClamp();
        if (isEliptical)
        {
            lTDescr.setEase(circleSpeedAnimation);
        }
        else
        {
            lTDescr.setEase(LeanTweenType.linear);
        }
    }
    public override EnemyStateMachine.EnemyStateType OnUpdate()
    {
        if (base.OnUpdate() == EnemyStateMachine.EnemyStateType.Bouncing)
        {
            return EnemyStateMachine.EnemyStateType.Bouncing;
        }
        return EnemyStateMachine.EnemyStateType.Moving;
    }
    public override void OnStateExit()
    {
        base.OnStateExit();
        for(int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if(child.CompareTag("Enemy Weapon"))
            {
                child.gameObject.SetActive(false);
            }
        }
        pathScript.RemoveAll();
        if (LeanTween.isTweening(gameObject))
        {
            LeanTween.cancel(gameObject);
        }
    }

    void MoveInCircle(float value)
    {
        float circleRadiusMultiplier = 0.0f;
        if (isShrinking)
        {
            if (isStartingFromMiddle)
                value = value < 0 ? value - 180 : value + 180;
            circleRadiusMultiplier = circleRadiusCurve2.Evaluate(Mathf.Abs(value) / 360.0f > 1 ?
                Mathf.Abs(value) / 360.0f -1.0f : Mathf.Abs(value) / 360.0f);
            startRotation += 1;
            if (chartPathEnable && pathScript != null)
            {
                foreach(GameObject point in pathScript.GetPoints())
                {
                    point.transform.position = initialPosition + (point.transform.position - initialPosition).normalized * circleRadius * 
                        circleRadiusMultiplier;
                }
            }
        }
        else
        {
            circleRadiusMultiplier = isEliptical ? circleRadiusCurve.Evaluate(Mathf.Abs(value) / 360.0f) : 1;
        }
        transform.position = initialPosition + Quaternion.Euler(0, 0, startRotation + value)
            * Vector3.right * circleRadius * circleRadiusMultiplier;

    }

    private void OnDestroy()
    {
        //OnStateExit();
    }
}
