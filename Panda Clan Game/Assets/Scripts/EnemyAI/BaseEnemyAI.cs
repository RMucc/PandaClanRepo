using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SocialPlatforms.Impl;

public class BaseEnemyAI : MonoBehaviour
{
    public Renderer model;
    public NavMeshAgent agent;
    public Canvas healthUI;
    [Range(1, 200)][SerializeField] public int HP;
    [Range(1, 200)][SerializeField] public int origHP; public bool alive = true;
    int amountSpawned;
    [SerializeField] List<GameObject> dropItemList;
    [SerializeField] int itemPotentialCountToDrop;
    public Color stored;


    void Start()
    {
        GameManager.instance.updateEnemyAmount(1);
        stored = model.material.color;
        origHP = HP;
    }

    public void OnDeath()
    {
        foreach (GameObject item in dropItemList)
        {
            int amountToSpawn = Random.Range(1, itemPotentialCountToDrop);
            for (int i = 1; i <= amountToSpawn; i++)
            {
                Quaternion itemRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
                Instantiate(item, this.transform.position, itemRotation);
            }
        }
        Destroy(gameObject);
    }

    public void CallUpdateUI()
    {
        if (healthUI != null)
        {
            healthUI.TryGetComponent<CanvasGroup>(out CanvasGroup cg);
            if (cg.alpha == 0)
            {
                cg.alpha = 1;
            }
            if (healthUI.TryGetComponent<EnemyHealthUI>(out EnemyHealthUI CanvasUI))
            {
                CanvasUI.UpdateUI(HP, origHP);
            }
        }
    }

    public IEnumerator FlashRed()
    {
        CallUpdateUI();
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = stored;
    }
}
