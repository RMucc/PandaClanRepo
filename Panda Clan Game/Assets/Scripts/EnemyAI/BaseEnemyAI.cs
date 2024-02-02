using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SocialPlatforms.Impl;

public class BaseEnemyAI : MonoBehaviour
{
    public Renderer model;
    public NavMeshAgent agent;
    public int HP;
    int amountSpawned;
    [SerializeField] List<GameObject> dropItemList;
    [SerializeField] int itemPotentialCountToDrop;
    public Color stored;



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

    public IEnumerator FlashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = stored;
    }
}
