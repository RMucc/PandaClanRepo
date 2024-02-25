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

        if (GameManager.instance.enemyGoal <= 0)
        {
            GameManager.instance.InstantiateArrow(null, false);
            GameManager.instance.InstantiateArrow(mapLocation.transform);
        }
    }
}
