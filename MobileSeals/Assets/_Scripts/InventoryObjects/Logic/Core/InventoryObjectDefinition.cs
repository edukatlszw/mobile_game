using UnityEngine;

public abstract class InventoryObjectDefinition : ScriptableObject
{
    public InventoryObjectDisplayData inventoryObjectDisplayData;
    [field: SerializeField]
    public Rarity Rarity { get; private set; }
    [field: SerializeField]
    public int GoldValue { get; private set; }    
    
    public int maxCount = 1;

    public abstract BaseInventoryObject CreateBaseItem();
}

public abstract class InventoryObjectDefinition<TBaseItem, TSelf> : InventoryObjectDefinition
where TBaseItem : BaseInventoryObject<TSelf>, new()
where TSelf : InventoryObjectDefinition<TBaseItem, TSelf>
{
    public override BaseInventoryObject CreateBaseItem()
    {
        return CreateTypedItem();
    }

    public TBaseItem CreateTypedItem()
    {
        TBaseItem baseItem = new ();
        baseItem.Initialize(this);
        return baseItem;
    }
}
