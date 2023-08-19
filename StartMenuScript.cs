using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuScript : MonoBehaviour
{
    // Start is called before the first frame update
    private bool isInitialized = false;
    void Start()
    {
        Setup();
    }
    private void OnEnable()
    {
        Setup();
    }
    private void Setup()
    {
        if (isInitialized) return;
        GameManager.Instance.EnemyFactorySetActive(false);
        GameManager.Instance.RefreshGameData();
        GameManager.Instance.RefreshLevel();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void LoadTestScene()
    {
        SceneManager.LoadScene("Scenes/SampleScene");
        Debug.Log("LoadingTestScene");
    }
    public void LoadInfinteRunner()
    {
        UIManager.Instance.ActivateUI("GameHUD");
        GameManager.Instance.SetInfiniteRunner();
    }
    public void LoadLevelSelect()
    {
        UIManager.Instance.ActivateUI("LevelSelect");
        GameManager.Instance.EnemyFactorySetActive(false);
    }
    //public void GoToLevelSelect()
    //{
    //    UIManager.Instance.ActivateUI("LevelSelect");
    //}
}
