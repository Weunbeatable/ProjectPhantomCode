using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class RadialMenu : MonoBehaviour
{
    [SerializeField]
    GameObject EntryPrefab;

    [SerializeField]
    float Radius = 200f; // for size of menu. Adjust numbers to shrink or grow how menu is spaced out

    [SerializeField]
    List<Texture> Icons; 
    
    //Stances added to radial menu
    List<RadialMenuEntry> Entries;

    RadialMenuEntry[] linearView;
    //Reference to player statemachine
   public PlayerStateMachine machine;

    // Menu Control logic
    bool ScrollActive = false;
    bool menuIsOpen = false;

    float cooldown;// cooldown to manage frequency of stance changing.
    //TODO: Audio integration to let the user know stances are  unavailable
    //TODO: UI intergration  to let users know when you can use UI again, (grey out if under CD, normal color if returned to state). 

 
    void Start()
    {
        Entries = new List<RadialMenuEntry>(); // list of all item entries
        cooldown = 0f; // cooldown to manage frequency of stance changing.      
    }
    private void Update()
    {
        if (machine.InputReader.StanceMenuActive && menuIsOpen == false)
        {
            Open();
        }

       
        if(cooldown > 0)
        {
            cooldown -= Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        if (menuIsOpen == true)
        {
            
            if (machine.InputReader.LeftStanceEnabled && cooldown <= 0f)
            {
                machine.FighterMode();
                cooldown = .5f; // Cooldown starts on opening because this should be incoorperated in case the user hasn't made up their mind yet.
                Close();
            }
            if (machine.InputReader.RightStanceEnabled && cooldown <= 0f)
            {
                machine.GunMode();
                cooldown = .5f;
                Close();
            }
            if (machine.InputReader.DownStanceEnabled && cooldown <= 0f)
            {
                machine.AssasinMode();
                cooldown = .5f;
                Close();
            }
            if (machine.InputReader.UpStanceEnabled && cooldown <= 0f)
            {
                machine.GreatSwordMode();
                cooldown = 
                    .5f;
                Close();
               
            }
        }
    }

    public bool ReturnScrollActive()
    {
        return ScrollActive;
    }

  public  void AddEntry(string pLabel, Texture pIcon, RadialMenuEntry.RadialMenuEntryDelegate pCallback) // function to add entries to the list, instantiate as gameobjects
    {
        GameObject entry = Instantiate(EntryPrefab, transform);

        RadialMenuEntry rme = entry.GetComponent<RadialMenuEntry>();
        rme.SetLabel(pLabel); // creating a pointer to radial menu entry in prefab, then adding to entries to keep track
        rme.SetIcon(pIcon);
        rme.SetCallback(pCallback);
        //rme.SetButton(pButton);
        Entries.Add(rme);
       
    }

    public void Open()
    {
        menuIsOpen = true;
        print("scrolled up");
        if (Entries.Count == 0)
            for (int i = 0; i < 1; i++) // the value of i will be extended with the more functionality added to this project. 
        {
            /*    AddEntry("Animations" + i.ToString(), Icons[i], ActivateAnimationPanel); // passing in buttons and icons from our 2 lists

                AddEntry("Effects" + i.ToString(), Icons[i+1], ActivateEffectPanel); // passing in buttons and icons from our 2 lists*/

            //AddEntry("Close" + i.ToString(), Icons[i +2], closeTheMenu); // passing in buttons and icons from our 2 lists
            AddEntry("Fighter", Icons[i], fighterMode);
            AddEntry("Gunner", Icons[i+1], gunMode);
            AddEntry("Great Sword", Icons[i + 2], Option3);
            AddEntry("Assasin", Icons[i + 3], Option4);
        }

        Rearrange();
    }
    
   public void Close()
   {
        menuIsOpen = false;
        for (int i = 0; i < Icons.Count; i++)
        {
            //Add animation for them to collapse towrds the center.
            RectTransform rect = Entries[i].GetComponent<RectTransform>();
            GameObject entry = Entries[i].gameObject;
            // also need to set delegate for a callback when action is complete to destroy cached entry.
            rect.DOAnchorPos(Vector3.zero, .3f).SetEase(Ease.OutQuad).onComplete = delegate ()
            {
                entry.SetActive(false);
            };
            //MouseAbilites(CurrentState);
        }
        Entries.Clear();
        ScrollActive = false;
        cooldown -= Time.deltaTime;

    }

 
    void closeTheMenu(RadialMenuEntry pentry)
    {
        Close();
    }


    void Rearrange()
    {
        float radiansOfSeperation = (Mathf.PI * 2) / Entries.Count; // This is to calculate how much we should turn essentially a full circle / entries (this is in radians)
        for(int i = 0; i < Entries.Count; i++)
        {
            float x = Mathf.Sin(radiansOfSeperation * i) * Radius;
            float y = Mathf.Cos(radiansOfSeperation * i) * Radius;

            // Adjusting objects position, using rect transform because its a UI element and anchored position to move parent with children object. 
            RectTransform rect = Entries[i].GetComponent<RectTransform>(); //caching rect transform

            rect.localScale = Vector3.zero;// make them invisible
            rect.DOScale(Vector3.one, .3f).SetEase(Ease.OutQuad).SetDelay(.05f * i);// animating them to scale 1 at .3 seconds and adding a setease delay to stagger things( just as nice animation)
            rect.DOAnchorPos(new Vector3(x, y, 0), .3f).SetEase(Ease.OutQuad).SetDelay(.05f * i); // animating the positions of the icons, each icon will be slightly slower than the last. ;
        }
    }

    void SetTargetIcon(RadialMenuEntry pEntry)
    {
        Debug.Log("work in progress");
    }

    void fighterMode(RadialMenuEntry pentry)
    {
        
         machine.FighterMode();
         Close();
    }

    void gunMode(RadialMenuEntry pentry)
    {

        machine.GunMode();
         Close();
    }
    void Option3(RadialMenuEntry pentry)
    {
        //machine.GreatSwordMode();
        Debug.Log("Option 3 has been sucessfully selected ");
        Close();
    }

    void Option4(RadialMenuEntry pentry)
    {
       // machine.AssasinMode();
        Debug.Log("Option 4 has been sucessfully selected ");
        Close();
    }

    public void LinearView()
    {
        if (menuIsOpen == false)
        {
            if (Entries.Count == 0)
                for (int i = 0; i < 1; i++) // the value of i will be extended with the more functionality added to this project. 
                {
                    /*    AddEntry("Animations" + i.ToString(), Icons[i], ActivateAnimationPanel); // passing in buttons and icons from our 2 lists

                        AddEntry("Effects" + i.ToString(), Icons[i+1], ActivateEffectPanel); // passing in buttons and icons from our 2 lists*/

                    //AddEntry("Close" + i.ToString(), Icons[i +2], closeTheMenu); // passing in buttons and icons from our 2 lists
                    AddEntry("Fighter", Icons[i], fighterMode);
                    AddEntry("Gunner", Icons[i + 1], gunMode);
                    AddEntry("Great Sword", Icons[i + 2], Option3);
                    AddEntry("Assasin", Icons[i + 3], Option4);
                }
        }
    }


   

}
   

