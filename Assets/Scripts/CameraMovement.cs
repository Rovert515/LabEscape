using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Vector3 desiredPos;
    private void Awake()
    {
        desiredPos = transform.position;
    }
    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, desiredPos, 0.01f);
    }
}
