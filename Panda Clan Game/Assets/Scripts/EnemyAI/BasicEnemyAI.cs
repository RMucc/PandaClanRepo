using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    [SerializeField] float popUpPosRand;
    [SerializeField] GameObject DamagePopUp;

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
        HP -= amount;
        StartCoroutine(flashRed());
        if (HP <= 0)
        {
            //temp.color = Color.black;
            GameManager.instance.playerScript.AddDrops(gun, AmmoAddedOnDeath);
            if (effectGameGoal)
            {
                GameManager.instance.updateGameGoal(-1);
            }
            Destroy(gameObject);
            //Add loot drops.
        }
        //CreatePopUp(new Vector3(transform.position.x + Random.Range(0, popUpPosRand), transform.position.y + Random.Range(0, popUpPosRand), transform.position.z + Random.Range(0, popUpPosRand)), amount.ToString());
    }

    public void CreatePopUp(Vector3 position, string text)
    {
        //Randomize Position
        //GameObject popUp = Instantiate(DamagePopUp, position, Quaternion.identity);
        //TextMeshProUGUI temp = popUp.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        //if (temp != null)
        //{
        //    temp.text = text;
        //}
        //else
        //{
        //    Debug.Log("Enemy Error: Not finding text gameobject");
        //}
        // Critical will be yellow
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
