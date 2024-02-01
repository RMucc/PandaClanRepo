using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class EnemyCountUI : MonoBehaviour
{
    public Text enemyCountUI;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        enemyCountUI.text = "Enemies: " + GameManager.instance.enemyCount;
    }
}
