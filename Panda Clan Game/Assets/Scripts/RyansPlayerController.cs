using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UIElements;
using Unity.VisualScripting;
using System.ComponentModel;

public class RyansPlayerController : MonoBehaviour, IDamage
{
    [Header("Debugging")]
    [SerializeField] GunStats gunToAdd;
    [SerializeField] int ammoToAdd;
    public bool invul;

    [Header("Audio")]
    [SerializeField] AudioSource src;
    [SerializeField] AudioSource invulSound;
    [SerializeField] AudioSource WeapSrc;
    [SerializeField] AudioSource SHOTSrc;
    [SerializeField] AudioSource bbComeback;

    [Header("Audio Files")]
    public AudioClip pain;
    public AudioClip revive;
    public AudioClip jump;
    public AudioClip uziReload;
    public AudioClip ARReload;
    public AudioClip shotgun;
    public AudioClip shotgunReload;
    public AudioClip getShotgun;
    public AudioClip uziShoot;
    public AudioClip metal;

    [Header("Player Interact Variables\n")]
    [SerializeField] int interactDistance;
    public int Currency = 0;
    public Transform cameraHolderPos;

    [Header("Player Movement Variables\n")]
    //Player Movement Variables
    public CharacterController controller;
    public int HP;
    public int currHealth;
    public int healthMax;
    public float playerSpeed;
    [SerializeField] float jumpHeight;
    [SerializeField] float jumpMax;
    [SerializeField] float gravity;
    [SerializeField] AnimationCurve leanCurve;
    Coroutine sprint;
    Coroutine sprintRecover;

    [Header("Player Camera Variables")]
    [SerializeField] Camera cam;
    public Animator camAnim;
    [SerializeField] float sprintFOV;
    private float initialFOV;
    [SerializeField] float timeBetweenTransition;
    Coroutine cameraSprint;
    Coroutine cameraInitial;

    [Header("Player Stamina/Regen Variables")]
    public float maxStam = 100.0f;
    public float dashCost = 20.0f;
    public bool isSprinting = false;
    public bool isStamRecovered = true;
    [Range(0, 50)] public float stamDrain = .1f;
    [Range(0, 50)] public float stamRegen = .1f;

    public float playerSprintSpeed;
    [SerializeField] float playerStam;
    //[SerializeField] float playerStamRecover;

    [Header("Player Gun Variables\n")]
    //Player Gun Variables
    [SerializeField] Transform shootPos;
    [SerializeField] Transform gunPosition;
    [SerializeField] CameraController cameraController;
    public GameManager.BulletType bulletType;
    ParticleSystem EffectHolder;
    public GameObject gunOut;

    [Header("Player Critical Damage Variables\n")]
    [SerializeField] bool useCrit;
    [SerializeField] int critRate; // The higher this number is the lower the crit chance since it's going to be based off of a random range of 1 to this int.
    [SerializeField] int critMultiplier; //The higher this number the lower the crit damage applied will be. Will crash if equal to 0.
    bool solveCrit;
    int damageHolder;

    [SerializeField] float popUpPosRand;
    [SerializeField] GameObject DamagePopUp;

    public Dictionary<GameManager.BulletType, GunStats> gunList;

    bool readyToShoot;
    bool reloading;
    bool shooting;
    [Header("Player Total Ammo Variables\n")]
    [SerializeField] int ARbulletsTotal;
    [SerializeField] int ShotgunbulletsTotal;
    [SerializeField] int SMGbulletsTotal;
    float ARbulletsTotalR;       //These are needed so I can use division for fillamount
    float ShotgunbulletsTotalR;  //These are needed so I can use division for fillamount
    float SMGbulletsTotalR;      //These are needed so I can use division for fillamount


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
    public int originalHP;
    private float originalPlayerVel;
    public float originalPlayerSpeed;
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

