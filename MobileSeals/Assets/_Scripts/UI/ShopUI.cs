using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private List<BaseAnimalDefinition> _animals;
    
    [SerializeField] private int _gachaCost;
    [SerializeField] private Button _gachaButton;
    
    [SerializeField] private int _storageCost;
    [SerializeField] private Button _storageButton;
    [SerializeField] private GameObject _storagePanel;
    
    [SerializeField] private Button _addGoldBtn;

    private void Awake()
    {
        _gachaButton.onClick.AddListener(BuyAnimal);
        _storageButton.onClick.AddListener(BuyStorage);
        _addGoldBtn.onClick.AddListener(AddGold);
    }

    private void BuyAnimal()
    {
        if(InventoryManager.Instance.Items.Count >= 4) return;
        if(!EconomyManager.Instance.CanBuy(_gachaCost)) return;
        EconomyManager.Instance.ChangeGoldAmount(-_gachaCost);
        RollAnimal();
    }
    
    private void BuyStorage()
    {
        if(!EconomyManager.Instance.CanBuy(_storageCost)) return;
        EconomyManager.Instance.ChangeGoldAmount(-_storageCost);
        _storageButton.gameObject.SetActive(false);
        _storagePanel.SetActive(true);
    }


    private void AddGold()
    {
        EconomyManager.Instance.ChangeGoldAmount(100);
    }

    private void RollAnimal()
    {
        int random = UnityEngine.Random.Range(0, _animals.Count);
        var animal = _animals[random];

        InventoryManager.Instance.AddItem(animal);
    }
}
