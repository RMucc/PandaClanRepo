using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;

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
        if(SceneManager.GetActiveScene().buildIndex == 0)
        {
            level1 = true;
            level2 = false;
            Debug.Log(level1);
        }
        else if(SceneManager.GetActiveScene().buildIndex == 1)
        {
            level1 = false;
            level2 = true;
        }
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(gameObject);
        }
        if(level1)
        {
            Debug.Log("Loading Default Data");
            LoadDefault();
        }
        else
        {
            LoadData();
        }
    }

    public void SaveData()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/playerInfo.dat");

        PlayerData data = new PlayerData();
        data.HP = GameManager.instance.playerScript.HP;
        data.healthMax = GameManager.instance.playerScript.healthMax;
        data.playerSpeed = GameManager.instance.playerScript.originalPlayerSpeed;
        //data.maxStam = maxStam;
        //data.dashCost = dashCost;
        //data.stamDrain = stamDrain;
        //data.stamRegen = stamRegen;

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

            //Load Data into SaveManager Variables
            HP = data.HP;
            healthMax = data.healthMax;
            playerSpeed = data.playerSpeed;
            //maxStam = data.maxStam;
            //dashCost = data.dashCost;
            //stamDrain = data.stamDrain;
            //stamRegen = data.stamRegen;

            //Load Data into GameManager Variables
            StartCoroutine(Intialize());
        }    
    }

    public void LoadDefault()
    {
        HP = 100;
        healthMax = 3;
        playerSpeed = 10;
        dashCost = 20;
        stamDrain = 5;
        stamRegen = 5;
        SetData();
    }

    public void SetData()
    {
        Debug.Log("In SetData");
        GameManager.instance.playerScript.HP = HP;
        GameManager.instance.playerScript.healthMax = healthMax;
        //GameManager.instance.playerScript.playerSpeed = playerSpeed;
        //GameManager.instance.playerScript.maxStam = maxStam;
        //GameManager.instance.playerScript.dashCost = dashCost;
        //GameManager.instance.playerScript.stamDrain = stamDrain;
        //GameManager.instance.playerScript.stamRegen = stamRegen;
        GameManager.instance.level1 = level1;
        Debug.Log(GameManager.instance.level1);
        GameManager.instance.level2 = level2;
    }

    IEnumerator Intialize()
    {
        yield return new WaitForSeconds(.5f);
        //LoadData();
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