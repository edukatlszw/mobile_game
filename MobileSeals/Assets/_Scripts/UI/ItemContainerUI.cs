using System;
using System.Collections.Generic;
using UIExtensionPackage.UISystem.UI.Components;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemContainerUI : MonoBehaviour, IPointerUpHandler
{
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private SelectableComponent _selectableComponent;
    [SerializeField] private DraggableUIComponent _draggableUIComponent;
    [SerializeField] private float _raycastMaxDistance = 100f;

    [SerializeField] private bool _enableDebug;
    [Header("Debug")]
    [SerializeField] private InventoryItemContainer _currentInventoryObjectContainer;
    [SerializeField] private ItemSlotUI _currentSlot;
    

    public List<InventoryItemContainer> MatchingItems { get; private set; } = new();
    
    private Vector3 _originalLocalPosition;
    public ItemSlotUI CurrentSlot => _currentSlot;
    public InventoryItemContainer CurrentInventoryObjectContainer => _currentInventoryObjectContainer;

    
    public static event Action<ItemContainerUI> OnItemDragBegin;
    public static event Action<ItemContainerUI> OnItemDragEnd;
    
    List<RaycastResult> _raycastResults = new ();
    private List<RaycastHit2D> _gameWorldRaycastResults = new ();
    private bool _isSelected;
    private Action<BaseInventoryObject> _onSelected;
    private Action _onUnSelected;

    protected virtual void Awake()
    {
        _originalLocalPosition = _rectTransform.anchoredPosition;
        _draggableUIComponent.OnDragBegin += HandleDragBegin;
        _draggableUIComponent.OnDragging += HandleDragging;
        _draggableUIComponent.OnDragEnd += HandleDrop;
        _selectableComponent.OnDeselectEvent += OnDeselect;
    }

    public void Init(Transform dragParent, Action<BaseInventoryObject> onSelected, Action onUnselected)
    {
        _draggableUIComponent.Init(dragParent);
        _onSelected = onSelected;
        _onUnSelected = onUnselected;
    }
    
    protected virtual void HandleDragBegin(PointerEventData eventData)
    {
        _draggableUIComponent.ResetPositionOnEnd = true;
        _selectableComponent.DeselectObject();
        OnItemDragBegin?.Invoke(this);
    }

    private void HandleDragging(PointerEventData eventData)
    {
        if (_enableDebug)
        {
            int index = 0;
            EventSystem.current.RaycastAll(eventData, _raycastResults);
            foreach (var result in _raycastResults)
            {
                Debug.Log($"{index} Whats under {result.gameObject.name}");
                index++;
            }
        }

    }
    
    protected virtual void HandleDrop(PointerEventData eventData)
    {
        OnItemDragEnd?.Invoke(this);
    
        if(TryUIRaycast(eventData)) 
            return;

        if (TryGameWorldRaycast(eventData))
        {
            InventoryManager.Instance.RemoveItem(_currentInventoryObjectContainer);
        }
        _gameWorldRaycastResults.Clear();
    }

    private bool TryUIRaycast(PointerEventData eventData)
    {
        EventSystem.current.RaycastAll(eventData, _raycastResults);
        foreach (var result in _raycastResults)
        {
            if (result.gameObject.TryGetComponent(out ItemSlotUI slot))
            {
                if(slot == _currentSlot) continue;
                
                _draggableUIComponent.ResetPositionOnEnd = false;
                slot.SetItemContainer(this);
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
                
                Debug.Log($"Pointer raycast hit: {_gameWorldRaycastResults[i].collider.name} beneath the cursor.");
                if (_gameWorldRaycastResults[i].collider.gameObject
                    .TryGetComponent(out IDraggableSlot<IAnimal> animalSlot))
                {
                    if (animalSlot.CanAccept(_currentInventoryObjectContainer.InventoryObject as IAnimal))
                    {
                        animalSlot.Accept(_currentInventoryObjectContainer.InventoryObject as IAnimal);
                        return true;
                    }
                }
            }
        }
        
        return false;
    }

    public void SetupItem(InventoryItemContainer itemContainer)
    {
        if (!itemContainer.HasItem)
        {
            ClearItemEvents();
            gameObject.SetActive(false);
            return;
        }
        gameObject.SetActive(true);
        _currentInventoryObjectContainer = itemContainer;
        UpdateDisplay();

    }

    public void SetNewParentSlot(ItemSlotUI slot)
    {
        _currentSlot = slot;
        _rectTransform.SetParent(slot.ItemAnchor, false);
        
        _rectTransform.anchoredPosition = _originalLocalPosition;
    }

    protected virtual void UpdateDisplay()
    {
        gameObject.name = _currentInventoryObjectContainer.InventoryObject.DisplayData.displayName;
    }

    private void ClearItemEvents()
    {
        _currentInventoryObjectContainer = null;
    }
    
    public void CleanUp()
    {
        gameObject?.SetActive(false);
        ClearItemEvents();
    }

    private void OnDestroy()
    {
        CleanUp();
        
        _draggableUIComponent.OnDragBegin -= HandleDragBegin;
        _draggableUIComponent.OnDragging -= HandleDragging;
        _draggableUIComponent.OnDragEnd -= HandleDrop;
        _selectableComponent.OnDeselectEvent -= OnDeselect;
    }

    private void OnValidate()
    {
        if (!_rectTransform) _rectTransform = (RectTransform)transform;
        
    }

    public void OnDeselect(BaseEventData eventData)
    {
        _onUnSelected?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_selectableComponent.IsSelected)
        {
            _onSelected?.Invoke(_currentInventoryObjectContainer.InventoryObject);
        }
    }
}
