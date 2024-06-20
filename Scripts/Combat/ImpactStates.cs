using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PFF.Combat;

public class ImpactStates : MonoBehaviour
{
    new List<ImpactData> impactData;
}

public class ImpactData : MonoBehaviour
{
   [field: SerializeField] public string AnimationName { get; set; }
   public WeaponSO weaponSO;
    
}
