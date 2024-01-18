using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour, IDamage
{
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] int HP;
    [SerializeField] float attackRate;
    [SerializeField] float attackRange;
    [SerializeField] int attackDamage;
    [SerializeField] GameObject AttackPos;
    [SerializeField] GunStats gun;
    [SerializeField] bool effectGameGoal;
    [SerializeField] int AmmoAddedOnDeath;

    bool isAttacking;
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
    }

    IEnumerator Attack()
    {
        isAttacking = true;
        Debug.DrawRay(AttackPos.transform.position, AttackPos.transform.forward * attackRange, Color.red, .5f);
        RaycastHit hit;
        if (Physics.Raycast(AttackPos.transform.position, AttackPos.transform.forward, out hit, attackRange))
        {
            IDamage dmg = hit.collider.GetComponent<IDamage>();
            if (dmg != null && hit.collider.gameObject.transform != this.transform)
            {
                dmg.TakeDamage(attackDamage);
            }
        }
        yield return new WaitForSeconds(attackRate);
        if (playerInRange)
        {
            StartCoroutine(Attack());
        }
        else
        {
            isAttacking = false;
        }
    }

    public void TakeDamage(int amount)
    {
        Debug.Log(amount);
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            playerInRange = true;
            if (!isAttacking)
            {
                StartCoroutine(Attack());
            }
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
