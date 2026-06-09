using UnityEngine;

public class SashimiAnimal : BaseAnimal<SashimiDefinition>
{
    
}
[CreateAssetMenu(fileName = "Sashimi_Definition", menuName = "MobileSeals/Definitions/Sashimi")]
public class SashimiDefinition : BaseAnimalDefinition<SashimiAnimal, SashimiDefinition>
{
    
}