using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameScript : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Transform[] points;
    public Transform BezierMiddle;
    public Transform Disc;
    public GameObject BezierPoint;
    public List<GameObject> BezierPoints;
    public GameObject AllPoint;

    public GameObject winPanel;
    public Button winRestart;

    public Transform point1;
    public Transform point2;

    private int numPoints = 20;
    private Vector3[] positions = new Vector3[20];
    private Vector3 StartMousePosition;
    private Vector3 EndMousePosisiton;
    private float middleX;
    private float middleZ;
    private float distance;
    private bool go;
    private int a = 0;
    private int b = 1;
    private int c = 0;


    private Transform targetPos;
    private Transform targetRot;
    public Transform pointLookTarget;
    public GameObject gameCam;
    public Vector3 dist;
    public Vector3 sPos;
    public Quaternion targetRotation;

    private void Start()
    {
        Time.timeScale = 1;
        lineRenderer.positionCount = numPoints;
        point1 = points[0];
        point2 = points[b];
        middleX = (point1.position.x + point2.position.x) / 2;
        middleZ = (point1.position.z + point2.position.z) / 2;
        lineRenderer.gameObject.SetActive(false);
        AllPoint.gameObject.SetActive(false);
        winPanel.SetActive(false);
        winRestart.onClick.AddListener(() => restartbutton());
        for (int d = 0; d < points.Length; d++)
        {
            c++;
        }
        for (int f = 0; f < positions.Length; f++)
        {
            GameObject newPoint = Instantiate(BezierPoint);
            newPoint.transform.parent = AllPoint.transform;
            BezierPoints.Add(newPoint);
        }
        targetPos = point1;
        targetRot = Disc;
    }

    private void Update()
    {
        if (Time.timeScale == 1)
        {
            if (Input.GetMouseButtonDown(0))
            {
                MouseDown();
            }
            if (Input.GetMouseButton(0))
            {
                MousePressed();
            }
            if (Input.GetMouseButtonUp(0))
            {
                MouseUp();
            }
            DrawQuadraticCurve();
            DiscPath();
        }
        pointLookTarget.position = new Vector3(0, point1.position.y, point1.position.z);
    }

    private void FixedUpdate()
    {
        DiscMovement();
        DiscMovementPos();
    }

    private void LateUpdate()
    {
        Vector3 dPos = targetPos.position + dist;
        sPos = Vector3.Lerp(gameCam.transform.position, dPos, 5f * Time.deltaTime);
        targetRotation = Quaternion.LookRotation(targetRot.position - gameCam.transform.position);
        gameCam.transform.position = sPos;
        gameCam.transform.rotation = Quaternion.Slerp(gameCam.transform.rotation, targetRotation, 5f * Time.deltaTime);
    }

    private void restartbutton()
    {
        SceneManager.LoadScene("GameScene");
    }

    private void MouseDown()
    {
        StartMousePosition = Camera.main.ScreenToViewportPoint(Input.mousePosition);
    }

    private void MousePressed()
    {
        if (go == false)
        {
            EndMousePosisiton = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            distance = (EndMousePosisiton - StartMousePosition).magnitude;
            AllPoint.gameObject.SetActive(true);
            if (StartMousePosition.x < EndMousePosisiton.x)
            {
                BezierMiddle.position = new Vector3(middleX + distance * -6f, BezierMiddle.position.y, BezierMiddle.position.z);
            }
            else
            {
                BezierMiddle.position = new Vector3(middleX + distance * 6f, BezierMiddle.position.y, BezierMiddle.position.z);
            }
            for (int g = 0; g < BezierPoints.Count; g++)
            {
                BezierPoints[g].transform.position = new Vector3(positions[g].x, BezierPoint.transform.position.y, positions[g].z);
            }
            dist.z = -5;
            dist.y = 7;
            targetPos = pointLookTarget;
            targetRot = pointLookTarget;
        }
    }

    private void MouseUp()
    {
        AllPoint.gameObject.SetActive(false);
        go = true;
        dist.z = -3;
        dist.y = 3;
        targetPos = point1;
        targetRot = Disc;
    }

    private void DiscMovement()
    {
        if (go)
        {
            Disc.position = Vector3.MoveTowards(Disc.position, positions[a], Time.deltaTime * 10f);
        }
    }

    private void DiscMovementPos()
    {
        if (go && Disc.position == positions[a])
        {
            a++;
            if (positions.Length <= a)
            {
                go = false;
                b++;
                point1 = point2;
                point2 = points[b];
                BezierMiddle.position = new Vector3(0, BezierMiddle.position.y, BezierMiddle.position.z);
                middleX = (point1.position.x + point2.position.x) / 2;
                middleZ = (point1.position.z + point2.position.z) / 2;
                BezierMiddle.position = new Vector3(middleX, BezierMiddle.position.y, middleZ);
                a = 0;
                targetPos = point1;
            }
        }
    }

    private void DiscPath()
    {
        if (Disc.position.x == points[c - 1].position.x)
        {
            winPanel.SetActive(true);
            Time.timeScale = 0;
        }
    }

    private void DrawQuadraticCurve()
    {
        for (int i = 1; i < numPoints + 1; i++)
        {
            float t = i / (float)numPoints;
            positions[i - 1] = CalculateQuadraticBezierPoint(t, point1.position, BezierMiddle.position, point2.position);
        }
        lineRenderer.SetPositions(positions);
    }

    private Vector3 CalculateQuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        Vector3 p = uu * p0;
        p += 2 * u * t * p1;
        p += tt * p2;
        return p;
    }
}

