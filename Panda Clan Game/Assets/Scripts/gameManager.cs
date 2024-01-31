using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    [SerializeField] GameObject menuLose;
    [SerializeField] GameObject test1;
    [SerializeField] GameObject test2;
    [SerializeField] GameObject test3;
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
    public int enemyCount;
    public int enemyGoal;


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
        instance = this;
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<RyansPlayerController>();
        playerSpawnPos = GameObject.FindGameObjectWithTag("Player Spawn Pos");
    }
    #endregion

    #region UPDATE CODE
    void Update()
    {
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
        if (enemyGoal <= 0)
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

    #region LOSE youSuck()
    public void youSuck()
    {
        statePaused();
        menuActive = menuLose;
        menuActive.SetActive(true);
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
