using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CameraController : MonoBehaviour {
    public Transform mainAxis;
    public TMP_Text scaleText;

    public float sensitivity = 1f;
    void Start() {
        if (SystemInfo.deviceType == DeviceType.Handheld) sensitivity *= 0.5f;
    }

    float lastPinchDistance = 0f;

    int deltaTouchCount = 0;
    void Update() {
        if (Input.GetMouseButton(0) && (SystemInfo.deviceType != DeviceType.Handheld || deltaTouchCount == 1 && Input.touchCount == 1)) {
            transform.Rotate(0, Mathf.Clamp(Input.GetAxis("Mouse X") * 12 * sensitivity, -35, 35), 0);
            transform.GetChild(0).Rotate(Mathf.Clamp(Input.GetAxis("Mouse Y") * 12 * sensitivity, -35, 35), 0, 0);
        }
        //pinch
        if (SystemInfo.deviceType == DeviceType.Handheld) {
            if (Input.touchCount == 2) {
                if (lastPinchDistance != 0) {
                    float dz = (Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position) - lastPinchDistance) * sensitivity * 0.05f;
                    //print(Camera.main.transform.localPosition + ", " + dz);
                    //if (dz < 0f && Camera.main.transform.localPosition.z > 0.3f || dz >= 0f && Camera.main.transform.localPosition.z < 100f)
                    Camera.main.transform.Translate(0, 0, dz);
                    if (Camera.main.transform.localPosition.z < 0.3f) {
                        Camera.main.transform.localPosition = new Vector3(0, 0, 0.3f);
                    }
                    if (Camera.main.transform.localPosition.z > 100f) {
                        Camera.main.transform.localPosition = new Vector3(0, 0, 100f);
                    }
                }
                lastPinchDistance = Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);
            } else {
                lastPinchDistance = 0;
            }
        } else {
            //scroll
            Camera.main.transform.Translate(0, 0, Input.mouseScrollDelta.y / 10f * sensitivity);
            if (Camera.main.transform.localPosition.z < 0.3f) {
                Camera.main.transform.localPosition = new Vector3(0, 0, 0.3f);
            }
            if (Camera.main.transform.localPosition.z > 100f) {
                Camera.main.transform.localPosition = new Vector3(0, 0, 100f);
            }
        }
        deltaTouchCount = Input.touchCount;
        scaleText.text = "Scale: " + Camera.main.transform.localPosition.z.ToString("0.00");
        mainAxis.transform.localScale = new Vector3(Camera.main.transform.localPosition.z / 2f, Camera.main.transform.localPosition.z / 2f, Camera.main.transform.localPosition.z / 2f);
    }
}
