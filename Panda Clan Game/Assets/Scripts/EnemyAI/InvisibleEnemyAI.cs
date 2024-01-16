using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class InvisbleEnemyAI : MonoBehaviour, IDamage
{
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] int HP;
    [SerializeField] float attackRate;

    // Create a Scriptable Object for a guns infomation and model which also uses the [CreateAssetMenu] to be able to save it as a pre-fab. A variable will store the enemys gun type here.
    [SerializeField] int AmmoAddedOnDeath = 5;


    bool isShooting;
    bool playerInRange;

    Color stored;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.updateGameGoal(1);
        stored = model.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(GameManager.instance.player.transform.position);
        //Check to see if player is within range and once in range will steal player ammo and despawn
    }

    IEnumerator shoot()
    {
        isShooting = true;

        yield return new WaitForSeconds(attackRate);
        isShooting = false;
    }

    public void TakeDamage(int amount)
    {
        HP -= amount;
        StartCoroutine(flashRed());
        if (HP <= 0)
        {
            GameManager.instance.updateGameGoal(-1);
            Destroy(gameObject);
        }
    }
    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = stored;
    }
}
