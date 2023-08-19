using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class MoveInRectState : MovementState
{
    [SerializeField] private Rect rect = new Rect(-1.0f, 1.0f, 2.0f, 2.0f);
    [SerializeField] private bool clockwise = false;
    [SerializeField] private float verticalAnimationTime = 1.0f;
    [SerializeField] private float horizontalAnimationTime = 1.0f;
    [SerializeField] private Vector2 startPos_normalized = new Vector2(-1.0f, 1.0f);
    [SerializeField] private LeanTweenType easeTypeVertical = LeanTweenType.linear;
    [SerializeField] private bool randomRotate = true;
    private PathScript pathScript;
    private int verticalTweenID;
    private int horizontalTweenID;
    private Vector3 offset= new Vector3(0.0f,0.0f,-10.0f);
    private float dvalue=0.0f;
    private Vector2 camOffset = Vector2.zero;
    private Vector2 camOffset2 = Vector2.zero;
    public override void OnStateEnter()
    {
        // if it is not configured or parent gameObject has no component PrefabScript;
        // also initialize vector3 offset; 
        //rect.width = UnityEngine.Random.Range(0.5f, 2.0f);
        //rect.height = UnityEngine.Random.Range(0.5f, 2.0f);
        // normalize startPos if its not ;
        base.OnStateEnter();
        if (randomRotate)
        {
            clockwise = transform.position.x > 0 ? false : true;
        }
        offset = Camera.main.transform.position-transform.position;
        rect.position = new Vector2(-(rect.width / 2) + transform.position.x 
            , -(rect.height / 2) + transform.position.y);
        pathScript = GetComponent<PathScript>();
        if (pathScript != null) { 
            pathScript.ChartPath(new Vector3[] { new Vector3(rect.xMin,rect.yMin), new Vector3(rect.xMin, rect.yMax) });
            pathScript.ChartPath(new Vector3[] { new Vector3(rect.xMin, rect.yMax), new Vector3(rect.xMax, rect.yMax) });
            pathScript.ChartPath(new Vector3[] { new Vector3(rect.xMax, rect.yMax), new Vector3(rect.xMax, rect.yMin) });
            pathScript.ChartPath(new Vector3[] { new Vector3(rect.xMax, rect.yMin), new Vector3(rect.xMin, rect.yMin) });
        }
        rect.position = new Vector2(((rect.width / 2) * startPos_normalized.x) + transform.position.x
            , ((rect.height / 2) * startPos_normalized.y) + transform.position.y);
        transform.position = rect.position;
        camOffset = Camera.main.transform.position;

        if (startPos_normalized.x == 1.0f && startPos_normalized.y == 1.0f)
        {
            if (clockwise)
                horizontalTweenID = -1;
            else
                verticalTweenID = -1;
        }
        else if (startPos_normalized.x == 1.0f && startPos_normalized.y == -1.0f)
        {
            if (clockwise)
                verticalTweenID = -1;
            else
                horizontalTweenID = -1;
        }
        else if (startPos_normalized.x == -1.0f && startPos_normalized.y == -1.0f)
        {
            if (clockwise)
                horizontalTweenID = -1;
            else
                verticalTweenID = -1;
        }
        else if (startPos_normalized.x == -1.0f && startPos_normalized.y == 1.0f)
        {
            if (clockwise)
                verticalTweenID = -1;
            else
                horizontalTweenID = -1;
        }
        //camOffset = Camera.main.transform.position;
        if (verticalTweenID != -1)
        verticalTweenID = LeanTween.value(gameObject, 0.0f, 1.0f, verticalAnimationTime).setOnUpdate(MoveInRect)
            .setOnComplete(SwitchTweening).setEase(easeTypeVertical).uniqueId;
        if(horizontalTweenID!=-1)
        horizontalTweenID = LeanTween.value(gameObject, 0.0f, 1.0f, horizontalAnimationTime).setOnUpdate(MoveInRect)
            .setOnComplete(SwitchTweening).uniqueId;

    }
    private void SwitchTweening()
    {
        if (isTweeningVertical())
        {
            verticalTweenID = -1;
            horizontalTweenID= LeanTween.value(gameObject, 0.0f, 1.0f, horizontalAnimationTime).setOnUpdate(MoveInRect)
            .setOnComplete(SwitchTweening).uniqueId;
        }else
        {
            horizontalTweenID = -1;
            verticalTweenID= LeanTween.value(gameObject, 0.0f, 1.0f, verticalAnimationTime).setOnUpdate(MoveInRect)
            .setOnComplete(SwitchTweening).uniqueId;
        }
        dvalue = 0.0f;
    }
    private bool isTweeningVertical() { return (horizontalTweenID < 0.0f); }

    private int DoMapping( bool clockwise)
    {
        int retVal = Convert.ToInt32(clockwise);
        retVal = (retVal * 2) - 1;
        return retVal;
    }
    private void MoveInRect(float value)
    {
        //offset = Camera.main.transform.position;
        dvalue = value - dvalue;
        Vector2 CamSpacePos = transform.position +offset;
        CamSpacePos.y =  CamSpacePos.y - camOffset.y;
        if (isTweeningVertical())
        {
            transform.position= new Vector3(transform.position.x,
               transform.position.y+ ((CamSpacePos.x > 0 ?-1 : 1) * DoMapping(clockwise) *
                rect.height * dvalue),transform.position.z);
        }
        else
        {
            transform.position = new Vector3(transform.position.x + ((CamSpacePos.y > 0 ? 1 : -1) * DoMapping(clockwise) *
                 rect.width * dvalue),transform.position.y, transform.position.z);
        }
        dvalue = value;
    }
    public override EnemyStateMachine.EnemyStateType OnUpdate()
    {
        return base.OnUpdate();
    }
    public override void OnStateExit()
    {
        base.OnStateExit();
        if(pathScript!=null)
            pathScript.RemoveAll();
        if(LeanTween.isTweening(gameObject))
            LeanTween.cancel(gameObject);
    }
    public void OnDestroy()
    {
        //OnStateExit();
    }

}
