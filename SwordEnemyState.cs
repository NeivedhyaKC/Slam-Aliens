using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordEnemyState : MovementState
{
    [SerializeField] private float horizontalSensitivity = 0.0f;
    [SerializeField] private float verticalSensitivity = 0.0f;
    [SerializeField] private bool mimicPlayerWeapon = true;
    [SerializeField] private bool rotateLikeFan = false;
    [SerializeField] private float rotateFanSpeed = 0.0f;
    [SerializeField] private bool disableSword = false;
    [SerializeField] private float clampDistance = -1.0f;
    [SerializeField] private bool clampWithinScreen = false;
    [SerializeField] private bool chartPathHorizontal = false;
    private Transform swordMountTransform;
    private PlayerClasses.PlayerController playerController;
    private Rigidbody2D rb;
    private Vector3 initialPosition = Vector3.zero;
    public override void OnStateEnter()
    {
        base.OnStateEnter();
        rb = GetComponent<Rigidbody2D>();
        rotateFanSpeed = Random.Range(0.0f, 10.0f) >= 5.0f ? rotateFanSpeed : -rotateFanSpeed;
        swordMountTransform = transform.GetChild(0);
        playerController = EventManager.Instance.GetPlayerTransform().GetComponent<PlayerClasses.PlayerController>();
        horizontalSensitivity = playerController.GetSensitivity() * horizontalSensitivity;
        verticalSensitivity = playerController.GetSensitivity() * verticalSensitivity;
        if(clampDistance >= 0)
        {
            initialPosition = transform.position;
        }
        if ((mimicPlayerWeapon || rotateLikeFan) && !disableSword)
        {
            swordMountTransform.GetChild(0).GetComponent<Rigidbody2D>().isKinematic = true;
        }
        if (disableSword)
        {
            swordMountTransform.GetChild(0).gameObject.SetActive(false);
        }
        else
        {
            swordMountTransform.GetChild(0).gameObject.SetActive(true);
        }
        if (chartPathHorizontal)
        {
            Vector3[] pathpoints = new Vector3[] { new Vector3(EventManager.Instance.camBottomLeftPos.x + 0.15f,transform.position.y,0),
            new Vector3(EventManager.Instance.camTopRightPos.x,transform.position.y,0)};
            GetComponent<PathScript>().ChartPath(pathpoints);
        }
    }
    public override EnemyStateMachine.EnemyStateType OnUpdate()
    {
        if(base.OnUpdate() == EnemyStateMachine.EnemyStateType.Bouncing)
        {
            return EnemyStateMachine.EnemyStateType.Bouncing;
        }
        if(Input.touchCount > 0)
        {
            Vector3 moveVector = Vector3.zero;
            moveVector.x = transform.position.x + Input.GetTouch(0).deltaPosition.x * horizontalSensitivity * Time.deltaTime;
            moveVector.y = transform.position.y + Input.GetTouch(0).deltaPosition.y * verticalSensitivity * Time.deltaTime;
            if (clampWithinScreen)
            {
                moveVector.x = Mathf.Clamp(moveVector.x, EventManager.Instance.camBottomLeftPos.x, EventManager.Instance.camTopRightPos.x);
                //moveVector.y = Mathf.Clamp(moveVector.y, EventManager.Instance.camBottomLeftPos.y, EventManager.Instance.camTopRightPos.y);
            }
            if(clampDistance >= 0 && (moveVector - initialPosition).magnitude >= clampDistance)
            {
                moveVector = transform.position;
            }
            rb.MovePosition(moveVector);
        }
        if (mimicPlayerWeapon && playerController.GetActiveWeaponTransform()!=null && !disableSword)
        {
            swordMountTransform.rotation = playerController.GetActiveWeaponTransform().rotation;
        }
        if(rotateLikeFan && !disableSword)
        {
            swordMountTransform.rotation *= Quaternion.Euler(new Vector3(0, 0, rotateFanSpeed * Time.deltaTime));
        }
        return EnemyStateMachine.EnemyStateType.Moving;
    }
    public override void OnStateExit()
    {
        base.OnStateExit();
        initialPosition = Vector3.zero;
        swordMountTransform.GetChild(0).gameObject.SetActive(false);
        if (chartPathHorizontal)
        {
            GetComponent<PathScript>().RemoveAll();
        }
        if((mimicPlayerWeapon  || rotateLikeFan) && !disableSword)
        {
            swordMountTransform.GetChild(0).GetComponent<Rigidbody2D>().isKinematic = false;
        }
    }
}
