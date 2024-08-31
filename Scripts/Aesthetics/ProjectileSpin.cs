using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSpin : MonoBehaviour
{
    [SerializeField] public Vector3 projectileRotation;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(projectileRotation.x * Time.deltaTime, projectileRotation.y * Time.deltaTime, projectileRotation.z * Time.deltaTime);
    }
}
