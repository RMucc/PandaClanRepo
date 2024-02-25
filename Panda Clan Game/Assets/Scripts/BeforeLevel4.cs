using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeforeLevel4 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (GameManager.instance.Story4Bool == true)
            {
                Debug.Log("Moving On To Next Level!");
                GameManager.instance.CheckScene();
                SaveManager.instance.SaveData();
                GameManager.instance.LoadNextScene();
            }
        }
    }
}
