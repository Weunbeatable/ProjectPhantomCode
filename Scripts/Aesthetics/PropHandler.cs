using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropHandler : MonoBehaviour
{
    [SerializeField] public GameObject phoneProp;
    public PlayerStateMachine stateMachine;

    public GameObject FighterProps;
    public GameObject GunProps;
    public GameObject GreatSwordProps;
    public GameObject AssasinProps;
    public GameObject GreatWeapon;
    public GameObject Kunai;

    public List<GameObject> StanceProps;

    //Weapon enable
    public void EnablePhone()
    {
        phoneProp.SetActive(true);
    }

    public void DisablePhone()
    {
        phoneProp.SetActive(false);
    }

    private void Update()
    {
        if(phoneProp != null)
        {
            IfMoved();
        }
    }
    public void IfMoved()
    {
        if(stateMachine.InputReader.MovementValue.magnitude != 0)
        {
            DisablePhone();
        }
    }

    public void FightingProps()
    {
        FighterProps.SetActive(true);
        foreach (GameObject prop in StanceProps)
        {
            if (prop.name != FighterProps.name)
            {
              //  Debug.Log("Turning off " + prop.name);
                prop.SetActive(false);
            }
        }
    }

    public void ShootingProps()
    {
        GunProps.SetActive(true);
        foreach (GameObject prop in StanceProps)
        {
            if (prop.name != GunProps.name)
            {
               // Debug.Log("Turning off " + prop.name);
                prop.SetActive(false);
            }
        }
    }
    public void GreatSwordsmanProps()
    {
        GreatSwordProps.SetActive(true);
        
        foreach (GameObject prop in StanceProps)
        {
            if (prop.name != GreatSwordProps.name)
            {
              //  Debug.Log("Turning off " + prop.name);
                prop.SetActive(false);
            }
        }
        GreatWeapon.SetActive(true);
    }
    public void AssasinationProps()// delegate to turn off props like I did with clear busy?
    {
        AssasinProps.SetActive(true);
        foreach (GameObject prop in StanceProps)
        {
            if (prop.name != AssasinProps.name)
            {
               // Debug.Log("Turning off " +  prop.name);
                prop.SetActive(false);
            }
        }
        Kunai.SetActive(true);
    }

    public void TurnOffAllProps() 
    {
        foreach (GameObject gameObject in StanceProps)
        {
            gameObject.SetActive(false);
        }
    }
}
