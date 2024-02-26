using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SocialPlatforms.Impl;

public class BaseEnemyAI : MonoBehaviour
{
    public List<Renderer> modelList;
    public NavMeshAgent agent;
    public Canvas healthUI;
    [Range(1, 1000)][SerializeField] public int HP;
    public bool alive = true;
    [SerializeField] List<GameObject> dropItemList;
    [SerializeField] int itemPotentialCountToDrop;

    [HideInInspector]
    public List<Color> storedColors;
    public int origHP;


    void Start()
    {
        GameManager.instance.updateEnemyAmount(1);
        foreach (Renderer model in modelList)
        {
            storedColors.Add(model.material.color);
        }
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
        foreach (Renderer model in modelList)
        {
            model.material.color = Color.red;
        }
        yield return new WaitForSeconds(0.1f);
        for (int i = 0; i < modelList.Count; i++)
        {
            modelList[i].material.color = storedColors[i];
        }
    }
}
