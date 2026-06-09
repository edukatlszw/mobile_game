using System;
using UnityEngine;

public class AnimalProductionModule
{
    
    private int _currentEnergy = 0;
    private int _currentLevel = 1;
    private int _maxEnergy;
    private int _outputPerCycle;
    private int _upgradeCost;

    private int _maxLevel = 3;
    public int OutputPerCycle => _outputPerCycle;
    public int MaxEnergy => _maxEnergy;
    public int CurrentLevel => _currentLevel;
    public int CurrentEnergy => _currentEnergy;
    public float NormalizedCurrentEnergy => (float) _currentEnergy/_maxEnergy;
    
    public bool IsMaxLevel => _currentLevel >= _maxLevel;
    public int UpgradeCost => _upgradeCost;
    public int SellCost => _upgradeCost/4;
    public event Action<float> OnEnergyAmountChanged;
    public event Action OnUpgraded;
    
    public AnimalProductionModule(BaseAnimalDefinition definition)
    {
        _maxEnergy = definition.MaxEnergy;
        _currentEnergy = _maxEnergy;
        _outputPerCycle = definition.WorkOutput;
        _upgradeCost = definition.BaseUpgradeCost;
    }

    public void ChangeEnergyAmount(int energy)
    {
        _currentEnergy += energy;
        _currentEnergy = Mathf.Clamp(_currentEnergy, 0, _maxEnergy);
        OnEnergyAmountChanged?.Invoke(NormalizedCurrentEnergy);
    }

    
    public bool TryToUpgrade()
    {
        if(_currentLevel >= _maxLevel) 
            return false;
        
        if (!EconomyManager.Instance.CanBuy(_upgradeCost))
            return false;
        
        
        _currentLevel++;
        _upgradeCost *= 2;

        var energyUpgrade = 50;
        _maxEnergy += energyUpgrade;
        ChangeEnergyAmount(energyUpgrade);
        _outputPerCycle += 10;
        
        OnUpgraded?.Invoke();
        return true;
    }
    
}
