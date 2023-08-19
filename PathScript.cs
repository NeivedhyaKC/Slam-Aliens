using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathScript : MonoBehaviour
{
    [SerializeField] GameObject point;
    [SerializeField] float spaceBetweenPoints = 0.4f;
    [SerializeField] private bool DestroyOnCollision = false;
    [SerializeField] private float magnitudeForRemoval = 0.5f;
    Vector3 prevPoint = Vector3.zero;
    List<GameObject> pointsCreated;
    public float GetSpaceBetweenPoints()
    {
        return spaceBetweenPoints;
    }
    public void SetOnDestroyCollision(bool active)
    {
        DestroyOnCollision = active;
    }
    private void Start()
    {
        pointsCreated = new List<GameObject>();
    }
    public void ChartPathExactly(Vector3[] pathPoints)
    {
        foreach(Vector3 pos in pathPoints)
        {
            pointsCreated.Add(Instantiate(point, pos, Quaternion.identity));
        }
    }
    public void ChartPath(Vector3[] pathPoints)
    {
        //RemoveAll();
        prevPoint = pathPoints[0];
        pointsCreated.Add(Instantiate(point, prevPoint, Quaternion.identity));
        for(int i = 1; i<pathPoints.Length;i++)
        {
            if((pathPoints[i] - prevPoint).magnitude > spaceBetweenPoints)
            {
                Recursive(prevPoint, pathPoints[i]);
            }
        }
    }
    public void Update()
    {
        if(DestroyOnCollision && pointsCreated.Count!=0)
        foreach(var point in pointsCreated)
        {
            if (((Vector2)point.transform.position - (Vector2)transform.position).magnitude < magnitudeForRemoval)
            {
                GameObject tempPoint = point;
                pointsCreated.Remove(point);
                Destroy(point);
                return;
            }
            else
            {
                return;
            }
        }
    }

    public void RemoveAll()
    {
        foreach (GameObject point in pointsCreated)
        {
            Destroy(point);
        }
        pointsCreated.Clear();
    }
    private void Recursive( Vector3 firstPoint,Vector3 lastPoint)
    {
        Vector3 distance = lastPoint - firstPoint;
        if(distance.magnitude > spaceBetweenPoints)
        {
            prevPoint = firstPoint + distance.normalized * spaceBetweenPoints;
            pointsCreated.Add(Instantiate(point, prevPoint, Quaternion.identity));
            Recursive(prevPoint,lastPoint);
        }
    }
    public List<GameObject> GetPoints()
    {
        return pointsCreated;
    }
}
