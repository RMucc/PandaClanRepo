using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RangedEnemyAI : MonoBehaviour, IDamage
{
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] int HP;
    [SerializeField] GameObject bullet;
    [SerializeField] Transform shootPos;
    [SerializeField] float shootRate;
    [SerializeField] GunStats gun;
    [SerializeField] bool effectGameGoal;
    [SerializeField] int AmmoAddedOnDeath;
    [SerializeField] int playerFaceSpeed;
    [SerializeField] GameObject headPos;

    Vector3 playerDir;

    bool isShooting;
    bool playerInRange;

    Color stored;

    void Start()
    {
        GameManager.instance.updateGameGoal(1);
        stored = model.material.color;
    }

    void Update()
    {
        agent.SetDestination(GameManager.instance.player.transform.position);
        if (!isShooting)
        {
            StartCoroutine(shoot());
        }

        if (agent.remainingDistance < agent.stoppingDistance)
        {
            playerDir = GameManager.instance.player.transform.position - headPos.transform.position;
            faceTarget();
        }
    }

    IEnumerator shoot()
    {
        isShooting = true;
        Instantiate(bullet, shootPos.position, transform.rotation);
        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    public void TakeDamage(int amount)
    {
        HP -= amount;
        StartCoroutine(flashRed());
        if (HP <= 0)
        {
            GameManager.instance.playerScript.AddDrops(gun, AmmoAddedOnDeath);
            if (effectGameGoal)
            {
                GameManager.instance.updateGameGoal(-1);
            }
            Destroy(gameObject);
        }
    }
    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = stored;
    }

    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(playerDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * playerFaceSpeed);
    }
}
