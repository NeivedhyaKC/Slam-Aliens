using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ShootingScript : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject exclamationPrefab;
    [SerializeField] private bool isShotgun = false;
    [SerializeField] private bool aimAtPlayer = true;
    [SerializeField] private float timeBeforeShooting = 1.0f;
    [SerializeField] private Vector3 shootDirection = Vector3.zero;
    [SerializeField] private int assualtAmmoCount = 10;
    [SerializeField] private float assaultReloadTime = 1.0f;
    [SerializeField] private float bulletSpeed = 5.0f;
    [SerializeField] private float angleLimit = 120.0f;
    [SerializeField] private bool fixTarget = false;
    private float shotgunShootTime = 2.5f;
    private float assaultShootTime = 0.1f;
    private float timer = 0.0f;
    private int bulletsShot = 0;
    public event Action<float> updateBulletValues;
    
    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= timeBeforeShooting - 0.4f && GetComponentInChildren<ExclamationScript>() ==null && timer <= timeBeforeShooting -0.3f)
        {
            GameObject exclamation = Instantiate(exclamationPrefab, transform.position + new Vector3(0, 0.65f, 0), Quaternion.identity);
            exclamation.transform.parent = transform;
        }
        if (timer >= timeBeforeShooting)
        {
            if (aimAtPlayer && bulletsShot == 0 && fixTarget)
            {
                shootDirection = EventManager.Instance.GetPlayerTransform().position - transform.position;
            }
            else
            {
                shootDirection = EventManager.Instance.GetPlayerTransform().position - transform.position;
            }
            if (Vector3.Angle(-transform.up, shootDirection) >= angleLimit)
            {
                shootDirection = Quaternion.Euler(0, 0, shootDirection.x >= 0 ? angleLimit : -angleLimit) * -transform.up;
            }
            if (isShotgun)
            {
                ShootShotgun();
                timeBeforeShooting = shotgunShootTime;
                timer = 0.0f;
            }
            else
            {
                if(bulletsShot < assualtAmmoCount)
                {
                    GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
                    bullet.GetComponent<BulletScript>().bulletSpeed = bulletSpeed;
                    bullet.GetComponent<BulletScript>().bulletDirection = shootDirection;
                    timeBeforeShooting = assaultShootTime;
                    timer = 0.0f;
                    bulletsShot++;
                }
                else
                {
                    bulletsShot = 0;
                    timer = 0.0f;
                    timeBeforeShooting = assaultReloadTime;
                }
            }
        }
    }
    void ShootShotgun()
    {
        for(int i = 0; i < 15; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            BulletScript bulletScript = bullet.GetComponent<BulletScript>();
            bulletScript.bulletSpeed = 0.2f;
            Vector3 shootDir = Quaternion.Euler(0, 0, UnityEngine.Random.Range(-20.0f, 20.0f)) * shootDirection;
            bulletScript.bulletDirection = shootDir;
            bulletScript.targetPos = bullet.transform.position + shootDir.normalized * UnityEngine.Random.Range(1.0f, 2.5f);
            bulletScript.shooter = gameObject;
            updateBulletValues += bulletScript.GetValue;
        }
        LeanTween.value(0.0f, 1.0f, 0.8f).setEase(LeanTweenType.easeOutSine).setOnUpdate(UpdateShotgunValues);
    }
    void UpdateShotgunValues( float value)
    {
        updateBulletValues(value);
    }
    public void OnStateExit()
    {
        timeBeforeShooting = 10000.0f;
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<ExclamationScript>() != null)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }
    }

}
