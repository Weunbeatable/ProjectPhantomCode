using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemyAttack 
{

    [SerializeField] private List<string> attackList = new List<string>();


    public List<string> GetAttackList() => attackList;
}
