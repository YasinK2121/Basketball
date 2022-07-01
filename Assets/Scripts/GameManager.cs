using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject pathPoint;
    public GameObject allPathPoint;
    public GameObject basketball;
    public Rigidbody basketballRig;
    public GameObject pot;
    public List<GameObject> allPathPointList;
    public Transform[] players;
    public Transform[] enemys;
    public Transform playerHand;
    public Transform playerOne;
    public Transform playerTwo;
    public Animator playerOneAnim;
    public Animator playerTwoAnim;
    public Transform bezierMiddlePoint;
    public LineRenderer lineRenderer;
    public Slider powerBar;

    private Vector3[] linePointsPositions = new Vector3[30];
    private Vector3 startMousePosition;
    private Vector3 endMousePosisiton;
    private Vector3 ballForcePosition;
    private Vector3 ballAndPlayerDifferenceBetween;

    private float middleX;
    private float middleZ;
    private float mouseDistance;
    private int positionNumber;
    private int lineNumberOfPoints = 30;
    private int playerOrder = 1;
    private bool powerBarReduce;
    private bool powerBarReplicate;
    private bool turn;
    private bool potTurn;
    private bool Shoot;

    public float ballForcePower;
    public float ballUpPower;
    public bool canMove;

    public bool Win;
    public bool Lose;

    private Vector3 targetPos;
    private Transform targetRot;
    public Transform pointLookTarget;
    public GameObject gameCam;
    public Vector3 dist;
    public Vector3 sPos;
    public Quaternion targetRotation;

    void Start()
    {
        lineRenderer.positionCount = lineNumberOfPoints;
        playerOne = players[0];
        playerTwo = players[playerOrder];
        playerOneAnim = playerOne.GetComponent<Animator>();
        playerTwoAnim = playerTwo.GetComponent<Animator>();
        middleX = (playerOne.position.x + playerTwo.position.x) / 2;
        middleZ = (playerOne.position.z + playerTwo.position.z) / 2;
        bezierMiddlePoint.position = new Vector3(middleX, 0.5f, middleZ);
        lineRenderer.gameObject.SetActive(false);
        allPathPoint.gameObject.SetActive(false);
        powerBar.gameObject.SetActive(false);
        for (int f = 0; f < linePointsPositions.Length; f++)
        {
            GameObject newPoint = Instantiate(pathPoint);
            newPoint.transform.parent = allPathPoint.transform;
            allPathPointList.Add(newPoint);
        }
        for (int f = 0; f < players.Length; f++)
        {
            players[f].rotation = Quaternion.Euler(players[f].rotation.x, 180, players[f].rotation.z);
        }
        playerOne.rotation = Quaternion.Euler(playerOne.rotation.x, 0, playerOne.rotation.z);
        basketballRig.GetComponent<Rigidbody>();
        basketballRig.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
        playerOneAnim.SetBool("Bounc", true);
        playerHand = playerOne.Find("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Spine2/Bip001 Neck/Bip001 R Clavicle/Bip001 R UpperArm/Bip001 R Forearm/Bip001 R Hand").transform;
        ballAndPlayerDifferenceBetween = new Vector3(playerOne.position.x + 0.5f, playerHand.transform.position.y + 0.5f, playerOne.position.z + 0.5f);
        Shoot = true;
        Win = false;
        Lose = false;
        targetPos = new Vector3(0, playerOne.position.y, playerOne.position.z);
        targetRot = pointLookTarget.transform;
    }

    void Update()
    {
        //FallowPath
        if (Lose == false)
        {
            if (canMove == false && players.Length != playerOrder)
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
                pointLookTarget.position = new Vector3(0, playerOne.position.y, middleZ);
            }
            //PotShoot
            if (players.Length == playerOrder)
            {
                basketballRig.constraints = RigidbodyConstraints.None;
                powerBar.gameObject.SetActive(true);
                if (Input.GetMouseButton(0))
                {
                    PowerBarSet();
                }
                if (Input.GetMouseButtonUp(0))
                {
                    Shoot = false;
                    playerTwoAnim.SetBool("Shoot", true);
                    playerTwoAnim.SetBool("Idle", true);
                    playerTwoAnim.SetBool("Bounc", false);
                    BallShoot();
                    powerBar.value = 0;
                }
            }
            else
            {
                Vector3 ballPos = new Vector3(basketball.transform.position.x, playerTwo.position.y, basketball.transform.position.z);
                playerTwo.LookAt(ballPos);
            }
            if (Shoot)
            {
                BasketballMovement();
                BasketballMovementPath();
            }
            if (canMove == false && Shoot)
            {
                basketball.transform.position = new Vector3(ballAndPlayerDifferenceBetween.x, playerHand.transform.position.y + -0.35f, ballAndPlayerDifferenceBetween.z);
            }
            if (canMove == false && players.Length == playerOrder)
            {

            }
            if (linePointsPositions.Length - 15 == positionNumber)
            {
                playerTwoAnim.SetBool("Catch", true);
                playerOneAnim.SetBool("Pass", false);
            }
            if (turn && players.Length != playerOrder)
            {
                playerOne.Rotate(playerOne.rotation.x, playerOne.rotation.y * Time.deltaTime * -300, playerOne.rotation.z);
                if (playerOne.rotation.y <= 0)
                {
                    turn = false;
                }
            }
            if (potTurn && players.Length == playerOrder)
            {
                Vector3 potPosLook = new Vector3(pot.transform.position.x, playerOne.position.y, pot.transform.position.z);
                playerOne.Rotate(playerOne.rotation.x, potPosLook.y * Time.deltaTime * -10, playerOne.rotation.z);
                if (playerOne.rotation.y <= potPosLook.y)
                {
                    potTurn = false;
                }
            }
        }
        else
        {
            for (int a = 0; a < players.Length; a++)
            {
                players[a].GetComponent<Animator>().SetBool("Bounc", false);
                players[a].GetComponent<Animator>().SetBool("Pass", false);
                players[a].GetComponent<Animator>().SetBool("Idle", false);
                players[a].GetComponent<Animator>().SetBool("Catch", false);
                players[a].GetComponent<Animator>().SetBool("Shoot", false);
                players[a].GetComponent<Animator>().SetBool("Happy", false);
                players[a].GetComponent<Animator>().SetBool("SadIdle", true);
            }
            for (int a = 0; a < enemys.Length; a++)
            {
                enemys[a].GetComponent<Animator>().SetBool("Happy", true);
                enemys[a].GetComponent<Animator>().SetBool("Idle", false);
                enemys[a].GetComponent<Animator>().SetBool("SadIdle", false);
                enemys[a].GetComponent<Enemy>().go = false;
            }
            if (players.Length == playerOrder)
            {
                targetPos = new Vector3(playerOne.position.x, playerOne.position.y, playerOne.position.z);
                targetRot = playerOne;
                dist.x = 0;
                dist.y = 3;
                dist.z = 5;
            }
            powerBar.gameObject.SetActive(false);
        }
        if (Win)
        {
            for (int a = 0; a < players.Length; a++)
            {
                players[a].GetComponent<Animator>().SetBool("Bounc", false);
                players[a].GetComponent<Animator>().SetBool("Pass", false);
                players[a].GetComponent<Animator>().SetBool("Idle", false);
                players[a].GetComponent<Animator>().SetBool("Catch", false);
                players[a].GetComponent<Animator>().SetBool("Shoot", false);
                players[a].GetComponent<Animator>().SetBool("Happy", true);
                players[a].GetComponent<Animator>().SetBool("SadIdle", false);
            }
            for (int a = 0; a < enemys.Length; a++)
            {
                enemys[a].GetComponent<Animator>().SetBool("Block", false);
                enemys[a].GetComponent<Animator>().SetBool("Idle", false);
                enemys[a].GetComponent<Animator>().SetBool("SadIdle", true);
                enemys[a].GetComponent<Animator>().SetBool("Happy", false);
                enemys[a].GetComponent<Enemy>().go = false;
            }
            powerBar.gameObject.SetActive(false);
        }
    }

    private void LateUpdate()
    {
        Vector3 dPos = targetPos + dist;
        sPos = Vector3.Lerp(gameCam.transform.position, dPos, 5f * Time.deltaTime);
        targetRotation = Quaternion.LookRotation(targetRot.position - gameCam.transform.position);
        gameCam.transform.position = sPos;
        gameCam.transform.rotation = Quaternion.Slerp(gameCam.transform.rotation, targetRotation, 5f * Time.deltaTime);
    }

    private void PowerBarSet()
    {
        if (powerBarReplicate)
        {
            powerBar.value += 0.01f;
        }
        if (powerBarReduce)
        {
            powerBar.value -= 0.01f;
        }
        if (powerBar.value == 1)
        {
            powerBarReduce = true;
            powerBarReplicate = false;
        }
        if (powerBar.value == 0)
        {
            powerBarReduce = false;
            powerBarReplicate = true;
        }
    }

    private void MouseDown()
    {
        startMousePosition = Camera.main.ScreenToViewportPoint(Input.mousePosition);
    }

    private void MousePressed()
    {
        if (canMove == false)
        {
            endMousePosisiton = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            mouseDistance = (endMousePosisiton - startMousePosition).magnitude;
            allPathPoint.gameObject.SetActive(true);
            if (startMousePosition.x < endMousePosisiton.x)
            {
                bezierMiddlePoint.position = new Vector3(middleX + mouseDistance * -24f, bezierMiddlePoint.position.y, bezierMiddlePoint.position.z);
            }
            else
            {
                bezierMiddlePoint.position = new Vector3(middleX + mouseDistance * 24f, bezierMiddlePoint.position.y, bezierMiddlePoint.position.z);
            }
            for (int g = 0; g < allPathPointList.Count; g++)
            {
                allPathPointList[g].transform.position = new Vector3(linePointsPositions[g].x, 0.5f, linePointsPositions[g].z);
            }
            dist.y = 16;
            dist.z = -13;
            targetPos = new Vector3(0, playerOne.position.y, playerOne.position.z);
            targetRot = pointLookTarget;
        }
    }

    private void MouseUp()
    {
        allPathPoint.gameObject.SetActive(false);
        canMove = true;
        playerOneAnim.SetBool("Pass", true);
        playerOneAnim.SetBool("Idle", true);
        playerOneAnim.SetBool("Bounc", false);
        dist.y = 25;
        dist.z = -13;
        targetPos = new Vector3(0, playerOne.position.y, playerOne.position.z);
        targetRot = basketball.transform;
    }

    private void BallShoot()
    {
        ballForcePosition = pot.transform.position - basketball.transform.position;
        basketballRig.AddForce(ballForcePosition * ballForcePower * powerBar.value);
        basketballRig.AddForce(Vector3.up * ballUpPower * powerBar.value);
    }

    private void BasketballMovement()
    {
        if (canMove)
        {
            basketball.transform.position = Vector3.MoveTowards(basketball.transform.position, linePointsPositions[positionNumber], Time.deltaTime * 10f);
        }
    }

    private void BasketballMovementPath()
    {
        if (canMove && basketball.transform.position == linePointsPositions[positionNumber])
        {
            positionNumber++;
            if (linePointsPositions.Length <= positionNumber)
            {
                canMove = false;
                playerOrder++;
                playerOne = playerTwo;
                middleX = (playerOne.position.x + playerTwo.position.x) / 2;
                if (players.Length == playerOrder)
                {
                    potTurn = true;
                }
                else
                {
                    playerTwo = players[playerOrder];
                }
                middleZ = (playerOne.position.z + playerTwo.position.z) / 2;
                bezierMiddlePoint.position = new Vector3(middleX, 0.5f, middleZ);
                positionNumber = 0;
                playerHand = playerOne.Find("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Spine2/Bip001 Neck/Bip001 R Clavicle/Bip001 R UpperArm/Bip001 R Forearm/Bip001 R Hand").transform;
                ballAndPlayerDifferenceBetween = new Vector3(playerOne.position.x + 0.5f, playerHand.transform.position.y + 0.5f, playerOne.position.z + 0.5f);
                playerOneAnim = playerOne.GetComponent<Animator>();
                playerTwoAnim = playerTwo.GetComponent<Animator>();
                playerOneAnim.SetBool("Catch", false);
                playerOneAnim.SetBool("Bounc", true);
                turn = true;
                targetPos = new Vector3(0, playerOne.position.y, playerOne.position.z);
                targetRot = pointLookTarget.transform;
                if (players.Length == playerOrder)
                {
                    targetPos = new Vector3(playerOne.position.x, playerOne.position.y, playerOne.position.z);
                    targetRot = pot.transform;
                    dist.x = -1;
                    dist.y = 1;
                    dist.z = -5;
                }
            }
        }
    }

    private void DrawQuadraticCurve()
    {
        for (int i = 1; i < lineNumberOfPoints + 1; i++)
        {
            float t = i / (float)lineNumberOfPoints;
            linePointsPositions[i - 1] = CalculateQuadraticBezierPoint(t, new Vector3(playerOne.position.x, 0.5f, playerOne.position.z), new Vector3(bezierMiddlePoint.position.x, 0.5f, bezierMiddlePoint.position.z), new Vector3(playerTwo.position.x, 0.5f, playerTwo.position.z));
        }
        lineRenderer.SetPositions(linePointsPositions);
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
