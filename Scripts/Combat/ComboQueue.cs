/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboQueue : MonoBehaviour
{
    *//*Goal
     * implement a system that cam be adjusted in the inspector
     * have a queue that checks an input with a timestamp
     * once an input has been entered check to see if the input was added within a timeframe e.g. 0.2 ms leeway
     * if the input is okay check to see if the inputs order matches an attack command
     * if the inputs match trigger  the special attack, otherwise empty the queue and refresh the combo queue
     *//*
     public enum AttackInputType { light = 0, heavy = 1, *//*leftSpecial = 2, rightSpecial = 3*//* }
   // public AttackInputType type; // can also add movement inputs for percise combos later
    Queue<InputReader> comboCommand = new Queue<InputReader>();
    InputReader checkInput;
    ComboInput lastInput = null;
    Attack basicAttack;
    Attack HeavyAttack;
    // public List<KeyMap> skillTree = new List<KeyMap>();

    public AttackInputType[] dork = new AttackInputType[0];
    public List<AttackInputType[]> KeyMap = new List<AttackInputType[]>();
   
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ComboInput input = null;
        if (checkInput.isBasicAttack)
        {
            input = new ComboInput(InputReader.AttackInputType.light);
        }

        if (checkInput.isHeavyAttack)
        {
            input = new ComboInput(InputReader.AttackInputType.heavy);
        }
        if(input == null) { return; }
        lastInput = input;
    }

    public bool returnInput(InputReader input)
    {
        return input;
    }

    public void tableTry()
    {
        Hashtable comboList = new Hashtable();
        Dictionary<List<AttackInputType>, Attack> ComboTree = new Dictionary<List<AttackInputType>, Attack>();
    }
    [System.Serializable]
    public class ComboMap
    {

    }
}
*/