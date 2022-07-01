using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public GameObject thisEnemyObject;
    public Transform thisEnemyTransform;
    public bool go;
    public Animator anim;
    private LayerMask layerMask;
    private int force;

    void Start()
    {
        thisEnemyObject = this.gameObject;
        thisEnemyTransform = this.gameObject.transform;
        anim = this.gameObject.GetComponent<Animator>();
        force = 3;
        if (go == false)
        {
            anim.SetBool("Idle", true);
        }
    }

    void Update()
    {
        RaycastHit RightHit;
        RaycastHit LeftHit;
        Vector3 RightForward = new Vector3(3, 0.5f, 0);
        Vector3 LeftForward = new Vector3(-3, 0.5f, 0);
        Vector3 enemyPos = new Vector3(thisEnemyTransform.position.x, 0.5f, thisEnemyTransform.position.z);
        Debug.DrawRay(enemyPos, RightForward, Color.green);
        Debug.DrawRay(enemyPos, LeftForward, Color.green);
        if (Physics.Raycast(enemyPos, RightForward, out RightHit, 3f))
        {
            if (RightHit.collider.transform.tag == "Wall")
            {
                force = -3;
            }
            if (RightHit.collider.transform.tag == "Ball")
            {
                go = false;
                this.gameObject.transform.localScale = new Vector3(1, this.gameObject.transform.localScale.y, this.gameObject.transform.localScale.z);
                anim.SetBool("Block", true);
            }
        }
        if (Physics.Raycast(enemyPos, LeftForward, out LeftHit, 3f))
        {
            if (LeftHit.collider.transform.tag == "Wall")
            {
                force = 3;
            }
            if (LeftHit.collider.transform.tag == "Ball")
            {
                go = false;
                this.gameObject.transform.localScale = new Vector3(-1, this.gameObject.transform.localScale.y, this.gameObject.transform.localScale.z);
                anim.SetBool("Block", true);
            }
        }
        if (go)
        {
            thisEnemyTransform.position = new Vector3(thisEnemyTransform.position.x + Time.deltaTime * force, thisEnemyTransform.position.y, thisEnemyTransform.position.z);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ball"))
        {
            anim.SetBool("Block", true);
        }
    }
}
