using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SwordScript : MonoBehaviour
{
    [SerializeField] GameObject[] impactPrefabs;
    [SerializeField] GameObject Trail;
    [SerializeField] GameObject weight;
    Rigidbody2D hingeRb;
    float originalScale  = 0.0f;
    PlayerClasses.PlayerController playerController;
    GameObject swordHinge = null;
    HingeJoint2D hingeJoint = null;
    Queue<float> previousScales = new Queue<float>();
    float hitPauseTime = 0.15f;
    float trailEmitThreshold = 100;

    private void Start()
    {
        originalScale = transform.localScale.x;
        swordHinge = transform.parent.gameObject;
        hingeRb = swordHinge.GetComponent<Rigidbody2D>();
        hingeJoint = swordHinge.GetComponent<HingeJoint2D>();
        playerController = this.GetComponentInParent<PlayerClasses.PlayerController>();
    }
    private void Update()
    {
        //float newScale = Mathf.Min(0.45f,originalScale);
        //if (previousScales.Count == 10)
        //{
        //    previousScales.Dequeue();
        //}
        //previousScales.Enqueue(newScale);
        //float averageScale = 0;
        //foreach(float scale in previousScales)
        //{
        //    averageScale += scale;
        //}
        //averageScale = averageScale / previousScales.Count;
        //transform.localScale = new Vector3(averageScale,averageScale,averageScale);

        if (Mathf.Abs(hingeJoint.jointSpeed) < trailEmitThreshold)
        {
            Trail.SetActive(false);
        }
        else
        {
            Trail.SetActive(true);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Vector3 weightVelocity = new Vector3(weight.GetComponent<Rigidbody2D>().velocity.x, weight.GetComponent<Rigidbody2D>().velocity.y, 0);
        if (collision.CompareTag("Enemy"))
        {
            float dotProduct = Vector3.Dot(transform.up.normalized, (weightVelocity - collision.GetComponent<EnemyStateMachine>().GetVelocity()).normalized);
            EventManager.Instance.enemyKilled(collision.GetComponent<EnemyStateMachine>().GetId(), CalculateScore(collision)
                , dotProduct <0? -transform.up : transform.up);
        }
        else if (collision.CompareTag("Negative Enemy"))
        {
            float dotProduct = Vector3.Dot(transform.up.normalized, (weightVelocity - collision.GetComponent<EnemyStateMachine>().GetVelocity()).normalized);
            EventManager.Instance.enemyKilled(collision.GetComponent<EnemyStateMachine>().GetId(), -CalculateScore(collision)
                , dotProduct < 0 ? -transform.up : transform.up);
        }
        else if (collision.CompareTag("Spike") || collision.CompareTag("Laser"))
        {
            if (playerController.BatSpinStruct.active)
            {
                if (!collision.CompareTag("Laser"))
                {
                    float dotProduct = Vector3.Dot(transform.up.normalized, (weightVelocity - collision.GetComponent<EnemyStateMachine>().GetVelocity()).normalized);
                    EventManager.Instance.enemyKilled(collision.GetComponent<EnemyStateMachine>().GetId(), CalculateScore(collision)
                        , dotProduct < 0 ? -transform.up : transform.up);
                }
            }
            else
            {
                playerController.PlayerDie();
            }
        }
        else if (collision.CompareTag("Coin"))
        {
            GameManager.Instance.AddCoin();
            Destroy(collision.gameObject);
        }
    }
    int CalculateScore(Collider2D collision)
    {
        Vector3 weightVelocity = new Vector3(weight.GetComponent<Rigidbody2D>().velocity.x, weight.GetComponent<Rigidbody2D>().velocity.y, 0);
        float dotProduct = Vector3.Dot(transform.up.normalized, (weightVelocity - collision.GetComponent<EnemyStateMachine>().GetVelocity()).normalized);

        int score = (int)(weight.GetComponent<Rigidbody2D>().velocity.magnitude + 80 * Mathf.Clamp(Vector3.Dot(dotProduct >= 0.0f ?
            transform.up.normalized : -transform.up.normalized, collision.GetComponent<EnemyStateMachine>().GetVelocity()), -10.0f, 0.0f) / -10.0f);

        if(score >= 50)
        {
            int randomNumber = Random.Range(0, impactPrefabs.Length);
            Vector3 enemyLocalPos = transform.InverseTransformPoint(collision.gameObject.transform.position);
            GameObject instance = Instantiate(impactPrefabs[randomNumber], Vector3.zero, Quaternion.identity);
            instance.transform.parent = transform;
            instance.transform.localRotation = Quaternion.identity;
            if (dotProduct < 0)
            {
                instance.transform.localRotation *= Quaternion.Euler(0, 0, 180);
            }
            instance.transform.localPosition = new Vector3(enemyLocalPos.x, enemyLocalPos.y, 0);
            instance.transform.parent = null;
        }
        return score; 
    }
}
//Vector3 batVelocity = transform.up.normalized * Vector3.Dot(transform.up.normalized, hingeRb.velocity);
//Vector3 enemyVelocity = transform.up.normalized * Vector3.Dot(transform.up.normalized, collision.GetComponent<EnemyStateMachine>().GetVelocity());
//Vector3 totalVelocity =/*/* batVelocity * 2-enemyVelocity * 4f; */weight.GetComponent<Rigidbody2D>().velocity * 0.8f; 
//Debug.Log(Vector3.Dot(hingeJoint.jointSpeed > 0 ? transform.up.normalized : -transform.up.normalized
//    , collision.GetComponent<EnemyStateMachine>().GetVelocity()));
////return (int)Mathf.Max(1, totalVelocity.magnitude);
//return (int)Mathf.Abs(hingeJoint.jointSpeed / 25.0f);
