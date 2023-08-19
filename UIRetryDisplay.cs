using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIRetryDisplay : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject scoreObject;
    [SerializeField] private GameObject bestScoreObject;
    [SerializeField] private GameObject YouDiedObject;
    [SerializeField] private List<GameObject> starBoxes;
    [SerializeField] private List<GameObject> stars;
    void Start()
    {
       //YouDiedObject.SetActive(false);
    }
    public void SetPlayerDead()
    {
        for(int i =0; i < 3; i++)
        {
            starBoxes[i].SetActive(false);
            stars[i].SetActive(false);
        }
        YouDiedObject.SetActive(true);
    }
    public void InitializeStars(int number)
    {
        //Debug.Assert(YouDiedObject.activeSelf == false);
        YouDiedObject.SetActive(false);
        foreach(GameObject star in starBoxes)
        {
            star.SetActive(true);
        }
        Debug.Assert(number >= 0 && number < 4);
        for(int i = 0; i < number; i++)
            stars[i].SetActive(true);
        for(int i =number; i < 3; i++)
            stars[i].SetActive(false);
    }
    public void SetScore(int number)
    {
        Debug.Assert(number >= 0);
        scoreObject.GetComponent<TextMeshProUGUI>().text = number.ToString();
    }
    public void SetBestScore(int number)
    {
        Debug.Assert(number >= 0);
        bestScoreObject.GetComponent<TextMeshProUGUI>().text = number.ToString();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
