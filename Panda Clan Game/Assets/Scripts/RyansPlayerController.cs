using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.EventSystems;

public class RyansPlayerController : MonoBehaviour, IDamage
{
    [Header("Debugging")]
    [SerializeField] GunStats gunToAdd;
    [SerializeField] int ammoToAdd;


    [Header("Player Movement Variables\n")]
    //Player Movement Variables
    [SerializeField] CharacterController controller;
    [SerializeField] int HP;
    [SerializeField] float playerSpeed;
    [SerializeField] float jumpHeight;
    [SerializeField] float jumpMax;
    [SerializeField] float gravity;
    Coroutine sprint;
    Coroutine sprintRecover;

    [Header("Player Stamina/Regen Variables")]
    [SerializeField] float maxStam = 100.0f;
    [SerializeField] float dashCost = 20.0f;
    public bool isSprinting = false;
    public bool isStamRecovered = true;
    [Range(0, 50)][SerializeField] float stamDrain = .1f;
    [Range(0, 50)][SerializeField] float stamRegen = .1f;

    [SerializeField] float playerSprintSpeed;
    [SerializeField] float playerStam;
    //[SerializeField] float playerStamRecover;

    [Header("Player Gun Variables\n")]
    //Player Gun Variables
    [SerializeField] int shootDamage;
    [SerializeField] float shootRate;
    [SerializeField] int shootDistance;
    [SerializeField] float bulletSpread;
    [SerializeField] float reloadTime;
    [SerializeField] int bulletsPerTap;
    [SerializeField] bool allowButtonHold;
    [SerializeField] int magazineSize;
    [SerializeField] int bulletsLeftInMag;
    [SerializeField] ParticleSystem bulletHitEffect;
    [SerializeField] GameObject bullet;
    float timeBetweenShots; // for burst weapons
    [SerializeField] GameManager.BulletType bulletType;
    ParticleSystem EffectHolder;
    float cameraShakeDuration; // for camera shake
    float cameraShakeMagnitude; // for camera shake


    Dictionary<GameManager.BulletType, GunStats> gunList;

    bool readyToShoot;
    bool reloading;
    bool shooting;

    [SerializeField] int ARbulletsTotal;
    [SerializeField] int ShotgunbulletsTotal;
    [SerializeField] int SMGbulletsTotal;

    [SerializeField] Transform shootPos;
    [SerializeField] CameraController cameraController;

    [Header("Player Private Variables\n")]
    //Player Private Variables
    private Vector3 playerVel;
    private Vector3 move;
    private Vector3 dashMove;
    private int jumpCount;

    [Header("Bool Values\n")]
    //Bool Values
    private bool groundedPlayer;
    private bool isShooting;

    [Header("Original Values\n")]
    //Original Values
    private int originalHP;
    private float originalPlayerVel;
    private float originalPlayerSpeed;
    private int originalHealthMax;
    private float originalPlayerStamina;

    [Header("Dashing Values\n")]
    //Dashing Variables
    [SerializeField] float dashSpeed;
    [SerializeField] float dashDuration;
    [SerializeField] float dashDebounce;
    [SerializeField] bool isDashing;
    private float dashCount;
    private float originalDashDebounce;
    [SerializeField] float dashCooldownTime;
    private bool canDash = true;

    [Header("Stats Variables for Player\n")]
    //Stat Variables for Player
    public int healthMax;

    // Start is called before the first frame update
    void Start()
    {
        gunList = new Dictionary<GameManager.BulletType, GunStats>();
        originalHealthMax = healthMax;
        originalHP = HP;
        originalPlayerSpeed = playerSpeed;
        originalPlayerStamina = playerStam;
        //originalPlayerVel = playerVel.x;
        originalDashDebounce = dashDebounce;
        readyToShoot = true;
        isShooting = false;
        AddDrops(gunToAdd, ammoToAdd);
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * gunList[bulletType].shootDistance, Color.green);

        if (gunList[bulletType].allowButtonHold && !isShooting)
        {
            shooting = Input.GetButton("Shoot");
        }
        else
        {
            shooting = Input.GetButtonDown("Shoot");
        }

        if (Input.GetButtonDown("Reload") && gunList[bulletType].bulletsLeftInMag < gunList[bulletType].magazineSize && !reloading)
        {
            StartCoroutine(Reload());
        }

