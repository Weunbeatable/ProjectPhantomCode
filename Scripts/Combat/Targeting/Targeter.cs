using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using TMPro;

public class Targeter : MonoBehaviour
{
    public static event Action<Transform, Transform> OnUpdateLockOnIcon;
    public static event Action OnTurnOffLockOnIcon;


    [SerializeField] private CinemachineTargetGroup cineTargetGroup; // uses cinemacihine (quick actions refactor)
    [SerializeField] private CinemachineTargetGroup finisherTargetGroup;

    private Camera mainCamera;
    [SerializeField] private List<Target> targets = new List<Target>();

    private bool canFinish { get; set; }

    private void Start()
    {
        mainCamera = Camera.main;
        canFinish = false;
    }
    public Target currentTarget { get; private set; }
    private void OnTriggerEnter(Collider other)
    {
       if( !other.TryGetComponent<Target>(out Target target)) { return; } // Adding targets that entered the targeting field

        if (targets.Contains(target)) { return; } else
        {
            targets.Add(target);
        }
            
        target.OnDestroyed += RemoveTarget;
            FinshableTarget(other);
        
    }

    // may have to modify triggerStay if causing unwated behaviour while targeting. 
    private void OnTriggerStay(Collider other)
    {
        if (!other.TryGetComponent<Target>(out Target target)) { return; } // Adding targets that entered the targeting field
        /*
                if(currentTarget != null)
                {
                    if (targets.Contains(target))
                    {
                        Vector3 targetPosition = currentTarget.transform.position - target.transform.position;
                        Vector3 left = currentTarget.transform.right;
                        float angle = Vector3.SignedAngle(currentTarget.transform.position, targetPosition, left);
                        Debug.Log("Targets position is " + targetPosition);
                        Debug.Log("the signed angle of the next closest target is " + angle);
                        Debug.Log("Name of the other enemy is " + target.name);
                        // TURN THIS INTO A boolean check 
                        if(angle > 0f && angle < 180f)
                        {
                            Debug.Log("He is ON YOUR RIGHT!!!");
                        }
                        else
                        {
                           Debug.Log("He's on YOUR LEFT!!!");
                        }
                    }
                }*/
     
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent<Target>(out Target target)) { return; } // removing targets that have exited the targeter field

