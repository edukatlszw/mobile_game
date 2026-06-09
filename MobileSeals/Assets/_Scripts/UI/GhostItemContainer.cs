using System;
using System.Collections.Generic;
using UIExtensionPackage.UISystem.UI.Components;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class GhostItemContainer : MonoBehaviour, IPointerClickHandler, IPointerUpHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private FollowPointerUIComponent _followPointerUI;
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private AnimalShortDisplay _animalShortDisplay;
    
    
    private float _raycastMaxDistance = 100f;

    private BaseInventoryObject _inventoryObject;
    private AnimalStorageSlot _animalSlot;
    List<RaycastResult> _raycastResults = new ();
    private List<RaycastHit2D> _gameWorldRaycastResults = new ();
    private void Awake()
    {
        _followPointerUI.Init(false);
        gameObject.SetActive(false);
    }
    
    void Update()
    {
        // Works for both Mouse click release and Mobile touch release
        if (Pointer.current != null && Pointer.current.press.wasReleasedThisFrame)
        {
            Debug.Log("Pointer Released!");
        }
    }

    public void SetupNewItem(BaseInventoryObject inventoryObject, AnimalStorageSlot slot)
    {
        _inventoryObject = inventoryObject;
        if (_inventoryObject is IAnimal animal)
        {
            _animalShortDisplay.SetAnimal(animal);
        }
        _animalSlot = slot;
        _followPointerUI.StartFollow();
    }
    
    public void MoveToPosition(Vector2 position) => _rectTransform.position = position;


    public void OnPointerUp(PointerEventData eventData)
    {

    }

    private void HandleStopDrag(PointerEventData eventData)
    {
        _followPointerUI.StopFollow();
        _gameWorldRaycastResults.Clear();

        if (TryUIRaycast(eventData))
        {
            ClearData();
            return;
        }

        if (TryGameWorldRaycast(eventData))
        {
            ClearData();
            return;
        }
        
        _animalSlot.SetNewAnimal(_inventoryObject as IAnimal);
        ClearData();
    }
    
    private bool TryUIRaycast(PointerEventData eventData)
    {
        EventSystem.current.RaycastAll(eventData, _raycastResults);
        foreach (var result in _raycastResults)
        {
            if(result.gameObject == gameObject) continue;
            
            if (result.gameObject.TryGetComponent(out ItemSlotUI slot))
            {
                slot.AddItemToInventory(_inventoryObject, _animalSlot);
                return true;
            }
        }
        
        return false;
    }
    
    bool TryGameWorldRaycast(PointerEventData eventData)
    {

        Vector2 screenPosition = eventData.position;
        Camera eventCamera = eventData.pressEventCamera ?? Camera.main;
        
        if (eventCamera != null)
        {
            Vector2 mouseWorldPos = eventCamera.ScreenToWorldPoint(screenPosition);
            Vector2 direction = Vector2.down;
            
            Physics2D.Raycast(mouseWorldPos, direction, ContactFilter2D.noFilter, _gameWorldRaycastResults, _raycastMaxDistance);
            
            for (int i = 0; i < _gameWorldRaycastResults.Count; i++)
            {
                if(_gameWorldRaycastResults[i].collider == null) break;
                
                if(_gameWorldRaycastResults[i].collider.gameObject == _animalSlot.gameObject) continue;

                Debug.Log($"Pointer raycast hit: {_gameWorldRaycastResults[i].collider.name} beneath the cursor.");
                if (_gameWorldRaycastResults[i].collider.gameObject
                    .TryGetComponent(out IDraggableSlot<IAnimal> animalSlot))
                {
                    var tempAnimal = animalSlot.GetDraggable();
                    animalSlot.ClearSlot();
                    animalSlot.Accept(_inventoryObject as IAnimal);
                    if(tempAnimal != null) _animalSlot.Accept(tempAnimal);
                    
                    return true;
                }
            }
        }
        
        return false;
    }

    private void ClearData()
    {
        _inventoryObject = null;
        _animalSlot = null;
        gameObject.SetActive(false);
    }
    

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("PointerDown");
    }

    public void OnDrag(PointerEventData eventData)
    {
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        HandleStopDrag(eventData);
    }
    
    

    private void OnValidate()
    {
        if (!_rectTransform) _rectTransform = (RectTransform)transform;
    }
}
