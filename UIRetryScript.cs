using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRetryScript : MonoBehaviour
{
    private PlayerData playerData;
    private UIRetryDisplay uIRetryDisplay;
    [SerializeField] private float TimeToAppear = 1.0f;
    [SerializeField] private GameObject nextLevelButton;
    [SerializeField] private Slider scoreSlider;
    [SerializeField] private RectTransform smallStar1;
    [SerializeField] private RectTransform smallStar2;
    [SerializeField] private RectTransform smallStar3;

    // temp noOfStarsRequired to go to the next level;
    [SerializeField] private int reqStars = 2;
    private CanvasGroup canvasGroup;
    private bool isInitialized = false;

    private float barEndScore = 600;
    private float oneStarScore= 600;
    private float twoStarScore = 600;
    private float threeStarScore = 600;

    private void Start()
    {
        if(!isInitialized)
            Setup();
        StartCoroutine(WaitForSeconds(TimeToAppear));
        Hide();
    }
    private void OnEnable()
    {
        if(!isInitialized)
            Setup();
        StartCoroutine(WaitForSeconds(TimeToAppear));
        Hide();
    }
    private void Setup()
    {
        GameManager.Instance.EnemyFactorySetActive(false);

        //GameManager.Instance.RefreshGameData();
        playerData = GameManager.Instance.GetPlayerData();
        canvasGroup = GetComponent<CanvasGroup>();
        uIRetryDisplay = GetComponentsInChildren<UIRetryDisplay>()[0];
        Button LevelSelectButton = transform.Find("LevelButton").GetComponent<Button>();
        nextLevelButton.SetActive(false);
        LevelSelectButton.onClick.AddListener(delegate ()
        {
            UIManager.Instance.ActivateUI("LevelSelect");
        });
        Button HomeButton = transform.Find("HomeButton").GetComponent<Button>();
        HomeButton.onClick.AddListener(delegate ()
        {
            UIManager.Instance.ActivateUI("StartMenu");
        });
        Button RetryButton = transform.Find("RetryButton").GetComponent<Button>();
        bool isInifiniteRunner = GameManager.Instance.CurrentLevel < 0;
        uIRetryDisplay.SetScore(GameManager.Instance.GetScore);
        if (isInifiniteRunner)
        {
            uIRetryDisplay.SetPlayerDead();
            scoreSlider.gameObject.SetActive(false);
            uIRetryDisplay.SetBestScore(playerData.GetArcadeHighScore());
            RetryButton.onClick.AddListener(delegate ()
            {
                GameManager.Instance.SetInfiniteRunner();
                UIManager.Instance.ActivateUI("GameHUD");
            });
        }
        else
        {
            int retryLevel= GameManager.Instance.CurrentLevel;
            PlayerData.LevelData nextLevelData = playerData.GetLevel(retryLevel + 1);
            uIRetryDisplay.SetBestScore(playerData.GetLevel(GameManager.Instance.CurrentLevel).levelHighScore);
            //temp NoOfStars ;
            if (GameManager.Instance.isLevelPassed)
            {
                int tempNoOfStars = 0;
                if (GameManager.Instance.GetScore >= 600)
                    tempNoOfStars = 3;
                else if (GameManager.Instance.GetScore >= 400)
                    tempNoOfStars = 2;
                else if (GameManager.Instance.GetScore >= 200)
                    tempNoOfStars = 1;

                uIRetryDisplay.InitializeStars(tempNoOfStars);
                scoreSlider.gameObject.SetActive(true);
            }
            else
            {
                uIRetryDisplay.SetPlayerDead();
                scoreSlider.gameObject.SetActive(false);
            }
            

            
            if (GameManager.Instance.LevelPassed() || (nextLevelData.NoOfStars!=-1 && nextLevelData.isLocked==false))
            {
                if (nextLevelData.NoOfStars == -1)
                {
                    nextLevelButton.SetActive(false);
                    return;
                }
                nextLevelButton.SetActive(true);
                nextLevelButton.GetComponent<Button>().onClick.AddListener(delegate ()
                {
                    int level = retryLevel+1;
                    GameManager.Instance.SetLevel(level);
                    UIManager.Instance.ActivateUI("GameHUD");
                });
            }
            RetryButton.onClick.AddListener(delegate ()
            {
                int level = retryLevel;
                GameManager.Instance.SetLevel(level);
                UIManager.Instance.ActivateUI("GameHUD");
            });

        }
        isInitialized = true;
    }
    private void ProgressBar(float value)
    {
        scoreSlider.value = (GameManager.Instance.GetScore/ barEndScore)* value ; 
    }
    private void OnDisable()
    {
        isInitialized = false;   
    }
    private IEnumerator WaitForSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Show();

        LevelReqStarScore levelReqStarScore = GameManager.Instance.GetLevelReqStarScore(GameManager.Instance.CurrentLevel);
        float oneStarScore = levelReqStarScore.oneStarScore;
        float twoStarScore = levelReqStarScore.twoStarScore;
        float threeStarScore = levelReqStarScore.threeStarScore;
        barEndScore = threeStarScore;

        RectTransform parentRect = smallStar1.parent as RectTransform;
        if (GameManager.Instance.GetScore > barEndScore)
            barEndScore = GameManager.Instance.GetScore;

        float oneStarPosNorm = (oneStarScore / barEndScore * parentRect.sizeDelta.x) - (parentRect.sizeDelta.x/2);
        float twoStarPosNorm = (twoStarScore / barEndScore * parentRect.sizeDelta.x) - (parentRect.sizeDelta.x/2);
        float threeStarPosNorm = (threeStarScore / barEndScore * parentRect.sizeDelta.x) - (parentRect.sizeDelta.x / 2);

        smallStar1.localPosition = new Vector3(oneStarPosNorm, 0, 0);
        smallStar2.localPosition = new Vector3(twoStarPosNorm, 0, 0);
        smallStar3.localPosition = new Vector3(threeStarPosNorm, 0, 0);

        LeanTween.value(gameObject, ProgressBar, 0.0f, 1.0f, 1.0f).setEaseLinear();
    }
    private void Hide()
    {
        canvasGroup.alpha = 0.0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
    }
    private void Show()
    {
        canvasGroup.alpha = 1.0f;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
    }
}
