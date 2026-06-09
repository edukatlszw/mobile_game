using UnityEngine;

public class SnakeAnimal : BaseAnimal<SnakeDefinition>
{
    
}

[CreateAssetMenu(fileName = "Snake_Definition", menuName = "MobileSeals/Definitions/Snake")]
public class SnakeDefinition : BaseAnimalDefinition<SnakeAnimal, SnakeDefinition>
{
    
}
