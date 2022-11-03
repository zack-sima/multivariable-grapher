using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Modeler : MonoBehaviour {
    public GameObject cube;
    private List<GameObject> recentObjects;

    void Start() {
        recentObjects = new List<GameObject>();
    }
    public void SimulateCallback(int num) {
        lastCoroutine = StartCoroutine(SimulateEquation(num));
    }
    //if the left side and right side are roughly equal, plot the point
    bool Similar(float a, float b) {
        return Mathf.Abs(a - b) < 0.05f; //add function where the closer a function gets, approximate smaller
    }
    bool CheckQuadrics(int equationNum, float x, float y, float z) {
        switch (equationNum) {
            case 0:
                return EllipticParaboloid(x, y, z);
            case 1:
                return EllipticCone(x, y, z);
            case 2:
                return HyperbolicParaboloid(x, y, z);
            case 3:
                return Ellipsoid(x, y, z);
            case 4:
                return OneSheetHyperboloid(x, y, z);
            case 5:
                return TwoSheetsHyperboloid(x, y, z);
            default:
                return false;
        }
    }
    bool EllipticParaboloid(float x, float z, float y) {
        //z=x^2+y^2
        if (Similar(Mathf.Pow(x, 2) + Mathf.Pow(y, 2), z)) {
            return true;
        }
        return false;
    }
    bool HyperbolicParaboloid(float x, float z, float y) {
        //z=x^2-y^2
        if (Similar(Mathf.Pow(x, 2) - Mathf.Pow(y, 2), z)) {
            return true;
        }
        return false;
    }
    bool EllipticCone(float x, float z, float y) {
        //z^2=x^2+y^2
        if (Similar(Mathf.Pow(x, 2) + Mathf.Pow(y, 2), Mathf.Pow(z, 2))) {
            return true;
        }
        return false;
    }
    bool Ellipsoid(float x, float z, float y) {
        //x^2+y^2+z^2=1
        if (Similar(Mathf.Pow(x, 2) + Mathf.Pow(y, 2) + Mathf.Pow(z, 2), 1)) {
            return true;
        }
        return false;
    }
    bool OneSheetHyperboloid(float x, float z, float y) {
        //x^2+y^2-z^2=1
        if (Similar(Mathf.Pow(x, 2) + Mathf.Pow(y, 2) - Mathf.Pow(z, 2), 1)) {
            return true;
        }
        return false;
    }
    bool TwoSheetsHyperboloid(float x, float z, float y) {
        //x^2+y^2-z^2=1
        if (Similar(Mathf.Pow(x, 2) - Mathf.Pow(y, 2) - Mathf.Pow(z, 2), 1)) {
            return true;
        }
        return false;
    }
    Coroutine lastCoroutine = null;
    IEnumerator SimulateEquation(int num) {
        if (lastCoroutine != null) {
            StopCoroutine(lastCoroutine);
        }
        foreach (GameObject g in recentObjects) {
            Destroy(g);
        }
        recentObjects = new List<GameObject>();

        for (float i = -1.5f; i < 1.5f; i += 0.05f) {
            for (float j = -1.5f; j < 1.5f; j += 0.05f) {
                for (float k = -1.5f; k < 1.5f; k += 0.05f) {
                    if (CheckQuadrics(num, i, j, k)) {
                        recentObjects.Add(Instantiate(cube, new Vector3(i, j, k), Quaternion.identity));
                    }
                }
            }
            yield return null;
        }
    }
}
