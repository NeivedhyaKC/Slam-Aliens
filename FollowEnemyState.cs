using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FollowEnemyState : MovementState
{
    [SerializeField] private float localYTime=0.5f;
    [SerializeField] private float YBelowTop = 1.0f;
    [SerializeField] private float moveSpeedMultiplier = 0.5f;
    [SerializeField] private float maxSpeedClamp = 1.0f;
    [SerializeField] private LeanTweenType easeType=LeanTweenType.linear;
    [SerializeField] private float maxHealth = 100.0f;
    [SerializeField] private float rotateSpeed = 0.5f;
    [SerializeField] private Slider healthSlider;


    private float health = 100.0f;
    private bool wait = true;
    private float moveSpeed = 0.0f;
    private Vector3 directionToPlayer = new Vector3();
    private bool spawned = false;
    private Rigidbody2D rb;
    private Vector3 moveDirection;

    public void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        //healthBar.transform.localPosition = new Vector3(0.0f,0.f, 0.0f);
        EventManager.Instance.onPlayerDeath += healthOnPlayerDeath;
    }
    private void healthOnPlayerDeath()
    {
        health = 0;
        healthSlider.value = 0;
    }
    private void CalculateHealth(int enemyId, int score, Vector3 bouncingDirection)
    {
        if(GetComponent<EnemyStateMachine>().GetId()== enemyId)
        {
            health = Mathf.Clamp(health - score, 0.0f, maxHealth);
            if(health<=0)
            {
                GetComponent<BouncingState>().SetLives(0);
            }else
            {
                GetComponent<BouncingState>().SetLives(1);
            }

            healthSlider.value = health / maxHealth;
        }
    }
    public override void OnStateEnter()
    {
        base.OnStateEnter();
        //transform.GetComponent<Rigidbody2D>().isKinematic = true;
        if (!spawned)
        {
            transform.parent = Camera.main.transform;
            LeanTween.moveLocalY(gameObject, (EventManager.Instance.camTopRightPos.y - YBelowTop) - Camera.main.transform.position.y, localYTime)
                .setOnComplete(MoveFunc);
            wait = true;
            spawned = true;
            health = maxHealth;
            GetComponent<BouncingState>().SetLives(0);
        }
        else
        {
            rb.isKinematic = true;
            rb.velocity = new Vector2(0, 0);
            MoveFunc();  
        }
        Vector3 playerCamPos = EventManager.Instance.GetPlayerTransform().position - Camera.main.transform.position;
        directionToPlayer = playerCamPos - transform.localPosition;
        moveDirection = directionToPlayer.normalized;
        EventManager.Instance.enemyKilled += CalculateHealth;

    }

    private void MoveFunc()
    {
        LeanTween.value(0.0f, 1.0f, 1.0f).setOnUpdate(AssignValue).setEase(easeType);
        wait = false;
    }
    private void AssignValue(float value)
    {
        moveSpeed = Mathf.Clamp(moveSpeedMultiplier * value, 0.0f, maxSpeedClamp);
    }
    public override EnemyStateMachine.EnemyStateType OnUpdate()
    {
        if (base.OnUpdate()==EnemyStateMachine.EnemyStateType.Bouncing) return EnemyStateMachine.EnemyStateType.Bouncing;
        if (wait) return EnemyStateMachine.EnemyStateType.Moving;

        Vector3 playerCamPos = EventManager.Instance.GetPlayerTransform().position - Camera.main.transform.position;
        directionToPlayer = playerCamPos- transform.localPosition;
        float degrees = Vector3.SignedAngle(moveDirection, directionToPlayer, Vector3.forward);
        degrees = rotateSpeed * Mathf.Sign(degrees) * Time.deltaTime; 
        float radians = degrees * Mathf.Deg2Rad;
        moveDirection = new Vector3((Mathf.Cos(radians) * moveDirection.x) - moveDirection.y * Mathf.Sin(radians),
                          Mathf.Sin(radians) * moveDirection.x + moveDirection.y * Mathf.Cos(radians), moveDirection.z);
        transform.position += (moveDirection.normalized * moveSpeed * Time.deltaTime);
        return EnemyStateMachine.EnemyStateType.Moving;
    }
    public override void OnStateExit()
    {
        base.OnStateExit();
        if (LeanTween.isTweening(gameObject))
        {
            LeanTween.cancel(gameObject);
        }
        healthSlider.value = health / maxHealth;
        EventManager.Instance.enemyKilled -= CalculateHealth;
        EventManager.Instance.onPlayerDeath -= healthOnPlayerDeath;

    }
    public void OnDestroy()
    {
        //OnStateExit();
    }
}
