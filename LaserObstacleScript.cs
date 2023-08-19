using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserObstacleScript : MonoBehaviour
{
    private float horizontalSensitivity = 0.5f;
    private float verticalSensitivity = 0.5f;
    PlayerClasses.PlayerController playerController;
    Vector3 playerPrevPos = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        playerController = EventManager.Instance.GetPlayerTransform().GetComponent<PlayerClasses.PlayerController>();
        if ((int)Time.time % 2 == 0)
        {
            horizontalSensitivity = playerController.GetSensitivity() * 2.0f;
            verticalSensitivity = playerController.GetSensitivity() * 0.0f;
        }
        else
        {
            horizontalSensitivity = playerController.GetSensitivity() * -1.5f;
            verticalSensitivity = playerController.GetSensitivity() * 0.0f;
        }
        playerPrevPos = EventManager.Instance.GetPlayerTransform().position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 moveVector = Vector3.zero;
        moveVector.x = Mathf.Clamp(transform.position.x +(EventManager.Instance.GetPlayerTransform().position - playerPrevPos).x * horizontalSensitivity,
            EventManager.Instance.camBottomLeftPos.x, EventManager.Instance.camTopRightPos.x);
        moveVector.y = transform.position.y;
        transform.position = moveVector;
        playerPrevPos = EventManager.Instance.GetPlayerTransform().position;
    }
}
