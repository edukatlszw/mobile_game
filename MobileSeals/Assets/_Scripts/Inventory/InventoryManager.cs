using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryManager : MonoBehaviourSingleton<InventoryManager>, IAttachListeners
{
    [SerializeField]
    List<InventoryItemContainer> _itemContainers = new ();

    [SerializeField] private int _tickAmount = 60;
    
    [field: SerializeField] 
    public int InventorySize { get; private set; } = 20;
    [SerializeField]
    private List<InventoryObjectDefinition> _startItems = new();


    public List<InventoryItemContainer> ItemContainers => _itemContainers;
    public List<BaseInventoryObject> Items => _itemContainers.Select(item => item.InventoryObject)
        .Where(item => item is not null).ToList();
    
    public event Action<InventoryItemContainer> OnNewItemAdded;
    public event Action<InventoryItemContainer, int> ItemAddedToSlot;
    public event Action<InventoryItemContainer> OnItemRemoved;


    private int _tickCounter = 0;
    
    protected override void Awake()
    {
        base.Awake();
        
        for (int i = 0; i < InventorySize; i++)
        {
            CreateNewContainer();
        }
        for (int i = 0; i < _startItems.Count; i++)
        {
            AddItem(_startItems[i].CreateBaseItem());
        }
    }
    
    public void AttachListeners()
    {
        TimeManager.Instance.OnTick += HandleTick;
        TimeManager.Instance.OnOfflineTicks += HandleOfflineTicks;
    }

    private void HandleTick()
    {
        _tickCounter++;

        if (_tickCounter >= _tickAmount)
        {
            _tickCounter = 0;
            RestoreEnergy();
        }
    }

    private void HandleOfflineTicks(int ticksAmount)
    {
        int restoreAmount = ticksAmount % _tickAmount;
        RestoreEnergy(restoreAmount);
    }

    private void RestoreEnergy(int restoreAmount = 1)
    {
        foreach (var itemContainer in _itemContainers)
        {
            if (itemContainer.InventoryObject is IAnimal animal)
            {
                animal.ProductionModule.ChangeEnergyAmount(restoreAmount);
            }
        }
    }
    
    public bool AddItem(InventoryObjectDefinition inventoryObject) => AddItem(inventoryObject.CreateBaseItem());

    public bool AddItemToSlot(BaseInventoryObject inventoryObject, int slot)
    {
        if(_itemContainers[slot].HasItem) return false;
        _itemContainers[slot].SetupItem(inventoryObject);
        ItemAddedToSlot?.Invoke(_itemContainers[slot], slot);
        return true;
    }
    
    public bool AddItem(BaseInventoryObject inventoryObject)
    {
        if (CanAddItem(inventoryObject, out var container) == false)
        {
            return false;
        }
        
        if (inventoryObject.MaxCount > 1)
        {
            container.IncreaseCount();
            return true;
        }

        
        container.SetupItem(inventoryObject);
        OnNewItemAdded?.Invoke(container);
        return true;
    }

    public bool CanAddItem(BaseInventoryObject inventoryObject, out InventoryItemContainer container)
    {


        if (inventoryObject.MaxCount > 1)
        {
            if (TryGetItemContainer(inventoryObject.Definition, out container))
            {
                if (container.ItemAmount < container.InventoryObject.MaxCount)
                {
                    return true;
                }
            }
        }

        if (TryGetEmptyContainer(out container))
        {
            return true;
        }
        return false;
    }

    public bool RemoveItem(BaseInventoryObject inventoryObject)
    {
        if (!TryGetItemContainer(inventoryObject, out var container))
        {
            return false;
        }
        
        RemoveItem(container);
        return true;
    }
    
    public bool RemoveItem(InventoryObjectDefinition inventoryObjectDefinition)
    {
        if (!TryGetItemContainer(inventoryObjectDefinition, out var container))
        {
            return false;
        }
        
        RemoveItem(container);
        return true;
    }
    
    public void RemoveItem(InventoryItemContainer container)
    {
        container.DecreaseCount();
    }

    public void ItemMoved(InventoryItemContainer container, int slotIndex)
    {
        int index = _itemContainers.IndexOf(container);
        if(index == -1 ) return;
        if (index == slotIndex || slotIndex >= _itemContainers.Count) return;

        var itemCopy = _itemContainers[slotIndex];
        _itemContainers[slotIndex] = container;
        _itemContainers[index] = itemCopy;
    }

    public bool HasItem(InventoryObjectDefinition inventoryObject)
    {
        return TryGetItemContainer(inventoryObject, out _);
    }
    
    public void RemoveItemFromContainer(InventoryItemContainer container)
    {
        OnItemRemoved?.Invoke(container);
        container.ClearItem();
    }

    private bool TryGetItemContainer(BaseInventoryObject inventoryObject, out InventoryItemContainer container)
    {
        container = null;
        
        for (int i = 0; i < _itemContainers.Count; i++)
        {
            if(!_itemContainers[i].HasItem) continue;
            if (_itemContainers[i].InventoryObject == inventoryObject)
            {
                container = _itemContainers[i];
                return true;

            }
        }
        
        return false;
    }
    
    private bool TryGetItemContainer(InventoryObjectDefinition inventoryObject, out InventoryItemContainer container)
    {
        container = null;
        
        for (int i = 0; i < _itemContainers.Count; i++)
        {
            if(!_itemContainers[i].HasItem) continue;
            if (_itemContainers[i].InventoryObject.Definition == inventoryObject)
            {
                container = _itemContainers[i];
                return true;

            }
        }
        
        return false;
    }


    private bool TryGetEmptyContainer(out InventoryItemContainer container)
    {
        container = null;
        for (int i = 0; i < _itemContainers.Count; i++)
        {
            if (!_itemContainers[i].HasItem)
            {
                container = _itemContainers[i];
                return true;
            }
        }
        return false;
    }
    private InventoryItemContainer CreateNewContainer()
    {
        InventoryItemContainer container = new InventoryItemContainer();
        container.Initialize(RemoveItemFromContainer);
        _itemContainers.Add(container);
        return container;
    }




    public void DetachListeners()
    {

        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnTick -= HandleTick;
            TimeManager.Instance.OnOfflineTicks -= HandleOfflineTicks;
        }
    }
}
