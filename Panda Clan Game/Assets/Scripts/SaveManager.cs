using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;

    //Player Variables
    public int HP;
    public float playerSpeed;
    public float maxStam = 100.0f;
    public float dashCost = 20.0f;
    public float stamDrain = .1f;
    public float stamRegen = .1f;
    //Gun Variables
    public int activeWeapon;
    public GameObject gunOut;
    //Next Level Bool Variables
    public bool level1;
    public bool level2;
    public bool isNotLevel1;

    private void Awake()
    {
        // start of new code
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        // end of new code

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
