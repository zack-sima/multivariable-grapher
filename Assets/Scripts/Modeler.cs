using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MeshFilter))]
public class Modeler : MonoBehaviour {
    //NOTE: cube is temporary fallback for iOS
    public GameObject cube;

    public TMP_InputField equationInput;
    public Slider spacingSlider, scaleSlider;
    public Text spacingText, scaleText;
    public TMP_Text progressText;

    bool handHeld;

    //MeshRenderer r;

    private List<GameObject> recentObjects;
    Mesh mesh;

    float tolerance = 0.03f;

    //Spacing cannot be allowed to be too small if scale is big
    public void ChangeSpacingValues() {
        float originalPercentage = spacingSlider.value / spacingSlider.maxValue;

        if (handHeld) {
            spacingSlider.minValue = Mathf.Pow(scaleSlider.value, 2) / 50f;
            spacingSlider.maxValue = Mathf.Pow(scaleSlider.value, 2) / 15f;
            spacingSlider.value = originalPercentage * spacingSlider.maxValue;
        } else {
            spacingSlider.minValue = Mathf.Pow(scaleSlider.value, 2) / 100f;
            spacingSlider.maxValue = Mathf.Pow(scaleSlider.value, 2) / 30f;
            spacingSlider.value = originalPercentage * spacingSlider.maxValue;
        }
    }
    void Update() {
        spacingText.text = "Model Spacing: " + spacingSlider.value.ToString("0.###");
        scaleText.text = "Model Scale: " + Mathf.Pow(scaleSlider.value, 2).ToString("0.##");

        tolerance = spacingSlider.value / 1.5f;
        spacing = spacingSlider.value;
        bound = Mathf.Pow(scaleSlider.value, 2);
        //r.material.shader.get
    }
    void Start() {
        Application.targetFrameRate = 60;

        handHeld = true; // SystemInfo.deviceType == DeviceType.Handheld;

        if (!handHeld) {
            mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = mesh;
            //r = GetComponent<MeshRenderer>();
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        }
        recentObjects = new List<GameObject>();

        ChangeSpacingValues();
    }
    public void SimulateCallback(int num) {
        switch (num) {
            case 0:
                equationInput.text = "z=x^2+y^2";
                break;
            case 1:
                equationInput.text = "z^2=x^2+y^2";
                break;
            case 2:
                equationInput.text = "z=x^2-y^2";
                break;
            case 3:
                equationInput.text = "x^2+y^2+z^2=1";
                break;
            case 4:
                equationInput.text = "x^2+y^2-z^2=1";
                break;
            case 5:
                equationInput.text = "x^2-y^2-z^2=1";
                break;
            default:
                break;
        }
        CustomEquation();
    }

    public void CustomEquation() {
        if (equationInput.text.Contains('=')) {
            lastCoroutine = StartCoroutine(SimulateCustomEquation(equationInput.text.ToLower()));
        }
    }

    //if the left side and right side are roughly equal, plot the point
    bool Similar(float a, float b) {
        return Mathf.Abs(a - b) < tolerance; //add function where the closer a function gets, approximate smaller
    }

    Coroutine lastCoroutine = null;

    List<Vector3> points = new List<Vector3>();
    List<int> indices = new List<int>();
    int count;

    public float bound = 1.5f;
    public float spacing = 0.05f;

    List<GameObject> lastCubes = new List<GameObject>();
    IEnumerator SimulateCustomEquation(string equationInput) {
        foreach (GameObject g in lastCubes) {
            if (g != null) Destroy(g);
        }
        lastCubes = new List<GameObject>();

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
        int index = 0;

        float spacingMin = spacing * 0.7f;
        float spacingMax = spacing * 1.25f;

        cube.transform.localScale = new Vector3(spacing, spacing, spacing);

        for (float i = -bound; i < bound; i += UnityEngine.Random.Range(spacingMin, spacingMax)) {
            for (float j = -bound; j < bound; j += UnityEngine.Random.Range(spacingMin, spacingMax)) {
                for (float k = -bound; k < bound; k += UnityEngine.Random.Range(spacingMin, spacingMax)) {
                    if (Similar(leftModel.Evaluate(i, k, j), rightModel.Evaluate(i, k, j))) {
                        if (handHeld) {
                            SpawnCube(new Vector3(i, j, k));
                        } else {
                            points.Add(new Vector3(i, j, k));
                            indices.Add(count);
                        }
                        count++;
                        //recentObjects.Add(Instantiate(cube, new Vector3(i, j, k), Quaternion.identity));
                    }
                }
            }
            progressText.text = ((int)((i + bound) / (bound * 2) * 100)).ToString() + "%";

            //note: iOS doesn't support Eric's shader rn for some reason
            if (!handHeld) {
                CreateMesh();
            }

            if (index % 2 == 0)
                yield return null;

            index++;
        }
        progressText.text = "100%";
        yield return null;
    }

    List<Color> colors = new List<Color>();
    float magnitude;
    public Gradient gradient;
    float value;

    private void SpawnCube(Vector3 position) {
        GameObject g = Instantiate(cube, position, Quaternion.identity);
        Color color = new Color((float)Math.Sqrt((position.x / bound + 1) / 2), (float)Math.Sqrt((position.y / bound + 1) / 2), (float)Math.Sqrt((position.z / bound + 1) / 2));

        g.GetComponent<MeshRenderer>().material.color = color;
        lastCubes.Add(g);
    }
    private void CreateMesh() {
        mesh.Clear();
        colors.Clear();
        mesh.vertices = points.ToArray();
//        print(points.Capacity);
//        print(mesh.vertexCount);
        for (int i = 0; i < mesh.vertexCount; i++) {
            magnitude = points[i].magnitude;
            value = (float)Math.Sqrt(3 * MathF.Pow(bound, 2)) / 2;
            //colors.Add(gradient.Evaluate(magnitude / 2.598f));
            colors.Add(new Color((float)Math.Sqrt((points[i][0] / bound + 1) / 2), (float)Math.Sqrt((points[i][1] / bound + 1) / 2), (float)Math.Sqrt((points[i][2] / bound + 1) / 2)));
            //colors.Add(new Color((magnitude / value)/1.5f, (1 - (magnitude / value))/1.5f, MathF.Sqrt((points[i][1]/bound+1)/2)));
        }
        mesh.colors = colors.ToArray();
        mesh.SetIndices(indices.ToArray(), MeshTopology.Points, 0);
    }

}
