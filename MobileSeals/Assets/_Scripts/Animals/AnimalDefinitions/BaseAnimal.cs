using System;
using UnityEngine;

public interface IAnimal
{
    BaseInventoryObject InvObj { get; }
    public BaseAnimalDefinition AnimalDefinition { get; }
    public AnimalProductionModule ProductionModule { get; }
    
    public ElementCategory Element{ get; }
}

public abstract class BaseAnimal<TDefinition> : BaseInventoryObject<TDefinition>, IAnimal
    where TDefinition : BaseAnimalDefinition
{
    private AnimalProductionModule _animalProductionModule;

    public BaseInventoryObject InvObj => this;
    public BaseAnimalDefinition AnimalDefinition => Definition;
    public AnimalProductionModule ProductionModule => _animalProductionModule;
    public ElementCategory Element => Definition.ElementCategory;

    protected override void InternalInitialize(TDefinition definition)
    {
        base.InternalInitialize(definition);
        _animalProductionModule = new AnimalProductionModule(definition);
    }
}

public abstract class BaseAnimalDefinition : InventoryObjectDefinition
{
    [SerializeField] private ElementCategory _elementCategory;
    [SerializeField] private int _workOutput;
    [SerializeField] private int _maxEnergy;
    [SerializeField] private int _baseUpgradeCost;
    
    public ElementCategory ElementCategory => _elementCategory;
    public int WorkOutput => _workOutput;
    public int MaxEnergy => _maxEnergy;
    public int BaseUpgradeCost => _baseUpgradeCost;

    public abstract IAnimal CreateBaseAnimal();

}

public abstract class BaseAnimalDefinition<TItem, TSelf> : BaseAnimalDefinition
    where TItem : BaseAnimal<TSelf>, new()
    where TSelf : BaseAnimalDefinition
{
    public override BaseInventoryObject CreateBaseItem() => CreateTypedItem();

    public override IAnimal CreateBaseAnimal()
    {
        return CreateTypedItem();
    }

    public TItem CreateTypedItem()
    {
        TItem item = new();
        item.Initialize(this as TSelf);
        return item;
    }
}