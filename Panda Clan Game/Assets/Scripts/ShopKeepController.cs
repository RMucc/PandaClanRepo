using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;

public class ShopKeepController : InteractableClass
{
    public Animator anim;
    interactPhases interactPhase;
    [SerializeField] GameObject questNoti;
    [SerializeField] Transform headPos;
    [SerializeField] Transform pos1;
    [SerializeField] Transform pos2;
    [SerializeField] GameObject BagsToDestroy;
    [SerializeField] Transform ArrowDownPos;
    [SerializeField] Transform inShopCamPos;
    [SerializeField] float toPositionDuration;
    float timeElapsed;

    GameObject currentArrow;


    void Start()
    {
        anim = GetComponent<Animator>();
        transform.SetPositionAndRotation(pos1.position, pos1.rotation);
        interactPhase = interactPhases.secondToEnd;
        InteractTaskOpen = true;
    }


    public override void Use()
    {
        switch (interactPhase)
        {
            case interactPhases.secondToEnd:
                IntiateMission();
                break;
            case interactPhases.firstToEnd:
                OpenStore();
                break;
            case interactPhases.end:
                inStore();
                return;
        }
    }
    void IntiateMission() // Notify the player to kill all enemies first
    {
        if (anim.GetBool("EnemiesRemain"))
        {
            Debug.Log("Kill all the enemies first");
            InteractTaskOpen = false;
            Instantiate(questNoti, GameManager.instance.questPos.position, GameManager.instance.questPos.rotation, GameManager.instance.transform.parent);
            StartCoroutine(WaitSeconds(5));
        }
    }

    public void TurnOnWave() // Get player's attention after enemys killed
    {
        anim.SetBool("EnemiesRemain", false);
        currentArrow = Instantiate(GameManager.instance.arrowToNext, ArrowDownPos.position, Quaternion.identity);
        if (!currentArrow.activeSelf) { currentArrow.SetActive(true); }
        InteractTaskOpen = true;
        interactPhase--;
    }


    void OpenStore() // Places shopkeep in position to have the store be avaible for the player
    {
        Destroy(currentArrow);
        anim.SetBool("playerInteract", true); // when dialogue is implemented
        anim.SetBool("ShopMade", true);
        timeElapsed = 0;
        transform.SetPositionAndRotation(pos2.position, pos2.rotation);
        currentArrow = Instantiate(GameManager.instance.arrowToNext, ArrowDownPos.position, Quaternion.identity);
        if (!currentArrow.activeSelf) { currentArrow.SetActive(true); }
        Destroy(BagsToDestroy);
        interactPhase--;
        do
        {
            timeElapsed += Time.deltaTime;
        } while (timeElapsed >= 1f);
        anim.SetBool("playerInteract", false);
    }

    void inStore() // Opening the store menu
    {
        if (currentArrow) { Destroy(currentArrow); }
        InteractTaskOpen = false;
        GameManager.instance.OpenShopMenu();
        timeElapsed = 0;
        Camera.main.transform.parent = transform.parent;
        do
        {
            timeElapsed += Time.deltaTime;
            float percentageComplete = timeElapsed / toPositionDuration;
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, inShopCamPos.position, percentageComplete);
            Camera.main.transform.rotation = Quaternion.Lerp(Camera.main.transform.rotation, inShopCamPos.rotation, percentageComplete);
        } while (Camera.main.transform.position != inShopCamPos.position);
        GameManager.instance.mainInterface.alpha = 0f;
        GameManager.instance.playerScript.gunOut.SetActive(false);
        //GameManager.instance.playerScript.controller.enabled = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    IEnumerator WaitSeconds(int seconds)
    {
        yield return new WaitForSeconds(seconds);
        InteractTaskOpen = true;
    }
}
