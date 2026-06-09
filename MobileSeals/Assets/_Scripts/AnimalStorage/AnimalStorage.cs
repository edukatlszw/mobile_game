using System;
using System.Collections.Generic;
using UnityEngine;

public class AnimalStorage : MonoBehaviour
{
    [Serializable]
    struct ElementCategoryColorData
    {
        public ElementCategory elementCategory;
        public Color color;
    }
    [SerializeField] private List<ElementCategoryColorData> _elementColor = new();
    [SerializeField] private ElementCategory _currentElement;
    [SerializeField] private SpriteRenderer _model;
    [SerializeField] private float _outputCategoryBonus = 0.25f;
    [SerializeField] private List<AnimalStorageSlot> _slots = new List<AnimalStorageSlot>();

    private void Awake()
    {
        foreach (AnimalStorageSlot slot in _slots)
        {
            slot.SetOnAnimalChangeDelegate(CheckCategoryBonus);
        }
    }

    public void SetCategory(ElementCategory category)
    {
        _currentElement = category;
        for (int i = 0; i < _elementColor.Count; i++)
        {
            if(_elementColor[i].elementCategory != category) continue;
            _model.color = _elementColor[i].color;
            break;
        }

        CheckCategoryBonus();
    }

    private void CheckCategoryBonus()
    {
        bool perfectCategory = true;
        foreach (var slot in _slots)
        {
            if (slot.Animal == null || slot.Animal.Element != _currentElement)
            {
                perfectCategory = false;
                break;
            }
        }

        foreach (var slot in _slots)
        {
            float bonus = 0;
            if (perfectCategory)
                bonus = _outputCategoryBonus * 2;
            else if (slot.Animal?.Element == _currentElement)
                bonus = _outputCategoryBonus;

            slot.SetCategoryBonus(bonus);
        }
    }

    private void OnValidate()
    {
        if (_model != null)
        {
            foreach (var col in _elementColor)
            {
                if (col.elementCategory == _currentElement)
                {
                    _model.color = col.color;
                    break;
                }
            }   
        }
    }
}
