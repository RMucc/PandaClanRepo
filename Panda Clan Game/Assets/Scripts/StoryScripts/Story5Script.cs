using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Story5Script : MonoBehaviour
{
    [SerializeField] GameObject tunnelLocation;
    public bool continueBool;
    // Start is called before the first frame update
    void Start()
    {
        continueBool = false;
        Debug.Log("Pausing Game and Activating Story3");
        GameManager.instance.Story7.SetActive(true);
        GameManager.instance.statePaused();
    }
    private void Update()
    {
        if (continueBool == false)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                continueBool = true;
                GameManager.instance.Story7.SetActive(false);
                GameManager.instance.simpleResume();
            }
        }

        if (GameManager.instance.enemyGoal <= 0)
        {
            GameManager.instance.Story5Bool = true;
            GameManager.instance.InstantiateArrow(null, false);
            GameManager.instance.InstantiateArrow(tunnelLocation.transform);
        }
    }
}
