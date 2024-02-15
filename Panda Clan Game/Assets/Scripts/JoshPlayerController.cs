using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using UnityEngine.Windows;

//This is a script for my own player I can use to test in my scene. I will make it a PreFab so that other's
// can use this player if they so choose
public class JoshPlayerController : MonoBehaviour
{
    public class PlayerController : MonoBehaviour //IDamage
    {
        #region Player Debugging

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

        #region Original Values
        [Header("Original Values\n")]
        public int originalHP;
        private float originalPlayerVel;
        public float originalPlayerSpeed;
        private int originalHealthMax;
        private float originalPlayerStamina;
        #endregion

        #region Player Interact Variables
        [Header("Player Interact Variables\n")]
        [SerializeField] int interactDistance;
        public int Currency = 0;
        public Transform cameraHolderPos;
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

        #region Bool Values
        [Header("Bool Values\n")]
        bool groundedPlayer;
        bool isShooting;
        bool magIsEmpty;
        bool isReloading;
        bool isFlashing;
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
            //AddDrops(gunToAdd, ammoToAdd);
            //updatePlayerUI();
        }

        // Update is called once per frame 
        void Update()
        {

        }

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
        }*/
        #endregion
    }
}


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


        */
    #endregion