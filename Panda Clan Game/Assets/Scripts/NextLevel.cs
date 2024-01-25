using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextLevel : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    //When trigger entered by player
    private void OnTriggerEnter(Collider other)
    {
        //Keeps from touching itself
        if (other.isTrigger)
        {
            return;
        }
        //If Player is found with trigger
        if(other.CompareTag("Player"))
        {
            //if Player has killed all the enemies, ask if Player would like to move on to the next level
            if(GameManager.instance.enemyCount <= 0)
            {
                
            }
            //If there are still enemies left, player cannot move on to the next level. Complete the wave first
            else
            {

            }
        }
    }
    //When trigger is left by player (Not sure if we need anything in here, just made it in case)
    private void OnTriggerExit(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }
        //If Player is found with trigger
        if (other.CompareTag("Player"))
        {

        }
    }
}
