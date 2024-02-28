using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Story7Script : MonoBehaviour
{
    [SerializeField] GameObject mapLocation;
    public bool continueBool;
    // Start is called before the first frame update
    void Start()
    {
        continueBool = false;
        //Debug.Log("Pausing Game and Activating Story3");
        GameManager.instance.Story9.SetActive(true);
        GameManager.instance.statePaused();
    }
    private void Update()
    {
        if (continueBool == false)
        {
            GameManager.instance.statePaused();
            if (Input.GetKeyDown(KeyCode.E))
            {
                continueBool = true;
                GameManager.instance.Story9.SetActive(false);
                GameManager.instance.simpleResume();
            }
        }
    }
}
