using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EventManager : MonoBehaviour
{

    private static EventManager instance = null;
    public static EventManager Instance { get { return instance; } }

    [SerializeField] private GameObject canvas;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Text coinScoreText;
    [SerializeField] private EnemyFactory enemyFactory;
    [HideInInspector] public Vector3 camBottomLeftPos;
    [HideInInspector] public Vector3 camTopRightPos;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }
    public delegate void EnemyKilled(int enemyId,int score,Vector3 bouncingDirection);
    public EnemyKilled enemyKilled;

    public delegate void EnemyDestroyed(int enemyId);
    public EnemyDestroyed enemyDestroyed;

    public delegate void OnLevelFinish();
    public OnLevelFinish onLevelFinish;

    public delegate void OnPlayerDeath();
    public OnPlayerDeath onPlayerDeath;

    //public EnemyFactory GetEnemyFactory() { return enemyFactory; }
    public void CoinCollected(int value)
    {
        coinScoreText.text = (int.Parse(coinScoreText.text) + value).ToString();
    }
    private void Update()
    {
        camBottomLeftPos = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));
        camTopRightPos = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
    }

    public GameObject GetCanvas()
    {
        return canvas;
    }
    public Transform GetPlayerTransform()
    {
        return playerTransform;
    }
}
