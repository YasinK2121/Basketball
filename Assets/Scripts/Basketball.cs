using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Basketball : MonoBehaviour
{
    public GameManager gameManager;
    public Rigidbody ballBody;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            ballBody.constraints = RigidbodyConstraints.None;
            gameManager.canMove = false;
            gameManager.Lose = true;
        }
        if (other.gameObject.CompareTag("Pot"))
        {
            gameManager.Win = true;
        }
        if (other.gameObject.CompareTag("Wall"))
        {
            ballBody.constraints = RigidbodyConstraints.None;
            gameManager.canMove = false;
            gameManager.Lose = true;
        }
        if (other.gameObject.CompareTag("Wall2"))
        {
            gameManager.Lose = true;
        }
    }
}
