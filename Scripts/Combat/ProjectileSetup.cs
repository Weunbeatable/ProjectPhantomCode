using System;
using System.Collections.Generic;
using UnityEngine;
// The purpose of this class is to use object pooling to store a set amount of objects that can be fired. 
// Launch the appropriate projectile when an event call has been made.
// This script should be shareable between player and enemy types as an event call is being made for the projectile to be launched.
// A condition for this script is that it must sit at the base layer of the respective player prefab.
// For now only one projectile type per instance of the script. More may be added to allow for extensibility
// A main objective of this script is to reduce cpu/gpu load by preloading a set number of projectiles each time. 
// This script is not intended to handle damage only direction, velocity and instantiation of a projectile. 
// This script takes a projectile prefab and listens for a call to fire. 
public class ProjectileSetup : MonoBehaviour
{
    [Header("Bullets")]
    public GameObject[] pooledProjectiles; // premade pool of objects
    public GameObject pooledObject; // object that will be replicated
    [SerializeField] public Transform launchPoint; // spawn point of projectile
    public int amountToPool; // controlled size of pool
   [SerializeField] public GameObject spawnPlace;
    public Vector3 updatePos;
    public Quaternion updateRot;

    //Assassin Throwables
    [Header("Assassin Tools")]
    [SerializeField]  private GameObject kunai_Throwable;
    [SerializeField]  private GameObject four_sided_Shuriken_Throwable;
    [SerializeField]  private GameObject throwing_Star_Throwable;
    public List<GameObject> assassinThrowables = new List<GameObject>();
    private int AssassinThrowablesPool = 15;

    [SerializeField] private PlayerStateMachine machine;
    void Start()
    {
        PopulatePool();
        PopulateAssassinThrowablesPool();
        machine.GetComponent<PlayerStateMachine>();
    }

    // Update is called once per frame
    void Update()
    {
        updatePos = launchPoint.transform.position;
        updateRot = launchPoint.transform.rotation;
    }



    public void UseAssassinThrowables()
    {
        // should take the first three items and launch the projectile. once each projectile has hit something or elapsed a certain period of time they should turn off and go back to the pool.
        // Objects ARE allowed to stick to walls. 
        // They will also be set on a timer if this happens. 
        // should also deactivate and go back into the pool if the pool of objects is about to be exhausted i.e. you are about to throw the last 3 objects in the pool. 
       /* for (int i = 0; i < 3; i++)
        {
           
        }*/
    }
    private void PopulateAssassinThrowablesPool()
    {
        if (assassinThrowables == null) { return; }
        GameObject tmp;
        for (int i = 0; i <= AssassinThrowablesPool; i++) // first three values will be prepopulated and all subsequent values will be enter the pool randomly. 
        {
            int nextItem = UnityEngine.Random.Range(1, 3);
            if (nextItem == 1)
            {
                tmp = Instantiate(kunai_Throwable);
                tmp.SetActive(false);
                assassinThrowables.Add(tmp);
            }
            else if (nextItem == 2)
            {
                tmp = Instantiate(four_sided_Shuriken_Throwable);
                tmp.SetActive(false);
                assassinThrowables.Add(tmp);
            }
            else if (nextItem == 3)
            {
                tmp = Instantiate(throwing_Star_Throwable);
                tmp.SetActive(false);
                assassinThrowables.Add(tmp);
            }

        }
    }
    private void PopulatePool()
    {
        pooledProjectiles = new GameObject[amountToPool];
        for (int i = 0; i < pooledProjectiles.Length; i++)
        {
            pooledProjectiles[i] = Instantiate(pooledObject, launchPoint.position, launchPoint.rotation);
            pooledProjectiles[i].SetActive(false);
        }
    }

    public void FirePooledProjectile()
    {
        foreach (GameObject gameObject in pooledProjectiles)
        {
            launchPoint.position = updatePos;
            launchPoint.rotation = updateRot;
            gameObject.SetActive(true);
            gameObject.GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * 45f);
        }
    }


    public void TestThrow()
    {
        float dice = UnityEngine.Random.Range(1, 3);

        if(dice == 1)
        {
            GameObject kunai = Instantiate(kunai_Throwable, launchPoint.position, Quaternion.identity);
            kunai.SetActive(true);
          /*  if (machine.targeter.currentTarget == null) { kunai.GetComponent<Rigidbody>().AddForce(kunai.transform.forward * 40, ForceMode.Impulse); } // make sure we have a target
            Vector3 facing = (machine.targeter.currentTarget.transform.position - kunai.transform.position); // subtract target from our postion
            facing.y = 0;// dont care about height so we clear it
            float angle = Vector3.Dot(kunai.transform.position, machine.targeter.currentTarget.transform.position);

            if (angle >= 0.25f)
            {
                kunai.transform.rotation = Quaternion.LookRotation(facing); // creating a vector 3 to a quaternion
            }*/
            
            kunai.GetComponent<Rigidbody>().AddForce(kunai.transform.position * 400 * Time.deltaTime, ForceMode.VelocityChange);
            //
           if(kunai.activeSelf == true)
            {
                kunai.transform.Rotate(0, 180, 0);
            }
        }
        else if(dice == 2)
        {
            GameObject four_Sided_Shuriken = Instantiate(four_sided_Shuriken_Throwable, launchPoint.position, Quaternion.identity);
           four_Sided_Shuriken.SetActive(true);
            /*if (machine.targeter.currentTarget == null) { four_Sided_Shuriken.GetComponent<Rigidbody>().AddForce(four_Sided_Shuriken.transform.forward * 80, ForceMode.Impulse); } // make sure we have a target
            Vector3 facing = (machine.targeter.currentTarget.transform.position - four_Sided_Shuriken.transform.position); // subtract target from our postion
            facing.y = 0;// dont care about height so we clear it
            float angle = Vector3.Dot(four_Sided_Shuriken.transform.position, machine.targeter.currentTarget.transform.position);

            if (angle >= 0.25f)
            {
                four_Sided_Shuriken.transform.rotation = Quaternion.LookRotation(facing); // creating a vector 3 to a quaternion
            }*/
            
            four_Sided_Shuriken.GetComponent<Rigidbody>().AddForce(four_Sided_Shuriken.transform.position * 320 * Time.deltaTime, ForceMode.Force);
            
            if(four_Sided_Shuriken.activeSelf == true)
            {
                four_Sided_Shuriken.transform.Rotate(0, 0, 180);
            }
        }
        else
        {
            GameObject throwing_Star = Instantiate(throwing_Star_Throwable, launchPoint.position, Quaternion.identity);
            throwing_Star.SetActive(true);
            //if (machine.targeter.currentTarget == null) { throwing_Star.GetComponent<Rigidbody>().AddForce(throwing_Star.transform.position * 40 * Time.deltaTime, ForceMode.Force); ; } // make sure we have a target
            /*  Vector3 facing = (machine.targeter.currentTarget.transform.position - throwing_Star.transform.position); // subtract target from our postion
              facing.y = 0;// dont care about height so we clear it
              float angle = Vector3.Dot(throwing_Star.transform.position, machine.targeter.currentTarget.transform.position);

              if (angle >= 0.25f)
              {
                  throwing_Star.transform.rotation = Quaternion.LookRotation(facing); // creating a vector 3 to a quaternion
              }*/
           
            throwing_Star.GetComponent<Rigidbody>().AddForce(throwing_Star.transform.position * 320 * Time.deltaTime, ForceMode.Force);
            
            if(throwing_Star.activeSelf == true)
            {
                throwing_Star.transform.Rotate(0, 0, 180);
            }
        }
    }
}
