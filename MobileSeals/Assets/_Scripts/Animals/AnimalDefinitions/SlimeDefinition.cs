using UnityEngine;


public class SlimeAnimal : BaseAnimal<SlimeDefinition>
{
    
}
[CreateAssetMenu(fileName = "Slime_Definition", menuName = "MobileSeals/Definitions/Slime")]
public class SlimeDefinition : BaseAnimalDefinition<SlimeAnimal, SlimeDefinition>
{
    
}
