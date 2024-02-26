using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class BossEnemyAI : BaseEnemyAI, IDamage
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

    [Header("----- Stage variables -----")]
    [SerializeField] int teleportInterval;
    [SerializeField] List<Transform> teleportPositions;
    [SerializeField] List<Transform> enemySpawnPositions;
    [SerializeField] List<GameObject> enemysToSpawn;

    //[Header("----- Lazer Cannon Variables -----")]
    //[SerializeField] List<GameObject> LaserShootPositions;
    //[SerializeField] GameObject LazerBullet;


    //[Header("----- Gun Attributes -----")]
    [SerializeField] GunStats gun;
    [SerializeField] GameObject bullet;

    Vector3 playerDir;
    bool isShooting;
    bool playerInRange;
    float angleToPlayer;
    Vector3 startingPos;
    float stoppingDistOrig;


    void Awake()
    {
        isShooting = false;
        StartCoroutine(Teleport());
    }

    IEnumerator Teleport()
    {
        yield return new WaitForSeconds(teleportInterval);
        int chosenPositions = Random.Range(0, teleportPositions.Count - 1);
        transform.position = teleportPositions[chosenPositions].position;

        int enemyToSpawn = Random.Range(0, enemysToSpawn.Count - 1);
        for (int i = 0; i < enemySpawnPositions[chosenPositions].childCount; i++)
        {
            GameManager.instance.updateGameGoal(1);
            Instantiate(enemysToSpawn[enemyToSpawn], enemySpawnPositions[chosenPositions].GetChild(i).transform.position, enemySpawnPositions[chosenPositions].GetChild(i).transform.rotation);
        }
        StartCoroutine(Teleport());
    }
    void Update()
    {
        agent.SetDestination(GameManager.instance.player.transform.position);

        if (anim && !isShooting)
        {
            anim.SetBool("isShooting", false);
        }
        else if (anim)
        {
            anim.SetBool("isShooting", true);
        }

        if (!isShooting)
        {
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
        //if (HP > origHP/2)
        //{

        //}
        if (HP <= 0 && alive)
        {
            GameManager.instance.playerScript.AddDrops(gun, AmmoAddedOnDeath);
            if (effectGameGoal)
            {
                GameManager.instance.updateGameGoal(-GameManager.instance.enemyGoal);
            }
            alive = false;
            GameManager.instance.playerPoints += 200;
            OnDeath();
        }
    }

    //IEnumerator shootLazers()
    //{
    //    yield return new WaitForSeconds(1);
    //    In
    //}

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
