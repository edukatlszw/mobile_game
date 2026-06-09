using System;
using UnityEngine;

public interface IDraggableSlot<T>
{
    bool CanAccept(T draggable);
    void Accept(T draggable);

    void ClearSlot();
    
    T GetDraggable();
}


public class AnimalStorageSlot : InGameMonobehaviour, IDraggableSlot<IAnimal>, IAttachListeners
{
    private IAnimal _animal;
    
    [SerializeField] private BaseAnimalDefinition _animalDefinition;
    [SerializeField] private AnimalSlotModel _slotModel;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    
    [SerializeField] private int _baseCycleTicks = 10;

    [Header("Debug")]
    [SerializeField] private int _outputPerSecond;
    [SerializeField] private float _currentProficiency;
    private int _tickCount;

    private int _cycleTicks;
    
    public int Level => _animal.ProductionModule.CurrentLevel;
    public int ProficiencyPercent => Mathf.RoundToInt((1f / _currentProficiency) * 100);
    public int OutputPerCycle => _animal.ProductionModule.OutputPerCycle;
    public int OutputPerSecond => _outputPerSecond;

    public float NormalizedProductionPercentage => (float)_tickCount / _cycleTicks;
    public IAnimal Animal => _animal;

    public event Action OnProductionProgressed;
    private Action _onAnimalChangeDelegate;

    public void SetOnAnimalChangeDelegate(Action changeDelegate)
    {
        _onAnimalChangeDelegate = changeDelegate;
        if (_animalDefinition != null)
        {
            SetNewAnimal(_animalDefinition.CreateBaseAnimal());
        }
    }
    
    public void AttachListeners()
    {
        TimeManager.Instance.OnTick += HandleTick;
    }

    public void SetNewAnimal(IAnimal animal)
    {
        _spriteRenderer.enabled = false;
        _animal = animal;
        _onAnimalChangeDelegate?.Invoke();
        _slotModel.SetAnimal(this, animal);
        UpdateOPS();

        _animal.ProductionModule.OnUpgraded += HandleAnimalUpgrade;
    }

    private void HandleAnimalUpgrade() => UpdateOPS();
    
    public void ClearSlot()
    {
        if(_animal == null) return;
        
        _animal.ProductionModule.OnUpgraded -= HandleAnimalUpgrade;
        _animal = null;
        _spriteRenderer.enabled = true;
        _slotModel.ClearAnimal();
    }

    public IAnimal GetDraggable()
    {
        return _animal;
    }


    public void SetCategoryBonus(float bonus)
    {
        _currentProficiency = 1 - bonus;
        UpdateOPS();
    }


    
    private void HandleTick()
    {
        if(_animal == null) return;
        if(_animal.ProductionModule.CurrentEnergy <= 0) return;
        _tickCount++;
        OnProductionProgressed?.Invoke();
        if (_tickCount >= _cycleTicks)
        {
            _tickCount = 0;
            Produce();
        }
    }

    private void UpdateOPS()
    {
        if(_animal == null) return;
        
        _cycleTicks = Mathf.RoundToInt(_baseCycleTicks * _currentProficiency); 
        float cycleSeconds = _cycleTicks *  TimeManager.Instance.TickIntervalSeconds;
        _outputPerSecond = Mathf.RoundToInt(_animal.ProductionModule.OutputPerCycle / cycleSeconds);
    }
    
    private void Produce()
    {
        EconomyManager.Instance.ChangeGoldAmount(_animal.ProductionModule.OutputPerCycle);
        _animal.ProductionModule.ChangeEnergyAmount(-5);
    }


    public void DetachListeners()
    {
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnTick -= HandleTick;
        }
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        if(_spriteRenderer == null) _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public bool CanAccept(IAnimal draggable)
    {
        return _animal == null;
    }

    public void Accept(IAnimal draggable)
    {
        SetNewAnimal(draggable);
    }


}
