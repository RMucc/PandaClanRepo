using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemyAI : MonoBehaviour, IDamage
{
    [Header("----- Components -----")]
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator anim;
    [SerializeField] Transform shootPos; //Position for him to shoot from
    [SerializeField] Transform headPos;

    [Header("----- Enemy Stats -----")]
    [Range(1, 200)][SerializeField] int HP;
    [SerializeField] int fov;
    [SerializeField] int fovshoot;
    [SerializeField] int targetFaceSpeed;
    [SerializeField] int animspeedTrans;
    [SerializeField] int roamDist;
    [SerializeField] int roamPauseTimer;

    [Header("----- Gun Attributes -----")]
    [SerializeField] GameObject bullet;
    [Range(0.1f, 2)][SerializeField] float shootrate;

    bool isShooting;
    bool playerInRange;
    Vector3 playerDir;
    float angleToPlayer;
    bool destChosen;
    Vector3 startingPos;
    float stoppingDistOrig;

    // Start is called before the first frame update
    void Start()
    {
        startingPos = transform.position;
        stoppingDistOrig = agent.stoppingDistance;
    }

    // Update is called once per frame
    void Update()
    {
        float animspeed = agent.velocity.normalized.magnitude;

        anim.SetFloat("Speed", Mathf.Lerp(anim.GetFloat("Speed"), animspeed, Time.deltaTime * animspeedTrans));


        if (playerInRange && !canSeePlayer())
        {
            StartCoroutine(roam());
        }

        else if (!playerInRange)
        {
            StartCoroutine(roam());
        }
    }

    IEnumerator roam()
    {
        if (agent.remainingDistance < 0.05f && !destChosen)
        {
            destChosen = true;
            agent.stoppingDistance = 0;
            yield return new WaitForSeconds(roamPauseTimer);
            destChosen = false;

            Vector3 randomPos = Random.insideUnitSphere * roamDist;
            randomPos += startingPos;

            NavMeshHit hit;
            NavMesh.SamplePosition(randomPos, out hit, roamDist, 1);
            agent.SetDestination(hit.position);
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
                StopCoroutine(roam());

                agent.SetDestination(GameManager.instance.player.transform.position);

                if (angleToPlayer <= fovshoot && !isShooting)
                    StartCoroutine(shoot());

                if (agent.remainingDistance < agent.stoppingDistance)
                {
                    faceTarget();
                }

                agent.stoppingDistance = stoppingDistOrig;

                return true;
            }

            Debug.Log(hit.transform.name);
        }

        return false;
    }

    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, transform.position.y, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * targetFaceSpeed);
    }


    IEnumerator shoot()
    {
        isShooting = true;
        Instantiate(bullet, shootPos.position, transform.rotation);
        yield return new WaitForSeconds(shootrate); //This must be after ^
        isShooting = false;
    }

    public void TakeDamage(int amount)
    {
        HP -= amount;
        agent.SetDestination(GameManager.instance.player.transform.position);

        if (HP <= 0)
        {
            GameManager.instance.updateGameGoal(-1);
            Destroy(gameObject);
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