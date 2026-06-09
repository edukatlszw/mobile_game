using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnimalShortDisplay : MonoBehaviour, IAttachListeners
{
    [SerializeField] private Image _itemIcon;
    [SerializeField] private Image _elementIcon;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private Slider _slider;
    
    IAnimal _animal;
    
    public void SetAnimal(IAnimal animal)
    {
        _animal = animal;
        _itemIcon.sprite = animal.AnimalDefinition.inventoryObjectDisplayData.icon;
        _elementIcon.sprite = UIManager.Instance.GetElementIcon(animal.Element);
        _nameText.text = animal.AnimalDefinition.inventoryObjectDisplayData.displayName;
        _slider.value = animal.ProductionModule.NormalizedCurrentEnergy;
        AttachListeners();
    }

    public void AttachListeners()
    {
        _animal.ProductionModule.OnEnergyAmountChanged += HandleEnergyChanged;
    }

    private void HandleEnergyChanged(float newEnergy)
    {
        _slider.value = newEnergy;
    }

    public void DetachListeners()
    {
        if (_animal != null)
        {
            _animal.ProductionModule.OnEnergyAmountChanged -= HandleEnergyChanged;
        }
    }

    private void OnDisable()
    {
        DetachListeners();
    }
}
