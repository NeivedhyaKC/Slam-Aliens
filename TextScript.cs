using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextScript : MonoBehaviour
{
    [SerializeField] float startScale;
    [SerializeField] float endScale;
    [SerializeField] float scaleTime;
    [SerializeField] float moveToDistance;
    [SerializeField] float moveToTime;
    //[SerializeField] AnimationCurve easeCurve;
    float timer;
    float initialYPos = 0.0f;
    RectTransform rectTransform;
    RectTransform canvasRectTransform;
    [SerializeField] private TMP_ColorGradient blueColorGradient;
    [SerializeField] private TMP_ColorGradient yellowColorGradient;
    [SerializeField] private float yellowGradientScore = 100.0f;
    [SerializeField] private float blueGradientScore = 50.0f;
    // Start is called before the first frame update
    void Start()
    {   
        timer = 0;
    }
    public void Init(int score)
    {
        rectTransform = GetComponent<RectTransform>();
        canvasRectTransform = EventManager.Instance.GetCanvas().transform as RectTransform;
        initialYPos = rectTransform.anchoredPosition.y;
        GetComponent<TextMeshProUGUI>().text = score >= 0 ? "+" + score.ToString() : score.ToString();
        if (score >= yellowGradientScore)
        {
            GetComponent<TextMeshProUGUI>().colorGradientPreset = yellowColorGradient;
        }
        else if(score >= blueGradientScore)
        {
            GetComponent<TextMeshProUGUI>().colorGradientPreset = blueColorGradient;
        }
        LeanTween.value(gameObject,0.0f, 1.0f, scaleTime).setOnUpdate(ScaleMoved).setEaseOutExpo();
        LeanTween.value(gameObject, 0, moveToDistance, moveToTime).setOnUpdate(DistanceMoved).setEaseOutExpo();
    }
    void DistanceMoved(float value)
    {
        rectTransform.anchoredPosition = new Vector2( 
            Mathf.Clamp(rectTransform.anchoredPosition.x,-(canvasRectTransform.sizeDelta.x/2) + rectTransform.sizeDelta.x,
            (canvasRectTransform.sizeDelta.x / 2) - rectTransform.sizeDelta.x),
            Mathf.Clamp(initialYPos + value,-(canvasRectTransform.sizeDelta.y/2) + rectTransform.sizeDelta.y,
            (canvasRectTransform.sizeDelta.y/2) - rectTransform.sizeDelta.y));
    }
    void ScaleMoved(float value)
    {
        GetComponent<TextMeshProUGUI>().fontSize = startScale + (endScale - startScale) * value;
    }
    void OnDestroy()
    {
        LeanTween.cancel(gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer > scaleTime)
        {
            Destroy(gameObject);
        }
    }
}
