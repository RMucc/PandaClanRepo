using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Story6Script : MonoBehaviour
{
    [SerializeField] GameObject bossLocation;
    public bool continueBool;
    // Start is called before the first frame update
    void Start()
    {
        continueBool = false;
        Debug.Log("Pausing Game and Activating Story3");
        GameManager.instance.Story8.SetActive(true);
        GameManager.instance.statePaused();
    }
    private void Update()
    {
        if (continueBool == false)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                continueBool = true;
                GameManager.instance.Story8.SetActive(false);
                GameManager.instance.simpleResume();
            }
        }

        if (GameManager.instance.enemyGoal <= 0)
        {
            GameManager.instance.InstantiateArrow(bossLocation.transform);
        }
    }
}
