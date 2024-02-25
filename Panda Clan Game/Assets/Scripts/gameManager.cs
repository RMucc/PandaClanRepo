using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Windows;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject textActive;
    [SerializeField] GameObject noReload;
    [SerializeField] GameObject reload;
    [SerializeField] GameObject stamninaVisable;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuScores;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuQuest;
    [SerializeField] GameObject levelMenuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] GameObject menuMain;
    [SerializeField] GameObject menuEnter;
    //[SerializeField] GameObject test1;
    //[SerializeField] GameObject test2;
    //[SerializeField] GameObject test3;
    [SerializeField] GameObject enemyLeft;
    public TextMeshProUGUI CurrCount;
    public ShopKeepController shopKeeper;
    public GameObject menuShop;
    public CanvasGroup mainInterface;
    public GameObject Interact;
    public GameObject Story1;
    public GameObject Story1Reminder;
    public GameObject Story2;
    public GameObject Story3;
    public GameObject Story4;
    public GameObject Story5;
    public GameObject Story6;
    public GameObject Story7;
    public GameObject Story8;
    public GameObject Story9;
    public GameObject Story10;
    public GameObject interfaceForStory;
    public event EventHandler OnLevel1Finished;
    GameObject currentArrow;
    [SerializeField] float pushDuration;
    [SerializeField] int pushBackSpeed;
    Coroutine pushBack;

    [Header("----- Point Tracker -----")]
    public int playerPoints;


    [Header("----- ShopKeeperVariables -----")]

    public Transform questPos;
    public bool inShop = false;

    [SerializeField] GameObject LivesObj;
    public Text waveCount;
    public Text weapSwitch;
    public GameObject damageScreen;
    public GameObject GodScreen;
    public Image HPBar;
    public Image AMMOBar;
    public Image StaminaWheel;
    public GameObject menuInteract;

    public GameObject player;
    public RyansPlayerController playerScript;
    public GameObject playerSpawnPos;
    public HighscoreTable topScore;
    private string nameInput;
    //public GameObject nextLevel2;

    public bool Story1Bool;
    public bool grabGunBool;
    public bool Story2Bool;
    public bool Story3Bool;
    public bool Story4Bool;
    public bool Story5Bool;
    public bool Story6Bool;
    public bool Story7Bool;
    public bool Story8Bool;
    public bool Story9Bool;
    public bool Story10Bool;

    public bool isPaused;
    public bool level1;
    public bool level2;
    public bool level3;
    public bool level4;
    public bool level5;
    public bool level6;
    public bool level7;
    //public bool isNotLevel1;
    //public bool levelOne;
    //public bool levelTwo;
    public int enemyCount;
    public int enemyGoal;

    public GameObject arrowToNext;
    private int temp;

    public enum BulletType
    {
        None,
        Shotgun,
        SMG,
        AR
    }


    #region AWAKE CODE
    void Awake()
    {
        playerPoints = 0; //Initialize points to show zero on start up
        temp = 5;
        if (instance == null)
        {
            //DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        //instance = this;
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<RyansPlayerController>();
        playerSpawnPos = GameObject.FindGameObjectWithTag("Player Spawn Pos");
        if (GameObject.FindGameObjectWithTag("ShopKeep"))
        {
            shopKeeper = GameObject.FindGameObjectWithTag("ShopKeep").GetComponent<ShopKeepController>();
        }
    }
    #endregion

    public void UpdateLivesUI()
    {
        Debug.Log(playerScript.healthMax);
        for (int i = LivesObj.transform.childCount - 1; i >= 0; i--)
        {
            if (i <= playerScript.healthMax - 1)
            {
                LivesObj.transform.GetChild(i).gameObject.SetActive(true);
            }
            else
            {
                LivesObj.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    #region UPDATE CODE
    void Update()
    {
        if (UnityEngine.Input.GetButtonDown("Cancel") && !menuActive)
        {
            statePaused();
            menuActive = menuPause;
            menuActive.SetActive(isPaused);
        }
        while (temp != 0)
        {
            //Debug.Log(temp);
            if (temp == 4)
            {
                SaveManager.instance.Load();
                playerScript.updatePlayerUI();
            }
            level1 = SaveManager.instance.level1;
            level2 = SaveManager.instance.level2;
            level3 = SaveManager.instance.level3;
            level4 = SaveManager.instance.level4;
            level5 = SaveManager.instance.level5;
            level6 = SaveManager.instance.level6;
            level7 = SaveManager.instance.level7;
            new WaitForSeconds(1);
            temp -= 1;
        }
    }
    #endregion

    #region PAUSE/RESUME MENU CODE
    public void statePaused() //Reuseable for shops and other screens I guess
    {
        isPaused = !isPaused;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void simpleResume()
    {
        isPaused = !isPaused;
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void stateResume()
    {
        isPaused = !isPaused;
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(false);
        menuActive = null;
    }
    #endregion


    public void InstantiateArrow(Transform transform = null, bool turnOn = true)
    {
        if (currentArrow) { Destroy(currentArrow); }
        if (turnOn)
        {
            currentArrow = Instantiate(arrowToNext, transform.position, Quaternion.identity);
            if (!currentArrow.activeSelf) { currentArrow.SetActive(true); }
        }
    }

    #region GAME GOAL
    public void updateGameGoal(int amount)
    {
        enemyGoal += amount;
        //Should pull up a win menu that we can close out of so that we can move on to the next level
        if (enemyGoal <= 0 && level1)
        {
            statePaused();
            try
            {
                OnLevel1Finished?.Invoke(this, EventArgs.Empty);
                menuActive = levelMenuWin;
                menuActive.SetActive(true);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning(e + "error: levelMenuWin not found");
            }
        }
        //Should pull up a win menu that we can close out of so that we can move on to the next level
        else if (enemyGoal <= 0 && level2 == true)
        {
            statePaused();
            try
            {
                /*if (arrowToNext && shopKeeper)
                {
                    shopKeeper.TurnOnWave();
                }*/
                menuActive = levelMenuWin;
                menuActive.SetActive(true);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning(e + "error: levelMenuWin not found");
            }
        }
        //Should pull up a win menu that we can close out of so that we can move on to the next level
        else if (enemyGoal <= 0 && level3 == true)
        {
            statePaused();
            try
            {
                /*if (arrowToNext && shopKeeper)
                {
                    shopKeeper.TurnOnWave();
                }*/
                menuActive = levelMenuWin;
                menuActive.SetActive(true);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning(e + "error: levelMenuWin not found");
            }
        }
        //Should pull up a win menu that we can close out of so that we can move on to the next level
        else if (enemyGoal <= 0 && level4 == true)
        {
            statePaused();
            menuActive = levelMenuWin;
            menuActive.SetActive(true);
        }
        //Should pull up a win menu that we can close out of so that we can move on to the next level
        else if (enemyGoal <= 0 && level5 == true)
        {
            statePaused();
            menuActive = levelMenuWin;
            menuActive.SetActive(true);
        }
        //Should pull up a win menu that we can close out of so that we can move on to the next level
        else if (enemyGoal <= 0 && level6 == true)
        {
            statePaused();
            menuActive = levelMenuWin;
            menuActive.SetActive(true);
        }
        //Should pull up a win menu because we won
        else if (enemyGoal <= 0 && level7 == true)
        {
            statePaused();
            menuActive = menuWin;
            menuActive.SetActive(true);
        }
        //Should pull up the win menu (No level after this so give option to restart or leave)
    }
    public void updateEnemyAmount(int amount)
    {
        enemyCount += amount;
    }
    #endregion

    #region Update Scene Bool
    public void CheckScene()
    {
        if (level1 == true)
        {
            CallBeforeLoadingScene2();
        }
        else if (level2 == true)
        {
            CallBeforeLoadingScene3();
        }
        else if (level3 == true)
        {
            CallBeforeLoadingScene4();
        }
        else if (level4 == true)
        {
            CallBeforeLoadingScene5();
        }
        else if (level5 == true)
        {
            CallBeforeLoadingScene6();
        }
        else if (level6 == true)
        {
            CallBeforeLoadingScene7();
        }
    }
    public void CallBeforeLoadingScene1()
    {
        SaveManager.instance.level1 = true;
        SaveManager.instance.level2 = false;
        SaveManager.instance.level3 = false;
        SaveManager.instance.level4 = false;
        SaveManager.instance.level5 = false;
        SaveManager.instance.level6 = false;
        SaveManager.instance.level7 = false;
    }
    public void CallBeforeLoadingScene2()
    {
        SaveManager.instance.level1 = false;
        SaveManager.instance.level2 = true;
        SaveManager.instance.level3 = false;
        SaveManager.instance.level4 = false;
        SaveManager.instance.level5 = false;
        SaveManager.instance.level6 = false;
        SaveManager.instance.level7 = false;
    }
    public void CallBeforeLoadingScene3()
    {
        SaveManager.instance.level1 = false;
        SaveManager.instance.level2 = false;
        SaveManager.instance.level3 = true;
        SaveManager.instance.level4 = false;
        SaveManager.instance.level5 = false;
        SaveManager.instance.level6 = false;
        SaveManager.instance.level7 = false;
    }
    public void CallBeforeLoadingScene4()
    {
        SaveManager.instance.level1 = false;
        SaveManager.instance.level2 = false;
        SaveManager.instance.level3 = false;
        SaveManager.instance.level4 = true;
        SaveManager.instance.level5 = false;
        SaveManager.instance.level6 = false;
        SaveManager.instance.level7 = false;
    }
    public void CallBeforeLoadingScene5()
    {
        SaveManager.instance.level1 = false;
        SaveManager.instance.level2 = false;
        SaveManager.instance.level3 = false;
        SaveManager.instance.level4 = false;
        SaveManager.instance.level5 = true;
        SaveManager.instance.level6 = false;
        SaveManager.instance.level7 = false;
    }
    public void CallBeforeLoadingScene6()
    {
        SaveManager.instance.level1 = false;
        SaveManager.instance.level2 = false;
        SaveManager.instance.level3 = false;
        SaveManager.instance.level4 = false;
        SaveManager.instance.level5 = false;
        SaveManager.instance.level6 = true;
        SaveManager.instance.level7 = false;
    }
    public void CallBeforeLoadingScene7()
    {
        SaveManager.instance.level1 = false;
        SaveManager.instance.level2 = false;
        SaveManager.instance.level3 = false;
        SaveManager.instance.level4 = false;
        SaveManager.instance.level5 = false;
        SaveManager.instance.level6 = false;
        SaveManager.instance.level7 = true;
    }
    #endregion

    #region LOSE youSuck()
    public void youSuck()
    {
        statePaused();
        menuActive = menuLose;
        menuActive.SetActive(true);
    }
    #endregion

    #region Next Level
    public void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    #endregion

    #region Stamina Visuals Code
    public void showStamina()
    {
        try
        {
            stamninaVisable.SetActive(true);
        }
        catch (System.Exception)
        {
            print("error: Variable staminaVisable might not be assigned");
        }
    }

    public void hideStamina()
    {
        try
        {
            stamninaVisable.SetActive(false);
        }
        catch (System.Exception)
        {
            print("error: Variable staminaVisable might not be assigned");
        }
    }
    #endregion

    #region ObstaclePushBack
    public void PushBack()
    {
        pushBack = StartCoroutine(PushBackIEnumerator());
    }
    IEnumerator PushBackIEnumerator()
    {

        float startTime = Time.time;
        while (Time.time < startTime + pushDuration)
        {
            playerScript.controller.Move(-playerScript.transform.forward * pushBackSpeed * Time.deltaTime);
            yield return null;
        }
        StopPushBack();
    }

    public void StopPushBack()
    {
        StopCoroutine(pushBack);
    }
    #endregion

    #region RELOAD VISUAL CODE
    public void showReload()
    {
        reload.SetActive(true);
    }

    public void hideReload()
    {
        reload.SetActive(false);
    }
    #endregion

    public void OpenOrCloseShopMenu(bool Open, Transform inShopCamPos = null)
    {
        menuActive = menuShop;
        menuActive.SetActive(Open);
        inShop = Open;
        Cursor.visible = Open;
        if (Open)
        {
            mainInterface.alpha = 0f;
            if (currentArrow) { InstantiateArrow(null, false); }
            CurrCount.text = playerScript.Currency.ToString();
            Cursor.lockState = CursorLockMode.Confined;
            Camera.main.transform.parent = inShopCamPos;
            Camera.main.transform.position = Vector3.zero;
            Camera.main.transform.rotation = Quaternion.Euler(0f, 80f, 0f);
        }
        else
        {
            mainInterface.alpha = 1f;
            Camera.main.transform.parent = player.transform;
            Camera.main.transform.SetPositionAndRotation(playerScript.cameraHolderPos.position, playerScript.cameraHolderPos.rotation);
            Cursor.lockState = CursorLockMode.Locked;
            shopKeeper.InteractTaskOpen = true;
        }
    }

    //public void ReadStringInput(string s)
    //{
    //    
    //    topScore.AddHighscoreEntry(playerPoints, s);
    //}

    #region TEST

    //public void showTEST1()
    //{
    //    test1.SetActive(true);
    //    test2.SetActive(false);
    //    test3.SetActive(false);
    //}
    //public void hideTESTS()
    //{
    //    test1.SetActive(false);
    //    test2.SetActive(false);
    //    test3.SetActive(false);
    //}
    //public void showTEST2()
    //{
    //    test1.SetActive(false);
    //    test2.SetActive(true);
    //    test3.SetActive(false);
    //}
    //public void showTEST3()
    //{
    //    test1.SetActive(false);
    //    test2.SetActive(false);
    //    test3.SetActive(true);
    //}/
    #endregion



}
