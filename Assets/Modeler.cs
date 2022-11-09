using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MeshFilter))]
public class Modeler : MonoBehaviour {
    public GameObject cube;
    public TMP_InputField equationInput;
    private List<GameObject> recentObjects;
    Mesh mesh;

    void Start() {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        recentObjects = new List<GameObject>();
    }
    void Update() {
        if (Input.GetMouseButton(0))
            transform.Rotate(0, Input.GetAxis("Mouse X") * 12, 0);
    }
    public void SimulateCallback(int num) {
        lastCoroutine = StartCoroutine(SimulateEquation(num));
    }

    public void CustomEquation() {
        if (equationInput.text.Contains('=')) {
            StartCoroutine(SimulateCustomEquation(equationInput.text));
        }
    }

    //if the left side and right side are roughly equal, plot the point
    bool Similar(float a, float b) {
        return Mathf.Abs(a - b) < 0.03f; //add function where the closer a function gets, approximate smaller
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

    List<UnityEngine.Vector3> points = new List<UnityEngine.Vector3>();
    List<int> indices = new List<int>();
    int count;
    public float bound = 1.5f;
    public float spacing = 0.05f;
    IEnumerator SimulateCustomEquation(string equationInput) {
        if (lastCoroutine != null) {
            StopCoroutine(lastCoroutine);
        }
        foreach (GameObject g in recentObjects) {
            Destroy(g);
        }
        recentObjects = new List<GameObject>();
        points.Clear();
        indices.Clear();

        string leftSide = equationInput.Split('=')[0].Replace(" ", "");
        string rightSide = equationInput.Split('=')[1].Replace(" ", "");

        EquationParser.Operator leftModel = new EquationParser.Operator(EquationParser.ConvertStringToListedEquation(leftSide));
        EquationParser.Operator rightModel = new EquationParser.Operator(EquationParser.ConvertStringToListedEquation(rightSide));

        count = 0;
        for (float i = -bound; i < bound; i += spacing) {
            for (float j = -bound; j < bound; j += spacing) {
                for (float k = -bound; k < bound; k += spacing) {
                    if (Similar(leftModel.Evaluate(i, k, j), rightModel.Evaluate(i, k, j))) {
                        points.Add(new UnityEngine.Vector3(i, j, k));
                        indices.Add(count);
                        count++;
                        //recentObjects.Add(Instantiate(cube, new Vector3(i, j, k), Quaternion.identity));
                    }
                }
            }
            yield return null;
        }
        CreateMesh();
    }
    IEnumerator SimulateEquation(int num) {
        if (lastCoroutine != null) {
            StopCoroutine(lastCoroutine);
        }
        foreach (GameObject g in recentObjects) {
            Destroy(g);
        }
        recentObjects = new List<GameObject>();
        points.Clear();
        indices.Clear();

        count = 0;
        for (float i = -bound; i < bound; i += spacing) {
            for (float j = -bound; j < bound; j += spacing) {
                for (float k = -bound; k < bound; k += spacing) {
                    if (CheckQuadrics(num, i, j, k)) {
                        points.Add(new UnityEngine.Vector3(i, j, k));
                        indices.Add(count);
                        count++;
                        //recentObjects.Add(Instantiate(cube, new Vector3(i, j, k), Quaternion.identity));
                    }
                }
            }
            //yield return null;
        }
        CreateMesh();
        yield return null;
    }

    List<Color> colors = new List<Color>();
    float magnitude;
    public Gradient gradient;
    public void CreateMesh()
    {
        mesh.Clear();
        colors.Clear();
        mesh.vertices = points.ToArray();
        print(points.Capacity);
        print(mesh.vertexCount);
        for (int i = 0; i < mesh.vertexCount; i++)
        {
            magnitude = points[i].magnitude;
            //colors.Add(gradient.Evaluate(magnitude / 2.598f));
            colors.Add(new Color((magnitude / 2.598f), (1 - (magnitude / 2.598f)), 0));
        }
        mesh.colors = colors.ToArray();
        mesh.SetIndices(indices.ToArray(), MeshTopology.Points, 0);
    }

}
