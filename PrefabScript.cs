using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabScript : MonoBehaviour
{
    public enum PrefabType { None, RandomEnemy,RandomSpike,EnemyPreset,CoinPreset,PowerUp,LevelPrefab};
    public PrefabType type;
    public bool canLock = false;
    public int difficulty =0;
    public float time = 0;
    public bool canChangeTimer;
    public bool canChangePosition;
    public bool spawnAtCenter;
    public float awayFromEdge;
    private void Update()
    {
        if ((type == PrefabType.EnemyPreset) && transform.childCount == 0)
        {
            if (EventManager.Instance.camBottomLeftPos.y - 1 >= transform.position.y)
            {
                Destroy(gameObject);
                //EventManager.Instance.onLevelFinish();
            }
        } else if (type == PrefabType.CoinPreset && EventManager.Instance.camBottomLeftPos.y - 1 >= transform.position.y)
        {
            Destroy(gameObject);
        }
        else if( type == PrefabType.LevelPrefab && transform.childCount == 0)
        {
            Destroy(gameObject); 
            Debug.Log("Level Finished ");
            EventManager.Instance.onLevelFinish();
        }
    }
}
