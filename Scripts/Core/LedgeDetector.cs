using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeDetector : MonoBehaviour
{
    public event Action<Vector3, Vector3> onLedgeDetect; // raising event to detect ledge
    private void OnTriggerEnter(Collider other)
    {
        onLedgeDetect?.Invoke(other.transform.forward, other.ClosestPointOnBounds(transform.position)); // in this case its the closest point on ledge to hands and direction ledg is facing in, forward vector is blue arrow
    }
}
