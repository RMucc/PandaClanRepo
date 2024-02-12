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


    void Update()
    {
        agent.SetDestination(GameManager.instance.player.transform.position);
    }


    public void TakeDamage(int amount)
    {
        HP -= amount;
        StartCoroutine(FlashRed());
        if (HP <= 0 && alive)
        {
            GameManager.instance.playerScript.AddDrops(null, Mathf.Abs(AmmoStolenOnDeath)); //Using absolute value to add instead of subtract
            if (effectGameGoal)
            {
                GameManager.instance.updateGameGoal(-1);
                GameManager.instance.updateEnemyAmount(-1);
            }
            alive = false;
            GameManager.instance.playerPoints += 120;
            OnDeath();
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.instance.playerScript.AddDrops(null, AmmoStolenOnDeath);
            if (effectGameGoal)
            {
                GameManager.instance.updateGameGoal(-1);
                GameManager.instance.updateEnemyAmount(-1);
            }
            if (other.TryGetComponent<IDamage>(out IDamage dmg))
            {
                dmg.TakeDamage(attackDamage);
            }
            Destroy(gameObject);
        }
    }
}
