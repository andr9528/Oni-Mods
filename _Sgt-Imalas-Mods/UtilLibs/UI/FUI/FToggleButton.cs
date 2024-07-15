﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.EventSystems;
using UnityEngine;
using UtilLibs.UIcmp;
using UnityEngine.UI;
using static STRINGS.BUILDING.STATUSITEMS.ACCESS_CONTROL;
using static Operational;
using static STRINGS.DUPLICANTS.MODIFIERS;

namespace UtilLibs.UI.FUI
{
    public class FToggleButton : KMonoBehaviour, IEventSystemHandler, IPointerUpHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public event System.Action OnClick;
        public event System.Action OnDoubleClick;

        public event System.Action OnPointerEnterAction;
        public event System.Action OnPointerExitAction;

        private bool interactable;
        private Material material;
        
        [MyCmpGet]
        private Image image;

        [MyCmpGet]
        private Button button;

        [SerializeField]
        bool IsHighlighted = false;

        [SerializeField]
        public Color disabledColor = new Color(0.78f, 0.78f, 0.78f);

        [SerializeField]
        public Color normalColor = new Color(0.243f, 0.263f, 0.341f);

        [SerializeField]
        public Color hoverColor = new Color(0.345f, 0.373f, 0.702f);

        [SerializeField]
        public Color highlightedColor = new Color(0.345f, 0.373f, 0.702f);

        public override void OnPrefabInit()
        {
            base.OnPrefabInit();
            if (image == null && button != null)
            {
                image = button.image;
            }
            disabledColor = button.colors.disabledColor;
            normalColor = button.colors.normalColor;
            hoverColor = button.colors.highlightedColor;
            highlightedColor = button.colors.selectedColor;


            button.enabled=false;

            material = image.material;
            interactable = true;
            SetColorState();
        }

        public void SetInteractable(bool interactable)
        {
            if (interactable == this.interactable)
            {
                return;
            }

            this.interactable = interactable;
            SetColorState();
        }

        

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!interactable)
            {
                return;
            }

            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            //if (KInputManager.isFocused)
            //{
            //    KInputManager.SetUserActive();
            //    //PlaySound(UISoundHelper.ClickOpen);
            //    if (!eventData.IsPointerMoving())
            //    {
            //        ToggleSelection();
            //        if (OnClick != null)
            //        {
            //            OnClick?.Invoke();
            //        }
            //    }
            //}
        }


        public void ToggleSelection()
        {
            IsHighlighted = !IsHighlighted;
            ChangeSelection(IsHighlighted);
        }
        public void ChangeSelection(bool _isHighlighted = false)
        {
            IsHighlighted = _isHighlighted;
            SetColorState();
        }
        void SetColorState()
        {
            if(image==null)
                return;

            if(!interactable)
            {
                image.color = disabledColor;
                return;
            }
            if (IsHighlighted)
            {
                image.color = highlightedColor;
                return;
            }
            if(isHovered)
            {
                image.color = hoverColor;
                return;
            }
            image.color = normalColor;
        }

        bool isHovered = false;
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (OnPointerEnterAction != null)
                OnPointerEnterAction.Invoke();

            if (!interactable)
            {
                return;
            }

            if (KInputManager.isFocused)
            {
                KInputManager.SetUserActive();
                PlaySound(UISoundHelper.MouseOver);
                isHovered = true;
            }
            SetColorState();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (OnPointerExitAction != null)
                OnPointerExitAction.Invoke();

            isHovered = false;
            SetColorState();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!interactable)
            {
                return;
            }

            if (KInputManager.isFocused)
            {
                KInputManager.SetUserActive();
                PlaySound(UISoundHelper.ClickOpen);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!interactable)
            {
                return;
            }
            if (OnDoubleClick != null && eventData.clickCount == 2)
            {
                OnDoubleClick.Invoke();
            }
            else
            {
                ToggleSelection();
                OnClick.Invoke();
            }
        }
    }
}
