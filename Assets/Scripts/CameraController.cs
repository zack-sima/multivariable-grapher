using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    void Start() {

    }

    float lastPinchDistance = 0f;
    void Update() {
        if (Input.GetMouseButton(0)) {
            transform.Rotate(0, Input.GetAxis("Mouse X") * 12, 0);
            transform.GetChild(0).Rotate(Input.GetAxis("Mouse Y") * 12, 0, 0);
        }
        //pinch
        if (SystemInfo.deviceType == DeviceType.Handheld) {
            if (Input.touchCount == 2) {
                if (lastPinchDistance != 0) {
                    float dz = (Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position) - lastPinchDistance) * 0.05f;
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
            Camera.main.transform.Translate(0, 0, Input.mouseScrollDelta.y / 10f);
            if (Camera.main.transform.localPosition.z < 0.3f) {
                Camera.main.transform.localPosition = new Vector3(0, 0, 0.3f);
            }
            if (Camera.main.transform.localPosition.z > 100f) {
                Camera.main.transform.localPosition = new Vector3(0, 0, 100f);
            }
        }

    }
}
