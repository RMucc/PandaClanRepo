using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;

    //public GameObject player;
    //public RyansPlayerController playerScript;
    //public GameObject playerSpawnPos;

    //Player Variables
    public int HP;
    public int healthMax;
    public float playerSpeed;
    public float maxStam;
    public float dashCost;
    public float stamDrain;
    public float stamRegen;
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

        //player = GameObject.FindGameObjectWithTag("Player");
        //playerScript = player.GetComponent<RyansPlayerController>();
        //playerSpawnPos = GameObject.FindGameObjectWithTag("Player Spawn Pos");

        //GameManager.instance.player = player;
        //GameManager.instance.playerScript = playerScript;
        //GameManager.instance.playerSpawnPos = playerSpawnPos;
        StartCoroutine(Intialize());
    }

    public void SaveData()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/playerInfo.dat");

        PlayerData data = new PlayerData();
        data.HP = HP;
        data.healthMax = healthMax;
        //data.playerSpeed = playerSpeed;
        //data.maxStam = maxStam;
        //data.dashCost = dashCost;
        //data.stamDrain = stamDrain;
        //data.stamRegen = stamRegen;
        data.level1 = level1;
        data.level2 = level2;

        bf.Serialize(file, data);
        file.Close();
    }

    public void LoadData()
    {
        if(File.Exists(Application.persistentDataPath + "/playerInfo.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);
            PlayerData data = (PlayerData)bf.Deserialize(file);
            file.Close();

            HP = data.HP;
            healthMax = data.healthMax;
            //playerSpeed = data.playerSpeed;
            //maxStam = data.maxStam;
            //dashCost = data.dashCost;
            //stamDrain = data.stamDrain;
            //stamRegen = data.stamRegen;
            level1 = data.level1;
            level2 = data.level2;
        }    
    }
    IEnumerator Intialize()
    {
        yield return new WaitForSeconds(.5f);
        LoadData();
        GameManager.instance.playerScript.HP = HP;
        GameManager.instance.playerScript.healthMax = healthMax;
        //GameManager.instance.playerScript.playerSpeed = playerSpeed;
        //GameManager.instance.playerScript.maxStam = maxStam;
        //GameManager.instance.playerScript.dashCost = dashCost;
        //GameManager.instance.playerScript.stamDrain = stamDrain;
        //GameManager.instance.playerScript.stamRegen = stamRegen;

        GameManager.instance.level1 = level1;
        GameManager.instance.level2 = level2;
    }
}

[Serializable]
class PlayerData
{
    //public GameObject player;
    //public RyansPlayerController playerScript;
    //public GameObject playerSpawnPos;

    //Player Variables
    public int HP;
    public int healthMax;
    public float playerSpeed;
    public float maxStam;
    public float dashCost;
    public float stamDrain;
    public float stamRegen;
    //Gun Variables

    //Next Level Bool Variables
    public bool level1;
    public bool level2;
}