using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Story3Script : MonoBehaviour
{
    [SerializeField] GameObject laptopLocation;
    public bool continueBool;
    // Start is called before the first frame update
    void Start()
    {
        continueBool = false;
        Debug.Log("Pausing Game and Activating Story3");
        GameManager.instance.Story3.SetActive(true);
        GameManager.instance.statePaused();
        GameManager.instance.OnLevelFinished += ActivateNextArrow;
    }
    private void Update()
    {
        if (continueBool == false)
        {
            GameManager.instance.statePaused();
            if (Input.GetKeyDown(KeyCode.E))
            {
                continueBool = true;
                GameManager.instance.Story3.SetActive(false);
                GameManager.instance.simpleResume();
            }
        }
    }

    void ActivateNextArrow(object sender, EventArgs e)
    {
        GameManager.instance.InstantiateArrow(null, false);
        GameManager.instance.InstantiateArrow(laptopLocation.transform);
    }
}
