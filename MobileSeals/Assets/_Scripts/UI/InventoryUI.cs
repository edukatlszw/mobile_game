using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class InventoryUI : MonoBehaviour
{
    [FormerlySerializedAs("_inventory")] [SerializeField] private InventoryManager _inventoryManager;
    [SerializeField] private ItemSlotUI _itemSlotUIPrefab;
    [SerializeField] private ItemContainerUI _itemContainerUIPrefab;
    [SerializeField] private Transform _slotsContainer;
    [SerializeField] private List<ItemSlotUI> _slots;
    [SerializeField] private Transform _dragLayer;
    private int _slotIndex = 0;
    
    private List<ItemSlotUI> _craftingOutlineCachedSlots = new List<ItemSlotUI>();

    public void Start()
    {
        _inventoryManager.OnNewItemAdded += HandleItemAdded;
        _inventoryManager.ItemAddedToSlot += ItemAddedToSlot;
        _inventoryManager.OnItemRemoved += HandleItemRemoved;
        var items = _inventoryManager.ItemContainers;

        foreach (var item in items)
        {
            CreateNewSlot(item);
        }
    }
    
    private void HandleItemAdded(InventoryItemContainer item)
    {
        if (TryGetEmptySlot(out ItemSlotUI slot))
        {
            slot.SetItem(item);
        }
    }


    private void ItemAddedToSlot(InventoryItemContainer item, int slotIndex)
    {
        _slots[slotIndex].SetItem(item);
    }
    
    private void HandleItemRemoved(InventoryItemContainer item)
    {
        if (TryGetSlotByItem(item, out var slot))
        {
            slot.RemoveItemContainer();
        }
    }

    private void HandleItemMoved(InventoryItemContainer item, int slotIndex)
    {
        _inventoryManager.ItemMoved(item, slotIndex);
    }
    
    private ItemSlotUI CreateNewSlot(InventoryItemContainer item)
    {
        var slot = Instantiate(_itemSlotUIPrefab, _slotsContainer);
        var itemContainer = Instantiate(_itemContainerUIPrefab, slot.ItemAnchor);
        _slots.Add(slot);
        slot.Init(_slotIndex, HandleItemMoved);
        itemContainer.Init(_dragLayer, OpenDisplayInfo, CloseDisplayInfo);
        _slotIndex++;
        itemContainer.SetupItem(item);
        slot.SetItemContainer(itemContainer);
        return slot;
    }

    private void OpenDisplayInfo(BaseInventoryObject inventoryObject)
    {
        
    }
    
    private void CloseDisplayInfo()
    {

    }
    
    private bool TryGetEmptySlot(out ItemSlotUI slot)
    {
        slot = null;
        for (var i = 0; i < _slots.Count; i++)
        {
            if (_slots[i].ItemContainerUI.CurrentInventoryObjectContainer == null)
            {
                slot = _slots[i];
                return true;
            }
        }
        return false;
    }
    
    private bool TryGetSlotByItem(InventoryItemContainer item, out ItemSlotUI slot)
    {
        slot = null;
        for (var i = 0; i < _slots.Count; i++)
        {
            if (_slots[i].ItemContainerUI.CurrentInventoryObjectContainer == item)
            {
                slot = _slots[i];
                return true;
            }
        }
        return false;
    }
    
    private void OnDestroy()
    {
        if (_inventoryManager != null)
        {
            _inventoryManager.OnNewItemAdded -= HandleItemAdded;
            _inventoryManager.OnItemRemoved -= HandleItemRemoved;
            _inventoryManager.ItemAddedToSlot -= ItemAddedToSlot;
        }
    }
}
