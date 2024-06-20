/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackType { light = 0, heavy = 1, leftSpecial = 2, rightSpecial = 3 }
public class FightingCombo : MonoBehaviour
{
    [Header("Attacks")]
    public Attack lightAttack;
    public Attack heavyAttack;
    public Attack leftSpecial;
    public Attack rightSpecial;

    public List<Combo> combos;
    public List<ComboInput> comboIn;



    [Header("Components")]
    Animator animator;

    Attack curAttack = null;
    ComboInput lastInput = null;
    float timer = 0;
    public float comboLeeway = 0.2f; // time between key presses
    float leeway = 0f;
    bool skip = false;

    InputReader checkInput; // keep track on inputs
    List<int> currentCombo = new List<int>();
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void PrimeCombos()
    {
        for (int i = 0; i < combos.Count; i++)
        {
            Combo c = combos[i];
            c.onInput.AddListener(() =>
            {
                // call Attack function with the combo's attack
                skip = true; // skipping frame so we dont skip a combo or call wrong one
                Attack(c.ComboAttack); // once input is invoked we attack using our combo attack;
                ResetCombos(); // if we finish combos reset rest of them            
            });
        }
        // Update is called once per frame

    }
    void Update()
    {
        if (curAttack != null)
        {
            if (timer > 0)
                timer -= Time.deltaTime;
            else
                curAttack = null;

            return;
        }

        if (currentCombo.Count > 0) // if combos are going through
        {
            leeway += Time.deltaTime; // leeway should increase if we are in a combo, but we will kick them out if they aren't doing the combo fast enough
            if (leeway >= comboLeeway) // if they take too long reset evrything and do last atack
            {
                if (lastInput != null)
                {
                    Attack(getAttackFromType(lastInput.type));
                    lastInput = null;
                }

                ResetCombos();
            }
        }
        else
            leeway = 0; // not accidentally resetting combos

        ComboInput input = null;
        if (checkInput.isBasicAttack)
            input = new ComboInput(AttackType.light);
        if (checkInput.isHeavyAttack)
            input = new ComboInput(AttackType.heavy);
        if (checkInput.isLeftSpecial)
            input = new ComboInput(AttackType.leftSpecial);
        if (checkInput.isRightSpecial)
            input = new ComboInput(AttackType.rightSpecial);

        if (input == null) { return; }
        lastInput = input;


        List<int> remove = new List<int>();
        for (int i = 0; i < currentCombo.Count; i++)
        {
            Combo C = combos[currentCombo[i]]; // one list is storing the index of a combo we are currently going through;
            if (C.continueCombo(input))
                leeway = 0; // if you're doing the right combo we won't kick you out of the combo so we'll just set leeway to 0;
            else
                remove.Add(i);
        }

        if (skip) // return back to false so we only skip one frame;
        {
            skip = false;
            return;
        }

        for (int i = 0; i < combos.Count; i++)
        {
            if (currentCombo.Contains(i)) continue; // if double i nput  is already in list we dont want to go ahead and check it
            if (combos[i].continueCombo(input)) // If not being checked already then we want to check for the combo
            {
                currentCombo.Add(i);
                leeway = 0;
            }
        }

        foreach (int i in remove)
            currentCombo.RemoveAt(i);

        if (currentCombo.Count <= 0)
            Attack(getAttackFromType(input.type)); // attack using one of thes attacks if combo counter is 0;
    }

    void ResetCombos()
    {
        leeway = 0;
        for (int i = 0; i < currentCombo.Count; i++)
        {
            Combo c = combos[currentCombo[i]]; // storing index of combos currently being inputted
            c.ResetCombo();
        }
        currentCombo.Clear();
    }
    void Attack(Attack att)
    {
        curAttack = att;
        timer = att.ComboAttackTime;
        animator.Play(att.AnimationName, -1, 0);
    }

    Attack getAttackFromType(AttackType type)
    {
        if (type == AttackType.light)
            return lightAttack;
        if (type == AttackType.heavy)
            return heavyAttack;
        if (type == AttackType.leftSpecial)
            return leftSpecial;
        if (type == AttackType.rightSpecial)
            return rightSpecial;

        return null;
    }

}
*/