    // Start is called before the first frame update
    void Start()
    {
        
        initialFOV = cam.fieldOfView;
        gunList = new Dictionary<GameManager.BulletType, GunStats>();
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

        if(!Input.GetKey(KeyCode.W) || !Input.GetKey(KeyCode.A) || !Input.GetKey(KeyCode.S) || !Input.GetKey(KeyCode.D))
        {
            //Debug.Log("Idle");
            camAnim.SetTrigger("idle");
        }
        //Jump Input
        #region Jump Input
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            src.clip = jump;
            src.Play();
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
            if(cameraSprint != null)
            {
                StopCoroutine(cameraSprint);
            }
            if(cameraInitial != null)
            {
                StopCoroutine(cameraInitial);
            }
            if (playerStam > 0)
            {
                isSprinting = true;
                cameraSprint = StartCoroutine(CameraSprint());
                Sprinting();
            }
        }
        if (playerStam <= 0)
        {
            isSprinting = false;
            playerSpeed = originalPlayerSpeed;
            StopCoroutine(cameraSprint);
            cameraInitial = StartCoroutine(CameraInitial());
            sprintRecover = StartCoroutine(SprintRecover());
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            isSprinting = false;
            playerSpeed = originalPlayerSpeed;
            StopCoroutine(cameraSprint);
            cameraInitial = StartCoroutine(CameraInitial());
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
                if (canDash && isSprinting)
                {
                    isDashing = true;
                    StartCoroutine(DashForward());
                }
                else if (canDash)
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
                if (canDash && isSprinting)
                {
                    isDashing = true;
                    StartCoroutine(DashBackward());
                }
                else if (canDash)
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
                if (canDash && isSprinting)
                {
                    isDashing = true;
                    StartCoroutine(DashRight());
                }
                else if (canDash)
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
                if (canDash && isSprinting)
                {
                    isDashing = true;
                    StartCoroutine(DashLeft());
                }
                else if (canDash)
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
                StopCoroutine(cameraSprint);
                cameraInitial = StartCoroutine(CameraInitial());
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
            StopCoroutine(cameraInitial);
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
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, sprintFOV, timeBetweenTransition);
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
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, sprintFOV, timeBetweenTransition);
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
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, sprintFOV, timeBetweenTransition);
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
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, sprintFOV, timeBetweenTransition);
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
        cameraInitial = StartCoroutine(CameraInitial());
        stUpdate();
        yield return new WaitForSeconds(dashCooldownTime);
        isDashing = false;
        StopCoroutine(cameraInitial);
    }
    #endregion

    #region Camera Sprint FOV IEumerator
    IEnumerator CameraSprint()
    {
        while(isSprinting || isDashing)
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, sprintFOV, timeBetweenTransition);
            yield return new WaitForSeconds(0);
        }
    }
    #endregion

    #region Camera Initial FOV IEumerator
    IEnumerator CameraInitial()
    {
        while(!isSprinting || !isDashing)
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, initialFOV, timeBetweenTransition);
            yield return new WaitForSeconds(0);
        }
    }
    #endregion

    #region Weapon and Interact System
    IEnumerator Reload()
    {
        if (bulletType == GameManager.BulletType.SMG)
        {
            WeapSrc.clip = uziReload;
            WeapSrc.Play();
        }
        else if (bulletType == GameManager.BulletType.AR)
        {
            WeapSrc.clip = ARReload;
            WeapSrc.Play();
        }
        else if (bulletType == GameManager.BulletType.Shotgun)
        {
            WeapSrc.clip = shotgunReload;
            WeapSrc.Play();
        }
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
        if (bulletType == GameManager.BulletType.Shotgun)
        {
            
            SHOTSrc.clip = shotgun;
            SHOTSrc.Play();
        }
        else if (bulletType != GameManager.BulletType.Shotgun)
        {
            WeapSrc.clip = uziShoot;
            WeapSrc.Play();
        }
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
        if (gun != null)
        {
            try
            {
                if (!gunList.ContainsKey(gun.bulletType))
                {
                    gunList.Add(gun.bulletType, gun);
                }
            }
            catch (System.Exception)
            {

            }
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
                    src.clip = getShotgun;
                    src.Play();
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
            StartCoroutine(GodScreen());
            invulSound.clip = metal;
            invulSound.Play();
            return;
        }
        else
        {

            HP -= amount;
            currHealth = HP;
            StartCoroutine(flashScreen());
            src.clip = pain;
            src.Play();
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
                    bbComeback.clip = revive;
                    bbComeback.Play();
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

    IEnumerator GodScreen()
    {
        try
        {
            GameManager.instance.GodScreen.SetActive(true);
        }
        catch (System.Exception)
        {
            print("error: GodScreen missing from GameManager");
        }

        yield return new WaitForSeconds(0.2f);

        try
        {
            GameManager.instance.GodScreen.SetActive(false);
        }
        catch (System.Exception)
        {
            print("error: GodScreen missing from GameManager");
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
            print(e + " error : missing HPBar");
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
}
