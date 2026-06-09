using System;
using UIExtensionPackage.UISystem.Core.Components;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UIExtensionPackage.UISystem.UI.Components
{
    /// <summary>
    /// Class extends <see cref="FollowPointerComponentBase"/>  for UI elements.
    /// </summary>
    public class FollowPointerUIComponent : FollowPointerComponentBase
    {

        [SerializeField]
        private RectTransform _rectTransform;
        
        [SerializeField]
        private Vector2 _offset;
        
        
        protected override void HandleOnPointerMove(PointerEventData eventData)
        {
            // Get RT position from left bottom corner point of view and compute offset
            Vector2 rtPos = _rectTransform.position;
            // Calculate offset
            _offset = rtPos - eventData.position;
            
            // Move to new position and add offset
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _rectTransform.parent as RectTransform, 
                eventData.position, 
                eventData.pressEventCamera, 
                out Vector2 localTouchPos
            );
    
            _rectTransform.position = eventData.position;
        }

        private void OnValidate()
        {
            if(!_rectTransform) _rectTransform = GetComponent<RectTransform>();
        }
    }
}