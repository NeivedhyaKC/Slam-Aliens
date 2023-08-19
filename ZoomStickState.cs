using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomStickState : MovementState
{
    [SerializeField] private LeanTweenType easeType = LeanTweenType.linear;
    [SerializeField] private float waitTime = 1.0f;
    [SerializeField] private float rotateSpeed = 1.0f;
    [SerializeField] private float YBelowTop = 1.5f;
    //[SerializeField] static private float SpawnFrequency = 0.6f;
    //[SerializeField] static private int NoOfSpikes = 4;
    [SerializeField] private int NoOfZooms = 3;
    private PathScript pathScript;
    private static GameObject zoomStickPrefab;
    private bool wait = true;
    private bool aiming = false;
    private float aimTimer;
    private int zoomsCompleted = 0;

    //private class Functor
    //{
    //    static private bool funcBool = true;
    //    static float spawnTimer = 0.0f;
        
    //    static public void FunctorFunc()
    //    {
    //        spawnTimer += Time.deltaTime;
    //        if (Camera.main.GetComponentsInChildren<ZoomStickState>().Length>= NoOfSpikes)
    //        {
    //            funcBool = false;
    //        }
    //        if(spawnTimer>= SpawnFrequency && funcBool)
    //        {
    //            spawnTimer = 0.0f; 
    //            Instantiate(zoomStickPrefab, new Vector3(Random.Range(-2.5f,2.5f),
    //                 Camera.main.transform.position.y + 8.5f, 0), Quaternion.identity);
    //        }
    //    }
    //    public static void SetFuncBool( bool value)
    //    {
    //        funcBool = value;
    //    }
    //};


    public override void OnStateEnter()
    {
        base.OnStateEnter();
        transform.parent = Camera.main.transform;
        pathScript = GetComponent<PathScript>();
        if(zoomStickPrefab==null)
            zoomStickPrefab = this.gameObject;
        zoomsCompleted = 0;
        LeanTween.moveLocalY(gameObject, (EventManager.Instance.camTopRightPos.y - YBelowTop) - Camera.main.transform.position.y, 1.0f)
            .setOnComplete(delegate() {
            wait = false;
            aiming = true;
            Vector3 playerLocalPos1 = EventManager.Instance.GetPlayerTransform().position - Camera.main.transform.position;
            playerLocalPos1 = playerLocalPos1 - transform.localPosition;
            float degrees2 = Vector3.SignedAngle(transform.up, playerLocalPos1, Vector3.forward);
            transform.RotateAround(Vector3.forward, degrees2);
        });
        wait = true;
        aiming = false;
        aimTimer = 0.0f;
        //Functor.SetFuncBool(true);

        float spaceBetweenPoints = pathScript.GetSpaceBetweenPoints();
        pathScript.ChartPath(new Vector3[] { new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 20.0f, 0.0f) });
        pathScript.SetOnDestroyCollision(false);
        foreach (GameObject point in pathScript.GetPoints())
        {
            point.SetActive(false);
            point.transform.parent = transform;
            point.transform.localPosition = point.transform.position;
        }
    }
    public override EnemyStateMachine.EnemyStateType OnUpdate()
    {
        if (base.OnUpdate() == EnemyStateMachine.EnemyStateType.Bouncing) return EnemyStateMachine.EnemyStateType.Bouncing;
        if (wait) return EnemyStateMachine.EnemyStateType.Moving;

        //Functor.FunctorFunc();

        if(aiming)
        {
            aimTimer += Time.deltaTime;
            Vector3 playerLocalPos = EventManager.Instance.GetPlayerTransform().position - Camera.main.transform.position;
            Vector3 endPos = ReturnEndPos(transform.localPosition, transform.up);
            endPos = endPos - transform.localPosition;
            Debug.DrawLine(transform.position, transform.position +endPos );
            float noOfActivePoints = endPos.magnitude / pathScript.GetSpaceBetweenPoints();
            noOfActivePoints = Mathf.Clamp(noOfActivePoints, 0.0f, pathScript.GetPoints().Count-1);

            for (int i = 0; i <= noOfActivePoints; i++)
            {
                pathScript.GetPoints()[i].SetActive(true);
            }
            for (int j = (int)Mathf.Floor(noOfActivePoints) + 1; j < pathScript.GetPoints().Count; j++)
            {
                pathScript.GetPoints()[j].SetActive(false);
            }

            float degrees = Vector3.SignedAngle(transform.up, playerLocalPos - transform.localPosition,Vector3.forward);
            if (Mathf.Abs(degrees) > 5) {
                float rotationSpeed = Mathf.Sign(degrees) * rotateSpeed * Time.deltaTime;
                transform.RotateAround(Vector3.forward, rotationSpeed);
            }

            if (aimTimer >= waitTime && Mathf.Abs(degrees)<5)
            {
                aiming = false;
                playerLocalPos = playerLocalPos-transform.localPosition;
                pathScript.RemoveAll();
                pathScript.SetOnDestroyCollision(true);
                if (pathScript != null){
                    pathScript.ChartPath(new Vector3[] { transform.localPosition, ReturnEndPos(transform.localPosition, playerLocalPos.normalized) });
                    var points = pathScript.GetPoints();
                    foreach(var point in points)
                    {
                        point.transform.parent = Camera.main.transform;
                        point.transform.localPosition = point.transform.position;
                    } 
                }

                var returnedEndPos = ReturnEndPos(transform.localPosition, playerLocalPos);
                var traveltime = 0.5f;
                if (zoomsCompleted - 1 >=NoOfZooms)
                {
                    returnedEndPos *= 1.5f;
                    traveltime += 0.3f;
                }
                LeanTween.moveLocal(gameObject,returnedEndPos, 0.5f).
                    setOnComplete(delegate () {
                        zoomsCompleted++;
                        if (zoomsCompleted >= NoOfZooms)
                            Destroy(this.gameObject);
                        aiming = true;
                        aimTimer = 0.0f;
                        pathScript.RemoveAll();
                        pathScript.SetOnDestroyCollision(false);
                        pathScript.ChartPath(new Vector3[] { new Vector3(0.0f, 0.0f, 0.0f) , new Vector3(0.0f, 20.0f, 0.0f)});
                        foreach(GameObject point in pathScript.GetPoints())
                        {
                            point.SetActive(false);
                            point.transform.parent = transform;
                            point.transform.localPosition = point.transform.position;
                        }
                        //Vector3 playerLocalPos1 = EventManager.Instance.GetPlayerTransform().position - Camera.main.transform.position;
                        //playerLocalPos1 = playerLocalPos1 - transform.localPosition;
                        //float degrees2 = Vector3.SignedAngle(transform.up, playerLocalPos1,Vector3.forward);
                        //transform.RotateAround(Vector3.forward, 180.0f);
                    });
            }
        }
        return EnemyStateMachine.EnemyStateType.Moving;
    }

private Vector3 ReturnEndPos(Vector3 startPos, Vector3 dir)
    {
        const float incrementStep = 0.2f;
        float width = EventManager.Instance.camTopRightPos.x- EventManager.Instance.camBottomLeftPos.x;
        float height = EventManager.Instance.camTopRightPos.y - EventManager.Instance.camBottomLeftPos.y-1.0f;
        Rect rect = new Rect(new Vector2(-width/2.0f,-height/2.0f),new Vector2(width,height));
        Vector3 endPos = startPos;
        while (rect.Contains(endPos))
        {
            endPos += dir.normalized * incrementStep;
        }
        endPos += -dir.normalized * incrementStep;
        return endPos;
    }

    public override void OnStateExit()
    {
        base.OnStateExit();
        if (LeanTween.isTweening(gameObject))
        {
            LeanTween.cancel(gameObject);
        }
        pathScript.RemoveAll();
    }
    public void OnDestroy()
    {
        //OnStateExit();
    }
}
