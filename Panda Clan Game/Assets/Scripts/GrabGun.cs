using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabGun : MonoBehaviour
{
    [SerializeField] GameObject doorLocation;
    [SerializeField] GunStats SMG;
    public GameManager.BulletType bullet;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && GameManager.instance.Story1Bool == true)
        {
            GameManager.instance.grabGunBool = true;
            GameManager.instance.InstantiateArrow(null, false);
            GameManager.instance.InstantiateArrow(doorLocation.transform);
            bullet = GameManager.BulletType.SMG;
            GameManager.instance.playerScript.AddDrops(SMG, 30);
            Destroy(gameObject);
        }
    }
}
