using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;

    public GameObject player;
    public RyansPlayerController playerScript;
    public GameObject playerSpawnPos;

    //Player Variables
    public int HP;
    public int healthMax;
    public float playerSpeed;
    public float maxStam = 100.0f;
    public float dashCost = 20.0f;
    public float stamDrain = .1f;
    public float stamRegen = .1f;
    //Gun Variables

    //Next Level Bool Variables
    public bool level1;
    public bool level2;

    private void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(gameObject);
        }

        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<RyansPlayerController>();
        playerSpawnPos = GameObject.FindGameObjectWithTag("Player Spawn Pos");

        //GameManager.instance.player = player;
        //GameManager.instance.playerScript = playerScript;
        //GameManager.instance.playerSpawnPos = playerSpawnPos;
        StartCoroutine(Intialize());
    }
    IEnumerator Intialize()
    {
        yield return new WaitForSeconds(.5f);
        GameManager.instance.playerScript.HP = HP;
        GameManager.instance.playerScript.healthMax = healthMax;
        GameManager.instance.playerScript.playerSpeed = playerSpeed;
        GameManager.instance.playerScript.maxStam = maxStam;
        GameManager.instance.playerScript.dashCost = dashCost;
        GameManager.instance.playerScript.stamDrain = stamDrain;
        GameManager.instance.playerScript.stamRegen = stamRegen;

        GameManager.instance.level1 = level1;
        GameManager.instance.level2 = level2;
    }
}
