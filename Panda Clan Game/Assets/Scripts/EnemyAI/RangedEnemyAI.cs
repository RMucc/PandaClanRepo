using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class RangedEnemyAI : BaseEnemyAI, IDamage
{
    [SerializeField] GameObject bullet;
    [SerializeField] Transform shootPos;
    [SerializeField] float shootRate;
    [SerializeField] GunStats gun;
    [SerializeField] bool effectGameGoal;
    [SerializeField] int AmmoAddedOnDeath;
    [SerializeField] int playerFaceSpeed;
    [SerializeField] float bulletSpread;
    [SerializeField] GameObject headPos;
    [SerializeField] Animator anim;

    Vector3 playerDir;
    bool isShooting;


    void Update()
    {
        agent.SetDestination(GameManager.instance.player.transform.position);
        if (!isShooting)
        {
            anim.SetFloat("Speed", agent.velocity.normalized.magnitude); //magnitude because we want the animations to be simplified for any direction
            
            StartCoroutine(Shoot());
        }

        if (agent.remainingDistance < agent.stoppingDistance)
        {
            playerDir = GameManager.instance.player.transform.position - headPos.transform.position;
            FaceTarget();
        }
    }

    IEnumerator Shoot()
    {
        float x = Random.Range(-bulletSpread, bulletSpread);
        float y = Random.Range(-bulletSpread, bulletSpread);
        Vector3 direction = shootPos.transform.forward + new Vector3(x, y, 0);
        isShooting = true;
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
            OnDeath();
        }
    }

    void FaceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(playerDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * playerFaceSpeed);
    }
}
