using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestNoti : MonoBehaviour
{
    [SerializeField] AnimationCurve xPositionCurve;
    Vector3 origin;
    private float time = 0;

    // Start is called before the first frame update
    void Start()
    {
        origin = transform.localPosition;
        Destroy(gameObject, 5f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition = origin - new Vector3(xPositionCurve.Evaluate(time), 0, 0);
        time += Time.deltaTime;
    }
}
