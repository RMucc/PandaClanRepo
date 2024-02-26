using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class RangedEnemyAI : BaseEnemyAI, IDamage
{
    [Header("----- Components -----")]
    [SerializeField] Animator anim;
    [SerializeField] Transform shootPos; //Position for him to shoot from
    [SerializeField] Transform headPos;
    [SerializeField] bool effectGameGoal;


    [Header("----- Enemy Stats -----")]
    [SerializeField] float shootRate;
    [SerializeField] int fov;
    [SerializeField] int fovshoot;
    [SerializeField] float bulletSpread;
    [SerializeField] int targetFaceSpeed;
    [SerializeField] int animspeedTrans;
    [SerializeField] int AmmoAddedOnDeath;
    [SerializeField] int playerFaceSpeed;


    [Header("----- Gun Attributes -----")]
    [SerializeField] GameObject bullet;
    [SerializeField] GunStats gun;

    Vector3 playerDir;
    bool isShooting;
    bool playerInRange;
    float angleToPlayer;
    Vector3 startingPos;
    float stoppingDistOrig;
    bool isDead = false; // Flag to track if the enemy is dead

    void Start()
    {
        anim = GetComponent<Animator>();
    }

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
                if (agent.remainingDistance < agent.stoppingDistance)
                {
                    playerDir = GameManager.instance.player.transform.position - headPos.transform.position;
                    FaceTarget();
                }

                if (!isDead)
                {
                    if (!isShooting)
                    {
                        anim.SetBool("isShooting", false);
                    }
                    else if (isShooting)
                    {
                        anim.SetBool("isShooting", true);
                    }

                    if (!isShooting)
                    {
                        StartCoroutine(Shoot());
                    }
                }

                else
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
            isDead = true;
            GameManager.instance.playerPoints += 200;
            anim.SetBool("isDead", true);
            OnDeath();
        }
    }

    bool canSeePlayer()
    {
        playerDir = GameManager.instance.player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(new Vector3(playerDir.x, 0, playerDir.z), transform.forward);

        Debug.Log(angleToPlayer);
        Debug.DrawRay(headPos.position, playerDir);

        RaycastHit hit;
        if (Physics.Raycast(headPos.position, playerDir, out hit))
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer <= fov)
            {

                agent.SetDestination(GameManager.instance.player.transform.position);

                if (angleToPlayer <= fovshoot && !isShooting)
                    StartCoroutine(Shoot());

                if (agent.remainingDistance < agent.stoppingDistance)
                {
                    FaceTarget();
                }

                agent.stoppingDistance = stoppingDistOrig;

                return true;
            }

            Debug.Log(hit.transform.name);
        }

        return false;
    }

    void FaceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(playerDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * playerFaceSpeed);
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
