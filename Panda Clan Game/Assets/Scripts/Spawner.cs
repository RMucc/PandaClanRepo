using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;

public class Spawner : MonoBehaviour
{
    [SerializeField] GameObject[] objectToSpawn;
    [SerializeField] float timeBetweenSpawns;
    [SerializeField] Transform[] spawnPos;
    [SerializeField] int wave1EnemyAmount;
    [SerializeField] int wave2EnemyAmount;
    [SerializeField] int wave3EnemyAmount;
    private int totalEnemyAmount;
    private int enemies;
    private int spawnCount;
    private bool isSpawning;
    private bool startSpawning;
    int wave = 1;

    // Start is called before the first frame update
    void Start()
    {
        totalEnemyAmount = wave1EnemyAmount + wave2EnemyAmount + wave3EnemyAmount;
        GameManager.instance.updateGameGoal(totalEnemyAmount);
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
            GameManager.instance.waveCount.text = "Wave: 1/3";
            WaveSpawn(wave1EnemyAmount);
        }
        else if (wave == 2 && !isSpawning)
        {
            GameManager.instance.waveCount.text = "Wave: 2/3";
            WaveSpawn(wave2EnemyAmount);
        }
        else if (wave == 3 && !isSpawning)
        {
            GameManager.instance.waveCount.text = "Wave: 3/3";
            WaveSpawn(wave3EnemyAmount);
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
