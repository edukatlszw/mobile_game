using UnityEngine;
using UnityEngine.UI;

public class AnimalContainerUI : ItemContainerUI
{
    [SerializeField] AnimalShortDisplay _shortDisplay;
    [SerializeField] Button _button;

    protected override void Awake()
    {
        base.Awake();
        _button.onClick.AddListener(HandleButtonClick);
    }

    private void HandleButtonClick()
    {
        UIManager.Instance.OpenWorkingAnimalWindow(CurrentInventoryObjectContainer.InventoryObject as IAnimal, null);
    }

    protected override void UpdateDisplay()
    {
        base.UpdateDisplay();

        if (CurrentInventoryObjectContainer.InventoryObject is not IAnimal animal)
        {
            return;
        }
        _shortDisplay.SetAnimal(animal);
    }
}
