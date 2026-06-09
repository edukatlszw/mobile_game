using UnityEngine;

public class PuddingAnimal : BaseAnimal<PuddingDefinition>
{
    
}

[CreateAssetMenu(fileName = "Pudding_Definition", menuName = "MobileSeals/Definitions/Pudding")]
public class PuddingDefinition : BaseAnimalDefinition<PuddingAnimal, PuddingDefinition>
{
    
}
