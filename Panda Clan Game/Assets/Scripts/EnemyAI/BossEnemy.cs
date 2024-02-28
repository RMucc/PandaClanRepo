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
    [SerializeField] List<Transform> enemySpawnPositions;
    [SerializeField] List<GameObject> enemysToSpawn;
    int enemyLimit;

    [Header("----- Sentry Variables -----")]
    [SerializeField] List<Transform> sentryShootPositions;
    [SerializeField] GameObject sentryBullet;
    [SerializeField] float timeBetweenSentryShots;
    [SerializeField] float sentryResetTime;
    bool sentryFiringStarted = false;

    [Header("----- Teleport Variables -----")]
    [SerializeField] int teleportIntervalMax;
    [SerializeField] int teleportIntervalMin;
    [SerializeField] List<Transform> teleportPositions;
    [SerializeField] ParticleSystem chargingBackback;
    [SerializeField] ParticleSystem teleportParticles;
    [SerializeField] ParticleSystem afterTeleportParticles;
    [SerializeField] float desiredPhaseTime;
    [SerializeField] AnimationCurve alphaCurve;

    float elapsedTime;
    bool startTimer = false;
    bool canShoot;


    [Header("----- Gun Attributes -----")]
    [SerializeField] GunStats gun;
    [SerializeField] GameObject bullet;

    Vector3 playerDir;
    bool isShooting;
    bool playerInRange;
    float angleToPlayer;
    Vector3 startingPos;
    float stoppingDistOrig;

    bool doOnce = true;


    void Awake()
    {
        isShooting = false;
        StartCoroutine(TeleportAndEnemySpawn());
        enemyLimit = Random.Range(30, 60);
        canShoot = true;
    }

    IEnumerator TeleportAndEnemySpawn()
    {
        yield return new WaitForSeconds(.1f);
        if (doOnce)
        {
            GameManager.instance.updateGameGoal(1);
            doOnce = false;
        }
        yield return new WaitForSeconds(Random.Range(teleportIntervalMin, teleportIntervalMax));
        canShoot = false;
        agent.enabled = false;
        chargingBackback.Play();
        yield return new WaitForSeconds(chargingBackback.main.duration);
        teleportParticles.Play();
        yield return new WaitForSeconds(teleportParticles.main.duration / 2);
        foreach (Renderer model in modelList)
        {
            model.enabled = false;
        }
        yield return new WaitForSeconds(teleportParticles.main.duration / 2);
        foreach (Renderer model in modelList)
        {
            model.enabled = true;
        }
        int chosenPositions = Random.Range(0, teleportPositions.Count - 1);
        transform.position = teleportPositions[chosenPositions].position; // TELEPORTS HERE


        float startTime = Time.time;
        elapsedTime = 0;

        while (teleportParticles.particleCount != 0)
        {
            yield return new WaitForSeconds(.2f);
        }
        canShoot = true;
        agent.enabled = true;

        for (int i = 0; i < enemySpawnPositions[chosenPositions].childCount; i++)
        {
            if (GameManager.instance.enemyGoal <= enemyLimit)
            {
                GameManager.instance.updateGameGoal(1);
                Instantiate(enemysToSpawn[Random.Range(0, enemysToSpawn.Count - 1)], enemySpawnPositions[chosenPositions].GetChild(i).transform.position, enemySpawnPositions[chosenPositions].GetChild(i).transform.rotation);
            }
        }
        StartCoroutine(TeleportAndEnemySpawn());
    }

    void Update()
    {

        if (agent.enabled)
        {
            agent.SetDestination(GameManager.instance.player.transform.position);
        }

        if (anim && !isShooting)
        {
            anim.SetBool("isShooting", false);
        }
        else if (anim)
        {
            anim.SetBool("isShooting", true);
        }

        if (!isShooting && canShoot)
        {
            StartCoroutine(Shoot());
        }

        if (agent.enabled)
        {
            if (agent.remainingDistance < agent.stoppingDistance)
            {
                playerDir = GameManager.instance.player.transform.position - headPos.transform.position;
                FaceTarget();
            }
        }
    }

    IEnumerator Shoot()
    {
        isShooting = true;
        float x = Random.Range(-bulletSpread, bulletSpread);
        float y = Random.Range(-bulletSpread, bulletSpread);
        Vector3 direction = shootPos.transform.forward + new Vector3(x, y, 0);
        for (int i = 0; i < 3; i++)
        {
            Instantiate(bullet, shootPos.position, Quaternion.LookRotation(direction));
            yield return new WaitForSeconds(.5f);
        }
        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    public void TakeDamage(int amount)
    {
        HP -= amount;
        if (agent.enabled) { agent.SetDestination(GameManager.instance.player.transform.position); }
        StartCoroutine(FlashRed());
        if (HP <= origHP / 2 && !sentryFiringStarted)
        {
            sentryFiringStarted = true;
            StartCoroutine(FireSentrys());
        }
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

    IEnumerator FireSentrys()
    {
        foreach (Transform item in sentryShootPositions)
        {
            yield return new WaitForSeconds(timeBetweenSentryShots);
            Instantiate(sentryBullet, item.position, item.rotation);
        }
        yield return new WaitForSeconds(sentryResetTime);
        StartCoroutine(FireSentrys());
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
