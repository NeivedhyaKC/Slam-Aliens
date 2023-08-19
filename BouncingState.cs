using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingState : EnemyState
{
    [SerializeField] float bouncingTime = 0.0f;
    [SerializeField] private int lives = 0;
    [SerializeField] private GameObject trailPrefab;
    [SerializeField] private GameObject soundBarrierPrefab;
    private Vector3 direction = Vector3.zero;
    private int score = 0;
    private Rigidbody2D rb;
    float timer = 0;
    GameObject trail;

    private float totalDistance = 0.0f;
    private bool isSlowMo = false;
    private float unscaledBounceTimer = 0.0f;
    private static float BounceSlowMoScore = 100.0f;
    private static int slowMoRef = 0;
    private static float slowMoBounceTime = 1.5f;
    private Vector3 prevPosition =Vector3.zero;
    private Vector3 movingDirection = Vector3.zero;
    private Vector2 closeToBoundary = Vector2.zero;
    private static Vector2 cameraRect = Vector2.zero;
    private void Start()
    {
        EventManager.Instance.enemyKilled += SetBouncingParams;
        cameraRect = new Vector2(EventManager.Instance.camTopRightPos.x - EventManager.Instance.camBottomLeftPos.x,
            EventManager.Instance.camTopRightPos.y - EventManager.Instance.camBottomLeftPos.y);
        cameraRect *= 0.5f;
    }
    public void SetBouncingParams(int enemyId ,int score, Vector3 bouncingDirection)
    {
        if(GetComponent<EnemyStateMachine>().GetId() == enemyId)
        {
            this.score = score;
            direction = bouncingDirection;
            if (score >= BounceSlowMoScore)
            {
                isSlowMo = true;
            }
        }
    }
    private IEnumerator BownceSlowMo()
    {
        rb.AddForce(new Vector2(direction.x, direction.y) * score / 3, ForceMode2D.Impulse);
        slowMoRef++;
        if (slowMoRef > 0)
        {
            Time.timeScale = 0.2f;
            Time.fixedDeltaTime = 0.01f * 0.4f;
        }
        while(unscaledBounceTimer < slowMoBounceTime)
        {
            unscaledBounceTimer += Time.unscaledDeltaTime;
            yield return null;
        }
        slowMoRef--;
        if (slowMoRef == 0)
        {
            Time.fixedDeltaTime = 0.01f;
            Time.timeScale = 1.0f;
        }
        OnStateExit();
        Destroy(gameObject);
        unscaledBounceTimer = 0.0f;
        yield  return null;
    }
    public void SetLives(int noOflives)
    {
        lives = noOflives;
    }
    public override void OnStateEnter()
    {
        prevPosition = transform.position;
        if(score >= 75.0f)
        {
            totalDistance = 0.0f;
        }
        else
        {
            totalDistance = float.MaxValue;
        }
        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = false;
        rb.gravityScale = 0;
        rb.useAutoMass = true;
        rb.drag = 3.0f;
        GetComponent<CircleCollider2D>().isTrigger = false;
        gameObject.layer = LayerMask.NameToLayer("Boundary");
        if (!isSlowMo)
            rb.AddForce(new Vector2(direction.x, direction.y) * score / 10, ForceMode2D.Impulse);
        else
            StartCoroutine(BownceSlowMo());
        //GetComponent<TrailRenderer>().enabled = true;
        trail = Instantiate(trailPrefab);
        trail.transform.parent = transform;
        trail.transform.localPosition = Vector3.zero;
        trail.transform.localRotation = Quaternion.identity;
        trail.SetActive(true);
        trail.transform.GetChild(0).gameObject.SetActive(true);
    }

    public override EnemyStateMachine.EnemyStateType OnUpdate()
    {
        timer += Time.deltaTime;
        totalDistance += Vector3.Distance(transform.position, prevPosition);
        if(totalDistance >= 2.0f && totalDistance !=float.MaxValue)
        {
            totalDistance = float.MaxValue;
            Instantiate(soundBarrierPrefab, transform.position, Quaternion.FromToRotation(transform.up, rb.velocity));
        }

        if (timer >= bouncingTime)
        {
            if (lives > 0)
            {
                lives--;
                timer = 0.0f;
                rb = GetComponent<Rigidbody2D>();
                rb.isKinematic = true;
                GetComponent<CircleCollider2D>().isTrigger = true;
                gameObject.layer = LayerMask.NameToLayer("Default");
                //GetComponent<TrailRenderer>().enabled = false;
                trail.SetActive(false);
                trail.transform.GetChild(0).gameObject.SetActive(false);
                return EnemyStateMachine.EnemyStateType.Moving;
            }
            else
            {
                timer = 0;
                Destroy(gameObject);
                return EnemyStateMachine.EnemyStateType.Moving;
            }
        }
        prevPosition = transform.position;
        return EnemyStateMachine.EnemyStateType.Bouncing;
    }
    private void FixedUpdate()
    {
        
    }
    public void OnCollisionEnter2D(Collision2D collision)
    {
        
        if (!isSlowMo) return;
        if (collision.gameObject.CompareTag("Boundary"))
        {
            Debug.Log("OnTrigger2D");
            movingDirection = direction;
            //prevPosition = transform.position;
            closeToBoundary = transform.position - Camera.main.transform.position;
            closeToBoundary = new Vector2(Mathf.Abs(closeToBoundary.x), Mathf.Abs(closeToBoundary.y));
            closeToBoundary = new Vector2(cameraRect.x - closeToBoundary.x, cameraRect.y - closeToBoundary.y);
            closeToBoundary = new Vector2(Mathf.Abs(closeToBoundary.x), Mathf.Abs(closeToBoundary.y));

            movingDirection.Normalize();
            movingDirection *= -1.0f;
            if (closeToBoundary.x > closeToBoundary.y)
                movingDirection.x = -movingDirection.x;
            else
                movingDirection.y = -movingDirection.y;
            direction = movingDirection;
            float tempVelocity = rb.velocity.magnitude;
            rb.velocity = new Vector2(0.0f, 0.0f);
            rb.velocity = movingDirection * tempVelocity;
        }
    }
    public override void OnStateExit()
    {
        rb.isKinematic = true;
    }
    private void OnDestroy()
    {
        EventManager.Instance.enemyKilled -= SetBouncingParams;
    }

}
