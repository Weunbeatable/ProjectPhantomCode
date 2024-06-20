using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpecialAttack : PlayerBaseState
{
    public PlayerSpecialAttack(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
    }

    public override void Tick(float deltaTime)
    {
        ComboInput input = null; // assign a light or heavy attack to an enum
        if (stateMachine.InputReader.isBasicAttack) 
        input = new ComboInput(InputReader.AttackInputType.light);
        if (stateMachine.InputReader.isHeavyAttack)
            input = new ComboInput(InputReader.AttackInputType.heavy);
    }
    public override void Exit()
    {
        

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
