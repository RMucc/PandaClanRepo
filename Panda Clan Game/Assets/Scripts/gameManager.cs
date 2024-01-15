using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;

    [SerializeField] GameObject menuActive; //Non tierd implementation. Will probably be changed later.
    //[SerializeField] GameObject menuTemp (Place holder for menu list??)
    [SerializeField] GameObject menuPause;

    public bool isPaused;

    //Place Holder
    //public bool isShopping;


    // Start is called before the first frame update
    void Awake()
    {
        instance = this;

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel") && menuActive == null)
        {
            statePaused();
            menuActive = menuPause;
            menuActive.SetActive(isPaused);
        }
    }

    public void statePaused() //Reuseable for shops and other screens
    {
        isPaused= !isPaused;
        Time.timeScale = 0;
        Cursor.visible= true;
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
}
