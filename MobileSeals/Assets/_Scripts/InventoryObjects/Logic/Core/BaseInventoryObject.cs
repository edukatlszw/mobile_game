using System;
using UnityEngine;

/// <summary>
/// Cannot be abstract due to unity serialization 
/// </summary>
[Serializable]
public class BaseInventoryObject
{
    [field: SerializeField]
    public InventoryObjectDefinition Definition { get; private set; }
    public InventoryObjectDisplayData DisplayData => Definition.inventoryObjectDisplayData;
    
    public int MaxCount => Definition.maxCount;
    

    public virtual void Initialize(InventoryObjectDefinition inventoryObjectDefinition)
    {

        Definition = inventoryObjectDefinition;
    }
}

public abstract class BaseInventoryObject<TDefinition> : BaseInventoryObject
    where TDefinition : InventoryObjectDefinition
{
    public new TDefinition Definition { get; private set; }

    public sealed override void Initialize(InventoryObjectDefinition inventoryObjectDefinition)
    {
        base.Initialize(inventoryObjectDefinition);
        Definition = (TDefinition)inventoryObjectDefinition;
        InternalInitialize(Definition);
    }

    protected virtual void InternalInitialize(TDefinition definition)
    {
        
    }
}