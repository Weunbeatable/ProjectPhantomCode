using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PFF.Abilities;

/// <summary>
/// The rage ability is a universal ability with separate implementation depending on player vs enemy vs enemyType. 
/// The Rage ability/System will be a passive ingrown ability rather than an external input driven ability
/// meaning instead of soely relying on an ability trigger, the rage ability will be triggered depending on a multitude of factors.
/// such as health, status effects and even external stimulus (rage fog causing enemies to be put in a rage like state etc). 
/// because this is an interface, it can be easily applied to either type player or enemy with extensibility. 
/// </summary>
public class RageAbility : MonoBehaviour, IAbilites
{

    public void UseAbility()
    {
        throw new System.NotImplementedException();
    }

    public void UseAbility(BaseAbilitySystem baseAbility)
    {
        throw new System.NotImplementedException();
    }
}
public class DelayedDecorator : IAbilites
{
    public DelayedDecorator(IAbilites ability)
    {
        
    }

    public void Use(GameObject ability)
    {
        throw new System.NotImplementedException();
    }

    public void UseAbility()
    {
        throw new System.NotImplementedException();
    }

    public void UseAbility(BaseAbilitySystem baseAbility)
    {
        throw new System.NotImplementedException();
    }
}
