using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Throw_SlashAbility : phantomAbilites
{
    // Start is called before the first frame update

    public override void Awake()
    {
        base.Awake();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Warp();
    }
    public void Warp()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            Debug.Log("THE CAKE IS A LIE");
           GetPhantomContainer().SetActive(true);
            if(GetTargeter().currentTarget != null)
            {
                transform.DOMove(GetTargeter().currentTarget.transform.position, 0.5f);
            }
            
        }
    }
}
