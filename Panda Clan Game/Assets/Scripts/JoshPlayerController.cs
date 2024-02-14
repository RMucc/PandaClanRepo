using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

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


        }
        /*
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



            // Dash 
            if (Input.GetKeyDown(dashKey) && dashCount < dashMax)
            {
                StartCoroutine(Dash());
                dashCount++;
            }
            // Dash



            //if ( Input.GetButtonDown( "Jump" ) && jumpCount < jumpMax ) 
            if (Input.GetKeyDown(jumpKey) && jumpCount < jumpMax)
            {
                playerVel.y = jumpHeight;
                jumpCount++;
            }

            playerVel.y += gravity * Time.deltaTime;
            controller.Move(playerVel * Time.deltaTime);
        }

        private IEnumerator Dash()
        {
            //isdashing = true;
            playerVel = new Vector3(move.x * dashForce, dashUpwardForce, move.z * dashForce);
            yield return new WaitForSeconds(dashTime);
            playerVel = Vector3.zero;
            //isdashing = false;
        }


        #region Reload 
        void Reload()
        {
            if (Input.GetKeyDown(reloadKey) && CurMag != MaxMag) // if you push the R key & you are not at full mag capacity 
            {
                int magFill = MaxMag - CurMag; // this is how much ammo is needed to fill the mag 

                if (CurAmmo > 0 && CurAmmo >= magFill) // if you have enough ammo to fully fill your mag 
                {
                    CurMag += magFill;
                    CurAmmo -= magFill;
                    gunList[selectedGun].CurGunMag = CurMag;
                    gunList[selectedGun].CurGunCapacity = CurAmmo;
                }
                else if (CurAmmo > 0 && CurAmmo < magFill) // if you don't have enough ammo to fully fill your mag, use CurrAmmo (less than magFill, greater than 0) 
                {
                    CurMag += CurAmmo;
                    CurAmmo = 0;
                    gunList[selectedGun].CurGunMag = CurMag;
                    gunList[selectedGun].CurGunCapacity = CurAmmo;
                }
                updatePlayerUI();


                if (CurMag > 0)
                {
                    magIsEmpty = false;
                }
            }
        }

        public void FillAmmo(int fillAmount)
        {
            if (CurAmmo + fillAmount > MaxAmmo)
            {
                CurAmmo = MaxAmmo;
            }
            else
            {
                CurAmmo += fillAmount;
            }

            updatePlayerUI();
        }

        public void RefillAmmo(AmmoTypes ammoType, int ammoAmount)
        {
            switch (ammoType)
            {
                case AmmoTypes.PISTOL:
                    FillAmmo(ammoAmount);
                    break;
                case AmmoTypes.SNIPER:
                    break;
                case AmmoTypes.SHOTGUN:
                    break;
                default:
                    break;
            }
        }

        #endregion

        IEnumerator shoot()
        {
            if (gunList[selectedGun].CurGunMag > 0)
            {
                isShooting = true;
                gunList[selectedGun].CurGunMag--;
                CurMag--;
                RaycastHit hit;
                if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, shootDist))
                {
                    // we need to damage stuff 
                    IDamage dmg = hit.collider.GetComponent<IDamage>();

                    if (hit.transform != transform && dmg != null) // if we did not hit ourselves & if what we hit can take damage 
                    {
                        dmg.takeDamage(shootDamage);
                    }

                    //Instantiate(gunList[selectedGun].hitEffect, hit.point, gunList[selectedGun].hitEffect.transform.rotation ); // gunshot effect, applicable for every gun 
                }
                if (CurMag == 0)
                {
                    magIsEmpty = true;
                }

                updatePlayerUI();
                yield return new WaitForSeconds(shootRate);
                isShooting = false;
            }
        }

        void selectGun()
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0 && selectedGun < gunList.Count - 1)
            {
                selectedGun++;
                changeGun();
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0 && selectedGun > 0)
            {
                selectedGun--;
                changeGun();
            }
        }

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


        void TPCheck()
        {
            if (canTP)
            {
                posSafe = safeTP.playerPos;
                rotYSafe = safeTP.yRot;
            }
        }

        public void takeDamageTP(int amount, bool TP)
        {
            if (!TP)  // teleport is false
                takeDamage(amount);
            else      // teleport is true and will teleport player to last safe pos
            {
                takeDamage(amount);
                transform.position = posSafe; // working on adjusting cam pos.
                gameObject.GetComponentInChildren<CameraController>().tp = true; //set players x to be leveled.
                                                                                 //transform.rotation = Quaternion.Euler(0, rotYSafe, 0); // is currently not setting the correct direction
            }
        }

        public void takeDamage(int amount)
        {
            HP -= amount;
            updatePlayerUI();
            StartCoroutine(flashDamage());

            if (HP <= 0)
            {
                GameManager.instance.youLose();
            }
        }

        public void respawn()
        {
            HP = HPOrig;
            updatePlayerUI();

            controller.enabled = false;
            transform.position = GameManager.instance.playerSpawnPos.transform.position;
            controller.enabled = true;
        }

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

        private void OnTriggerEnter(Collider other)
        {
            switch (other.tag)
            {
                case "Pistol":
                    {

                        break;
                    }
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

        */
    }

}
