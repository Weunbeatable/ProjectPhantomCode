using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreventLanding : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {

        if (hit.gameObject.GetComponent<CharacterController>() != null)
        {
            Debug.Log("checking for interaction");
            CharacterController self = this.gameObject.GetComponent<CharacterController>();
            CharacterController character = hit.gameObject.GetComponent<CharacterController>();
            if (self.transform.position.y < character.transform.position.y)
            {
                Vector3 tempVal = self.transform.position;
                tempVal.y = 0f;
                Debug.Log("remove thy self knave");
                character.Move((character.transform.position - tempVal).normalized);
            }
        }

    }
}
