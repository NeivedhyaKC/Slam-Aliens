using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectScript : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject buttonPrefab;
    private bool isInitialized = false;
    private GameObject content; 
    private void Setup()
    {
        if( isInitialized ) return;
        content = GetComponentInChildren<GridLayoutGroup>().gameObject;
        GameManager.Instance.EnemyFactorySetActive(false);
        GameManager.Instance.RefreshGameData();
        PlayerData playerData = GameManager.Instance.GetPlayerData();
        int lockedCount = playerData.GetLevelCount() - playerData.GetUnlockedLevelCount();
        Transform[] oldButtons = content.GetComponentsInChildren<Transform>();
        for(int i=1; i<oldButtons.Length;i++)
        {
            Transform button = oldButtons[i];
            Destroy(button.gameObject);
        }
        for (int i = 0; i < playerData.GetLevelCount() - lockedCount; i++)
        {
            GameObject newButton = Instantiate(buttonPrefab);
            newButton.transform.parent = content.transform;
            LevelButtonScript script = newButton.GetComponent<LevelButtonScript>();
            script.SetText("Level " + (i + 1).ToString());
            script.InitializeStars(playerData.GetLevel(i).NoOfStars);
            script.level = i;
            newButton.GetComponent<Button>().onClick.AddListener(delegate ()
            {
                GameManager.Instance.SetLevel(script.level);
                UIManager.Instance.ActivateUI("GameHUD");
            });
        }
        for (int i = playerData.GetLevelCount() - lockedCount; i < playerData.GetLevelCount(); i++)
        {
            GameObject newButton = Instantiate(buttonPrefab);
            newButton.transform.parent = content.transform;
            LevelButtonScript script = newButton.GetComponent<LevelButtonScript>();
            script.SetLocked();
        }
        isInitialized = true;
    }
    private void Start()
    {
        Setup();
    }
    private void OnEnable()
    {
        Setup();
    }
    private void OnDisable()
    {
        isInitialized = false;
    }
}
