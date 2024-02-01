using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BaseEnemyAI : MonoBehaviour
{
    public Renderer model;
    public NavMeshAgent agent;
    public int HP;
    int amountSpawned;
    [SerializeField] List<GameObject> dropItemList;
    [SerializeField] int itemPotentialCountToDrop;


    public void OnDeath()
    {
        foreach (GameObject item in dropItemList)
        {
            int amountToSpawn = Random.Range(1, itemPotentialCountToDrop);
            for (int i = 1; i <= amountToSpawn; i++)
            {
                Quaternion itemRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
                Instantiate(item, this.transform.position, itemRotation);
            }
        }
        Destroy(gameObject);
    }
}
