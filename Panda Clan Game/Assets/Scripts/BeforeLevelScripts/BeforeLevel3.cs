using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeforeLevel3 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (GameManager.instance.Story3Bool == true)
            {
                Debug.Log("Moving On To Next Level!");
                GameManager.instance.CheckScene();
                SaveManager.instance.SaveData();
                GameManager.instance.LoadNextScene();
            }
            if (GameManager.instance.Story3Bool == false)
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
