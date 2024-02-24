using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroToNextLevel : MonoBehaviour
{
    [SerializeField] GameObject Story1Location;
    [SerializeField] GameObject UziLocation;
    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.interfaceForStory.SetActive(false);
        GameManager.instance.InstantiateArrow(Story1Location.transform);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (GameManager.instance.Story1Bool == true && GameManager.instance.grabGunBool == true)
            {
                Debug.Log("Moving On To Next Level!");
                GameManager.instance.CheckScene();
                SaveManager.instance.SaveData();
                GameManager.instance.LoadNextScene();
            }
            if (GameManager.instance.Story1Bool == false && GameManager.instance.grabGunBool == false)
            {
                StartCoroutine(reminder());
            }
            if (GameManager.instance.Story1Bool == true && GameManager.instance.grabGunBool == false)
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
