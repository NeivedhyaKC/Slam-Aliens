using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public float bulletSpeed;
    public Vector3 targetPos;
    public Vector3 bulletDirection;
    public GameObject shooter = null;
    private Vector3 initialPosition = Vector3.zero;
    private void Start()
    {
        initialPosition = transform.position;
    }
    public void GetValue(float value)
    {
        transform.position = initialPosition + (targetPos - initialPosition) * value;
        if(value >= 1)
        {
            shooter.GetComponent<ShootingScript>().updateBulletValues -= GetValue;
        }
    }
    // Update is called once per frame
    void Update()
    {
        transform.position += bulletDirection.normalized * bulletSpeed * Time.deltaTime;
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Weapon"))
        {
            if(shooter !=null && shooter.GetComponent<ShootingScript>() != null)
            {
                shooter.GetComponent<ShootingScript>().updateBulletValues -= GetValue;
            }
            Destroy(gameObject);
        }
    }
}
