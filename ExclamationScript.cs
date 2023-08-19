using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExclamationScript : MonoBehaviour
{
    [SerializeField] private float existTime = 0.3f;
    private float existTimer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        existTimer += Time.deltaTime;
        if (existTimer > existTime)
        {
            Destroy(gameObject);
        }
    }
}
