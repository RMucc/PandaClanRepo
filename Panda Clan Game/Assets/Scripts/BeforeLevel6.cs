using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeforeLevel6 : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (GameManager.instance.Story6Bool == true)
            {
                Debug.Log("Moving On To Next Level!");
                GameManager.instance.CheckScene();
                SaveManager.instance.SaveData();
                GameManager.instance.LoadNextScene();
            }
        }
    }
}
