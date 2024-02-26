using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeforeLevel5 : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (GameManager.instance.Story5Bool == true)
            {
                Debug.Log("Moving On To Next Level!");
                GameManager.instance.CheckScene();
                SaveManager.instance.SaveData();
                GameManager.instance.LoadNextScene();
            }
            if (GameManager.instance.Story5Bool == false)
            {
                StartCoroutine(reminder());
            }
        }
    }

    IEnumerator reminder()
    {
        GameManager.instance.Story1Reminder.SetActive(true);
        yield return new WaitForSeconds(3);
        GameManager.instance.Story1Reminder.SetActive(false);
    }
}
