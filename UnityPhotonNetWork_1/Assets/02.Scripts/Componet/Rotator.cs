using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    public float rotSpeed = 60f;

    void Update()
    {
        transform.Rotate(0f, rotSpeed * Time.deltaTime, 0f);
    }
}
