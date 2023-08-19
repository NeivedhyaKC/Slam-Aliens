using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class EnemyFactory : MonoBehaviour
{
    [SerializeField] int presetIndex = -1;
    [SerializeField] bool isInfiniteRunner = true;
    [SerializeField] GameObject[] prefabs = null;
    [SerializeField] GameObject[] levels = null;
    float spawnAboveCamPosY = 2.5f;
    float timer;
    float time;
    private int id;

    //Difficulty variables
    int difficulty = 1;
    float difficultyTimer = 0.0f;
    float difficultyTime = 6.0f;
    float timeLowerRange = 2.0f;
    float timeUpperRange = 3.5f;

    //PresetMode variables
    int lockingGameobjectId = -1;
    float yCoordinate = float.MaxValue;
    float distance = float.MaxValue;
    bool inPresetMode = false;
    GameObject nextGameobject = null;
    GameObject presetInstance = null;

    bool levelSpawnComplete = false;

    public int GetID()
    {
        int i = id;
        id++;
        return i;
    }
    // Start is called before the first frame update
    void Start()
    {
        time = isInfiniteRunner ? Random.Range(timeLowerRange, timeUpperRange) : 0.0f;
        EventManager.Instance.enemyDestroyed += CheckLockEnemyKilled;
        GameManager.Instance.CurrentLevel = isInfiniteRunner ? -1:presetIndex;
    }
    public void SetInfiniteRunner()
    {
        isInfiniteRunner = true;
        presetIndex = -1;
        time =Random.Range(timeLowerRange, timeUpperRange) ;
        lockingGameobjectId = -1;
        inPresetMode = false;
        timer = 0.0f;
    }
    public void SetLevel(int levelIndex)
    {
        isInfiniteRunner = false;
        presetIndex = levelIndex;
        inPresetMode = false;
        timer = 0.0f;
        levelSpawnComplete = false;
    }
    public LevelReqStarScore GetLevelReqStarScore(int level)
    {
        Debug.Assert(level >= 0 && level < levels.Length && levels[level].GetComponent<LevelReqStarScore>()!=null 
            && levels[level].GetComponent<PrefabScript>().type == PrefabScript.PrefabType.LevelPrefab);
        return levels[level].GetComponent<LevelReqStarScore>();
    }
    public int LevelsCount()
    {
        lockingGameobjectId = -1;
        return levels.Length;
    }

    void Update()
    {
        timer += Time.deltaTime;
        difficultyTimer += Time.deltaTime;
        if(difficultyTimer > difficultyTime)
        {
            difficulty = Mathf.Min(10, difficulty + 1);
            difficultyTimer = 0.0f;
            //if(difficulty == 3)
            //{
            //    timeLowerRange = 0.5f;
            //    timeUpperRange = 2.5f;
            //}
            if (difficulty == 4)
                difficultyTime = 5.0f;
        }
        bool isLocked = false;
        for (int i = 0; i < Camera.main.transform.childCount; i++)
        {
            Transform child = Camera.main.transform.GetChild(i);
            if (child.CompareTag("Enemy") || child.CompareTag("Spike"))
            {
                isLocked = true;
                break;
            }
        }
        if (timer > time && !isLocked && !inPresetMode)
        {
            if (!isInfiniteRunner)
            {
                if (levelSpawnComplete)
                {
                    //if (lockingGameobjectId == -1)
                    //    EventManager.Instance.onLevelFinish();
                    GameManager.Instance.EnemyFactorySetActive(false);
                    levelSpawnComplete = false;
                    Debug.Log("a");
                }
                else
                {
                    int index = presetIndex < 0 ? Random.Range(0, levels.Length) : presetIndex;
                    SpawnGameobject(levels[index]);
                }
            }
            else
            {
                if(presetIndex < 0)
                {
                    SpawnGameobject(ChoosePrefabOfDifficulty(Random.Range(Mathf.Max(difficulty - 5, 0), difficulty), prefabs));
                }
                else
                {
                    SpawnGameobject(prefabs[presetIndex]);
                }
            }
        }
        if (inPresetMode)
        {
            if (Camera.main.transform.position.y - yCoordinate >= distance)
            {
                Activate(nextGameobject);
            }
        }
    }
    void Activate(GameObject inactiveObject)
    {
        inactiveObject.SetActive(true);
        float oldPositionY = inactiveObject.transform.position.y;
        inactiveObject.transform.position = new Vector3(inactiveObject.transform.position.x, EventManager.Instance.camTopRightPos.y
            + spawnAboveCamPosY, 0);
        yCoordinate = Camera.main.transform.position.y;
        PrefabScript prefabScript = inactiveObject.GetComponent<PrefabScript>();
        if (prefabScript != null)
        {
            if (prefabScript.canLock)
            {
                lockingGameobjectId = inactiveObject.GetComponent<EnemyStateMachine>().GetId();
                yCoordinate = float.MaxValue;
            }
            if (prefabScript.canChangePosition)
            {
                if (prefabScript.spawnAtCenter)
                    inactiveObject.transform.position = new Vector3(0, EventManager.Instance.camTopRightPos.y + spawnAboveCamPosY, 0);
                else
                    inactiveObject.transform.position = new Vector3(Random.Range(EventManager.Instance.camBottomLeftPos.x +
                    prefabScript.awayFromEdge, EventManager.Instance.camTopRightPos.x - prefabScript.awayFromEdge),
                    EventManager.Instance.camTopRightPos.y + spawnAboveCamPosY, 0);
            }
        }

        nextGameobject = GetNextInactiveObject(presetInstance);
        if (nextGameobject == null)
        {
            inPresetMode = false;
            levelSpawnComplete = true;
            distance = float.MaxValue;
            yCoordinate = float.MaxValue;
            presetInstance = null;
            timer = 0.0f;
            return;
        }
        distance = nextGameobject.transform.position.y - oldPositionY;
        if(distance <= 0)
        {
            Activate(nextGameobject);
        }
    }
    GameObject GetNextInactiveObject(GameObject rootInstance)
    {
        for(int i = 0; i < rootInstance.transform.childCount; i++)
        {
            if (!rootInstance.transform.GetChild(i).gameObject.activeSelf)
            {
                return rootInstance.transform.GetChild(i).gameObject;
            }
        }
        return null;
    }

    GameObject SpawnGameobject(GameObject prefab)
    {
        PrefabScript prefabScript = prefab.GetComponent<PrefabScript>();
        float positionX = prefabScript.spawnAtCenter ? 0.0f : Random.Range(EventManager.Instance.camBottomLeftPos.x +
            prefabScript.awayFromEdge, EventManager.Instance.camTopRightPos.x - prefabScript.awayFromEdge);
        time = prefabScript.canChangeTimer ? prefabScript.time : Random.Range(timeLowerRange, timeUpperRange);
        GameObject enemy = Instantiate(prefab, new Vector3(positionX,
            EventManager.Instance.camTopRightPos.y + spawnAboveCamPosY, 0), Quaternion.identity);
        switch (prefabScript.type)
        {

            case PrefabScript.PrefabType.RandomEnemy:
            case PrefabScript.PrefabType.RandomSpike:
                enemy.GetComponent<EnemyStateMachine>().Init(this);
                break;
            case PrefabScript.PrefabType.EnemyPreset:
            case PrefabScript.PrefabType.LevelPrefab:
                void InitAllChildren(GameObject parent)
                {
                    for (int i = 0; i < parent.transform.childCount; i++)
                    {
                        GameObject child = parent.transform.GetChild(i).gameObject;
                        if (child.CompareTag("Enemy") || child.CompareTag("Spike"))
                        {
                            child.GetComponent<EnemyStateMachine>().Init(this);
                            child.GetComponent<EnemyStateMachine>().isConfigured = true;
                        }
                        if (child.transform.childCount > 0)
                            InitAllChildren(child);
                    }
                };
                InitAllChildren(enemy);
                presetInstance = enemy;
                for (int i = 0; i < presetInstance.transform.childCount; i++)
                {
                    presetInstance.transform.GetChild(i).gameObject.SetActive(false);
                }

                nextGameobject = GetNextInactiveObject(presetInstance);
                if (nextGameobject == null)
                {
                    inPresetMode = false;
                    distance = float.MaxValue;
                    yCoordinate = float.MaxValue;
                    presetInstance = null;
                    timer = 0.0f;
                }
                else
                {
                    yCoordinate = Camera.main.transform.position.y;
                    distance = nextGameobject.transform.position.y - presetInstance.transform.position.y;
                    inPresetMode = true;
                }
                break;
            default:
                break;
        }
        timer = 0.0f;
        return enemy;
    }
    GameObject ChoosePrefabOfDifficulty(int difficultyParam,GameObject[] array)
    {
        int range = 0;
        while (true)
        {
            int upperRange = difficultyParam + range;
            int lowerRange = Mathf.Max(0, difficultyParam - range);
            for (int i = 0; i < 30; i++)
            {
                int index = Random.Range(0, array.Length);
                PrefabScript prefabScript = array[index].GetComponent<PrefabScript>();

                if (prefabScript.difficulty <= upperRange && prefabScript.difficulty >= lowerRange)
                {
                    return array[index];
                }
            }
            range++;
        }
    }
    void CheckLockEnemyKilled(int enemyId)
    {
        if(yCoordinate == float.MaxValue && lockingGameobjectId == enemyId)
        {
            yCoordinate = Camera.main.transform.position.y;
            lockingGameobjectId = -1;
            distance = 0;
        }
    }

    private void OnDestroy()
    {
        if (EventManager.Instance)
        {
            EventManager.Instance.enemyDestroyed -= CheckLockEnemyKilled;
        }
    }
}



