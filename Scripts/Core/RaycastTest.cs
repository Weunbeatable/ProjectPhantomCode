using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastTest : MonoBehaviour
{
    /// <summary>
    /// Test concept using raycasts at various angles to figure out objects in front of the player. rays will shoot at
    /// different angles and heights to grant better understanding of what kind of parkour should be done at what point. 
    /// </summary>
    public GameObject lastHit;
    public Vector3 collision =  Vector3.forward;
    public int Distance;
    Vector3 angle = new Vector3(0, 1, 0);
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /*   Vector3 upRayRotation = Quaternion.AngleAxis(45f, this.transform.forward) * transform.up ;
           Vector3 placeholder = this.transform.position;
           placeholder.y = 2f;
           var ray = new Ray(this.transform.position,  upRayRotation);

           RaycastHit hit;
           if(Physics.Raycast(ray, out hit, Distance))
           {
               lastHit = hit.transform.gameObject;
               collision = hit.point;
               //Vector3.Reflect(lastHit.transform.forward, collision);
           }

           */

        Ray ray = new Ray(transform.position, transform.forward);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(this.transform.position, collision);
        Gizmos.DrawRay(this.transform.position, collision);
       
    }
}
