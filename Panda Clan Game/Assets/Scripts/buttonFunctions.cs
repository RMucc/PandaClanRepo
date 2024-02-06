using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonFunctions : MonoBehaviour
{
    public RyansPlayerController playerScript;

    //Pause Screen Buttons
    public void resume()
    {
        GameManager.instance.stateResume();
    }

    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GameManager.instance.stateResume();
    }

    public void Quit()
    {
        Application.Quit();

    }

    public void continueGame()
    {

        GameManager.instance.stateResume();
    }

    public void respawn()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GameManager.instance.stateResume();

    }

    public void Save()
    {
        SaveManager.instance.SaveData();
    }

    public void Load()
    {
        SaveManager.instance.LoadData();
    }

    //Shop Buttons
    public void buyShotgun()
    {
        //GameManager.instance.playerScript.AddDrops(GameManager.BulletType.Shotgun, )
    }



    //Title Screen Buttons



    //Options Menu??
}
