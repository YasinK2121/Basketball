using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoseWall : MonoBehaviour
{
    public GameManager gameManager;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            gameManager.Lose = true;
        }
    }
}
