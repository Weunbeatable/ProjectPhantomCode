using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parkour : MonoBehaviour
{
    public GameObject lasthit;
    public Vector3 collision = Vector3.zero;
    public float sphereRadius;
    public float maxDistance;
    public LayerMask parkour;

    private Vector3 origin;
    private Vector3 direction;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        origin = transform.position;
        direction = transform.forward;
        RaycastHit hit;
        if(Physics.SphereCast(origin, sphereRadius, direction, out hit, maxDistance, parkour, QueryTriggerInteraction.Ignore))
        {
            lasthit = hit.transform.gameObject;
            collision = hit.point;
            //Vector3.Reflect(lasthit.transform.forward, collision);
           // Debug.Log("current object hit is" + lasthit.layer);

        }
    }

    public int GetObjectParkourStatus()
    {
        int layersValue = lasthit.layer;
        return layersValue;
        
    }

    public float GetSlideDistance()
    {
        float slideDisctance = maxDistance;
        return slideDisctance;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(collision, sphereRadius);
    }

    public void ResetLastHit()
    {
        lasthit = null;
    }
}
