using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerState : MovementState
{
    [SerializeField] private float YBelowTop = 1.5f;
    [SerializeField] private float shootInterval = 5.0f;
    [SerializeField] private float HorizontalMoveSpeed = 0.5f;
    [SerializeField] private float offPosMoveTime = 1.0f;
    [SerializeField] private LeanTweenType offPosEaseType = LeanTweenType.linear;
    [SerializeField] private float playerNextPosLength = 0.0f;
    [SerializeField] private float stopMovingXRange = 0.0f;
    [SerializeField] private int ammo = 5;
    [SerializeField] private float enemyChildChance = 50.0f;
    [SerializeField] private float offPositionChance = 50.0f;
    private GameObject spikeChild;
    private GameObject enemyChild;
    private GameObject exclamation;
    private Vector3 playerPrevPos;
    private float shootTimer = 0.0f;
    private bool wait = true;
    private bool offPosition = false;
    private int tweenID = -1;
    private float CamRange = 2.95f;
    private int scriptedIndex = -1;
    [SerializeField] private ScriptedStruct[] scriptedStructs;


    public override void OnStateEnter()
    {
        base.OnStateEnter();
        transform.parent = Camera.main.transform;
        LeanTween.moveLocalY(gameObject, (EventManager.Instance.camTopRightPos.y - YBelowTop) - Camera.main.transform.position.y, 0.7f)
            .setOnComplete(delegate () {
            wait = false;
        });
        wait = true;
        spikeChild = transform.Find("SpikeAmmo").gameObject;
        spikeChild.SetActive(false);
        enemyChild = transform.Find("EnemyAmmo").gameObject;
        enemyChild.SetActive(false);
        exclamation = transform.Find("Exclamation").gameObject;
        exclamation.SetActive(false);
        shootTimer = 0.0f;
        playerPrevPos = EventManager.Instance.GetPlayerTransform().position;
        float tempRandom = Random.Range(0.0f, 1.0f);
        offPosition = tempRandom < offPositionChance / 100.0f ? true : false;
        if (offPosition)
        {

            scriptedIndex =Mathf.FloorToInt(Random.Range(0.0f, scriptedStructs.Length-0.1f));
            if (scriptedIndex >= 0)
            {
                ammo = scriptedStructs[scriptedIndex].XCoordEnemyChanceMovetime.Length;
                scriptedStructs[scriptedIndex].index = 0;
            }
        }
        tweenID = -1;
        CamRange = Mathf.Abs(EventManager.Instance.camBottomLeftPos.x - EventManager.Instance.camTopRightPos.x) / 2.0f;
    }
    public override EnemyStateMachine.EnemyStateType OnUpdate()
    {
        shootTimer += Time.deltaTime;
        if (base.OnUpdate() == EnemyStateMachine.EnemyStateType.Bouncing) return EnemyStateMachine.EnemyStateType.Bouncing;
        if (wait) return EnemyStateMachine.EnemyStateType.Moving;

        if (!offPosition)
        {
            Vector3 dirNormalized = (EventManager.Instance.GetPlayerTransform().position - playerPrevPos).normalized;
            Vector3 playerNextPos = EventManager.Instance.GetPlayerTransform().position + dirNormalized * playerNextPosLength;
            Vector3 playerNextCamPos = playerNextPos - Camera.main.transform.position;
            if (Mathf.Abs(transform.position.x - playerNextCamPos.x) > stopMovingXRange)
            {
                Vector3 moveDirX = (new Vector3(playerNextCamPos.x, 0.0f, 0.0f) -
                    new Vector3(transform.localPosition.x, 0.0f, 0.0f)).normalized;
                transform.localPosition += HorizontalMoveSpeed * moveDirX;
            }

            if (shootTimer >= shootInterval - 0.3f && Mathf.Abs(transform.position.x - playerNextCamPos.x) <= stopMovingXRange
                 && !exclamation.activeSelf)
            {
                exclamation.SetActive(true);
            }

            if (Mathf.Abs(transform.position.x - playerNextCamPos.x) <= stopMovingXRange && shootTimer >= shootInterval
                 && ammo > 0)
            {
                float randomFloat = Random.Range(0.0f, 1.0f);
                GameObject spike = Instantiate((randomFloat < enemyChildChance / 100.0f ? enemyChild : spikeChild), transform.position, Quaternion.identity);
                spike.SetActive(true);
                AmmoState ammoState = spike.GetComponent<AmmoState>();
                ammoState.SetEndPosition(new Vector3(transform.localPosition.x, transform.localPosition.y - 13.0f, 0.0f));
                ammo--;
                shootTimer = 0.0f;
                exclamation.SetActive(false);
            }
        }
        else
        {
            if (tweenID == -1)
            {
                OffPosFunc();
            }
            if (shootTimer >= offPosMoveTime - 0.3f && ammo > 0)
                exclamation.SetActive(true);
        }
        if (ammo == 0)
        {
            LeanTween.moveLocalY(gameObject, 8.5f, 1.0f).setOnComplete(delegate () {
                Destroy(gameObject);
            });
        }
        playerPrevPos = EventManager.Instance.GetPlayerTransform().position;
        return EnemyStateMachine.EnemyStateType.Moving;

    }

    private void OffPosFunc()
    {
        if (ammo < 1) return;
        if (scriptedIndex < 0)
        {
            float XCoord = 0.0f;
            XCoord = Random.Range(-CamRange, CamRange);
            Vector3 randomPos = new Vector3(XCoord, transform.localPosition.y, transform.localPosition.z);
            tweenID = LeanTween.moveLocal(gameObject, randomPos, offPosMoveTime).setEase(offPosEaseType)
                .setOnComplete(delegate ()
                {
                    Shoot();
                    OffPosFunc();
                }).uniqueId;
        }
        else
        {
            float XCoord = scriptedStructs[scriptedIndex].XCoordEnemyChanceMovetime[scriptedStructs[scriptedIndex].index].x;
            float moveTime = scriptedStructs[scriptedIndex].XCoordEnemyChanceMovetime[scriptedStructs[scriptedIndex].index].z;
            if (scriptedStructs[scriptedIndex].XCoordEnemyChanceMovetime[scriptedStructs[scriptedIndex].index].w== 100)
               XCoord = EventManager.Instance.GetPlayerTransform().position.x;
            offPosEaseType = scriptedStructs[scriptedIndex].leanTweenTypes[scriptedStructs[scriptedIndex].index];
            Vector3 randomPos = new Vector3(XCoord, transform.localPosition.y, transform.localPosition.z);
            tweenID = LeanTween.moveLocal(gameObject, randomPos, moveTime).setEase(offPosEaseType)
                .setOnComplete(delegate ()
                {
                    Shoot();
                    OffPosFunc();
                }).uniqueId;
        }
    }
    private void Shoot()
    {
        float randomFloat = Random.Range(0.0f, 1.0f);
        if (offPosition && scriptedIndex >= 0)
            enemyChildChance = scriptedStructs[scriptedIndex].XCoordEnemyChanceMovetime[scriptedStructs[scriptedIndex].index].y;
        GameObject spike = Instantiate((randomFloat < enemyChildChance / 100.0f ? enemyChild : spikeChild), transform.position, Quaternion.identity);
        spike.SetActive(true);
        if (scriptedIndex >= 0)
        {
            if (!scriptedStructs[scriptedIndex].pathScriptEnable)
                Destroy(spike.GetComponent<PathScript>());
            scriptedStructs[scriptedIndex].index++;
        }
        AmmoState ammoState = spike.GetComponent<AmmoState>();
        ammoState.SetEndPosition(new Vector3(transform.localPosition.x, transform.localPosition.y - 13.0f, 0.0f));
        ammo--;
        shootTimer = 0.0f;
        exclamation.SetActive(false);
    }
    private void OnDestroy()
    {
        //OnStateExit();
    }
}
