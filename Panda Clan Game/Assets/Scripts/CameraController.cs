using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] int sensitivity;
    [SerializeField] int lockVertMin;
    [SerializeField] int lockVertMax;
    [SerializeField] bool invertY;
    float xRot;

    // Start is called before the first frame update
    void Start()
    {
        //Cursor Settings -Turns Visibility off and locks cursor in its start state
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * sensitivity;
        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * sensitivity;

        if(invertY)
        {
            xRot += mouseY;
        }
        else
        {
            xRot -= mouseY;
        }

        xRot = Mathf.Clamp(xRot, lockVertMin, lockVertMax);
        transform.localRotation = Quaternion.Euler(xRot, 0, 0);
        transform.parent.Rotate(Vector3.up * mouseX);
    }
}
