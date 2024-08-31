using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PFF.UI
{
    public class SkillSlots : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
    {
        // The skill slot will just have the UI image
        // this just acts as a visual representation so its easy to see whats happening
        // all that will be needed is the UI and the ability to get the sprite component. 
        // by default value should be null 
        [SerializeField] private BoxCollider2D collider;
        public Image skillSprite;
        [SerializeField] public string skillName;

        public void OnPointerDown(PointerEventData eventData)
        {
            UnSetSprite();
            UnSetName();
        }

        public Sprite GetSprite() { return skillSprite.sprite; }

        public string GetName() { return skillName; }

        public void SetSprite(Sprite sprite) { skillSprite.sprite = sprite; }

        public void SetName(string name) { skillName = name; }

        public void UnSetSprite() {  skillSprite.sprite = null; }

        public void UnSetName() {  skillName = ""; }

        public void OnPointerEnter(PointerEventData eventData)
        {
           if( this.gameObject.transform.GetChild(0).TryGetComponent<Image>(out Image icon))
            {
                icon.color = Color.green;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (this.gameObject.transform.GetChild(0).TryGetComponent<Image>(out Image icon))
            {
                icon.color = Color.white;
            }
        }
    }
}

