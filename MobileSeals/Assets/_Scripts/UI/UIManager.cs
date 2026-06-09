using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviourSingleton<UIManager>
{
    [Serializable]
    struct ElementCategorySpriteData
    {
        public ElementCategory category;
        public Sprite icon;
    }
    
    [SerializeField]
    AnimalWindowUI _animalWindowUI;
    
    [SerializeField] private List<UIScaleComponent> _scaleComponents = new List<UIScaleComponent>();
    [SerializeField] ElementCategorySpriteData[] _elementCategorySpriteData;
    [SerializeField] private GhostItemContainer _ghostItemContainer;
    [SerializeField] private ShopUI _shopUI;
    [SerializeField] private SettingsWindow _settingsWindow;
    public void OpenWorkingAnimalWindow(IAnimal animal, AnimalStorageSlot slot)
    {
        var tempAnimal = _animalWindowUI.Animal;

        CloseAnimalWindow();
        
        //When touch animal with already opened window, it should only get closed
        if (tempAnimal != null && tempAnimal == animal)
        {
            return;
        }
        
        if (slot == null)
        {
            _animalWindowUI.OpenWindowWithAnimal(animal);
        }
        else
        {
            _animalWindowUI.OpenWindowWithWorkingAnimal(slot, animal);
        }
        
        _settingsWindow.gameObject.SetActive(false);
        _shopUI.gameObject.SetActive(false);
    }
    
    public void StartDragging(AnimalStorageSlot slot, IAnimal animal, PointerEventData eventData)
    {
        _settingsWindow.gameObject.SetActive(false);
        _shopUI.gameObject.SetActive(false);
        _ghostItemContainer.gameObject.SetActive(true);
        _ghostItemContainer.MoveToPosition(eventData.position);
        _ghostItemContainer.SetupNewItem(animal as BaseInventoryObject, slot);
        eventData.pointerDrag = _ghostItemContainer.gameObject;
    }

    public void SetNewScale(float scale)
    {
        for (int i = 0; i < _scaleComponents.Count; i++)
        {
            _scaleComponents[i].SetScale(scale);
        }
    }


    public void CloseAnimalWindow()
    {
        _animalWindowUI.gameObject.SetActive(false);
    }

    public Sprite GetElementIcon(ElementCategory category)
    {
        foreach (var data in _elementCategorySpriteData)
        {
            if(data.category == category) return data.icon;
        }
        return null;
    }
}


