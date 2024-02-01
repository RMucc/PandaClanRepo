using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class DashEnemyAI : BaseEnemyAI, IDamage
{
    [SerializeField] bool effectGameGoal;
    [SerializeField] int itemCountToDrop;

    [Header("Dash Variables\n")]
    [SerializeField] float TimeToWait;
    [SerializeField] float dashTime;
    [SerializeField] float speedMultiplier;


    [Header("Explosion Variables\n")]
    [SerializeField] int explosionRange;
    [SerializeField] int explosionForce;
    [SerializeField] int explosionDamage;
    [SerializeField] float timefromYellowToBlack;
    [SerializeField] int tickCount;
    [SerializeField] ParticleSystem explosionEffect;
    [SerializeField] bool showExplosionRange;
    [SerializeField] SphereCollider explosionRangeSP;


    float speed;
    bool playerInRange;
    bool readyToDestroy;
    bool dashing;
    bool exploding;
    bool alive;
    Color storedColor;


    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.updateEnemyAmount(1);
        storedColor = model.material.color;
        speed = agent.speed;
        StartCoroutine(Dash());
        dashing = false;
        exploding = false;
        alive = true;
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
        if (!exploding)
        {
            StartCoroutine(Dash());
        }
    }

    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(GameManager.instance.player.transform.position);
        if (playerInRange && dashing && !exploding)
        {
            Explode();
        }
        ShowRange();
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
            GameManager.instance.updateGameGoal(-1);
            GameManager.instance.updateEnemyAmount(-1);
            if (effectGameGoal)
            {
                //GameManager.instance.updateGameGoal(-1);
                //GameManager.instance.updateEnemyAmount(-1);
            }
            // Add grenades to players inventory.
            if (alive)
            {
                alive = false;
                OnDeath();
            }
            Explode();
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
        exploding = true;
        StartCoroutine(explodeFlash());
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

    IEnumerator explodeFlash()
    {
        for (int i = tickCount; i > 0; i--)
        {
            model.material.color = Color.yellow;
            yield return new WaitForSeconds(timefromYellowToBlack);
            model.material.color = Color.black;
            yield return new WaitForSeconds(timefromYellowToBlack);
        }

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
                dmg.TakeDamage(explosionDamage / 2); // Becuase the range goes over both the player's controller and capsule collider causing this to run twice.
            }
        }
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.LookRotation(transform.forward));
        }
        GameManager.instance.updateEnemyAmount(-1);
        GameManager.instance.updateGameGoal(-1);

        Destroy(gameObject);
    }

    void ShowRange()
    {
        if (explosionRangeSP != null)
        {
            explosionRangeSP.radius = explosionRange;
        }
    }
}

