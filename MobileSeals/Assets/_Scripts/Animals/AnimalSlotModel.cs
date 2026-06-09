using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


[RequireComponent(typeof(Collider2D))]
public class AnimalSlotModel : MonoBehaviour, IPointerUpHandler, IDragHandler, IBeginDragHandler, IPointerClickHandler
{
    [SerializeField] private Animator _animator;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    [SerializeField] private List<RuntimeAnimatorController> _controllers;
    [SerializeField] private AnimalStorageSlot _slot;
    
    private IAnimal _animal;
    
    static int _touchTrigger = Animator.StringToHash("doTouch");

    private bool _isDragging = false; 
    public AnimalStorageSlot Slot => _slot;
    


    
    public void SetAnimal(AnimalStorageSlot slot, IAnimal animal)
    {
        _slot = slot;
        _animal = animal;
        _spriteRenderer.sprite = _animal.AnimalDefinition.inventoryObjectDisplayData.icon;
        _animator.runtimeAnimatorController = _controllers[animal.ProductionModule.CurrentLevel-1];
        gameObject.SetActive(true);
        _animal.ProductionModule.OnUpgraded += HandleUpgrade;
    }

    private void HandleUpgrade()
    {
        _animator.runtimeAnimatorController = _controllers[_animal.ProductionModule.CurrentLevel-1];
    }

    public void ClearAnimal()
    {
        _animal.ProductionModule.OnUpgraded -= HandleUpgrade;
        _slot.ClearSlot();
        _animal = null;
        gameObject.SetActive(false);
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        _isDragging = true;
        UIManager.Instance.CloseAnimalWindow();
        UIManager.Instance.StartDragging(_slot, _animal, eventData);
        _slot.ClearSlot();
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        

    }
    public void OnPointerClick(PointerEventData eventData)
    {
    }
    
    
    public void OnPointerUp(PointerEventData eventData)
    {
        if (_isDragging)
        {
            _isDragging = false;
        }
        else
        {
            _animator.SetTrigger(_touchTrigger);
            UIManager.Instance.OpenWorkingAnimalWindow(_animal, _slot);
        }
    }


}