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
    /*public class PlayerController : MonoBehaviour, IDamage
    {
        #region Player Debugging

        #endregion

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

        #region Speed & Sprint Variables 
        [Header("----- Player Speed & Sprint Settings -----")]
        [SerializeField] float playerSpeed;
        [SerializeField] float sprintSpeed;
        float playerSpeedOrig;
        bool isSprinting;
        #endregion

        #region Jump Variables 
        [Header("----- Player Jump Settings -----")]
        public KeyCode jumpKey = KeyCode.Space;
        [SerializeField] int jumpMax;
        [SerializeField] float jumpHeight;
        [Range(-25.0f, 0)] public float gravity;
        #endregion

        #region Dash 
        [Header("----- Player Dash Settings -----")]
        [SerializeField] float dashForce;
        [SerializeField] float dashUpwardForce;
        [SerializeField] float dashTime;
        [SerializeField] int dashMax;
        public KeyCode dashKey = KeyCode.E;
        //private bool isdashing; //commenting out for now cause annoying warning - use this for bullet time or immunity frames 
        private int dashCount;
        #endregion

        #region Gun Variables 
        [Header("----- Player Gun Settings -----")]
        [SerializeField] List<GunStats> gunList = new List<GunStats>();
        [SerializeField] GameObject gunModel;
        [SerializeField] int shootDamage;
        [SerializeField] float shootRate;
        [SerializeField] int shootDist;
        [SerializeField] int CurMag;
        [SerializeField] int MaxMag;
        [SerializeField] int CurAmmo;
        [SerializeField] int MaxAmmo;
        #endregion

        Vector3 move;
        Vector3 playerVel;
        public bool groundedPlayer;
        int jumpCount;
        bool isShooting;
        int HPOrig;
        int selectedGun;

        float gravityCurr;
        float startJumpHeight;

        bool isFlashing;
        bool magIsEmpty;

        bool isReloading;



        // Start is called before the first frame update 
        void Start()
        {
            HPOrig = HP;
            playerSpeedOrig = playerSpeed;
            gravityCurr = gravity;
            startJumpHeight = jumpHeight;
            // wip 
            //MaxPistolAmmo = PistolAmmoCapacity;
            //MaxPistolMag = PistolMagCapacity;
            //CurrPistolAmmo = MaxPistolAmmo;
            //CurrPistolMag = MaxPistolMag;
            //updateAmmoText();
            // wip

            respawn();  // moved down to bottom of start.  if UI doesn't exist in scene player original speed is not set
            canTP = safeTP.canTP;
        }

        // Update is called once per frame 
        void Update()
        {
            if (!GameManager.instance.isPaused)
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
    }*/

}
