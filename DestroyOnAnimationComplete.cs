using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnAnimationComplete : MonoBehaviour
{
    // Start is called before the first frame update
    public void OnAnimationComplete(int i)
    {
        Destroy(gameObject);
    }
}
