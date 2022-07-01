using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Disc : MonoBehaviour
{
    public GameObject losepanel;
    public Button loserestart;
    public Button restart;

    void Start()
    {
        losepanel.SetActive(false);
        restart.onClick.AddListener(() => restartbutton());
        loserestart.onClick.AddListener(() => restartbutton());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            losepanel.SetActive(true);
            Time.timeScale = 0;
        }
    }

    private void restartbutton()
    {
        SceneManager.LoadScene("GameScene");
    }
}
