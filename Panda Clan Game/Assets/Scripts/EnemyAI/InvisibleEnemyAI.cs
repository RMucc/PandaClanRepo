using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class InvisbleEnemyAI : BaseEnemyAI, IDamage
{
    [SerializeField] int attackDamage;
    [SerializeField] bool effectGameGoal;
    [SerializeField] int AmmoStolenOnDeath;
    [SerializeField] int itemCountToDrop;
    bool alive;
    Color stored;

    void Start()
    {
        alive = true;
        GameManager.instance.updateEnemyAmount(1);
        stored = model.material.color;
    }

    void Update()
    {
        agent.SetDestination(GameManager.instance.player.transform.position);
        //Check to see if player is within range and once in range will steal player ammo and despawn
    }


    public void TakeDamage(int amount)
    {
        HP -= amount;
        StartCoroutine(flashRed());
        if (HP <= 0)
        {
            GameManager.instance.playerScript.AddDrops(null, Mathf.Abs(AmmoStolenOnDeath));
            if (effectGameGoal)
            {
                GameManager.instance.updateGameGoal(-1);
                GameManager.instance.updateEnemyAmount(-1);
            }
            if (alive)
            {
                alive = false;
                OnDeath();
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
            GameManager.instance.playerScript.AddDrops(null, AmmoStolenOnDeath);
            if (effectGameGoal)
            {
                GameManager.instance.updateGameGoal(-1);
            }
            IDamage dmg = other.gameObject.GetComponent<IDamage>();
            dmg.TakeDamage(attackDamage);
            Destroy(gameObject);
        }
    }
}
