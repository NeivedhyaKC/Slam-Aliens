using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance = null;
    public static GameManager Instance { get 
       
        { 
            //Debug.Assert(Instance != null); 
            return instance; 
        } 
    }

    [SerializeField] PlayerClasses.PlayerController playerController;
    [SerializeField] float camUpSpeed = 3.0f;
    private EnemyFactory enemyFactory=null;
    //Game Data
    private int totalScore = 0;
    private int coinsCollected = 0;
    private int currentLevel = -1;
    private bool levelPassed = false;
    private PlayerData playerData;

    private void Awake()
    {
        if(instance!=this && instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
        enemyFactory = GetComponent<EnemyFactory>();
        playerData = new PlayerData();
        playerData.Serialize();
    }
    private void Start()
    {
        EventManager.Instance.enemyKilled += AddScore;
        EventManager.Instance.onLevelFinish += SetOnLevelComplete;
        EventManager.Instance.onLevelFinish += RefreshLevelCoroutine;
        EventManager.Instance.onPlayerDeath += SetOnPlayerDeath;
        EventManager.Instance.onPlayerDeath += RefreshLevelCoroutine;
    }

    public void RefreshGameData()
    {
        totalScore = 0;
        coinsCollected = 0;
        levelPassed = false;
        currentLevel = -1;
        playerController.gameObject.SetActive(true);
        playerController.transform.position = new Vector3(0.0f, Camera.main.transform.position.y, 0.0f);
        EnemyFactorySetActive(false);
    }
    public PlayerData GetPlayerData()
    {
        if (playerData == null)
        {
            playerData = new PlayerData();
            playerData.Serialize();
        }
        return playerData;
    }
    public EnemyFactory GetEnemyFactory() { return enemyFactory; }
    public float GetCamUpSpeed() { return camUpSpeed; }
    public bool LevelPassed() { return levelPassed; }
    private void AddScore(int enemyId, int score, Vector3 bouncingDirection) { totalScore += score; }

    public void AddCoin() { coinsCollected += 1; }
    public bool isLevelPassed { get { return levelPassed; } }
    public int GetCoins { get { return coinsCollected; } }
    public int GetScore { get { return totalScore; } }
    public int CurrentLevel { get { return currentLevel; } set { currentLevel = value; } }

    private IEnumerator WaitForSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        RefreshLevel();
        playerController.gameObject.SetActive(false);
        //playerController.gameObject.transform.position = Camera.main.transform.position;
        //also destroy stray Coins and dots if any ;
    }
    public void RefreshLevel()
    {
        EnemyStateMachine[] InactiveEnemies = GameObject.FindObjectsOfType<EnemyStateMachine>(true);
        foreach (EnemyStateMachine inactiveEnemy in InactiveEnemies)
        {
            inactiveEnemy.gameObject.SetActive(true);
            Destroy(inactiveEnemy.gameObject);
        }
        PrefabScript[] prefabScripts = GameObject.FindObjectsOfType<PrefabScript>(true);
        foreach (PrefabScript prefabScript in prefabScripts)
        {
            prefabScript.gameObject.SetActive(true);
            Destroy(prefabScript.gameObject);
        }
        playerController.gameObject.SetActive(true);
        //playerController.gameObject.transform.position = Camera.main.transform.position;
    }
    private void RefreshLevelCoroutine()
    {
        StartCoroutine(WaitForSeconds(3.0f));
    }
    public void SetLevel(int levelIndex)
    {
        RefreshGameData();
        GameManager.Instance.CurrentLevel = levelIndex;
        EnemyFactorySetActive(true);
        enemyFactory.SetLevel(levelIndex);

    }
    public void SetInfiniteRunner()
    {
        RefreshGameData();
        EnemyFactorySetActive(true);
        CurrentLevel = -1;
        enemyFactory.SetInfiniteRunner();
    }
    //public void ToggleIdle()
    //{
    //    enemyFactory.enabled = !enemyFactory.enabled;
    //}
    //public bool IsIdle()
    //{
    //    return enemyFactory.enabled;
    //}
    public void EnemyFactorySetActive(bool value)
    {
        enemyFactory.enabled = value;
    }
    private void Update()
    {
        Camera.main.transform.position += new Vector3(0, camUpSpeed * Time.deltaTime, 0);
    }
    private void SetOnPlayerDeath()
    {
        //PlayerData.LevelData leveldata = playerData.GetLevel(currentLevel);
        //int noOfStars = 0;
        //playerData.SetLevel(currentLevel, false
        //    , noOfStars >= leveldata.NoOfStars ? noOfStars : leveldata.NoOfStars,
        //    leveldata.levelHighScore <= totalScore ? totalScore : leveldata.levelHighScore);
        if (currentLevel>=0)
        {
            levelPassed = false;
        }
        else
        {
            playerData.SetArcadeHighScore
              (playerData.GetArcadeHighScore() >= totalScore ? playerData.GetArcadeHighScore() : totalScore);
        }

        UIManager.Instance.ActivateUI("Retry");
    }
    public LevelReqStarScore GetLevelReqStarScore(int level)
    {
        return enemyFactory.GetLevelReqStarScore(level);
    }
    private void SetOnLevelComplete()
    {
        levelPassed = false;
        int noOfStars = 0;
        if (totalScore >= 600)
            noOfStars = 3;
        else if (totalScore >= 400)
            noOfStars = 2;
        else if (totalScore >= 200)
            noOfStars = 1;
        else noOfStars = 0;

        PlayerData.LevelData leveldata = playerData.GetLevel(currentLevel);
        playerData.SetLevel(currentLevel, false
            , noOfStars >= leveldata.NoOfStars ? noOfStars : leveldata.NoOfStars,
            leveldata.levelHighScore <= totalScore ? totalScore : leveldata.levelHighScore);

        if (noOfStars >= 2)
        {
            levelPassed = true;
            var nextLevelData = playerData.GetLevel(currentLevel + 1);
            if (playerData.GetLevel(currentLevel + 1).isLocked == true && nextLevelData.NoOfStars != -1)
                playerData.SetLevel(currentLevel + 1, false, 0, 0);
        }

        UIManager.Instance.ActivateUI("Retry");
    }
    private void OnDestroy()
    {
        EventManager.Instance.enemyKilled -= AddScore;
        EventManager.Instance.onLevelFinish -= SetOnLevelComplete;
        EventManager.Instance.onLevelFinish -= RefreshLevelCoroutine;
        EventManager.Instance.onPlayerDeath -= RefreshLevelCoroutine;
        EventManager.Instance.onPlayerDeath -= SetOnPlayerDeath;
        playerData.Deserialize();
    }

}
