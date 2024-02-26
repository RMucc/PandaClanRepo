using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Story4Script : MonoBehaviour
{
    [SerializeField] GameObject mapLocation;
    public bool continueBool;
    // Start is called before the first frame update
    void Start()
    {
        continueBool = false;
        Debug.Log("Pausing Game and Activating Story3");
        GameManager.instance.Story5.SetActive(true);
        GameManager.instance.statePaused();
        GameManager.instance.OnLevelFinished += ActivateNextArrow;
    }
    private void Update()
    {
        if (continueBool == false)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                continueBool = true;
                GameManager.instance.Story5.SetActive(false);
                GameManager.instance.simpleResume();
            }
        }
    }

    void ActivateNextArrow(object sender, EventArgs e)
    {
            GameManager.instance.InstantiateArrow(mapLocation.transform);
    }
}
