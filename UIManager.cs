using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private EnemyFactory enemyFactory;
    private static UIManager uiManager=null;
    private void Awake()
    {
        if(uiManager!=null && uiManager != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            uiManager = this;
        }
    }
    public static UIManager Instance
    {
        get {
            return uiManager;
        }
    }
    public EnemyFactory GetEnemyFactory()
    {
        Debug.Assert(enemyFactory != null);
        return enemyFactory;
    }
    public void ActivateUI(string UiName)
    {
        for(int i =0; i <Instance.transform.childCount; i++)
        {
            var temp = Instance.transform.GetChild(i).gameObject;
            if (temp != null)
                temp.SetActive(false);
        }
        Transform tempTransform =transform.Find(UiName);
        Debug.Assert(tempTransform != null , " UI Screen not found ");
        tempTransform.gameObject.SetActive(true);
    }
}
