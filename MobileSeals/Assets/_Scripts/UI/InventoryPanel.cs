using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryPanel : MonoBehaviour
{
    [SerializeField] private Button _showButton;
    [SerializeField] private Button _hideButton;
    [SerializeField] private GameObject _invPanel;

    [SerializeField]
    List<BaseAnimalDefinition> _startingAnimals = new List<BaseAnimalDefinition>();
    
    
    private void Awake()
    {
        _showButton.onClick.AddListener(ShowPanel);
        _hideButton.onClick.AddListener(HidePanel);
        _invPanel.SetActive(false);
        _showButton.gameObject.SetActive(true);
    }

    public void ShowPanel()
    {
        _showButton.gameObject.SetActive(false);
        _hideButton.gameObject.SetActive(true);
        _invPanel.SetActive(true);
    }

    public void HidePanel()
    {
        _showButton.gameObject.SetActive(true);
        _hideButton.gameObject.SetActive(false);
        _invPanel.SetActive(false);
    }
}
