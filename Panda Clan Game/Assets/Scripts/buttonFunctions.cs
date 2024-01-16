using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonFunctions : MonoBehaviour
{
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

    //Shop Buttons




    //Title Screen Buttons



    //Options Menu??
}
