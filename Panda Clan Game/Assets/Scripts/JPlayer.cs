using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JPlayer : MonoBehaviour
{
    //Player Movement Variables
    [SerializeField] CharacterController controller;
    [SerializeField] int HP;
    [SerializeField] float playerSpeed;
    [SerializeField] float jumpHeight;
    [SerializeField] float jumpMax;
    [SerializeField] float gravity;

    //Player Gun Variables
    [SerializeField] int shootDamage;
    [SerializeField] float shootRate;
    [SerializeField] int shootDistance;
    [SerializeField] GameObject cube;

    //Player Private Variables
    private Vector3 playerVel;
    private Vector3 move;
    private bool groundedPlayer;
    private int jumpCount;
    private bool isShooting;
    private int originalHP;

    // Start is called before the first frame update
    void Start()
    {
        originalHP = HP;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDistance, Color.green);

        if(Input.GetButton("Shoot") && !isShooting)
        {
            StartCoroutine(Shoot());
        }
        Movement();
    }

    void Movement()
    {
        groundedPlayer = controller.isGrounded;
        if(groundedPlayer)
        {
            jumpCount = 0;
        }

        move = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;
        controller.Move(move * playerSpeed * Time.deltaTime);

        if(Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            playerVel.y = jumpHeight;
            jumpCount++;
        }
        playerVel.y += gravity * Time.deltaTime;
        controller.Move(playerVel * Time.deltaTime);
    }

    IEnumerator Shoot()
    {
        isShooting = true;

        RaycastHit hit;
        if(Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(.5f, .5f)), out hit, shootDistance))
        {
            IDamage dmg = hit.collider.GetComponent<IDamage>();
            if(dmg != null)
            {
                dmg.TakeDamage(shootDamage);
            }
        }
        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }
}
