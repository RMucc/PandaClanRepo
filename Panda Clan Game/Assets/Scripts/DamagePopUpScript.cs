using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class DamagePopUpScript : MonoBehaviour
{
    Vector3 playerDir;
    [SerializeField] float popUpDestroyTime;
    [SerializeField] int playerFaceSpeed;
    [SerializeField] AnimationCurve opacityCurve;
    [SerializeField] AnimationCurve scaleCurve;
    [SerializeField] AnimationCurve heightCurve;
    [SerializeField] AnimationCurve rotationCurve;
    // Add rotation curve instead of player direction;
    [SerializeField] Color popUpColor;
    TextMeshProUGUI tmp;
    private float time = 0;
    Vector3 origin;


    void Start()
    {
        Destroy(gameObject, playerFaceSpeed);
        playerDir = GameManager.instance.player.transform.position - transform.position;
        transform.rotation = Quaternion.LookRotation(-playerDir);
    }

    private void Awake()
    {
        tmp = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        origin = transform.position;
    }

    private void Update()
    {
        playerDir = GameManager.instance.player.transform.position - transform.position;
        tmp.color = new Color(tmp.color.r, tmp.color.g, tmp.color.b, opacityCurve.Evaluate(time));
        transform.localScale = Vector3.one * scaleCurve.Evaluate(time);
        transform.position = origin + new Vector3(0, 1 + heightCurve.Evaluate(time), 0);
        transform.Rotate(Vector3.up * rotationCurve.Evaluate(time));
        time += Time.deltaTime;
    }
}
