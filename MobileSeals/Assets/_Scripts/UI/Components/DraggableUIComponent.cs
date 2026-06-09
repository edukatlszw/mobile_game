using System.Collections.Generic;
using UIExtensionPackage.UISystem.Core.Components;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UIExtensionPackage.UISystem.UI.Components
{
    /// <summary>
    /// Class extends <see cref="DraggableComponentBase"/> for UI elements.
    /// </summary>
    /// <remarks>Shouldn't be added to Object by hand</remarks>
    public sealed class DraggableUIComponent : DraggableComponentBase
    {
        
        [SerializeField]
        private RectTransform _rectTransform;

        [SerializeField]
        private List<Graphic> _graphics = new List<Graphic>();

        [SerializeField]
        private Vector2 _startPosition;

        private Dictionary<Graphic, bool> _graphicsRaycastableDict = new ();
        private void Awake()
        {
            _graphics.AddRange(GetComponentsInChildren<Graphic>());
            foreach (var graphic in _graphics)
            {
                _graphicsRaycastableDict.Add(graphic, graphic.raycastTarget);
            }
            _rectTransform = _graphics[0].transform as RectTransform;
        }

        protected override void HandleRegisterStartPosition()
        {
            _startPosition = _rectTransform.anchoredPosition;
        }

        protected override void HandleDragBegin(PointerEventData eventData)
        {
            for (int i = 0; i < _graphics.Count; i++)
            { 
                _graphics[i].raycastTarget = false;
            }

        }

        protected override void HandleOnDrag(PointerEventData eventData)
        {
            _rectTransform.position = eventData.position + dragOffset;
        }

        protected override void HandleOnDragEnd(PointerEventData eventData)
        {
            foreach (var kvp in _graphicsRaycastableDict)
            {
                kvp.Key.raycastTarget = kvp.Value;
            }
        }

        protected override void HandleResetPosition()
        {
            _rectTransform.anchoredPosition = _startPosition;
        }


    }
}