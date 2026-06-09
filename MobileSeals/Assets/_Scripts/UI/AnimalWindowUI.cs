using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnimalWindowUI : MonoBehaviour, IAttachListeners
{
    [SerializeField] AnimalShortDisplay _shortDisplay;
    [SerializeField] AnimalProductionInfoUI _productionInfoUI;
    [SerializeField] Button _upgradeButton;
    [SerializeField] TMP_Text _upgradeCostText;

    [SerializeField] Button _sellButton;
    [SerializeField] TMP_Text _sellCostText;
    
    [SerializeField] Button _closeButton;
    [SerializeField] Slider _productionSlider;
    [SerializeField] TMP_Text _productionText;
    [SerializeField] TMP_Text _opsText;
    
    IAnimal _animal;
    AnimalStorageSlot _storageSlot;

    public IAnimal Animal => _animal;
    private void Awake()
    {
        _closeButton.onClick.AddListener(()=>gameObject.SetActive(false));
        _sellButton.onClick.AddListener(HandleSellButton);
        _upgradeButton.onClick.AddListener(HandleUpgradeButton);
    }

    private void HandleUpgradeButton()
    {
        _animal.ProductionModule.TryToUpgrade();
    }

    private void HandleSellButton()
    {
        EconomyManager.Instance.ChangeGoldAmount(_animal.ProductionModule.SellCost);
        if (_storageSlot != null)
        {
            _storageSlot.ClearSlot();
            gameObject.SetActive(false);
            return;
        }
        if (InventoryManager.Instance.RemoveItem(_animal.InvObj))
        {
            gameObject.SetActive(false);
            return;
        }
    }

    public void OpenWindowWithAnimal(IAnimal animal)
    {
        if (_animal == animal)
        {
            gameObject.SetActive(false);
            return;
        }
        _animal = animal;
        _shortDisplay.SetAnimal(_animal);
        UpdateDetailedInfo();
        _productionSlider.transform.parent.gameObject.SetActive(false);
        AttachListeners();
        gameObject.SetActive(true);
    }
    
    public void OpenWindowWithWorkingAnimal(AnimalStorageSlot slot, IAnimal animal)
    {
        if (_animal == animal)
        {
            gameObject.SetActive(false);
            return;
        }
        
        _animal = animal;
        _storageSlot = slot;
        _shortDisplay.SetAnimal(animal);
    
        UpdateDetailedInfo();
        UpdateProductionSlider(_storageSlot.NormalizedProductionPercentage);
        AttachListeners();
        _productionSlider.transform.parent.gameObject.SetActive(true);
        gameObject.SetActive(true);
    }

    private void UpdateDetailedInfo()
    {
        _upgradeCostText.text = _animal.ProductionModule.UpgradeCost.ToString();
        _sellCostText.text = _animal.ProductionModule.SellCost.ToString();
        _productionInfoUI.UpdateDisplay(_animal);
        if (_storageSlot != null)
        {
            _opsText.text = _storageSlot.OutputPerSecond.ToString();
        }
    }

    private void UpdateProductionSlider(float normalizedProduction)
    {
        _productionSlider.value = normalizedProduction;
        var value = normalizedProduction *100;
        _productionText.text = value.ToString(format:"F0") + "%";
    }
    

    public void AttachListeners()
    {
        _animal.ProductionModule.OnUpgraded += UpdateDetailedInfo;
        if (_storageSlot != null)
        {
            _storageSlot.OnProductionProgressed += UpdateSlider;
        }
    }
    
    private void UpdateSlider() => UpdateProductionSlider(_storageSlot.NormalizedProductionPercentage);
    
    public void DetachListeners()
    {
        _shortDisplay.DetachListeners();
        if (_storageSlot != null)
        {
            _storageSlot.OnProductionProgressed -= UpdateSlider;
        }
        
        _storageSlot = null;
        _animal = null;
    }

    private void OnDisable()
    {
        DetachListeners();
    }
}