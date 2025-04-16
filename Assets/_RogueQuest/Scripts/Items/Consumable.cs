using UnityEngine;

[CreateAssetMenu(fileName = "Consumable", menuName = "Items/Consumable")]
public class Consumable : Item // Change 'class' to 'public class'
{
    [Header("Consumable Properties")]
    public Effect effect;
    public override void Use() { }
    public override void PickUp() { }
}
