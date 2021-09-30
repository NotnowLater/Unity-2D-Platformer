using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    [Range(2,10)]
    public float smoothFactor;
    private void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, target.position + offset, smoothFactor * Time.fixedDeltaTime);
    }
}
