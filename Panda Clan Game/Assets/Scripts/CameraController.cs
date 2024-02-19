using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Camera cam;

    [SerializeField] int sensitivity;
    [SerializeField] int lockVertMin;
    [SerializeField] int lockVertMax;
    [SerializeField] bool invertY;

    float xRot;

    // Start is called before the first frame update
    void Start()
    {
        cam.orthographic = false;
        cam.ResetProjectionMatrix();
        //Cursor Settings -Turns Visibility off and locks cursor in its start state
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance != null && !GameManager.instance.inShop)
        {
            float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * sensitivity;
            float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * sensitivity;

            if (invertY)
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

    public IEnumerator Shake(float duration, float magntiude)
    {
        Debug.Log("Shake");
        Vector3 originalPos = transform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1, 1f) * magntiude;
            float y = Random.Range(-1, 1f) * magntiude;
            transform.localPosition = new Vector3(x, y, originalPos.z);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = originalPos;
    }
}
