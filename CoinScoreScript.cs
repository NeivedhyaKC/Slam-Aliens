using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinScoreScript : MonoBehaviour
{
    // Start is called before the first frame update
    Text text;
    void Start()
    {
        text = this.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        text.text =  GameManager.Instance.GetCoins.ToString();
    }
}
