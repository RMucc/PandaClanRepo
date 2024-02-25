using System;
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
    [SerializeField] Transform ArrowDownPos;
    [SerializeField] Transform inShopCamPos;
    [SerializeField] float toPositionDuration;


    void Start()
    {
        interactPhase = interactPhases.secondToEnd;
        InteractTaskOpen = true;
        GameManager.instance.OnLevelFinished += TurnOnWave;
    }


    public override void Use()
    {
        switch (interactPhase)
        {
            case interactPhases.secondToEnd:
                IntiateMission();
                break;
            case interactPhases.firstToEnd:
                InstantiateStore();
                break;
            case interactPhases.end:
                OpenStore();
                return;
        }
    }
    public void IntiateMission() // Notify the player to kill all enemies first
    {
        if (anim.GetBool("EnemiesRemain"))
        {
            InteractTaskOpen = false;
            Instantiate(questNoti, GameManager.instance.questPos.position, GameManager.instance.questPos.rotation, GameManager.instance.transform.parent);
            StartCoroutine(WaitSeconds(5));
        }
    }

    public void TurnOnWave(object sender, EventArgs e) // Get player's attention after enemys killed
    {
        anim.SetBool("EnemiesRemain", false);
        InteractTaskOpen = true;
        interactPhase--;
    }


    void InstantiateStore() // Places shopkeep in position to have the store be avaible for the player
    {
        anim.SetBool("ShopMade", true);
        interactPhase--;
        anim.SetBool("playerInteract", false);
        OpenStore();
    }

    void OpenStore() // Opening the store menu
    {
        InteractTaskOpen = false;
        GameManager.instance.OpenOrCloseShopMenu(true, inShopCamPos);
    }

    IEnumerator WaitSeconds(int seconds)
    {
        yield return new WaitForSeconds(seconds);
        InteractTaskOpen = true;
    }
}
