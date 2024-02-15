using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UIElements;
using Unity.VisualScripting;
using System.ComponentModel;

//This is a script for my own player I can use to test in my scene. I will make it a PreFab so that other's
// can use this player if they so choose
public class JoshPlayerController : MonoBehaviour, IDamage
{
    #region Player Debugging
    [SerializeField] GunStats gunToAdd;
    [SerializeField] int ammoToAdd;
    public bool invul;
    #endregion

    #region Player Movement Variables
    [Header("Player Movement Variables\n")]
    public CharacterController controller;
    public int HP;
    public int currHealth;
    public int healthMax;
    public float playerSpeed;
    [SerializeField] float jumpHeight;
    [SerializeField] float jumpMax;
    [SerializeField] float gravity;
    Coroutine sprint;
    Coroutine sprintRecover;
    public float playerSprintSpeed;
    [SerializeField] float playerStam;

    //Player Private Variables
    private Vector3 playerVel;
    private Vector3 move;
    private Vector3 dashMove;
    private int jumpCount;

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
    #endregion

    #region Player Interact Variables
    [Header("Player Interact Variables\n")]
    [SerializeField] int interactDistance;
    public int Currency = 0;
    public Transform cameraHolderPos;
    #endregion

    #region Original Values
    [Header("Original Values\n")]
    public int originalHP;
    private float originalPlayerVel;
    public float originalPlayerSpeed;
    private int originalHealthMax;
    private float originalPlayerStamina;
    #endregion

    #region Bool Values
    [Header("Bool Values\n")]
    private bool groundedPlayer;
    private bool isShooting;
    bool magIsEmpty;
    bool isReloading;
    bool isFlashing;
    #endregion

    #region Player Stamina/Regen Variables 
    [Header("Player Stamina/Regen Variables")]
    public float maxStam = 100.0f;
    public float dashCost = 20.0f;
    public bool isSprinting = false;
    public bool isStamRecovered = true;
    [Range(0, 50)] public float stamDrain = .1f;
    [Range(0, 50)] public float stamRegen = .1f;
    #endregion

    #region Player Camera Variables
    [Header("Player Camera Variables")]
    [SerializeField] Camera cam;
    public Animator camAnim;
    [SerializeField] float sprintFOV;
    private float initialFOV;
    [SerializeField] float timeBetweenTransition;
    #endregion

    #region Gun Variables 
    [Header("----- Player Gun Settings -----")]
    public Dictionary<GameManager.BulletType, GunStats> gunList; [SerializeField] GameObject gunModel;

    //Common Stats
    [SerializeField] int shootDamage;
    [SerializeField] float shootRate;
    [SerializeField] int shootDistance;
    int activeWeapon;
    public GameObject gunOut;


    //Bullet Characteristics
    [SerializeField] float bulletSpread;
    [SerializeField] float reloadTime;
    [SerializeField] int bulletsPerTap;
    [SerializeField] bool allowButtonHold;
    [SerializeField] int magazineSize;
    [SerializeField] int bulletsLeftInMag;
    [SerializeField] ParticleSystem bulletHitEffect;
    [SerializeField] GameObject bullet;
    public GameManager.BulletType bulletType;
    ParticleSystem EffectHolder;


    //Shooting Positions
    [SerializeField] Transform shootPos;
    [SerializeField] Transform gunPosition;
    [SerializeField] CameraController cameraController;


    //Burst Weapon Characteristics
    float timeBetweenShots;

    //Camera Shake
    float cameraShakeDuration;
    float cameraShakeMagnitude;

    //Enemy Visual Damage
    [SerializeField] float popUpPosRand;
    [SerializeField] GameObject DamagePopUp;

    //Shooting Bools
    bool readyToShoot;
    bool reloading;
    bool shooting;
    #endregion

    #region Player Critical Damage Variables
    [Header("Player Critical Damage Variables\n")]
    [SerializeField] bool useCrit;
    [SerializeField] int critRate; // The higher this number is the lower the crit chance since it's going to be based off of a random range of 1 to this int.
    [SerializeField] int critMultiplier; //The higher this number the lower the crit damage applied will be. Will crash if equal to 0.
    bool solveCrit;
    int damageHolder;
    #endregion

