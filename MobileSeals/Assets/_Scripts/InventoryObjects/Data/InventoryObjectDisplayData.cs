using UnityEngine;

[CreateAssetMenu(fileName = "ItemDefinition", menuName = "Inventory/ItemDisplayData")]
public class InventoryObjectDisplayData : ScriptableObject
{
    [SerializeField]
    internal string displayName;
    [SerializeField]
    internal string description;
    [SerializeField]
    internal Sprite icon;
}
