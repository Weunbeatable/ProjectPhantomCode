using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class Combo : MonoBehaviour
{
    public List<ComboInput> inputs;
    public Attack ComboAttack;
    public UnityEvent onInput;
   // FightingCombo fightingCombo;
    int curInput = 0;

    public bool continueCombo(ComboInput i)// check to see if we can continue  combo or not
    {
        if (inputs[curInput].isSameAs(i)) // in the future fo rinput Add && i.movement == inputs[curInput].movement
        {
            curInput++;
            if(curInput >=inputs.Count) // finished inputs and we should do the attack i.e if we reach the thresholdl for that required combo, do the attack
            {
                onInput.Invoke();
                curInput = 0;
            }
            return true;
        }
        else
        {
            curInput = 0; // if combo isn't next in sequence
            return false; // restart the combo
        }
    }

    public ComboInput currentComboInput()
    {
        if (curInput >= inputs.Count) return null; 
        return inputs[curInput];
    }

    public void ResetCombo()
    {
        curInput = 0; // reset our combo counter to 0
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
