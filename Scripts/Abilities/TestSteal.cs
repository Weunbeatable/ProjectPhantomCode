using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSteal : MonoBehaviour
{
    public Collider Player;
    private void Awake()
    {
        Player.GetComponent<Collider>();
    }

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        
    }
}
