using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using static DG.Tweening.DOTweenModuleUtils;

public class EnemyPlayerResponse : MonoBehaviour
{
    public static EventHandler<bool> OnAbovePlayer;
    public string fightController = "Controller_Player";
    public string gunController = "GunslingerStyle";
    public string greatSwordController = "GreatSword_Controller";
    public string AssasinController = "Assasin_Controller";

    public bool isRespondingToFighter;
    public bool isRespondingToGunner;
    public bool isRespondingToGreatSword;
    public bool isRespondingToAssassin;

    public string readValue { get; set; }

    [SerializeField] public EnemyStateMachine statemachine;

    [SerializeField] private LayerMask playerLlayer;
    [SerializeField] private GameObject playerObject { get; set; }

    bool isAbovePlayer;// = Physics.Raycast(ray, out _, 1f, playerPlayer);
    // Start is called before the first frame update
    void Start()
    {
        /* fightController = "Controller_Player";
         gunController = "GunslingerStyle";
         greatSwordController = "GreatSword_Controller";
         AssasinController = "Assasin_Controller";*/
        statemachine.GetComponent<EnemyStateMachine>();
}
    
    // Update is called once per frame
    void Update()
    {
        SwitchResponder(statemachine.player.GetComponent<Animator>().runtimeAnimatorController.name);
        if (isStandingOnPlayer() == true)
        {
            OnAbovePlayer?.Invoke(this, isAbovePlayer);
        }

      //  Debug.Log(readValue);
    }
    public bool SwitchResponder(string responseValue)
    {
        if(responseValue == fightController)
        {
            isRespondingToFighter = true;

            isRespondingToGunner = false;
            isRespondingToGreatSword = false;
            isRespondingToAssassin = false;

          
        }

        if (responseValue == gunController)
        {
            isRespondingToGunner = true;

            isRespondingToFighter = false;
            isRespondingToGreatSword = false;
            isRespondingToAssassin = false;

         
        }


        if (responseValue == greatSwordController)
        {
            isRespondingToGreatSword = true;

            isRespondingToGunner = false;
            isRespondingToFighter = false;
            isRespondingToAssassin = false;

           
        }


        if (responseValue == AssasinController)
        {
            isRespondingToAssassin = true;

            isRespondingToGunner = false;
            isRespondingToGreatSword = false;
            isRespondingToFighter = false;

           
        }
        return isRespondingToFighter;
        return isRespondingToAssassin;
        return isRespondingToGreatSword;
        return isRespondingToGunner;
    }

    public bool isStandingOnPlayer()
    {
        Ray ray = new Ray(this.transform.position, Vector3.down);

        Debug.DrawLine(ray.origin, ray.origin + Vector3.down * 3f, Color.red, 4f);
       
        
        if (Physics.Raycast(ray, out RaycastHit hit, 3f, playerLlayer))
        {
            isAbovePlayer = true;
            playerObject = hit.transform.gameObject;
        }
        else
        {
            isAbovePlayer = false;
        }
       
            return isAbovePlayer;
    }

    public GameObject GetPlayer() => playerObject;
    // Vector3 direction = (other.transform.position - myCollider.transform.position).normalized; Logic used for moving off players head 
}
