using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonFunctions : MonoBehaviour
{
    //Pause Screen Buttons
    public void resume()
    {
        gameManager.instance.stateResume();
    }

    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        gameManager.instance.stateResume();
    }

    public void Quit()
    {
        Application.Quit();

    }

    //Shop Buttons




    //Title Screen Buttons



    //Options Menu??
}
