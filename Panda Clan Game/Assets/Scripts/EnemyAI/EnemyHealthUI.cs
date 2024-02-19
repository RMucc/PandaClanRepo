using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyHealthUI : MonoBehaviour
{
    [SerializeField] Image healthWhite;
    [SerializeField] Image healthRed;
    [SerializeField] float timeToDecrease;
    float currHealth;
    float origHealth;
    // Start is called before the first frame update
    private void Update()
    {
        Quaternion rot = Quaternion.LookRotation(GameManager.instance.player.transform.position - transform.position);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * 100);
        healthWhite.fillAmount = Mathf.Lerp(healthWhite.fillAmount, healthRed.fillAmount, timeToDecrease);
    }

    public void UpdateUI(float _currHealth, float _origHealth)
    {
        //Debug.Log("currHealth = " + _currHealth + "  original health = " + _origHealth + "   Divided curr/orig = " + (float)_currHealth / _origHealth);
        healthRed.fillAmount = (float)_currHealth / _origHealth;
        //->Debug.Log(Mathf.Lerp(currHealth, _currHealth, timeToDecrease));
        //healthWhite.fillAmount = Mathf.Lerp(currHealth/_origHealth, _currHealth/origHealth, timeToDecrease);
        //currHealth = _currHealth;
        //origHealth = _origHealth;
    }
}
