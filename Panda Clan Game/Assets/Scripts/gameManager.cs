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
    public GameObject damageScreen;
    public Image HPBar;
    public Image AMMOBar;
    public Image StaminaWheel;
    
    public GameObject player;
    public RyansPlayerController playerScript;
    public GameObject playerSpawnPos;
    
    public bool isPaused;
    int enemyCount;

    
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
        enemyCount += amount;

        if (enemyCount <= 0)
        {
            statePaused();
            menuActive = menuWin;
            menuActive.SetActive(true);
        }
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
        stamninaVisable.SetActive(true);
    }

    public void hideStamina()
    {
        stamninaVisable.SetActive(false);
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
}
