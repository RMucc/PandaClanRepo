using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class BasicEnemyAI : BaseEnemyAI, IDamage
{
    [SerializeField] float attackRate;
    [SerializeField] float attackRange;
    [SerializeField] int attackDamage;
    [SerializeField] GameObject AttackPos;
    [SerializeField] GunStats gun;
    [SerializeField] bool effectGameGoal;
    [SerializeField] int AmmoAddedOnDeath;

    bool isAttacking;
    //bool playerInRange;

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
        isAttacking = false;
    }

    public void TakeDamage(int amount)
    {
        HP -= amount;
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
            GameManager.instance.playerPoints += 175;
            OnDeath();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            //playerInRange = true;
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
            //playerInRange = false;
        }
    }
}
