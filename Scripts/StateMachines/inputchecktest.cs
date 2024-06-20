using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class inputchecktest : MonoBehaviour
{
  
    public PointList ListOfCombos = new PointList();
    public displayCombo comboTree = new displayCombo();
    
  
    [System.Serializable]
    public class Point
    {
        public List<Attack> NumOfInputs;
    }

    [System.Serializable]
    public class PointList
    {
        public List<Point> attackForInput;
        public List<InputReader> availableInputs; 
    }

    [System.Serializable]
    public class ComboManager
    {
        //instead fof  alist of inputs, pass the inputs as bools to the attack scripts like you did before. Then check your dictionary against the list of inputs.
        InputReader playerInput;
         List<bool> playerInputs = new List<bool>();
        public Attack[] currentAttack;
        Dictionary<List<bool>, Attack[]> womboCombo = new Dictionary<List<bool>, Attack[]>();
        public void createTree()
        {          
            if(playerInput.isBasicAttack == true)
            {
                playerInputs.Add(playerInput.isBasicAttack);
            }
            if (playerInput.isHeavyAttack)
            {
                playerInputs.Add(playerInput.isHeavyAttack);
            }
            if (playerInput.isLeftSpecial)
            {
                playerInputs.Add(playerInput.isLeftSpecial);
            }
            if (playerInput.isRightSpecial)
            {
                playerInputs.Add(playerInput.isRightSpecial);
            }
            else
                return;
            for(int i = 0; i < playerInputs.Count; i++)
            {
                for(int j = 0; j < currentAttack.Length; j++)
                {
                    womboCombo.Add(playerInputs, currentAttack);
                }
            }
        }

    }
    [System.Serializable]
    public class displayCombo
    {
        public List<ComboManager> comboTree = new List<ComboManager>();
    }

}
