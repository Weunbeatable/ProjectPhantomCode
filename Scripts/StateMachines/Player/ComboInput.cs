using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ComboInput 
{
    public InputReader.AttackInputType type; // can also add movement inputs for percise combos later

    public ComboInput(InputReader.AttackInputType t) // Constructor
    {
        type = t;
    }
    public bool isSameAs(ComboInput test)
    {
        return (type == test.type); // in the future fo rinput Add && i.movement == inputs[curInput].movement
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
