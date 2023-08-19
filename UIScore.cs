using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIScore : MonoBehaviour
{
    [SerializeField]
    private float scoreAnimationTime = 0.7f;
    private TextMeshProUGUI text;
    private int prevScore = 0;
    private PlayerClasses.PlayerController playerController;

    private void Start()
    {
        text = this.GetComponent<TextMeshProUGUI>();
        text.text = "0";
        playerController = EventManager.Instance.GetPlayerTransform().GetComponent<PlayerClasses.PlayerController>();
    }
    private void Update()
    {
        int actualScore = GameManager.Instance.GetScore;
        if (actualScore != prevScore)
        {
            LeanTween.cancel(gameObject);
            LeanTween.scale(gameObject, Vector3.one * 1.5f, scoreAnimationTime).setEasePunch();
            LeanTween.value(gameObject, prevScore, actualScore, scoreAnimationTime).setOnUpdate(OnIncrementScore).setEaseOutCirc();
            prevScore = actualScore;
        }
    }
    void OnIncrementScore(float value)
    {
        text.text = ((int)value).ToString();
    }
    void OnDestroy()
    {
        LeanTween.cancel(gameObject);
    }
}
