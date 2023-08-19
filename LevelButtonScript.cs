using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelButtonScript : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Sprite lockedRock;
    [SerializeField] private Transform StarsParent ;
    public int level;
    void Start()
    {
        Button button= this.GetComponent<Button>();
        //UIStarsScript[] tempTransforms = gameObject.GetComponentsInChildren<UIStarsScript>();
        //StarsParent =tempTransforms[0].transform;
        GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        //SetLocked();
    }
    public void InitializeStars(int number)
    {
        StarsParent.GetComponent<UIStarsScript>().InitializeStars(number);
    }
    public void SetText(string text)
    {
        transform.Find("Text").GetComponent<Text>().text = text;
    }
    // Update is called once per frame
    public void SetLocked()
    {
        Transform[] childTransforms = GetComponentsInChildren<Transform>();
        for (int i = 1; i < childTransforms.Length; i++)
            childTransforms[i].gameObject.SetActive(false);
        transform.GetComponent<Image>().sprite = lockedRock;
    }
    void Update()
    {
        
    }
}
