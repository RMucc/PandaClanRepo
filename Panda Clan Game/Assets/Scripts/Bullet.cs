using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("-----  Components  -----\n")]
    [SerializeField] Rigidbody rb;

    [Header("----- Stats -----\n")]
    [SerializeField] int speed;
    [SerializeField] int destroyTime;
    [SerializeField] int damage;
    [SerializeField] bool enemyBullet;

    void Start()
    {
        rb.velocity = transform.forward * speed;
        Destroy(gameObject, destroyTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) { return; }

        IDamage damageable = other.GetComponent<IDamage>();

        if (damageable != null && other.gameObject.tag == "Player" && enemyBullet)
        {
            damageable.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
