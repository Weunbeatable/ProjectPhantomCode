using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    

   [SerializeField] private int health;
    private bool isInVulnerable = false;
    private string generate;

    public event Action onTakeDamage, onDie;
    public event Action<bool> onFinishable;

    public bool isDead => health == 0; // if isdead return the health value as 0. 
    void Start()
    {
       
        health = maxHealth;
    }

    public void setInVulnerable(bool isInvulnerable)
    {
        this.isInVulnerable = isInvulnerable;
    }
    public void DealDamage(int damageDealt)
    {
        //if (!isAlive) { return; }
        if(health <= 0)
        { return;  }

        if (isInVulnerable) { return; }
        
        {
            onTakeDamage?.Invoke();

            health = Mathf.Max(health - damageDealt, 0); // making sure our health doesn't go negative, if it does set it to 0 otherwise whatever your damage value wass

            //  check for health state, if at any point health becomes severly low invoke a finishable method where it is possible for the enemy to be  finished. 
            // additionally this opens the opportunity for a "damaged" state where player or enemy will move in a hurt fashion. 
           // if awkward beheavior occurs where you can do finishers in air just switch logic to add another bool where finishers are only done grounded (Unless air finishers are introduced...).
          onFinishable?.Invoke(CriticalHealthPercentage());
            

          if(health == 0)
            {
                onDie?.Invoke();
            }
           // Debug.Log(health);
        }

        
    }

    public void GenerateString(string generate)
    {
        this.generate = generate;
    }
    public bool CriticalHealthPercentage()
    {
        float remaining_Health_percentage = (float) health/maxHealth *100; // Dont forget to cast to float otherwise returning float value will default to 0 since health and max health are ints (learned that hte hard way)
        bool bro_Your_Health_Is_LOW;
       // Debug.Log("remaining health % is " + remaining_Health_percentage);
        if(remaining_Health_percentage <= 20f)
        {
            bro_Your_Health_Is_LOW = true;
        }
        else
        {
            bro_Your_Health_Is_LOW = false;
        }

        return bro_Your_Health_Is_LOW;
    }
    public int getCurrentHealth() => health;

    public float GetNormalizedHealth() => (float)health / maxHealth;

    public void SetHealth(int updatedHealthValue)
    {
        health = updatedHealthValue;
    }

    public bool GetVulnerabilityStatus() => isInVulnerable;
}