        if (readyToShoot && shooting && !reloading && gunList[bulletType].bulletsLeftInMag > 0)
        {
            for (int i = gunList[bulletType].bulletsPerTap; i > 0; i--)
            {
                StartCoroutine(Shoot());
            }
        }
        else if (gunList[bulletType].bulletsLeftInMag == 0 && !reloading)
        {
            StartCoroutine(Reload());
        }
        //if (Input.GetButton("Shoot"))
        //{
        //    StartCoroutine(Shoot());
        //}
        Movement();
    }

    void Movement()
    {
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer)
        {
            jumpCount = 0;
            playerVel.y = 0;
        }

        move = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;
        controller.Move(move * playerSpeed * Time.deltaTime);
        //Jump Input
        #region Jump Input
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            playerVel.y = jumpHeight;
            jumpCount++;
        }
        #endregion
        //Sprint Input
        #region Sprint Input
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isShooting && isStamRecovered)
        {
            if(sprintRecover != null)
            {
                StopCoroutine(sprintRecover);
            }
            if (playerStam > 0)
            {
                isSprinting = true;
                Sprinting();
            }
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            isSprinting = false;
            playerSpeed = originalPlayerSpeed;
            sprintRecover = StartCoroutine(SprintRecover());
        }
        #endregion
        //Forward Dash Input
        #region Forward Dash Input
        if (Input.GetKeyDown(KeyCode.W) && !isDashing && isStamRecovered)
        {
            //Set Dash Count to how many taps you want minus 1
            if (dashDebounce > 0 && dashCount == 1)
            {
                StamDash();
                if(canDash)
                {
                    isDashing = true;
                    StartCoroutine(DashForward());
                    sprintRecover = StartCoroutine(SprintRecover());
                }
            }
            else
            {
                dashDebounce = originalDashDebounce;
                dashCount += 1;
            }
        }
        #endregion
        //Backward Dash Input
        #region Backward Dash Input
        if (Input.GetKeyDown(KeyCode.S) && !isDashing && isStamRecovered)
        {
            //Set Dash Count to how many taps you want minus 1
            if (dashDebounce > 0 && dashCount == 1)
            {
                StamDash();
                if(canDash)
                {
                    isDashing = true;
                    StartCoroutine(DashBackward());
                    sprintRecover = StartCoroutine(SprintRecover());
                }
            }
            else
            {
                dashDebounce = originalDashDebounce;
                dashCount += 1;
            }
        }
        #endregion
        //Right Dash Input
        #region Right Dash Input
        if (Input.GetKeyDown(KeyCode.D) && !isDashing && isStamRecovered)
        {
            //Set Dash Count to how many taps you want minus 1
            if (dashDebounce > 0 && dashCount == 1)
            {
                StamDash();
                if(canDash)
                {
                    isDashing = true;
                    StartCoroutine(DashRight());
                    sprintRecover = StartCoroutine(SprintRecover());
                }
            }
            else
            {
                dashDebounce = originalDashDebounce;
                dashCount += 1;
            }
        }
        #endregion
        //Left Dash Input
        #region Left Dash Input
        if (Input.GetKeyDown(KeyCode.A) && !isDashing && isStamRecovered)
        {
            //Set Dash Count to how many taps you want minus 1
            if (dashDebounce > 0 && dashCount == 1)
            {
                StamDash();
                if(canDash)
                {
                    isDashing = true;
                    StartCoroutine(DashLeft());
                    sprintRecover = StartCoroutine(SprintRecover());
                }
            }
            else
            {
                dashDebounce = originalDashDebounce;
                dashCount += 1;
            }
        }
        #endregion
        playerVel.y += gravity * Time.deltaTime;
        controller.Move(playerVel * Time.deltaTime);

        //Dash Debounce (Handles how long you have to press the specific key twice to dash)
        #region Dash Debounce
        if (dashDebounce > 0)
        {
            dashDebounce -= .5f * Time.deltaTime;
        }
        else
        {
            dashCount = 0;
        }
        #endregion
    }
    //Sprinting Function (Will Call Sprint IEnumerator Function in here)
    #region Sprinting Function
    public void Sprinting()
    {
        if(isStamRecovered)
        {
            isSprinting = true;
            playerSpeed = playerSprintSpeed;
            sprint = StartCoroutine(Sprint());
        }
    }
    #endregion
    //StamDash Cost Function (Whenever dashing, will take away stam)
    #region StamDash Cost Function
    public void StamDash()
    {
        if (playerStam - dashCost >= 0)
        {
            playerStam -= dashCost;
            //Make UI stam bar equal to playerStam here

            canDash = true;
        }
        else
        {
            canDash = false;
        }
    }
    #endregion
    //Sprint IEnumerator (Handles the Stam drain when sprinting)
    #region Sprint IEnumerator
    IEnumerator Sprint()
    {
        yield return new WaitForSeconds(0);
        while(isSprinting)
        {
            if(playerStam > 0)
            {
                playerStam -= stamDrain;
                //Make UI stam bar equal to playerStam here

            }
            else
            {
                playerSpeed = originalPlayerSpeed;
                isStamRecovered = false;
                isSprinting = false;
                StopCoroutine(sprint);
            }
            yield return new WaitForSeconds(.1f);
        }
    }
    #endregion
    //Sprint Recover IEnumerator (Handles Stam regen when not sprinting or dashing)
    #region Sprint Recover IEnumerator
    IEnumerator SprintRecover()
    {
        yield return new WaitForSeconds(0);
        while(playerStam < maxStam)
        {
            playerStam += stamRegen;
            //Make UI stam bar equal to playerStam here

            yield return new WaitForSeconds(.1f);
        }
        
        if(playerStam >= maxStam)
        {
            isStamRecovered = true;
            StopCoroutine(sprintRecover);
        }
    }
    #endregion
    //Dash Forward IEnumerator (Handles moving the player foward dash)
    #region Dash Forward IEnumerator
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
    #endregion
    //Dash Backward IEnumrator (Handles moving the player backward dash)
    #region Dash Backward IEnumerator
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
    #endregion
    //Dash Right IEnumerator (Handles moving the player right dash)
    #region Dash Right IEnumerator
    IEnumerator DashRight()
    {
        float startTime = Time.time;
        while (Time.time < startTime + dashDuration)
        {
            controller.Move(transform.right * dashSpeed * Time.deltaTime);
            yield return null;
        }
        StartCoroutine(DashCoolDown());

    }
    #endregion
    //Dash Left IEnumerator (Handles moving the player left dash)
    #region Dash Left IEnumerator
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
    #endregion
    //Dash CoolDown IEnumerator Handles the cooldown between dashing)
    #region Dash Cooldown IEnumerator
    IEnumerator DashCoolDown()
    {
        yield return new WaitForSeconds(dashCooldownTime);
        isDashing = false;
    }
    #endregion

    #region Weapon System
    IEnumerator Reload()
    {
        reloading = true;
        GameManager.instance.showReload();
        yield return new WaitForSeconds(gunList[bulletType].reloadTime);

        switch (bulletType)
        {
            case GameManager.BulletType.Shotgun:
                if (ShotgunbulletsTotal >= gunList[bulletType].magazineSize)
                {
                    gunList[bulletType].bulletsLeftInMag = gunList[bulletType].magazineSize;
                    ShotgunbulletsTotal -= gunList[bulletType].magazineSize;
                }
                else if (ShotgunbulletsTotal > 0)
                {
                    gunList[bulletType].bulletsLeftInMag += ShotgunbulletsTotal;
                    ShotgunbulletsTotal = 0;
                }
                else
                {
                    // Notify player they are fully out of shotgun Ammo;
                }
                break;
            case GameManager.BulletType.AR:
                if (ARbulletsTotal >= gunList[bulletType].magazineSize)
                {
                    gunList[bulletType].bulletsLeftInMag = gunList[bulletType].magazineSize;
                    ARbulletsTotal -= gunList[bulletType].magazineSize;
                }
                else if (ARbulletsTotal > 0)
                {
                    gunList[bulletType].bulletsLeftInMag += ARbulletsTotal;
                    ARbulletsTotal = 0;
                }
                else
                {
                    // Notify player they are fully out of AR Ammo;
                }
                break;
            case GameManager.BulletType.SMG:
                if (SMGbulletsTotal >= gunList[bulletType].magazineSize)
                {
                    gunList[bulletType].bulletsLeftInMag = gunList[bulletType].magazineSize;
                    SMGbulletsTotal -= gunList[bulletType].magazineSize;
                }
                else if (SMGbulletsTotal > 0)
                {
                    gunList[bulletType].bulletsLeftInMag += SMGbulletsTotal;
                    SMGbulletsTotal = 0;
                }
                else
                {
                    // Notify player they are fully out of SMG Ammo;
                }
                break;
        }
        gunList[bulletType].bulletsLeftInMag = gunList[bulletType].magazineSize;
        reloading = false;
        GameManager.instance.hideReload();
        updatePlayerUI();
    }

    IEnumerator Shoot()
    {
        isShooting = true;
        readyToShoot = false;
        float x = Random.Range(-gunList[bulletType].bulletSpread, gunList[bulletType].bulletSpread);
        float y = Random.Range(-gunList[bulletType].bulletSpread, gunList[bulletType].bulletSpread);
        Vector3 direction = Camera.main.transform.forward + new Vector3(x, y, 0);
        GameObject _bullet = Instantiate(gunList[bulletType].bullet, shootPos.transform.position, Quaternion.LookRotation(direction));
        Debug.DrawRay(shootPos.transform.position, direction * gunList[bulletType].shootDistance, Color.red, 1f);
        RaycastHit hit;
        if (Physics.Raycast(shootPos.transform.position, direction, out hit, gunList[bulletType].shootDistance))
        {
            EffectHolder = new ParticleSystem();
            EffectHolder = Instantiate(gunList[bulletType].bulletHitEffect, hit.point, Quaternion.Euler(0, 180, 0));
            Destroy(EffectHolder, 2);
            IDamage dmg = hit.collider.GetComponent<IDamage>();
            if (dmg != null && hit.collider.gameObject.transform != this.transform)
            {
                dmg.TakeDamage(gunList[bulletType].shootDamage);
            }
        }
        //StartCoroutine(cameraController.Shake(cameraShakeDuration, cameraShakeMagnitude)); For potential future camera shake
        Destroy(_bullet, gunList[bulletType].BulletExistanceTime); // Destorying the "bullet" after a certain amount of time dependent on the variable in the gunStats variable
        yield return new WaitForSeconds(gunList[bulletType].shootRate);
        isShooting = false;
        readyToShoot = true;
        Mathf.Clamp(gunList[bulletType].bulletsLeftInMag--, 0, 300);
        updatePlayerUI();
    }

    public void AddDrops(GunStats gun = null, int AmmoChange = 0)
    {
        if (gun != null && !gunList.ContainsKey(gun.bulletType))
        {
            gunList.Add(gun.bulletType, gun);
            bulletType = gun.bulletType;
        }

        if (AmmoChange != 0)
        {
            switch (bulletType)
            {
                case GameManager.BulletType.Shotgun:
                    ShotgunbulletsTotal += AmmoChange;
                    break;
                case GameManager.BulletType.AR:
                    ARbulletsTotal += AmmoChange;
                    break;
                case GameManager.BulletType.SMG:
                    SMGbulletsTotal += AmmoChange;
                    break;
            }
        }
    }
    #endregion

    public void TakeDamage(int amount)
    {
        HP -= amount;
        StartCoroutine(flashScreen());
        updatePlayerUI();

        if (HP <= 0)
        {
            healthMax -= 1;

            if (healthMax <= 0)
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

    public void updatePlayerUI()
    {
        try // for debugging purposes
        {
            GameManager.instance.HPBar.fillAmount = (float)HP / originalHP;
            GameManager.instance.AMMOBar.fillAmount = gunList[bulletType].bulletsLeftInMag / (float)gunList[bulletType].magazineSize;
        }
        catch (System.Exception e)
        {
            print("error : missing HPBar");
        }
        //Ammo update when ammo is added

        //Stamina update
    }

    IEnumerator flashScreen()
    {
        try
        {
            GameManager.instance.damageScreen.SetActive(true);
        }
        catch (System.Exception)
        {
            print("error: damageScreen missing from GameManager");
        }

        yield return new WaitForSeconds(0.2f);

        try
        {
            GameManager.instance.damageScreen.SetActive(false);
        }
        catch (System.Exception)
        {
            print("error: damageScreen missing from GameManager");
        }
    }

}
