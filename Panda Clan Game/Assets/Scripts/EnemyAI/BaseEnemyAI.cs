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

    public Vector3 playerDir;
    [SerializeField] protected int playerFaceSpeed;
    public Transform headPos;
    [SerializeField] int fov;
    float angleToPlayer;



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

    public void FaceTarget()
    {
        playerDir = (GameManager.instance.player.transform.position) - headPos.position;
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, 0, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * playerFaceSpeed);
    }



    protected bool canSeePlayer()
    {
        playerDir = GameManager.instance.player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(new Vector3(playerDir.x, 0, playerDir.z), transform.forward);

        Debug.Log(angleToPlayer);
        Debug.DrawRay(headPos.position, playerDir);

        RaycastHit hit;
        if (Physics.Raycast(headPos.position, playerDir, out hit))
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer <= fov)
            {

                agent.SetDestination(GameManager.instance.player.transform.position);

                return true;
            }

            Debug.Log(hit.transform.name);
        }
        return false;
    }
}
