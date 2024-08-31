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

    int count = 5;
   [SerializeField] public Transform updatedRaycastPosition;
    Vector3 takenPos; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        takenPos = updatedRaycastPosition.forward;
        origin = transform.position;
        direction = transform.forward;
        RaycastHit hit;
        Ray[] queryrays = new Ray[count];
        if(Physics.SphereCast(origin, sphereRadius, direction, out hit, maxDistance, parkour, QueryTriggerInteraction.Ignore))
        {
            lasthit = hit.transform.gameObject;
            collision = hit.point;
            //Vector3.Reflect(lasthit.transform.forward, collision);
           // Debug.Log("current object hit is" + lasthit.layer);

        }
        for(int i = 0; i < count; i++)
        {
            takenPos.y += 20f;
            Debug.DrawRay(updatedRaycastPosition.position, transform.forward, Color.green);
            

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


/*
      Vector3 updatedRaycastPosition = stateMachine.characterController.transform.position;
        for (int i  = 0; i < hits.Length; i++)
        {
            hits = Physics.RaycastAll(updatedRaycastPosition, stateMachine.characterController.transform.forward, 15.0F);
            Debug.DrawRay(updatedRaycastPosition, stateMachine.characterController.transform.forward, Color.green);
            updatedRaycastPosition.y += 3f;
            
        }
        
        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];
            Debug.Log(hit.transform.name);
        }*/
}
