using System;
using UnityEngine;
using UnityEngine.Serialization;


[Serializable]
public class InventoryItemContainer : IDisposable
{
    [SerializeField] private BaseInventoryObject _inventoryObject = null;
    [SerializeField] private int _itemAmount;
    
    public BaseInventoryObject InventoryObject => _inventoryObject;
    public int ItemAmount => _itemAmount;

    public event Action<int> OnCountChanged; 
    
    private Action<InventoryItemContainer> _clearItemAction;
    public bool HasItem => ItemAmount > 0;
    public void Initialize(Action<InventoryItemContainer> removeAction)
    {
        _inventoryObject = null;
        _clearItemAction = removeAction;    
    }

    public void SetupItem(BaseInventoryObject inventoryObject)
    {
        _inventoryObject = inventoryObject;
        _itemAmount = 1;
        OnCountChanged?.Invoke(_itemAmount);
    }

    public void IncreaseCount(int amount = 1)
    {
        _itemAmount += amount;
        OnCountChanged?.Invoke(_itemAmount);
    }

    public void DecreaseCount(int amount = 1)
    {
        _itemAmount -= amount;
        if (_itemAmount <= 0)
        {
            _clearItemAction?.Invoke(this);
        }
        else
        {
            OnCountChanged?.Invoke(_itemAmount);
        }
    }

    public void ClearItem()
    {
        _inventoryObject = null;
        _itemAmount = 0;
    }

    public void Dispose()
    {
        OnCountChanged = null;
    }
}