    #region Player Total Ammo Variables
    [Header("Player Total Ammo Variables\n")]
    [SerializeField] int ARbulletsTotal;
    [SerializeField] int ShotgunbulletsTotal;
    [SerializeField] int SMGbulletsTotal;
    float ARbulletsTotalR;       //These are needed so I can use division for fillamount
    float ShotgunbulletsTotalR;  //These are needed so I can use division for fillamount
    float SMGbulletsTotalR;      //These are needed so I can use division for fillamount
    #endregion


    // Start is called before the first frame update 
    void Start()
    {
        //On Start this is what is set up at the beginning of the game
        initialFOV = cam.fieldOfView;
        ARbulletsTotalR = ARbulletsTotal;
        ShotgunbulletsTotalR = ShotgunbulletsTotal;
        SMGbulletsTotalR = SMGbulletsTotal;
        originalHealthMax = healthMax;
        originalHP = HP;
        currHealth = HP;
        originalPlayerSpeed = playerSpeed;
        playerStam = maxStam;
        originalPlayerStamina = playerStam;
        originalDashDebounce = dashDebounce;
        readyToShoot = true;
        isShooting = false;
        AddDrops(gunToAdd, ammoToAdd);
        updatePlayerUI();
    }

    // Update is called once per frame 
    void Update()
    {
        if (!GameManager.instance.inShop)
        {
            stFX();
            if (!gunList.ContainsKey(bulletType))
            {
                bulletType = GameManager.BulletType.SMG;
                //AddDrops(gunToAdd, ammoToAdd);
            }
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
            Interact();
            Movement();
        }
    }

    #region Player Movement
    void Movement()
    {
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer)
        {
            jumpCount = 0;
            playerVel.y = 0;
        }

