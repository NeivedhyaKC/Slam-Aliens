using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovementState : EnemyState
{
    private bool isHit;
    public override void OnStateEnter()
    {
        isHit = false;
        Rigidbody2D rb;
        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = true;
        GetComponent<CircleCollider2D>().isTrigger = true;
        gameObject.layer = LayerMask.NameToLayer("Default");
        EventManager.Instance.onPlayerDeath += BounceRandom;
    }
    private void BounceRandom()
    {
        isHit =true;
        int thisId = GetComponent<EnemyStateMachine>().GetId();
        var playerTransform=EventManager.Instance.GetPlayerTransform();
        GetComponent<BouncingState>().SetBouncingParams(thisId, 99,(transform.position - playerTransform.position).normalized);
    }

    public override EnemyStateMachine.EnemyStateType OnUpdate()
    {
        if (EventManager.Instance.camBottomLeftPos.y - 1 >= transform.position.y)
        {
            LeanTween.cancel(gameObject);
            Destroy(gameObject);
        }
        if (isHit)
        {
            return EnemyStateMachine.EnemyStateType.Bouncing;
        }
        return EnemyStateMachine.EnemyStateType.Moving;
    }

    public override void OnStateExit()
    {
        EventManager.Instance.onPlayerDeath -= BounceRandom;
        if ( GetComponent<ShootingScript>() != null)
        {
            GetComponent<ShootingScript>().OnStateExit();
        }
    }
    private void OnDestroy()
    {
        //OnStateExit();
        if (LeanTween.isTweening(gameObject))
        {
            LeanTween.cancel(gameObject);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Weapon"))
        {
            isHit = true;
        }
    }
}
