using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RyansPlayerController : MonoBehaviour, IDamage
{
    //Player Movement Variables
    [SerializeField] CharacterController controller;
    [SerializeField] int HP;
    [SerializeField] float playerSpeed;
    [SerializeField] float playerSprintSpeed;
    [SerializeField] float playerStamina;
    [SerializeField] float playerStaminaRecover;
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
    private Vector3 dashMove;
    private int jumpCount;

    //Bool Values
    private bool groundedPlayer;
    private bool isShooting;
    private bool isSprinting;
    private bool isStaminaRecovering;

    //Original Values
    private int originalHP;
    private float originalPlayerVel;
    private float originalPlayerSpeed;
    private int originalHealthMax;
    private float originalPlayerStamina;

    //Dashing Variables
    [SerializeField] float dashSpeed;
    [SerializeField] float dashDuration;
    [SerializeField] float dashDebounce;
    [SerializeField] bool isDashing;
    private float dashCount;
    private float originalDashDebounce;
    [SerializeField] float dashCooldownTime;

    //Stat Variables for Player
    public int healthMax;

    // Start is called before the first frame update
    void Start()
    {
        originalHealthMax = healthMax;
        originalHP = HP;
        originalPlayerSpeed = playerSpeed;
        originalPlayerStamina = playerStamina;
        //originalPlayerVel = playerVel.x;
        originalDashDebounce = dashDebounce;
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
            playerVel.y = 0;
        }

        move = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;
        controller.Move(move * playerSpeed * Time.deltaTime);
        //Jump Input
        if(Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            playerVel.y = jumpHeight;
            jumpCount++;
        }
        //Sprint Input
        if(Input.GetKeyDown(KeyCode.LeftShift) && !isShooting && !isStaminaRecovering)
        {
            StartCoroutine(Sprint());
        }
        if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            playerSpeed = originalPlayerSpeed;
            isSprinting = false;
            StopCoroutine(Sprint());
        }
        //Forward Dash Input
        if (Input.GetKeyDown(KeyCode.W) && !isDashing)
        {
            //Set Dash Count to how many taps you want minus 1
            if (dashDebounce > 0 && dashCount == 1)
            {
                isDashing = true;
                StartCoroutine(DashForward());
            }
            else
            {
                dashDebounce = originalDashDebounce;
                dashCount += 1;
            }
        }
        //Backward Dash Input
        if (Input.GetKeyDown(KeyCode.S) && !isDashing)
        {
            //Set Dash Count to how many taps you want minus 1
            if (dashDebounce > 0 && dashCount == 1)
            {
                isDashing = true;
                StartCoroutine(DashBackward());
            }
            else
            {
                dashDebounce = originalDashDebounce;
                dashCount += 1;
            }
        }
        //Right Dash Input
        if (Input.GetKeyDown(KeyCode.D) && !isDashing)
        {
            //Set Dash Count to how many taps you want minus 1
            if (dashDebounce > 0 && dashCount == 1)
            {
                isDashing = true;
                StartCoroutine(DashRight());
            }
            else
            {
                dashDebounce = originalDashDebounce;
                dashCount += 1;
            }
        }
        //Left Dash Input
        if (Input.GetKeyDown(KeyCode.A) && !isDashing)
        {
            //Set Dash Count to how many taps you want minus 1
            if (dashDebounce > 0 && dashCount == 1)
            {
                StartCoroutine(DashLeft());
            }
            else
            {
                dashDebounce = originalDashDebounce;
                dashCount += 1;
            }
        }
        playerVel.y += gravity * Time.deltaTime;
        controller.Move(playerVel * Time.deltaTime);


        //Dash Debounce
        if(dashDebounce  > 0)
        {
            dashDebounce -= 1 * Time.deltaTime;
        }
        else
        {
            dashCount = 0;
        }
    }

    IEnumerator Sprint()
    {
        isSprinting = true;
        playerSpeed = playerSprintSpeed;
        yield return new WaitForSeconds(playerStamina);
        playerSpeed = originalPlayerSpeed;
        isSprinting = false;
        isStaminaRecovering = true;
        StartCoroutine(SprintRecover());
    }
    IEnumerator SprintRecover()
    {
        yield return new WaitForSeconds(playerStaminaRecover);
        isStaminaRecovering = false;
    }
    IEnumerator DashForward()
    {
        float startTime = Time.time;
        while (Time.time < startTime + dashDuration)
        {
            controller.Move(transform.forward * dashSpeed * Time.deltaTime);
            yield return null;
        }
        StartCoroutine(DashCoolDown());
    }

    IEnumerator DashBackward()
    {
        float startTime = Time.time;
        while (Time.time < startTime + dashDuration)
        {
            controller.Move(-transform.forward * dashSpeed * Time.deltaTime);
            yield return null;
        }
        StartCoroutine(DashCoolDown());
    }
    IEnumerator DashRight()
    {
        float startTime = Time.time;
        while(Time.time < startTime + dashDuration)
        {
            controller.Move(transform.right * dashSpeed * Time.deltaTime);
            yield return null;
        }
        StartCoroutine(DashCoolDown());

    }
    IEnumerator DashLeft()
    {
        float startTime = Time.time;
        while (Time.time < startTime + dashDuration)
        {
            controller.Move(-transform.right * dashSpeed * Time.deltaTime);
            yield return null;
        }
        StartCoroutine(DashCoolDown());
    }

    IEnumerator DashCoolDown()
    {
        yield return new WaitForSeconds(dashCooldownTime);
        isDashing = false;
    }

    IEnumerator Shoot()
    {
        isShooting = true;

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(.5f, .5f)), out hit, shootDistance))
        {
            IDamage dmg = hit.collider.GetComponent<IDamage>();
            if (dmg != null)
            {
                dmg.TakeDamage(shootDamage);
            }
        }
        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    public void TakeDamage(int amount)
    {
        HP -= amount;
        flashScreen();
        updatePlayerUI();

        if (HP <= 0)
        {
            healthMax -= 1;

            if(healthMax <= 0)
            {
                GameManager.instance.youSuck();
            }
            else
            {
                respawn();
            }
        }
    }

    public void respawn()
    {
        HP = originalHP;
        updatePlayerUI();

        controller.enabled = false;
        transform.position = GameManager.instance.playerSpawnPos.transform.position;
        controller.enabled = true;
    }

    public void updatePlayerUI() {
        GameManager.instance.HPBar.fillAmount = (float)HP / originalHP;
        //Ammo update when ammo is added
        //Stamina update
    }

    IEnumerator flashScreen()
    {
        GameManager.instance.damageScreen.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        GameManager.instance.damageScreen.SetActive(false);
    }
}
