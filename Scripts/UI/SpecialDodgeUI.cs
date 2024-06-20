using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpecialDodgeUI : MonoBehaviour
{
    [SerializeField] List<Sprite> DirectionalInputImageIcons = new List<Sprite>();
    [SerializeField] Image DirectionalInputIcon;
    // Update is called once per frame
    void Update()
    {
        DetermineImageIconToDisplay();
    }

    private void DetermineImageIconToDisplay()
    {
        if (InputReference.Instance.GetDeviceName() == "Mouse" ||
            InputReference.Instance.GetDeviceName() == "Keyboard")
        {
            // grab the sprite and set the value equal to a specific array value depending on the input currently detected. 
            DirectionalInputIcon.sprite = DirectionalInputImageIcons[0];
        }
        else
        {
            DirectionalInputIcon.sprite = DirectionalInputImageIcons[1];
        }

    }
}
