using UnityEngine;

public class PikachuAnimal : BaseAnimal<PikachuDefinition>
{
    
}
[CreateAssetMenu(fileName = "Pikachu_Definition", menuName = "MobileSeals/Definitions/Pikachu")]
public class PikachuDefinition : BaseAnimalDefinition<PikachuAnimal, PikachuDefinition>
{
    
}
