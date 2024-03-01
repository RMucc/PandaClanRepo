using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class RangedEnemyAI : BaseEnemyAI, IDamage
{
    [Header("----- Components -----")]
    [SerializeField] Transform shootPos; //Position for him to shoot from
    [SerializeField] bool effectGameGoal;


    [Header("----- Enemy Stats -----")]
    [SerializeField] float shootRate;
    [SerializeField] int fovshoot;
    [SerializeField] float bulletSpread;
    [SerializeField] int targetFaceSpeed;
    [SerializeField] int animspeedTrans;
    [SerializeField] int AmmoAddedOnDeath;


    [Header("----- Gun Attributes -----")]
    [SerializeField] GameObject bullet;
    [SerializeField] GunStats gun;

    bool isShooting;
    bool playerInRange;
    Vector3 startingPos;
    float stoppingDistOrig;


    void Awake()
    {
        isShooting = false;
    }
    void Update()
    {
        agent.SetDestination(GameManager.instance.player.transform.position);

        if (playerInRange)
        {
            if (canSeePlayer())
            {
                if (alive)
                {
                    if (!isShooting && canSeePlayer())
                    {
                        if (anim) { anim.SetBool("isShooting", false); }
                        StartCoroutine(Shoot());
                        agent.stoppingDistance = stoppingDistOrig;
                    }
                    else
                    {
                        if (anim) { anim.SetBool("isShooting", true); }
                    }
                }
                else if (anim)
                {
                    // Play death animation or perform any other actions
                    anim.SetBool("isDead", true);
                }
            }
        }
    }



    IEnumerator Shoot()
    {
        isShooting = true;
        float x = Random.Range(-bulletSpread, bulletSpread);
        float y = Random.Range(-bulletSpread, bulletSpread);
        Vector3 direction = shootPos.transform.forward + new Vector3(x, y, 0);
        Instantiate(bullet, shootPos.position, Quaternion.LookRotation(direction));
        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    public void TakeDamage(int amount)
    {
        HP -= amount;
        agent.SetDestination(GameManager.instance.player.transform.position);
        StartCoroutine(FlashRed());
        if (HP <= 0 && alive)
        {
            GameManager.instance.playerScript.AddDrops(gun, AmmoAddedOnDeath);
            if (effectGameGoal)
            {
                GameManager.instance.updateGameGoal(-1);
                GameManager.instance.updateEnemyAmount(-1);
            }
            alive = false;
            GameManager.instance.playerPoints += 200;
            if (anim) anim.SetBool("isDead", true);
            OnDeath();
        }
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            agent.stoppingDistance = 0;
        }
    }
}
