using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Story1Script : MonoBehaviour
{
    [SerializeField] GameObject uziLocation;
    public void Start()
    {
        GameManager.instance.Story1Bool = false;
        GameManager.instance.grabGunBool = false;
    }
    public void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            GameManager.instance.Story1Bool = true;
            GameManager.instance.InstantiateArrow(null, false);
            GameManager.instance.InstantiateArrow(uziLocation.transform);
            GameManager.instance.Story1.SetActive(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            GameManager.instance.Story1.SetActive(false);
        }
    }
}
