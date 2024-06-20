using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ForceReceiver : MonoBehaviour
{
    [SerializeField] private CharacterController controller;
    [SerializeField] private NavMeshAgent agent;

    [SerializeField] private float drag = 0.3f;

    private float verticalVelocity;

    private Vector3 impact;

    private ForceMode force;

    private Vector3 dampingVelocity; // pass by reference

    public Vector3 Movement => impact + Vector3.up * verticalVelocity; // the => is so we return a value
    private void Update()
    {
        if(verticalVelocity < 0f && controller.isGrounded)
        {
            verticalVelocity = Physics.gravity.y * Time.deltaTime - 0.5f; // Here we are setting our velocity to that value so its a better solution than setting velocity to 0f to avoid issues with animations when falling from any height.
        }
        else
        {
            verticalVelocity += Physics.gravity.y * Time.deltaTime; // Here we are falling so we will keep falling
        }
        impact = Vector3.SmoothDamp(impact, Vector3.zero, ref dampingVelocity, drag);

        if (agent != null)
        {
            if (impact.sqrMagnitude <= 0.2f * 0.2f)
            {
                impact = Vector3.zero;
                agent.enabled = true; // turn the navmesh back on after hit. avoid nav mesh fighting with knockback
            }
        }
    }

    internal void Reset()
    {
        impact = Vector3.zero;
        verticalVelocity = 0f;
    }
    //TODO ADD VERTICAL KNOCKBACK
    public void AddForce(Vector3 force)
    {
        impact += force;
        if(agent != null) // we check if we have an agent because the player doesn't have one i.e. if we are an enemy
        {
            agent.enabled = false;
        }
    }
    public void AddVerticalForce(Vector3 force)
    {
        verticalVelocity += force.y;
        force.y = 32;
        //force.x = 10;
        force.z = 10;
        impact += force;
        if (agent != null) // we check if we have an agent because the player doesn't have one i.e. if we are an enemy
        {
            agent.enabled = false;
        }
    }
    public void AddFlybackForce(Vector3 force)
    {
        verticalVelocity += force.y;
        force.y = 12;
        
        force.z = 50;
        impact += force;
        if (agent != null) // we check if we have an agent because the player doesn't have one i.e. if we are an enemy
        {
            agent.enabled = false;
        }
    }
    public void AddKnockdownForce(Vector3 force)
    {
        verticalVelocity += force.y;
        force.y = -6.5f;
        force.x = 2;
        force.z = 4;
        impact += force;
        if (agent != null) // we check if we have an agent because the player doesn't have one i.e. if we are an enemy
        {
            agent.enabled = false;
        }
    }
    public void WallRunForce(Vector3 crossProductOfWall, ForceMode forceMode)
    {
         impact = crossProductOfWall;

        force = forceMode;
    }

    public void WallJumpForce(Vector3 crossProductOfWall, ForceMode forceMode)
    {
        impact = crossProductOfWall;

        force = forceMode;
    }
    public void DashForce(Vector3 forceToApply, ForceMode forceMode)
    {
        impact = forceToApply;

        force = forceMode;
    }
    public void Jump(float jumpforce)
    {
        verticalVelocity += jumpforce;
    }

    // add functions for modifying knockback values, params should take values passed in from weapon damage that are public setters so I can modify in animation event. 
}
