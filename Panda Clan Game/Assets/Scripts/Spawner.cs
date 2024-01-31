using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] GameObject[] objectToSpawn;
    [SerializeField] int numToSpawn;
    [SerializeField] float timeBetweenSpawns;
    [SerializeField] Transform[] spawnPos;
    private int enemies;
    private int spawnCount;
    private bool isSpawning;
    private bool startSpawning;
    int wave = 1;

    // Start is called before the first frame update
    void Start()
    {
        //GameManager.instance.updateGameGoal(numToSpawn);
    }

    // Update is called once per frame
    void Update()
    {
        enemies = GameManager.instance.enemyCount;
        if (startSpawning && !isSpawning)
        {
            StartCoroutine(Spawn());
        }
    }

    IEnumerator Spawn()
    {
        if (wave == 1 && !isSpawning)
        {
            WaveSpawn(5);
        }
        else if (wave == 2 && !isSpawning)
        {
            WaveSpawn(9);
        }
        else if (wave == 3 && !isSpawning)
        {
            WaveSpawn(11);
        }
        yield return new WaitForSeconds(timeBetweenSpawns);
    }

    void WaveSpawn(int counter)
    {
        if(spawnCount != counter)
        {
            isSpawning = true;
            int arrayPos = Random.Range(0, spawnPos.Length);
            int randEnemy = Random.Range(0, objectToSpawn.Length);
            GameObject enemySpawned = Instantiate(objectToSpawn[randEnemy], spawnPos[arrayPos].transform.position, spawnPos[arrayPos].transform.rotation);
            Debug.Log(enemySpawned);
            spawnCount++;
            isSpawning = false;
        }
        if(spawnCount >= counter && enemies <= 0)
        {
            Debug.Log("Updating to next wave");
            wave++;
            spawnCount = 0;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            startSpawning = true;
        }
    }
}
