using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float followSpeed;
    
    private Vector3 desiredPos;
    private void Awake()
    {
        desiredPos = transform.position;
    }
    private void Update()
    {
        desiredPos.y = PlayerMovement.instance.transform.position.y;
        transform.position += (desiredPos - transform.position) * followSpeed * Time.deltaTime;
    }
}
