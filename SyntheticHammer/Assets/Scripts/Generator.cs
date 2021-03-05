using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class Generator : MonoBehaviour
{
    public float headScaleUpperbound;
    public float headScaleLowerbound;
    public float barScaleUpperbound;
    public float barScaleLowerbound;
    public float barRotationUpperbound;
    public float barRotationLowerbound;
    public int maxNum;
    public List<GameObject> hammers;
    public string directory;
    public List<GameObject> calibrationBalls;

    private int counter;
    private float headXScale;
    private float headYScale;
    private float headZScale;
    private float barXScale;
    private float barYScale;
    private float barZScale;
    private float barYRotation;
    private float barZRotation;
    private GameObject head;
    private GameObject bar;
    
    // Start is called before the first frame update
    void Start()
    {
        counter = 0;
        headXScale = 1.0f;
        headYScale = 1.0f;
        headZScale = 1.0f;
        barXScale = 1.0f;
        barYScale = 1.0f;
        barZScale = 1.0f;
        barYRotation = 0;
        barZRotation = 0;
        DirectoryInfo info = new DirectoryInfo(directory);
        if (!info.Exists)
        {
            Directory.CreateDirectory(directory);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (counter >= maxNum)
            return;
        
        // Get hammer type
        int thisIndex = counter % hammers.Count;

        // Deal with head and bar.
        Scaler[] scalers = hammers[thisIndex].GetComponentsInChildren<Scaler>();
        if (scalers.Length == 0 || scalers == null)
        {
            counter++;
            Debug.LogError(string.Format("GameObject {0} is invalid, please check!", hammers[thisIndex].name));
            return;
        }
        if (scalers[0].name.StartsWith("Head"))
        {
            head = scalers[0].gameObject;
            bar = scalers[1].gameObject;
        }
        else if (scalers[1].name.StartsWith("Head"))
        {
            head = scalers[1].gameObject;
            bar = scalers[0].gameObject;
        }
        else
        {
            counter++;
            Debug.LogError(string.Format("GameObject {0} is invalid, please check!", hammers[thisIndex].name));
            return;
        }

        // Generate random scales and rotation
        GenerateRandom();
        head.transform.localScale = new Vector3(headXScale, headYScale, headZScale);
        head.transform.localPosition = Vector3.zero;
        bar.transform.localScale = new Vector3(barXScale, barYScale, barZScale);
        bar.transform.localEulerAngles = new Vector3(
            bar.GetComponent<Scaler>().XRotation,
            bar.GetComponent<Scaler>().YRotation + barYRotation,
            bar.GetComponent<Scaler>().ZRotation + barZRotation
        );

        // For Cylinder, its height is default 2x to its scale.
        if (bar.name.Contains("Cylinder"))
        {
            bar.transform.localPosition = new Vector3(bar.transform.localScale.y, 0, 0);
        }
        // For Cube, its height is default 1x to its scale, and we didn't rotate it, so here's localScale.x
        else
        {
            bar.transform.localPosition = new Vector3(bar.transform.localScale.x / 2, 0, 0);
        }

        VisualizeKeypointCalibration();
        RecordKeypoint();

        MeshExporter.DoExport(hammers[thisIndex], counter, directory);
        counter++;
    }

    void GenerateRandom()
    {
        // head scale
        float originalHeadXScale = head.GetComponent<Scaler>().XScale;
        headXScale = Random.Range(originalHeadXScale * headScaleLowerbound, originalHeadXScale * headScaleUpperbound);
        float originalHeadYScale = head.GetComponent<Scaler>().YScale;
        headYScale = Random.Range(originalHeadYScale * headScaleLowerbound, originalHeadYScale * headScaleUpperbound);
        float originalHeadZScale = head.GetComponent<Scaler>().ZScale;
        headZScale = Random.Range(originalHeadZScale * headScaleLowerbound, originalHeadZScale * headScaleUpperbound);
        // bar scale
        float originalBarXScale = bar.GetComponent<Scaler>().XScale;
        barXScale = Random.Range(originalBarXScale * barScaleLowerbound, originalBarXScale * barScaleUpperbound);
        float originalBarYScale = bar.GetComponent<Scaler>().YScale;
        barYScale = Random.Range(originalBarYScale * barScaleLowerbound, originalBarYScale * barScaleUpperbound);
        float originalBarZScale = bar.GetComponent<Scaler>().ZScale;
        barZScale = Random.Range(originalBarZScale * barScaleLowerbound, originalBarZScale * barScaleUpperbound);
        // bar rotation
        barYRotation = Random.Range(barRotationLowerbound, barRotationUpperbound);
        barZRotation = Random.Range(barRotationLowerbound, barRotationUpperbound);
    }

    void VisualizeKeypointCalibration()
    {
        // Visualization calibration
        calibrationBalls[0].transform.localPosition = bar.transform.position;
        if (head.name.Contains("Cylinder"))
        {
            calibrationBalls[1].transform.localPosition = new Vector3(head.transform.position.x, head.transform.position.y + headYScale, head.transform.position.z);
            calibrationBalls[2].transform.localPosition = new Vector3(head.transform.position.x, head.transform.position.y + headYScale + 0.05f, head.transform.position.z);
        }
        else
        {
            calibrationBalls[1].transform.localPosition = new Vector3(head.transform.position.x, head.transform.position.y + headYScale / 2, head.transform.position.z);
            calibrationBalls[2].transform.localPosition = new Vector3(head.transform.position.x, head.transform.position.y + headYScale / 2 + 0.05f, head.transform.position.z);
        }
    }

    void RecordKeypoint()
    {
        StringBuilder stringBuilder = new StringBuilder();
        Vector3 barPosition = bar.transform.localPosition;
        Vector3 headPosition = head.transform.localPosition;
        stringBuilder.Append(string.Format("# Generated keypoints for hammer_{0:d}.obj\n", counter));
        stringBuilder.Append(string.Format("{0} {1} {2}\n", -barPosition.x, -barPosition.y, barPosition.z));
        if (head.name.Contains("Cylinder"))
        {
            stringBuilder.Append(string.Format("{0} {1} {2}\n", -headPosition.x, -1 * (headPosition.y + headYScale), headPosition.z));
            stringBuilder.Append(string.Format("{0} {1} {2}\n", -headPosition.x, -1 * (headPosition.y + headYScale + 0.05f), headPosition.z));
        }
        else
        {
            stringBuilder.Append(string.Format("{0} {1} {2}\n", -headPosition.x, -1 * (headPosition.y + headYScale / 2), headPosition.z));
            stringBuilder.Append(string.Format("{0} {1} {2}\n", -headPosition.x, -1 * (headPosition.y + headYScale / 2 + 0.05f), headPosition.z));
        }

        string fileName = Path.Combine(directory, string.Format("Hammer_{0}_kp.txt", counter));
        WriteToFile(stringBuilder.ToString(), fileName);
        Debug.Log("Export keypoint file: " + fileName);
    }

    static void WriteToFile(string s, string filename)
    {
        using (StreamWriter sw = new StreamWriter(filename))
        {
            sw.Write(s);
        }
    }
}
