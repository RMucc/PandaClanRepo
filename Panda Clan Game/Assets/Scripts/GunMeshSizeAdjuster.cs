using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunMeshSizeAdjuster : MonoBehaviour
{
    [SerializeField] Transform OrigTransform;
    [SerializeField] AnimationCurve sizeCurve;
    [SerializeField] AnimationCurve distCurve;
    [SerializeField] float currSize;
    [SerializeField] float currDist;
    [SerializeField] int iterations = 0;


    private void Start()
    {
        currSize = 1;
        currDist = 1;
        OrigTransform = this.transform;
    }
    //    void Update()
    //    {
    //        if (Vector3.Distance(transform.position, OrigTransform.position) )
    //            }
    //}
    //    }

    private void Update()
    {
        //Debug.Log(Vector3.Distance(Camera.main.transform.position, this.transform.position) / Vector3.Distance(Camera.main.transform.position, OrigTransform.position)-.1f);
    }

    void OnTriggerStay(Collider other)
    {
        Debug.Log(Vector3.Distance(Camera.main.transform.position, this.transform.position) + " / " + Vector3.Distance(Camera.main.transform.position, OrigTransform.position) + " == " + Vector3.Distance(Camera.main.transform.position, this.transform.position) / Vector3.Distance(Camera.main.transform.position, OrigTransform.position) + " - Tag: " + other.gameObject.tag);

        //other.
        if (other.gameObject.tag == "Environment")
        {
            
            this.transform.position = Vector3.Lerp(OrigTransform.position, Camera.main.transform.position, distCurve.Evaluate(Vector3.Distance(Camera.main.transform.position, this.transform.position) / Vector3.Distance(Camera.main.transform.position, OrigTransform.position)) - .1f);
        }
        //transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, distCurve.Evaluate(Vector3.Distance(Camera.main.transform.position, this.transform.position) / Vector3.Distance(Camera.main.transform.position, OrigTransform.position)));
    }
}
