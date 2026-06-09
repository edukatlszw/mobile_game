using TMPro;
using UnityEngine;

public class AnimalProductionInfoUI : MonoBehaviour
{
    [SerializeField] TMP_Text _maxEnergy;
    [SerializeField] TMP_Text _rarity;
    [SerializeField] TMP_Text _level;
    [SerializeField] TMP_Text _output;

    public void UpdateDisplay(IAnimal animal)
    {
        AnimalProductionModule module = animal.ProductionModule; 
        _maxEnergy.SetText(module.MaxEnergy.ToString());
        _rarity.SetText(animal.AnimalDefinition.Rarity.ToString());
        _level.SetText(module.CurrentLevel.ToString());
        _output.SetText(module.OutputPerCycle.ToString());
    }
}
