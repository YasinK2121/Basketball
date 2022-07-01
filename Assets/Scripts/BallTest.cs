using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallTest : MonoBehaviour
{
    public float power;
    public float uppower;
    public Rigidbody basketball;
    public Transform basketballtra;
    public GameObject pota;
    public Slider slider;
    public Vector3 gopos;
    public bool azalt;
    public bool cogalt;
    void Start()
    {
        basketball.GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            if (cogalt)
            {
                slider.value += 0.01f;
            }
            if (azalt)
            {
                slider.value -= 0.01f;
            }
            if (slider.value == 1)
            {
                azalt = true;
                cogalt = false;
            }
            if (slider.value == 0)
            {
                azalt = false;
                cogalt = true;
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            shoot();
            slider.value = 0;
        }
    }

    private void shoot()
    {
        gopos = pota.transform.position - basketballtra.position;
        basketball.AddForce(gopos * power * slider.value);
        basketball.AddForce(Vector3.up * uppower * slider.value);
    }
}
