using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpScript : MonoBehaviour
{
    public PlayerClasses.PlayerController.PowerUpState state;
    public void Init(PlayerClasses.PlayerController.PowerUpState state)
    {
        this.state = state;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
