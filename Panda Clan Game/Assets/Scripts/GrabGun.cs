using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabGun : MonoBehaviour
{
    [SerializeField] GameObject doorLocation;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && GameManager.instance.Story1Bool == true)
        {
            GameManager.instance.InstantiateArrow(null, false);
            GameManager.instance.InstantiateArrow(doorLocation.transform);
            GameManager.instance.grabGunBool = true;
            Destroy(gameObject);
        }
    }
}