        move = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;
        //transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, transform.rotation.z + (leanCurve.Evaluate(Input.GetAxis("Horizontal") + 100)));
        controller.Move(move * playerSpeed * Time.deltaTime);
        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.LeftShift))
        {
            //Debug.Log("W and Shift");
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, sprintFOV, timeBetweenTransition);
        }
        else if (Input.GetKey(KeyCode.W))
        {
            //Debug.Log("W");
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, initialFOV, timeBetweenTransition);
        }
        else
        {
            //Debug.Log("Idle");
            camAnim.SetTrigger("idle");
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, initialFOV, timeBetweenTransition);
        }
    }
    #endregion

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

    //Whenever dashing, will take away stam
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

    //Handles the Stam drain when sprinting
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

    //Handles Stam regen when not sprinting or dashing
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

    //Handles moving the player foward dash
    #region Dash Forward IEnumerator
    IEnumerator DashForward()
    {
        float startTime = Time.time;
        while (Time.time < startTime + dashDuration)
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, sprintFOV, timeBetweenTransition);
            controller.Move(transform.forward * dashSpeed * Time.deltaTime);
            yield return null;
        }
        StartCoroutine(DashCoolDown());
    }
    #endregion

    //Handles moving the player backward dash
    #region Dash Backward IEnumerator
    IEnumerator DashBackward()
    {
        float startTime = Time.time;
        while (Time.time < startTime + dashDuration)
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, sprintFOV, timeBetweenTransition);
            controller.Move(-transform.forward * dashSpeed * Time.deltaTime);
            yield return null;
        }
        StartCoroutine(DashCoolDown());
    }
    #endregion

    //Handles moving the player right dash
    #region Dash Right IEnumerator
    IEnumerator DashRight()
    {
        float startTime = Time.time;
        while (Time.time < startTime + dashDuration)
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, sprintFOV, timeBetweenTransition);
            controller.Move(transform.right * dashSpeed * Time.deltaTime);
            yield return null;
        }
        StartCoroutine(DashCoolDown());

    }
    #endregion

    //Handles moving the player left dash
    #region Dash Left IEnumerator
    IEnumerator DashLeft()
    {
        float startTime = Time.time;
        while (Time.time < startTime + dashDuration)
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, sprintFOV, timeBetweenTransition);
            controller.Move(-transform.right * dashSpeed * Time.deltaTime);
            yield return null;
        }
        StartCoroutine(DashCoolDown());
    }
    #endregion

    //Handles the cooldown between dashing
    #region Dash Cooldown IEnumerator
    IEnumerator DashCoolDown()
    {
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, initialFOV, timeBetweenTransition);
        stUpdate();
        yield return new WaitForSeconds(dashCooldownTime);
        isDashing = false;
    }
    #endregion

    #region Weapon and Interact System
    IEnumerator Reload()
    {
        //Debug.Log("Reload Called");
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


        gunList[bulletType].bulletsLeftInMag = gunList[bulletType].magazineSize;
        reloading = false;
        GameManager.instance.hideReload();
        updatePlayerUI();
    }
    void Interact()
    {
        Debug.DrawRay(cam.transform.position, cam.transform.forward * interactDistance, Color.green);
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, interactDistance))
        {
            if (hit.collider.TryGetComponent<IInteractable>(out IInteractable Inter) && hit.collider.gameObject.transform != this.transform)
            {
                updatePlayerUI(Inter.showNotiUI());
                if (Input.GetButtonDown("Interact") && Inter.showNotiUI())
                {
                    updatePlayerUI(Inter.CallUse());
                }
            }
            else
            {
                updatePlayerUI(false);
            }
        }
        else { updatePlayerUI(false); }
    }
    IEnumerator Shoot()
    {
        isShooting = true;
        readyToShoot = false;
        float x = Random.Range(-gunList[bulletType].bulletSpread, gunList[bulletType].bulletSpread);
        float y = Random.Range(-gunList[bulletType].bulletSpread, gunList[bulletType].bulletSpread);
        Vector3 direction = cam.transform.forward + new Vector3(x, y, 0);
        GameObject _bullet = Instantiate(gunList[bulletType].bullet, gunOut.transform.GetChild(0).transform.position, Quaternion.LookRotation(direction));
        Debug.DrawRay(shootPos.transform.position, direction * gunList[bulletType].bulletDistance, Color.red, 1f);
        if (Physics.Raycast(shootPos.transform.position, direction, out RaycastHit hit, gunList[bulletType].bulletDistance))
        {
            EffectHolder = Instantiate(gunList[bulletType].bulletHitEffect, hit.point, Quaternion.Euler(0, 180, 0));
            Destroy(EffectHolder, 2);
            if (hit.collider.TryGetComponent<IDamage>(out IDamage dmg) && hit.collider.gameObject.transform != this.transform)
            {
                solveCrit = Random.Range(1, critRate).Equals(1);
                if (useCrit && solveCrit)
                {
                    //Debug.Log("CRITICAL DAMAGE!   -Continued\n" + "Added Damage should be: " + (gunList[bulletType].shootDamage * (int)critMultiplier).ToString() + "\n" + "Crit Damage should be: " + (gunList[bulletType].shootDamage + (gunList[bulletType].shootDamage * (int)critMultiplier)).ToString() + "\n" + "Normal Damage should be: " + gunList[bulletType].shootDamage.ToString());
                    damageHolder = gunList[bulletType].shootDamage + (gunList[bulletType].shootDamage / critMultiplier);
                }
                else
                {
                    damageHolder = gunList[bulletType].shootDamage;
                }
                if (hit.distance > gunList[bulletType].damageRange)
                {
                    for (float i = hit.distance; i > gunList[bulletType].damageRange; i++)
                    {
                        damageHolder -= damageHolder / gunList[bulletType].damageDropOffRate;
                        i -= 2;
                    }
                }
                dmg.TakeDamage(damageHolder);
                CreatePopUp(hit);
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
                temp.fontSize += 5;
                temp.color = Color.black;

            }
            else if (useCrit && solveCrit)
            {
                temp.fontSize += 2;
                temp.color = Color.yellow;
            }
        }
    }

    public void AddDrops(GunStats gun = null, int AmmoChange = 0, GameManager.BulletType _bulletType = GameManager.BulletType.None)
    {
        if (gun != null && !gunList.ContainsKey(gun.bulletType))
        {
            gunList.Add(gun.bulletType, gun);
            bulletType = gun.bulletType;
            if (gunOut != null)
            {
                Destroy(gunOut);
            }
            gunOut = Instantiate(gun.weaponPrefabSkin, gunPosition.transform);
        }

        if (AmmoChange != 0 && _bulletType == GameManager.BulletType.None)
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
                    activeWeapon = 3;
                    updatePlayerUI();
                    break;
            }
        }
        else if (AmmoChange != 0 && _bulletType != GameManager.BulletType.None)
        {
            switch (_bulletType)
            {
                case GameManager.BulletType.Shotgun:
                    ShotgunbulletsTotal += AmmoChange;
                    updatePlayerUI();
                    break;
                case GameManager.BulletType.AR:
                    ARbulletsTotal += AmmoChange;
                    updatePlayerUI();
                    break;
                case GameManager.BulletType.SMG:
                    SMGbulletsTotal += AmmoChange;
                    updatePlayerUI();
                    break;
            }
        }
    }
    #endregion

    #region DAMAGE/RESPAWN
    public void TakeDamage(int amount)
    {
        if (invul)
        {
            return;
        }
        else
        {

            HP -= amount;
            currHealth = HP;
            StartCoroutine(flashScreen());
            updatePlayerUI();

            if (HP <= 0)
            {
                healthMax -= 1;

                if (healthMax <= 0)
                {
                    GameManager.instance.UpdateLivesUI();
                    GameManager.instance.youSuck();
                }
                else
                {
                    respawn();
                    GameManager.instance.UpdateLivesUI();
                }
            }
        }
    }

    public void respawn()
    {
        HP = originalHP;
        currHealth = HP;
        updatePlayerUI();

        controller.enabled = false;
        transform.position = GameManager.instance.playerSpawnPos.transform.position;
        controller.enabled = true;
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
    #endregion

    #region UI 
    public void updatePlayerUI(bool showInteractNoti = false)
    {
        if (showInteractNoti)
        {
            GameManager.instance.menuInteract.SetActive(true);
        }
        else
        {
            GameManager.instance.menuInteract.SetActive(false);
        }
        try // for debugging purposes
        {
            GameManager.instance.HPBar.fillAmount = (float)HP / originalHP;
            GameManager.instance.AMMOBar.fillAmount = gunList[bulletType].bulletsLeftInMag / (float)gunList[bulletType].magazineSize;



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
    #endregion

    #region Nimble Bison Code
    /*
    public void updatePlayerUI()
    {
    GameManager.instance.playerHPBar.fillAmount = (float)HP / HPOrig;
    GameManager.instance.playerAmmoBar.fillAmount = (float)CurMag / MaxMag;
    updateHealthText();
    updateAmmoText();

    }

    public void updateHealthText()
    {
    GameManager.instance.HPTxt.text = HP + "/" + HPOrig;
    }

    public void updateAmmoText()
    {
    GameManager.instance.AmmoTxt.text = CurMag + "/" + CurAmmo;
    }


    IEnumerator flashDamage()
    {
    GameManager.instance.playerDamageFlash.SetActive(true);
    yield return new WaitForSeconds(0.1f);
    GameManager.instance.playerDamageFlash.SetActive(false);
    }

    public void HealMe(int amount)
    {
    HP += amount;
    if (HP > HPOrig)
    HP = HPOrig;
    updatePlayerUI();
    // UI make flash green
    }

    IEnumerator promptReload()
    {
    isFlashing = true;
    GameManager.instance.reloadPrompt.SetActive(true);
    yield return new WaitForSeconds(0.5f);
    GameManager.instance.reloadPrompt.SetActive(false);
    yield return new WaitForSeconds(0.3f);
    isFlashing = false;
    }

    public void OutOfAmmo()
    {
    GameManager.instance.outOfAmmoPrompt.SetActive(true);
    if (CurAmmo > 0 || CurMag > 0)
    {
    GameManager.instance.outOfAmmoPrompt.SetActive(false);
    }
    }

    public void WallRunStart()
    {
    gravity = -2.5f;
    jumpHeight = startJumpHeight / 4;
    jumpCount = 0;
    playerVel.y = 0;
    dashCount = 0;
    }

    public void WallRunEnd()
    {
    gravity = gravityCurr;
    jumpHeight = startJumpHeight;
    }


    void movement()
    {
    move = Input.GetAxis("Horizontal") * transform.right
    + Input.GetAxis("Vertical") * transform.forward;

    controller.Move(move * playerSpeed * Time.deltaTime); // this is telling the player object to move at a speed based on time 


    // sprint 
    isSprinting = Input.GetKey(sprintKey);
    if (!isSprinting)
    {
    playerSpeed = playerSpeedOrig;
    }
    else if (isSprinting)
    {
    playerSpeed = sprintSpeed;
    }
    // sprint 

    groundedPlayer = controller.isGrounded;

    if (groundedPlayer)
    {
    jumpCount = 0;
    playerVel.y = 0;
    dashCount = 0;
    }

    //if ( Input.GetButtonDown( "Jump" ) && jumpCount < jumpMax ) 
    if (Input.GetKeyDown(jumpKey) && jumpCount < jumpMax)
    {
    playerVel.y = jumpHeight;
    jumpCount++;
    }

    playerVel.y += gravity * Time.deltaTime;
    controller.Move(playerVel * Time.deltaTime);
    }

    #region changeGun()
    void changeGun()
    {
    shootDamage = gunList[selectedGun].shootDamage;
    shootDist = gunList[selectedGun].shootDist;
    shootRate = gunList[selectedGun].shootRate;

    CurMag = gunList[selectedGun].CurGunMag;
    MaxMag = gunList[selectedGun].MaxGunMag;
    CurAmmo = gunList[selectedGun].CurGunCapacity;
    MaxAmmo = gunList[selectedGun].MaxGunCapacity;

    gunModel.GetComponent<MeshFilter>().sharedMesh = gunList[selectedGun].model.GetComponent<MeshFilter>().sharedMesh; // this gives us the gun model 
    gunModel.GetComponent<MeshRenderer>().sharedMaterial = gunList[selectedGun].model.GetComponent<MeshRenderer>().sharedMaterial;
    updatePlayerUI();
    OutOfAmmo();
    if (CurMag == 0)
    {
    magIsEmpty = true;
    }
    else
    {
    magIsEmpty = false;
    }
    }
    #endregion

    #region getGunstats
    public void getGunStats(GunStats gun)
    {
    gunList.Add(gun);
    selectedGun = gunList.Count - 1;

    shootDamage = gun.shootDamage;
    shootDist = gun.shootDist;
    shootRate = gun.shootRate;

    CurMag = gunList[selectedGun].CurGunMag;
    MaxMag = gunList[selectedGun].MaxGunMag;
    CurAmmo = gunList[selectedGun].CurGunCapacity;
    MaxAmmo = gunList[selectedGun].MaxGunCapacity;

    gunModel.GetComponent<MeshFilter>().sharedMesh = gun.model.GetComponent<MeshFilter>().sharedMesh; // this gives us the gun model 
    gunModel.GetComponent<MeshRenderer>().sharedMaterial = gun.model.GetComponent<MeshRenderer>().sharedMaterial;

    updatePlayerUI();
    if (CurMag == 0)
    {
    magIsEmpty = true;
    }
    else
    {
    magIsEmpty = false;
    }
    }
    #endregion

    #region Update?
    /*if (!GameManager.instance.isPaused)
    {
    movement();
    TPCheck();
    OutOfAmmo();
    // Debug.DrawRay( Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.yellow ); 

    if (gunList.Count > 0)
    {
        selectGun();
        //Reload( ref CurrPistolMag, ref MaxPistolMag, ref CurrPistolAmmo, ref MaxPistolAmmo );
        Reload();

        if (Input.GetButton("Shoot") & !isShooting)
        {
            StartCoroutine(shoot());
        }

        if (magIsEmpty && !isFlashing && CurAmmo > 0)
        {
            StartCoroutine(promptReload());
        }

    }
    }

    */


    #endregion

}