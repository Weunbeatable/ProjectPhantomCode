using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackManager : MonoBehaviour
{
    [field: SerializeField] public List<AnimationClip> animationClips { get; set; }
    [field: SerializeField] public List<int> animationNamesHash { get; set; }

    [field: SerializeField] public Animator enemyAnimator { get; set; }

    private void Awake()
    {
        enemyAnimator = GetComponent<Animator>();
    }
    // if using animator controller instead, store a list of string names and when swapping the animator controller 
    // just pass that value with the controller and have it randomly chosen from the list 


}
