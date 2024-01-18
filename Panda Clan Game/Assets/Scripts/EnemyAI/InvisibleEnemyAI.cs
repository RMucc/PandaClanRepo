using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class InvisbleEnemyAI : MonoBehaviour, IDamage
{
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] int HP;
    [SerializeField] int attackDamage;
    [SerializeField] bool effectGameGoal;
    [SerializeField] int AmmoStolenOnDeath;

    Color stored;

    void Start()
    {
        GameManager.instance.updateGameGoal(1);
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
