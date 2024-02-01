using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]

public class GunStats : ScriptableObject
{
    public int shootDamage;
    public float shootRate;
    public int shootDistance;
    public float bulletSpread;
    public float reloadTime;
    public float timeBetweenShots;
    public int bulletsPerTap;
    public bool allowButtonHold;
    public int magazineSize;
    public int bulletsLeftInMag;
    public float cameraShakeDuration;
    public float cameraShakeMagnitude;
    public ParticleSystem bulletHitEffect;
    public GameObject bullet;
    public GameManager.BulletType bulletType;
    public float BulletExistanceTime;
    public GameObject weaponPrefabSkin;



    private void OnValidate()
    {
        bulletsLeftInMag = magazineSize;
    }
}
