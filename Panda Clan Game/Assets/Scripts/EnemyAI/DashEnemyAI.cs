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
    // Create a Scriptable Object for a guns infomation and model which also uses the [CreateAssetMenu] to be able to save it as a pre-fab. A variable will store the enemys gun type here.
    [SerializeField] int AmmoAddedOnDeath = 5;
    [Header("Dash Variables\n")]
    [SerializeField] float TimeToWait;
    [SerializeField] float dashTime;
    [SerializeField] float speedMultiplier;
    [SerializeField] int explosionRange;
    [SerializeField] int explosionForce;
    [SerializeField] int explosionDamage;





    float speed;
    bool isShooting;
    bool playerInRange;
    bool dashing;
    Color storedColor;


    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.updateGameGoal(1);
        storedColor = model.material.color;
        speed = agent.speed;
        StartCoroutine(Dash());
        dashing = false;
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

        if (playerInRange && dashing)
        {
            Explode();
        }
        //Check to see if player is within range and once in range will explode and despawn.
    }

    IEnumerator shoot()
    {
        isShooting = true;

        yield return new WaitForSeconds(attackRate);
        isShooting = false;
    }

    public void TakeDamage(int amount)
    {
        if (dashing) // Can only damage enemy if they are in dash mode
        {
            HP -= amount;
            StartCoroutine(flashRed());
        }
        if (HP <= 0)
        {
            GameManager.instance.updateGameGoal(-1);
            Explode();
            Destroy(gameObject);
            //Intiate explosion on death and if player kills this enemy give player gernades.
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
            if (dmg != null)
            {
                dmg.TakeDamage(explosionDamage/2);
            }
        }
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        playerInRange = true;
    }

    private void OnTriggerExt(Collider other)
    {
        playerInRange = false;
    }
}
