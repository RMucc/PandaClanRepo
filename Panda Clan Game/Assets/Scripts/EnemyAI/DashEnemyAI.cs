using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    [SerializeField] Animator anim;
    [SerializeField] int animSpeedTrans;



    float speed;
    bool playerInRange;
    bool dashing = false;
    bool exploding = false;


    // Start is called before the first frame update
    void Awake()
    {
        speed = agent.speed;
        StartCoroutine(Dash());
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
        model.material.color = stored;
        agent.speed = speed;
        agent.acceleration = agent.speed;


        if (!exploding)
        {
            //Roll Animation
            StartCoroutine(Dash());
        }
    }

    // Update is called once per frame
    void Update()
    {
        float animSpeed = agent.velocity.normalized.magnitude;
        anim.SetFloat("Speed", Mathf.Lerp(anim.GetFloat("Speed"), animSpeed, Time.deltaTime * animSpeedTrans));
        
        
        agent.SetDestination(GameManager.instance.player.transform.position);
        if (playerInRange && dashing && !exploding)
        {
            //Go into a ball
            Explode();
        }
        ShowRange();
    }

    public void TakeDamage(int amount)
    {
        if (dashing) // player can only damage enemy if they are in dash mode
        {
            HP -= amount;
            StartCoroutine(FlashRed());
        }
        if (HP <= 0 && alive)
        {
            if (effectGameGoal)
            {
                GameManager.instance.updateGameGoal(-1);
                GameManager.instance.updateEnemyAmount(-1);
            }
            // Add grenades to players inventory.
            alive = false;
            OnDeath();
            Explode();
        }
    }

    void Explode()
    {
        exploding = true;
        StartCoroutine(ExplodeFlash());
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
        }
    }

    IEnumerator ExplodeFlash()
    {
        for (int i = tickCount; i > 0; i--)
        {
            model.material.color = Color.yellow;
            yield return new WaitForSeconds(timefromYellowToBlack);
            model.material.color = Color.black;
            yield return new WaitForSeconds(timefromYellowToBlack);
        }

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRange);

        List<string> alreadyHit = new List<string>();
        foreach (Collider toDamage in colliders)
        {
            // Add Force
            //Rigidbody rb = toDamage.GetComponent<Rigidbody>();
            //if (rb != null)
            //{
            //    rb.AddExplosionForce(explosionForce, transform.position, explosionRange);
            //}
            if (toDamage.TryGetComponent<IDamage>(out IDamage dmg) && !alreadyHit.Contains(toDamage.gameObject.name))
            {
                dmg.TakeDamage(explosionDamage);
                alreadyHit.Add(toDamage.gameObject.name);
            }
        }
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.LookRotation(transform.forward));
        }
        if (effectGameGoal)
        {
            GameManager.instance.updateEnemyAmount(-1);
            GameManager.instance.updateGameGoal(-1);
        }

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

