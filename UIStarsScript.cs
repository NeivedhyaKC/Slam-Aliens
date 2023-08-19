using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIStarsScript : MonoBehaviour
{
    // Start is called before the first frame update
    RectTransform[] stars = new RectTransform[3];
    private void Awake()
    {
        RectTransform[] childGameObjects = GetComponentsInChildren<RectTransform>();
        for(int i=0; i < 3; i++)
        {
            stars[i] = childGameObjects[i+4];
            stars[i].transform.gameObject.SetActive(false);
        }
        //InitializeStars(1);
    }
    public void InitializeStars(int number)
    {
        Debug.Assert(number < 4 && number >=0);
        for(int i =0; i < number; i++)
        {
            stars[i].gameObject.SetActive(true);
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
