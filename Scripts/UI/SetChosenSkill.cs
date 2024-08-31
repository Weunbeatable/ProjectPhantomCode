using DG.Tweening;
using PFF.core;
using PFF.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class SetChosenSkill : MonoBehaviour
{
    //TODO: if a finisher is active prevent access to this UI


    [SerializeField] GameObject skillslotList;
    //Reference to player statemachine
    public PlayerStateMachine machine;

    //audio source for UI 
    AudioSource selectSound;
    public Sprite defaultSprite;

    public List<Image> skillSpriteList;
    public List<string> skillNameList;

    Action skillAction;

    int currentSkill;
    float selectionTimer = 0.0f;
    float nextItemTimer;
    bool canStartTimer;
    float refValue;
    private void Awake()
    {
        for (int i = 0; i < skillNameList.Count; i++)
        {
            skillNameList[i] = null;
        }
        selectSound = GetComponent<AudioSource>();
        PauseMenu1.onMenuPaused += Pausemenu1_onMenuPaused;
        PauseMenu1.onMenuClosed += PauseMenu1_onMenuClosed;
    }
    private void OnEnable()
    {
       
    }

    private void OnDisable()
    {
        
    }
    private void Pausemenu1_onMenuPaused(object sender, EventArgs e)
    {
        this.gameObject.SetActive(false);
    }
    private void PauseMenu1_onMenuClosed(object sender, EventArgs e)
    {
        this.gameObject.SetActive(true);
        for (int i = 0; i < skillSpriteList.Count; i++)
        {
            if (skillslotList.transform.GetChild(i).transform.GetComponent<SkillSlots>().GetSprite() == null)
            {
                skillSpriteList[i].sprite = defaultSprite;
            }
            else
            {
                skillSpriteList[i].sprite = skillslotList.transform.GetChild(i).transform.GetComponent<SkillSlots>().GetSprite();
                // have a dictionary of skills that are key coded. On top of adding the sprite add the key code. 
                // so when scrolled up or down the method the keyCode can be looked up and crossreferenced in the dictionary.
                // the value will be the component to add
                //Hashtable hashtable = new Hashtable();
                
            }

            if (skillslotList.transform.GetChild(i).transform.GetComponent<SkillSlots>().GetName() == null)
            {
                skillNameList[i] = "";
            }
            else
            {
                skillNameList[i] = skillslotList.transform.GetChild(i).transform.GetComponent<SkillSlots>().GetName();
                Debug.Log("name is " + skillNameList[i]);
            }

        }

    }
   

    void Start()
    {
        currentSkill = -1;
        canStartTimer = false;
    }

    // Update is called once per frame
    void Update()
    {
        ChangeActiveSkill();

        if (nextItemTimer > 0)
        {
            nextItemTimer -= 1 * Time.deltaTime;
        }
        if (canStartTimer == true)
        {
            selectionTimer -= 1 * Time.deltaTime;
        }
        if (selectionTimer <= 0)
        {
            if (currentSkill != -1)
            {
                ChosenSkillVisual(currentSkill);
                ActivateChosenSkill(currentSkill);
            }
            canStartTimer = false;
            selectionTimer = 0.9f;
            //currentSkill = 0; // Create UI menu option to toggle reset or continuous cycle so players can choose what they like best 
        }
        for (int i = 0; i < skillSpriteList.Count; i++)
        {
        }
    }


    private void SelectStance(int _index)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(i == _index);
        }
    }

    private void ChangeActiveSkill()
    {
        if (machine.InputReader.LeftStanceEnabled && nextItemTimer <= 0)
        {
            if (currentSkill != -1)
            {
                RemoveChosenSkil(currentSkill);
            }
                if (currentSkill == -1) { 
                currentSkill = 0;
                nextItemTimer = 0.18f;
                selectionTimer = 1f;
                canStartTimer = true;
            }
            else
            {
                RemoveChosenSkil(currentSkill);
                currentSkill += 1;
                //SelectStance(currentSkill);
                if (currentSkill > transform.childCount -1)
                {
                    currentSkill = 0;
                }
                nextItemTimer = 0.18f;
                selectionTimer = 1f;
                canStartTimer = true;
                // POST INCREMENT IS WHEN I SHOULD PASS VALUES
            }
            
        }
    }

    private void ActivateChosenSkill(int _index)
    {
        string skillName;
        skillName = skillNameList[_index];
        Debug.Log("Current index value is" + _index);
        Debug.Log($"{skillName}");
        if(skillName != null)
        {
            if (SkillsManager.Instance.GetAddSkillsDictionary().TryGetValue(skillName, out skillAction))
            {
                Debug.Log("Add Skill");
                skillAction.Invoke();
            }
        }
       
    }
    private void RemoveChosenSkil( int _index)
    {
        SkillsManager.Instance.RemoveAnyPhantomAbilities();
    }
    private void ChosenSkillVisual(int _chosenStance)
    {
        if (_chosenStance == 0)
        {
            Vector3 newPos = new Vector3(0f, 0f, -45f);
            transform.DORotate(newPos, 0.6f, RotateMode.Fast);
        }
        else if (_chosenStance == 1)
        {
            Vector3 newPos = new Vector3(0f, 0f, -135f);
            transform.DORotate(newPos, 0.6f, RotateMode.Fast);

        }
        else if (_chosenStance == 2)
        {
            Vector3 newPos = new Vector3(0f, 0f, -225f);
            transform.DORotate(newPos, 0.6f, RotateMode.Fast);

        }
        else if (_chosenStance == 3)
        {
            Vector3 newPos = new Vector3(0f, 0f, 45f);
            transform.DORotate(newPos, 0.6f, RotateMode.Fast);

        }
    }
}

