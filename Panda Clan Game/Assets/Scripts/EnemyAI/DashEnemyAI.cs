using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DashEnemyAI : MonoBehaviour, IDamage
{
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] int HP;
    [SerializeField] float attackRate;
    [SerializeField] bool effectGameGoal;

    [Header("Dash Variables\n")]
    [SerializeField] float TimeToWait;
    [SerializeField] float dashTime;
    [SerializeField] float speedMultiplier;
    [SerializeField] int explosionRange;
    [SerializeField] int explosionForce;
    [SerializeField] int explosionDamage;


    float speed;
    bool playerInRange;
    bool dashing;
    bool exploding;
    Color storedColor;


    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.updateGameGoal(1);
        storedColor = model.material.color;
        speed = agent.speed;
        StartCoroutine(Dash());
        dashing = false;
        exploding = false;
    }

    IEnumerator Dash()
    {
        yield return new WaitForSeconds(TimeToWait);
        dashing = true;
        model.material.color = Color.black;
        agent.speed *= speedMultiplier;
        agent.acceleration = agent.speed;
        yield return new WaitForSeconds(dashTime);
        dashing = false;
        agent.velocity = Vector3.zero;
        model.material.color = storedColor;
        agent.speed = speed;
        agent.acceleration = agent.speed;
        StartCoroutine(Dash());
    }

    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(GameManager.instance.player.transform.position);
        if (playerInRange && dashing && !exploding) 
        {
            Explode();
        }
    }

    public void TakeDamage(int amount)
    {
        if (dashing) // player can only damage enemy if they are in dash mode
        {
            HP -= amount;
            StartCoroutine(flashRed());
        }
        if (HP <= 0)
        {
            if (effectGameGoal)
            {
                GameManager.instance.updateGameGoal(-1);
            }
            // Add grenades to players inventory.
            Explode();
            Destroy(gameObject);
        }
    }
    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = storedColor;
    }

    void Explode()
    {
        Debug.Log("Here");
        exploding = true;
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRange);

        foreach (Collider toDamage in colliders)
        {
            // Add Force
            //Rigidbody rb = toDamage.GetComponent<Rigidbody>();
            //if (rb != null)
            //{
            //    rb.AddExplosionForce(explosionForce, transform.position, explosionRange);
            //}
            IDamage dmg = toDamage.GetComponent<IDamage>();
            if (dmg != null && toDamage.gameObject.transform != this.transform)
            {
                Destroy(gameObject);
            }
        }
        Debug.Log("Here2");
        Destroy(gameObject);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            playerInRange = false;
        }
    }
}
