using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundObjectScript : MonoBehaviour
{
    [Header("-----  Variables  -----\n")]
    [SerializeField] float floatSpeedMin;
    [SerializeField] float floatSpeedMax;
    [SerializeField] float rotateSpeedMin;
    [SerializeField] float rotateSpeedMax;
    [SerializeField] float objectForwardMovementSpeed;
    [SerializeField] float objectUpMovementSpeed;
    float floatSpeed;
    float rotateSpeed;
    [SerializeField] bool startFloat;

    [Header("-----  Components  -----\n")]
    [SerializeField] Rigidbody rb;
    [SerializeField] SphereCollider sphereCollider;

    // Object to add

    Vector3 initialPosition;


    public void Start()
    {
        Physics.IgnoreLayerCollision(3, 6);
        if (rb)
        {
            rb.velocity = transform.forward * objectForwardMovementSpeed;
            rb.velocity += transform.up * objectUpMovementSpeed;
        }
        if (!transform.parent)
        {
            floatSpeed = Random.Range(floatSpeedMin, floatSpeedMax);
            rotateSpeed = Random.Range(rotateSpeedMin, rotateSpeedMax);
            initialPosition = transform.position;
        }
    }

    public void Update()
    {
        if (startFloat)
        {
            float floatOffset = Mathf.Sin(Time.time * floatSpeed) * 0.2f;
            Vector3 newPosition = initialPosition + new Vector3(0, floatOffset, 0);
            transform.position = newPosition;
            transform.Rotate(Time.deltaTime * rotateSpeed * Vector3.up);
        }
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // Give player object
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!startFloat)
        {
            if (collision.collider.gameObject.CompareTag("Floor"))
            {
                sphereCollider.enabled = false;
                initialPosition = transform.position;
                startFloat = true;
            }
        }
    }
}

