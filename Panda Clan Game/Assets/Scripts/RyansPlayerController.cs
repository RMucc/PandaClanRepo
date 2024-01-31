using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UIElements;

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
    int activeWeapon;
    int solveCrit;
    [SerializeField] bool useCrit;
    [SerializeField] int critRate; // The higher this number is the lower the crit chance since it's going to be based off of a random range of 1 to this int.
    [SerializeField] int critMultiplier; //The higher this number the lower the crit damage applied will be. Will crash if equal to 0.
    int damageHolder;

    [SerializeField] float popUpPosRand;
    [SerializeField] GameObject DamagePopUp;

    Dictionary<GameManager.BulletType, GunStats> gunList;

    bool readyToShoot;
    bool reloading;
    bool shooting;

    [SerializeField] int ARbulletsTotal;
    [SerializeField] int ShotgunbulletsTotal;
    [SerializeField] int SMGbulletsTotal;
    float ARbulletsTotalR;       //These are needed so I can use division for fillamount
    float ShotgunbulletsTotalR;  //These are needed so I can use division for fillamount
    float SMGbulletsTotalR;      //These are needed so I can use division for fillamount

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
        ARbulletsTotalR = ARbulletsTotal;
        ShotgunbulletsTotalR = ShotgunbulletsTotal;
        SMGbulletsTotalR = SMGbulletsTotal;
        originalHealthMax = healthMax;
        originalHP = HP;
        originalPlayerSpeed = playerSpeed;
        playerStam = maxStam;
        originalPlayerStamina = playerStam;
        originalDashDebounce = dashDebounce;
        readyToShoot = true;
        isShooting = false;
        AddDrops(gunToAdd, ammoToAdd);
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * gunList[bulletType].shootDistance, Color.green);
        stFX();
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
            if (sprintRecover != null)
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
                stUpdate();
                if (canDash)
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
                stUpdate();
                if (canDash)
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
                stUpdate();
                if (canDash)
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
                stUpdate();
                if (canDash)
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
        if (isStamRecovered)
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
        if (playerStam - dashCost > 0)
        {
            playerStam -= dashCost;
            //stUpdate();

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
        while (isSprinting)
        {
            if (playerStam > 0)
            {
                playerStam -= stamDrain;
                stUpdate();

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
        while (playerStam < maxStam)
        {
            playerStam += stamRegen;
            stUpdate();

            yield return new WaitForSeconds(.1f);
        }

        if (playerStam >= maxStam)
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
        stUpdate();
        yield return new WaitForSeconds(dashCooldownTime);
        isDashing = false;
    }
    #endregion

    #region Weapon System
    IEnumerator Reload()
    {
        Debug.Log("Reload Called");
        reloading = true;
        try
        {
            GameManager.instance.showReload();
        }
        catch (System.Exception)
        {
            print("error: Reload couldn't be shown");
        }
        yield return new WaitForSeconds(gunList[bulletType].reloadTime);

        switch (bulletType)
        {
            case GameManager.BulletType.Shotgun:
                if (ShotgunbulletsTotal >= gunList[bulletType].magazineSize)
                {
                    gunList[bulletType].bulletsLeftInMag = gunList[bulletType].magazineSize;
                    ShotgunbulletsTotal -= gunList[bulletType].magazineSize - gunList[bulletType].bulletsLeftInMag; //Had to add this to your reload
                    //Before, it was taking a flat rate of magazine size instead of the difference between what's left in current mag and full mag -J.G 
                }
                else if (ShotgunbulletsTotal > 0)
                {
                    gunList[bulletType].bulletsLeftInMag += ShotgunbulletsTotal;
                    ShotgunbulletsTotal = 0;
                }
                else
                {
                    print("Out of Ammo");
                    // Notify player they are fully out of shotgun Ammo;
                }
                break;
            case GameManager.BulletType.AR:
                if (ARbulletsTotal >= gunList[bulletType].magazineSize)
                {
                    gunList[bulletType].bulletsLeftInMag = gunList[bulletType].magazineSize;
                    ARbulletsTotal -= gunList[bulletType].magazineSize - gunList[bulletType].bulletsLeftInMag;
                }
                else if (ARbulletsTotal > 0)
                {
                    gunList[bulletType].bulletsLeftInMag += ARbulletsTotal;
                    ARbulletsTotal = 0;
                }
                else
                {
                    print("Out of Ammo");
                    // Notify player they are fully out of AR Ammo;
                }
                break;
            case GameManager.BulletType.SMG:
                if (SMGbulletsTotal >= gunList[bulletType].magazineSize)
                {
                    gunList[bulletType].bulletsLeftInMag = gunList[bulletType].magazineSize;
                    SMGbulletsTotal -= gunList[bulletType].magazineSize - gunList[bulletType].bulletsLeftInMag;
                }
                else if (SMGbulletsTotal > 0)
                {
                    gunList[bulletType].bulletsLeftInMag += SMGbulletsTotal;
                    SMGbulletsTotal = 0;
                }
                else
                {
                    print("Out of Ammo");
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
                solveCrit = Random.Range(1, critRate);
                Debug.Log("CritRate = " + critRate );
                if (useCrit && solveCrit == 1)
                {
                    //Debug.Log("CRITICAL DAMAGE!   -Continued\n" + "Added Damage should be: " + (gunList[bulletType].shootDamage * (int)critMultiplier).ToString() + "\n" + "Crit Damage should be: " + (gunList[bulletType].shootDamage + (gunList[bulletType].shootDamage * (int)critMultiplier)).ToString() + "\n" + "Normal Damage should be: " + gunList[bulletType].shootDamage.ToString());
                    damageHolder = gunList[bulletType].shootDamage + (gunList[bulletType].shootDamage / critMultiplier); 
                    dmg.TakeDamage(gunList[bulletType].shootDamage + (gunList[bulletType].shootDamage / critMultiplier));
                }
                else
                {
                    damageHolder = gunList[bulletType].shootDamage;
                    dmg.TakeDamage(gunList[bulletType].shootDamage);
                }
                //CreatePopUp(hit);
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

    public void CreatePopUp(RaycastHit hit)
    {
        Transform hitGameobjectTransform = hit.collider.gameObject.transform;

        //Randomize Position
        GameObject popUp = Instantiate(DamagePopUp, new Vector3(hitGameobjectTransform.position.x + Random.Range(0, popUpPosRand), hitGameobjectTransform.position.y + Random.Range(0, popUpPosRand), hitGameobjectTransform.position.z + Random.Range(0, popUpPosRand)), Quaternion.identity);
        TextMeshProUGUI temp = popUp.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        if (temp != null)
        {
            temp.text = damageHolder.ToString(); 
        }
        else
        {
            Debug.Log("Enemy Error: Not finding text gameobject");
        }
        BaseEnemyAI baseEnemyAI = hit.collider.gameObject.GetComponent<BaseEnemyAI>();
        temp.color = Color.red;
        if (baseEnemyAI != null)
        {
            if (baseEnemyAI.HP <= 0)
            {
                temp.fontSize += 4;
                temp.color = Color.black;

            }
            else if (useCrit && solveCrit == 1)
            {
                temp.fontSize += 2;
                temp.color = Color.yellow; // why doesnt this work although its passing through
                Debug.Log("Call Check"); // Call Check
            }
        }
        solveCrit = 0;
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
                    activeWeapon = 1;
                    updatePlayerUI();
                    break;
                case GameManager.BulletType.AR:
                    ARbulletsTotal += AmmoChange;
                    activeWeapon = 2;
                    updatePlayerUI();
                    break;
                case GameManager.BulletType.SMG:
                    SMGbulletsTotal += AmmoChange;
                    gunList[bulletType].bulletsLeftInMag = gunList[bulletType].magazineSize;
                    activeWeapon = 3;
                    updatePlayerUI();
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

            switch (activeWeapon)
            {
                case 1:
                    GameManager.instance.AMMOReserve.fillAmount = ShotgunbulletsTotal / ShotgunbulletsTotalR;
                    break;

                case 2:
                    GameManager.instance.AMMOReserve.fillAmount = ARbulletsTotal / ARbulletsTotalR;
                    break;

                case 3:
                    GameManager.instance.AMMOReserve.fillAmount = SMGbulletsTotal / SMGbulletsTotalR;
                    break;

            }

        }
        catch (System.Exception e)
        {
            print("error : missing HPBar");
        }
    }
    public void stUpdate()
    {

        GameManager.instance.StaminaWheel.fillAmount = playerStam / maxStam;
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
    public void stFX()
    {
        if (playerStam == maxStam)
        {
            GameManager.instance.hideStamina();
        }
        else
        {
            GameManager.instance.showStamina();
        }
    }

}
