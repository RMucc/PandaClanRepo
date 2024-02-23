using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DangerousObstacle : MonoBehaviour
{
    [SerializeField] int damageAmount;
    private int timer;
    private bool canDamage;
    Coroutine pushAndWait;
    // Start is called before the first frame update
    void Start()
    {
        canDamage = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.isTrigger)
        {
            return;
        }

        if(other.CompareTag("Player") && canDamage)
        {
            GameManager.instance.playerScript.TakeDamage(damageAmount);
            canDamage = false;
            pushAndWait = StartCoroutine(PushBackandWait());
        }
    }
    IEnumerator PushBackandWait()
    {
        GameManager.instance.PushBack();
        yield return new WaitForSeconds(timer);
        canDamage = true;
        StopCoroutine(pushAndWait);
    }
}
