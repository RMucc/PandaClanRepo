using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Story2Script : MonoBehaviour
{
    public bool continueBool;
    // Start is called before the first frame update
    void Start()
    {
        continueBool = false;
        Debug.Log("Pausing Game and Activating Story2");
        GameManager.instance.Story2.SetActive(true);
        GameManager.instance.statePaused();
    }
    private void Update()
    {
        if(continueBool == false)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                continueBool = true;
                GameManager.instance.Story2.SetActive(false);
                GameManager.instance.simpleResume();
            }
        }
    }
}
