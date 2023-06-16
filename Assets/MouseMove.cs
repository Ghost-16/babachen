using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseMove : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private Transform target;

    private Vector3 previousPosition;
    private Vector3 camTranslation = new Vector3(-6, 0, -30);

    void Start()
    {
        cam.transform.position = target.position;
        cam.transform.Translate(camTranslation);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && cam.ScreenToViewportPoint(Input.mousePosition).x>0.2)
        {
            previousPosition = cam.ScreenToViewportPoint(Input.mousePosition);
        }

        if (Input.GetMouseButton(0) && cam.ScreenToViewportPoint(Input.mousePosition).x > 0.2)
        {
            Vector3 direction = previousPosition - cam.ScreenToViewportPoint(Input.mousePosition);
            cam.transform.position = target.position;

            cam.transform.Rotate(new Vector3(1, 0, 0), direction.y * 180);
            cam.transform.Rotate(new Vector3(0, 1, 0), -direction.x * 180, Space.World);
            cam.transform.Translate(camTranslation);

            previousPosition = cam.ScreenToViewportPoint(Input.mousePosition);
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            cam.transform.position += (cam.transform.forward);

            //camTranslation.z += 1f;
            //cam.transform.Translate(camTranslation);
            //Debug.Log(camTranslation.z);
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            cam.transform.position -= (cam.transform.forward);

            //camTranslation.z -= 1f;
            //cam.transform.Translate(camTranslation);
            //Debug.Log(camTranslation.z);
        }
    }
}