        RemoveTarget(target);
    }

    public bool SelectTarget() // boolean for knowing if there are targets or not.
    {
        if(targets.Count == 0) { return false; } // if there is no target don't bother 

        Target closestTarget = null; // if in the loop we dont find any targets it will still be  null
        float closestTargetDistance = Mathf.Infinity; // distance from screen center to closest target, using infinity to ensure we are always finding closest target


        foreach (Target target in targets)
        {
            Vector2 viewPos = mainCamera.WorldToViewportPoint(target.transform.position); // get each targets position
            if (viewPos.x < 0 || viewPos.x > 1 || viewPos.y < 0 || viewPos.y > 1) // if camera is in viewport check recently readded this block of code, may Remove. 
            {
                continue;
            }
            if (!target.GetComponentInChildren<Renderer>().isVisible) { continue; } // checking in child because parent gameobject doesn't have a renderer the children do

            Vector2 toCenter = viewPos - new Vector2(0.5f, 0.5f);
            if(toCenter.sqrMagnitude < closestTargetDistance)
            {
                closestTarget = target;
                closestTargetDistance = toCenter.magnitude;
            }
        }

        if(closestTarget == null) { return false; }

        currentTarget = closestTarget;
        if (targets.Contains(currentTarget))
        {
            cineTargetGroup.RemoveMember(currentTarget.transform);
            finisherTargetGroup.RemoveMember(currentTarget.transform);
        }
        // TODO: 
        // change priority of target switch, member is added and removed on look value going past a certain point. 
        cineTargetGroup.AddMember(currentTarget.transform, 1f, 2f);
        finisherTargetGroup.AddMember(currentTarget.transform, 1f, 1f);

        return true;
    }
    public void SwitchTarget(bool checkLeft, bool checkRight)
    {
        //TODO: 
        // Revamp target switching logic, it works... but its kind of spotty. 
        if (targets.Count == 0) { return; } // if there is no target don't bother 
        if (currentTarget == null) { return; }
      
       

    }

    public void Cancel()
    {
        if (currentTarget == null) { return; } // remove target from target group if already null

        cineTargetGroup.RemoveMember(currentTarget.transform);
        finisherTargetGroup.RemoveMember(currentTarget.transform);
        OnTurnOffLockOnIcon?.Invoke();
        currentTarget = null;
    }

    private void RemoveTarget(Target target)
    {
        if (currentTarget == target) // if current target is the same as the one that got out of range then remove it from the target list
        {
            cineTargetGroup.RemoveMember(currentTarget.transform);
            finisherTargetGroup.RemoveMember(currentTarget.transform);
            currentTarget = null;
        }
        // regardless of target that was destoryed we want to unsubscribe from this event
        target.OnDestroyed -= RemoveTarget;
        targets.Remove(target);
    }
    public void EmptyTarget()
    {
        currentTarget = null;
    }
    public void RemoveMultiTargets()
    {
        if(targets.Count == 0) return;
        else
        {
            targets.Clear();
        }
       
    }
    public void DoFOV(float endValue)
    {
        mainCamera.DOFieldOfView(endValue, 0.25f);
    }

    public void DoTile(float zTilt)
    {
        mainCamera.transform.DOLocalRotate(new Vector3(0, 0, zTilt), 0.25f);
    }

    public void FinshableTarget(Collider other)
    {
        if(other.TryGetComponent<Health>(out Health targetHealth))
        {
            targetHealth.onFinishable += targetHealth_onFinishable;
            Debug.Log("Can you actually finish" + canFinish);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.TryGetComponent<Health>(out Health targetHealth))
        {
            targetHealth.onFinishable -= targetHealth_onFinishable;
            Debug.Log("Can you actually finish" + canFinish);
        }
    }
    private void targetHealth_onFinishable(bool finishable)
    {
        if (finishable == true)
        {
            canFinish = true;
        }
        else if (finishable == false)
        {
            canFinish = false;
            return;
        }
        
    }

    public void ForceRemoveTarget(Target target)
    {
        if (currentTarget == target) // if current target is the same as the one that got out of range then remove it from the target list
        {
            cineTargetGroup.RemoveMember(currentTarget.transform);
            finisherTargetGroup.RemoveMember(currentTarget.transform);
            currentTarget = null;
        }
        // regardless of target that was destoryed we want to unsubscribe from this event
        target.OnDestroyed -= ForceRemoveTarget;
        targets.Remove(target);
    }

    public bool GetFinishState() => canFinish;
    public void SetFinishState(bool finish)
    {
        canFinish = finish;
    }

    public void RemoveDuplicates()
    {
        if (targets.Contains(currentTarget))
        {
            cineTargetGroup.RemoveMember(currentTarget.transform);
            finisherTargetGroup.RemoveMember(currentTarget.transform);
        }
    }

    public bool isDuplicate()
    {
        bool result;
        if (targets.Contains(currentTarget))
        {
            result = true;
        }
        else { result = false; }
        return result;
    }

    //TODO: maybe add some interpolation when switching targets so its not so jarring (possibly in the camera switching aspect). 
    public void CheckForTargetOnRight()
    {
        if (currentTarget != null)
        {
            if (targets.Count == 0) { return; }
            Target closestTarget = null;
            float closestTargetDistance = Mathf.Infinity; // our loop is going to compare which is closest, first target will always be the closest. so when we loop we will then get the next closest target.

            foreach (Target target in targets)
            {
                Vector3 viewPos = mainCamera.WorldToViewportPoint(target.transform.position);
                float angle = Vector3.Angle(this.transform.forward * -1f, transform.right);
                float targetsAngle = Vector3.Angle(this.transform.forward * -1f, target.transform.right * -1f);

                Debug.Log("the signed angle of the next closest target is " + targetsAngle);

                float closeDistanceCheck = Vector3.Distance( currentTarget.transform.forward, viewPos);
              /*  if (viewPos.x < 0 || viewPos.x > 1 || viewPos.y < 0 || viewPos.y > 1)
                {
                    continue;
                }*/
                if (viewPos.x >= 0.5f && viewPos.x < 1 && closeDistanceCheck < closestTargetDistance)
                {
                 closestTarget = target;
                        //closestTargetDistance = closeDistanceCheck;
                        if (closestTarget == null) { return; }
                        OnUpdateLockOnIcon?.Invoke(currentTarget.transform, closestTarget.transform);

                        if (targets.Contains(currentTarget))
                        {
                            cineTargetGroup.RemoveMember(currentTarget.transform);
                            finisherTargetGroup.RemoveMember(currentTarget.transform);
                        }

                        currentTarget = closestTarget;
                        cineTargetGroup.AddMember(currentTarget.transform, 1f, 2f);
                        finisherTargetGroup.AddMember(currentTarget.transform, 1f, 1f);
                        return;
                    
                    
                    Debug.Log("Name of the other enemy is " + target.name);
                    
                }//dlasfjlaskdjf

            }
            
           
           
            //currentTarget = closestTarget;

        }
    }
    public void CheckForTargetOnLeft()
    {
        if (currentTarget != null)
        {
            if (targets.Count == 0) { return; }
            Target closestTarget = null;
            float closestTargetDistance = Mathf.Infinity; // our loop is going to compare which is closest, first target will always be the closest. so when we loop we will then get the next closest target.

            foreach (Target target in targets)
            {
                Vector3 viewPos = mainCamera.WorldToViewportPoint(target.transform.position);
                float angle = Vector3.Angle(this.transform.forward * -1f, -transform.right);
                float targetsAngle = Vector3.Angle(this.transform.forward * -1f, viewPos);
                Debug.Log("your position is " + this.transform.position);
                Debug.Log("the signed angle of the next closest target is " + targetsAngle);

                float closeDistanceCheck = Vector3.Distance( currentTarget.transform.forward, viewPos);
              /*  if (viewPos.x < 0 || viewPos.x > 1 || viewPos.y < 0 || viewPos.y > 1)
                {
                    continue;
                }*/
                if(viewPos.x < 0.5f && viewPos.x >= 0f && closeDistanceCheck < closestTargetDistance)
                {
                   
                        closestTarget = target;
                        //closestTargetDistance = closeDistanceCheck;
                        if (closestTarget == null) { return; }
                        OnUpdateLockOnIcon?.Invoke(currentTarget.transform, closestTarget.transform);

                        if (targets.Contains(currentTarget))
                        {
                            cineTargetGroup.RemoveMember(currentTarget.transform);
                            finisherTargetGroup.RemoveMember(currentTarget.transform);
                        }
                        currentTarget = closestTarget;


                        cineTargetGroup.AddMember(currentTarget.transform, 1f, 2f);
                        finisherTargetGroup.AddMember(currentTarget.transform, 1f, 1f);
                        return;
                    }
                    
                    Debug.Log("Name of the other enemy is " + target.name);
                   
                }

               
                
            }
          
            //currentTarget = closestTarget;
          
        }
    }

