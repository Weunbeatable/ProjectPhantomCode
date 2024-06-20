using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] CharacterController characterController;
    private Collider[] allColliders;
    private Rigidbody[] allRigidbodies;
    void Start()
    {
        allColliders = GetComponentsInChildren<Collider>(true);// set to true so it gets disabled rigidbody components as well
        allRigidbodies = GetComponentsInChildren<Rigidbody>(true);

        ToggleRagdoll(false);
    }

    public  void ToggleRagdoll(bool isRagdoll)
    {
        foreach(Collider collider in allColliders)
        {
            if (collider.gameObject.CompareTag("Ragdoll")){
                collider.enabled = isRagdoll;
            }
        }

        foreach (Rigidbody rigidbody in allRigidbodies)
        {
            if (rigidbody.gameObject.CompareTag("Ragdoll")){ 
                rigidbody.isKinematic = !isRagdoll;
                rigidbody.useGravity = isRagdoll;
            }

            characterController.enabled = !isRagdoll;
            animator.enabled = !isRagdoll;
        }
    }
}
