using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject textActive;
    [SerializeField] GameObject noReload;
    [SerializeField] GameObject reload;
    [SerializeField] GameObject stamninaVisable;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject level1MenuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] GameObject test1; // OUT OF STAMINA
    [SerializeField] GameObject test2; // OUT OF AMMO
    [SerializeField] GameObject test3; // LOW HEALTH
    [SerializeField] GameObject enemyLeft;
    [SerializeField] GameObject livesCount;
    public Text totalLives;
    public GameObject damageScreen;
    public Image HPBar;
    public Image AMMOBar;
    public Image AMMOReserve;
    public Image StaminaWheel;

    public GameObject player;
    public RyansPlayerController playerScript;
    public GameObject playerSpawnPos;
    //public GameObject nextLevel2;

    public bool isPaused;
    public static bool level1;
    public static bool level2;
    public static bool isNotLevel1;
    public bool levelOne;
    public bool levelTwo;
    public int enemyCount;
    public int enemyGoal;

    public GameObject ArrowToNext;

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
        if (!isNotLevel1)
        {
            level1 = true;
        }
        levelOne = level1;
        levelTwo = level2;
        instance = this;
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<RyansPlayerController>();
        playerSpawnPos = GameObject.FindGameObjectWithTag("Player Spawn Pos");
    }
    #endregion

    #region UPDATE CODE
    void Update()
    {
        totalLives.text = "Lives: " + playerScript.healthMax;
        if (Input.GetButtonDown("Cancel") && menuActive == null)
        {
            statePaused();
            menuActive = menuPause;
            menuActive.SetActive(isPaused);
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

    #region GAME GOAL
    public void updateGameGoal(int amount)
    {
        enemyGoal += amount;
        //Should pull up a win menu that we can close out of so that we can move on to the next level
        if (enemyGoal <= 0 && level1 == true)
        {
            statePaused();
            try
            {
                if (ArrowToNext != null)
                {
                    ArrowToNext.SetActive(true);
                }
                menuActive = level1MenuWin;
                menuActive.SetActive(true);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("error: level1MenuWin not found");
            }
        }
        //Should pull up the win menu (No level after this so give option to restart or leave)
        if (enemyGoal <= 0 && level2 == true)
        {
            statePaused();
            menuActive = menuWin;
            menuActive.SetActive(true);
        }
    }
    public void updateEnemyAmount(int amount)
    {
        enemyCount += amount;
    }
    #endregion

    #region Update Scene Bool
    public void CallBeforeLoadingScene2()
    {
        level1 = false;
        level2 = true;
        isNotLevel1 = true;
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

    #region RELOAD VISUAL CODE
    public void showReload()
    {
        reload.SetActive(true);
        noReload.SetActive(false);
    }

    public void hideReload()
    {
        reload.SetActive(false);
        noReload.SetActive(true);
    }
    #endregion

    #region TEST

    public void showTEST1()
    {
        test1.SetActive(true);
        test2.SetActive(false);
        test3.SetActive(false);
    }
    public void hideTESTS()
    {
        test1.SetActive(false);
        test2.SetActive(false);
        test3.SetActive(false);
    }
    public void showTEST2()
    {
        test1.SetActive(false);
        test2.SetActive(true);
        test3.SetActive(false);
    }
    public void showTEST3()
    {
        test1.SetActive(false);
        test2.SetActive(false);
        test3.SetActive(true);
    }
    #endregion



}