//[SerializeField] Vector2 randomRange = new Vector2(0.0f, 100.0f);
//[SerializeField] GameObject[] randomEnemyPrefabs;
//[SerializeField] GameObject[] randomSpikePrefabs;
//[SerializeField] GameObject[] coinPresetPrefabs;
//[SerializeField] GameObject[] enemyPresetPrefabs;
//[SerializeField] GameObject[] powerUps;

//if(presetVector.y >= 0)
//{
//    switch (presetVector.x)
//    {
//        case 1:
//            SpawnGameobject(randomEnemyPrefabs[(int)presetVector.y]);
//            break;
//        case 2:
//            SpawnGameobject(randomSpikePrefabs[(int)presetVector.y]);
//            break;
//        case 3:
//            spawnCoinPreset = true;
//            selectedPresetprefab = coinPresetPrefabs[(int)presetVector.y];
//            break;
//        case 4:
//            spawnPreset = true;
//            selectedPresetprefab = enemyPresetPrefabs[(int)presetVector.y];
//            break;
//    }
//    randomRange.x = -1;
//    randomRange.y = -1;
//}

//        float randomNumber = Random.Range(randomRange.x, randomRange.y);
//        if (randomNumber > 0.0f && randomNumber < 35.0f)
//        {
//            SpawnGameobject(ChoosePrefabOfDifficulty(Random.Range(Mathf.Max(difficulty - 5, 0), difficulty), randomEnemyPrefabs));
//        }
//        else if (randomNumber >= 35.0f && randomNumber < 55.0f)
//        {
//            SpawnGameobject(ChoosePrefabOfDifficulty(Random.Range(Mathf.Max(difficulty - 5, 0), difficulty), randomSpikePrefabs));
//        }
//        else if (randomNumber >= 55.0f && randomNumber < 60.0f)
//        {
//            spawnCoinPreset = true;
//            selectedPresetprefab = coinPresetPrefabs[Random.Range(0, coinPresetPrefabs.Length)];
//        }
//        else if (randomNumber >= 60.0f && randomNumber <= 100.0f)
//        {
//            spawnPreset = true;
//            selectedPresetprefab = ChoosePrefabOfDifficulty(Random.Range(Mathf.Max(difficulty - 5, 0), difficulty), enemyPresetPrefabs);
//        }
//    }
//}
//if (spawnPreset && timer > time + 0.5f)
//{
//    presetInstance = SpawnGameobject(selectedPresetprefab);
//    for (int i = 0; i < presetInstance.transform.childCount; i++)
//    {
//        presetInstance.transform.GetChild(i).gameObject.SetActive(false);
//    }

//    nextGameobject = GetNextInactiveObject(presetInstance);
//    if (nextGameobject == null)
//    {
//        inPresetMode = false;
//        distance = float.MaxValue;
//        yCoordinate = float.MaxValue;
//        presetInstance = null;
//        timer = 0.0f;
//    }
//    else
//    {
//        yCoordinate = Camera.main.transform.position.y;
//        distance = nextGameobject.transform.position.y - presetInstance.transform.position.y;
//        inPresetMode = true;
//    }
//    spawnPreset = false;
//}
//if (spawnCoinPreset && timer > time + 0.5f)
//{
//    SpawnGameobject(selectedPresetprefab);
//    spawnCoinPreset = false;
//}