using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Story4Map : MonoBehaviour
{
    [SerializeField] GameObject ExitLocation;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.instance.Story4Bool = true;
            GameManager.instance.InstantiateArrow(null, false);
            GameManager.instance.InstantiateArrow(ExitLocation.transform);
            GameManager.instance.Story6.SetActive(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.instance.Story6.SetActive(false);
        }
    }
}
