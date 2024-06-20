using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
/// <summary>
/// purpose of this class is to make it possible to to easily select finishers and appropriate finished animation
/// Enums to select the appropriate Combat style 
/// e.g. Assasin combat type will trigger the use of animation calls from the assassin fighting style allowing a 
/// cascading effect of other attributes attributed with that particular combat typw.
/// Once an enum is picked, a string reference to the animation will be called.
/// The Finisher string reference will have a coressponding enemy Finished string reference. 
/// A class containing a list of the finisher class and other supporting properties will also be called.
/// A random value will be selected from the list allowing for unique finishers variety. 
/// The main Finisher class will run the operation with supporting classes also running within this script.
/// The choice of approach for this system is to allow for flexibility, with the hopes of expanding on 
/// this system to allow for enemies to do finishers on the player as well.
/// </summary>
public class Finisher : MonoBehaviour
{
    [SerializeField] public List<SetupFinishers> finishers;
    private void Start()
    {
       // Debug.Log("current list size is " + finishers.Count);
    }

}

[Serializable]
public class SetupFinishers
{
    public enum FightingStyle { PhantomFighter = 0, PhantomGunslinger = 1, PhantomGreatSword = 2, PhantomAssassin = 3 }
    public FightingStyle chosenFightingStyle;
    [field: SerializeField] public string FinisherName { get; private set; }   // I'll make these strings public to make it easier to call on them in the finisher class. 
    [field: SerializeField] public string FinishedName { get; private set; }
    private int currentFightingStyle;
    public SetupFinishers(FightingStyle style, string FinisherName, string FinishedName) // setup constructor to take values assigned at start. 
    {
        style = this.chosenFightingStyle;
        FinisherName = this.FinisherName;
        FinishedName = this.FinishedName;
    }

    public int GetCurrentFightingStyleValue()
    {
        if(chosenFightingStyle == FightingStyle.PhantomFighter)
        {
            currentFightingStyle = 0;
        }
        if (chosenFightingStyle == FightingStyle.PhantomGunslinger)
        {
            currentFightingStyle = 1;
        }
        if (chosenFightingStyle == FightingStyle.PhantomGreatSword)
        {
            currentFightingStyle = 2;
        }
        if (chosenFightingStyle == FightingStyle.PhantomAssassin)
        {
            currentFightingStyle = 3;
        }
        return currentFightingStyle;
    }
}
