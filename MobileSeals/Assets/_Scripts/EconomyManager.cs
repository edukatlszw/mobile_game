using System;
using UnityEngine;

public class EconomyManager : PersistentMonoSingleton<EconomyManager>
{
    [SerializeField] private int _goldAmount;
    
    public int GoldAmount => _goldAmount;
    
    public event Action<int> OnGoldAmountChanged;

    public bool CanBuy(int price)
    {
        return _goldAmount >= price;
    }

    public void PayCosts(int price)
    {
        if(!CanBuy(price)) return;
        
        ChangeGoldAmount(price);
    }

    public void ChangeGoldAmount(int amount)
    {
        _goldAmount += amount;
        OnGoldAmountChanged?.Invoke(_goldAmount);
    }
}
