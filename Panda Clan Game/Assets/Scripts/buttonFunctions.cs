using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonFunctions : MonoBehaviour
{
    [SerializeField] int AKprice;
    [SerializeField] int SMGprice;
    [SerializeField] int shotgunPrice;
    [SerializeField] int AKammoPrice;
    [SerializeField] int SMGammoPrice;
    [SerializeField] int shotgunAmmoPrice;
    [SerializeField] GunStats AK;
    [SerializeField] GunStats SMG;
    [SerializeField] GunStats shotgun;
    public HighscoreTable _topScore;


    public RyansPlayerController playerScript;

    //Pause Screen Buttons
    public void resume()
    {
        GameManager.instance.stateResume();
    }

    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GameManager.instance.stateResume();
    }

    public void scoreEnter()
    {
        
    }

    public void gamestart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        GameManager.instance.stateResume();
    }

    public void Quit()
    {
        Application.Quit();

    }

    public void continueGame()
    {

        GameManager.instance.stateResume();
    }

    public void respawn()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GameManager.instance.stateResume();

    }

    public void Save()
    {
        SaveManager.instance.SaveData();
    }

    public void Load()
    {
        SaveManager.instance.LoadData();
    }

    //Shop Buttons
    public void buyLives()
    {
        //Forgo the shotgun buy lives

    }



    //Title Screen Buttons



    //Options Menu??


    public void buyAK()
    {
        if (GameManager.instance.playerScript.Currency >= AKprice)
        {
            GameManager.instance.playerScript.Currency -= AKprice;
            GameManager.instance.playerScript.AddDrops(AK);
            StartCoroutine(SucessfulPurchase());
        }
        else { StartCoroutine(CantAfford()); }
    }

    public void buySMG()
    {
        if (GameManager.instance.playerScript.Currency >= SMGprice)
        {
            GameManager.instance.playerScript.Currency -= SMGprice;
            GameManager.instance.playerScript.AddDrops(SMG);
            StartCoroutine(SucessfulPurchase());
        }
        else { StartCoroutine(CantAfford()); }
    }

    public void buyShotgun()
    {
        if (GameManager.instance.playerScript.Currency >= shotgunPrice)
        {
            GameManager.instance.playerScript.Currency -= shotgunPrice;
            GameManager.instance.playerScript.AddDrops(shotgun);
            StartCoroutine(SucessfulPurchase());
        }
        else { StartCoroutine(CantAfford()); }
    }

    public void buyAKammo()
    {
        if (GameManager.instance.playerScript.Currency >= AKammoPrice)
        {
            GameManager.instance.playerScript.Currency -= AKammoPrice;
            GameManager.instance.playerScript.AddDrops(null, 10, GameManager.BulletType.AR);
            StartCoroutine(SucessfulPurchase());
        }
        else { StartCoroutine(CantAfford()); }
    }

    public void buySMGammo()
    {
        if (GameManager.instance.playerScript.Currency >= SMGammoPrice)
        {
            GameManager.instance.playerScript.Currency -= SMGammoPrice;
            GameManager.instance.playerScript.AddDrops(null, 10, GameManager.BulletType.SMG);
            StartCoroutine(SucessfulPurchase());
        }
        else { StartCoroutine(CantAfford()); }
    }

    public void buyShotgunAmmo()
    {
        if (GameManager.instance.playerScript.Currency >= shotgunAmmoPrice)
        {
            GameManager.instance.playerScript.Currency -= shotgunAmmoPrice;
            GameManager.instance.playerScript.AddDrops(null, 10, GameManager.BulletType.Shotgun);
            StartCoroutine(SucessfulPurchase());
        }
        else { StartCoroutine(CantAfford()); }
    }

    public void CloseShopMenu()
    {
        if(GameManager.instance.menuShop.activeSelf)
        {
            GameManager.instance.menuShop.SetActive(false);
            GameManager.instance.inShop = false;
            Camera.main.transform.parent = GameManager.instance.player.transform;
            Camera.main.transform.SetPositionAndRotation(GameManager.instance.playerScript.cameraHolderPos.position, GameManager.instance.playerScript.cameraHolderPos.rotation);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            GameManager.instance.mainInterface.alpha = 1f;
            GameManager.instance.playerScript.gunOut.SetActive(true);
            //GameManager.instance.playerScript.controller.enabled = true;
            GameManager.instance.shopKeeper.InteractTaskOpen = true;
            GameManager.instance.shopKeeper.anim.SetBool("", true);
        }
        else { StartCoroutine(CantAfford()); }
    }

    IEnumerator SucessfulPurchase()
    {
        GameManager.instance.CurrCount.text = GameManager.instance.playerScript.Currency.ToString();
        GameManager.instance.shopKeeper.anim.SetBool("OnBuy", true);
        yield return new WaitForSeconds(.2f);
        GameManager.instance.shopKeeper.anim.SetBool("OnBuy", false);
        Save();
    }

    IEnumerator CantAfford()
    {
        Color tempColor = GameManager.instance.CurrCount.color;
        GameManager.instance.CurrCount.color = Color.red;
        yield return new WaitForSeconds(.1f);
        GameManager.instance.CurrCount.color = tempColor;
        yield return new WaitForSeconds(.1f);
        GameManager.instance.CurrCount.color = Color.red;
        yield return new WaitForSeconds(.1f);
        GameManager.instance.CurrCount.color = tempColor;
    }
}
