using System;
using UnityEngine;

public class ItemSlotUI : MonoBehaviour
{
    [SerializeField] private Transform _itemAnchor;
    [Header("Debug")]
    [SerializeField] private ItemContainerUI _itemContainerUI;
    private Action<InventoryItemContainer, int> _onItemMovedAction;
    private int _slotIndex;
    
    public Transform ItemAnchor => _itemAnchor;
    public ItemContainerUI ItemContainerUI => _itemContainerUI;


    public void Init(int slotIndex, Action<InventoryItemContainer, int> onItemMoved)
    {
        _slotIndex = slotIndex;
        _onItemMovedAction = onItemMoved;

        gameObject.name = $"itemSlotUI_{_slotIndex}";
    }

    public void SetItem(InventoryItemContainer item)
    {
        _itemContainerUI.SetupItem(item);
    }
    
    public void SetItemContainer(ItemContainerUI p_containerUI)
    {
        if (_itemContainerUI != null)
        {
            SwapItemContainer(p_containerUI);
            return;
        }    
        _itemContainerUI = p_containerUI;
        _itemContainerUI.SetNewParentSlot(this);
        _onItemMovedAction?.Invoke(_itemContainerUI.CurrentInventoryObjectContainer, _slotIndex);
    }
    
    public void ClearItemContainer() => _itemContainerUI = null;

    public void SwapItemContainer(ItemContainerUI newContainer)
    {
        newContainer.CurrentSlot.ClearItemContainer();
        newContainer.CurrentSlot.SetItemContainer(_itemContainerUI);
        
        ClearItemContainer();
        SetItemContainer(newContainer);
    }

    public void AddItemToInventory(BaseInventoryObject inventoryObject, AnimalStorageSlot animalStorageSlot)
    {
        if (_itemContainerUI.CurrentInventoryObjectContainer != null)
        {
            animalStorageSlot.ClearSlot();
            animalStorageSlot.Accept(_itemContainerUI.CurrentInventoryObjectContainer.InventoryObject as IAnimal);
            InventoryManager.Instance.RemoveItem(_itemContainerUI.CurrentInventoryObjectContainer);
        }

        InventoryManager.Instance.AddItemToSlot(inventoryObject, _slotIndex);   
    }
    
    public void RemoveItemContainer()
    {
        _itemContainerUI.CleanUp();
    }
}
