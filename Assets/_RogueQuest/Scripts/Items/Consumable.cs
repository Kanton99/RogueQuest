using UnityEngine;

[CreateAssetMenu(fileName = "Consumable", menuName = "Items/Consumable")]
class Consumable : Item
{
	[Header("Consumable Properties")]
	public Effect effect;
	public override void Use(GameObject target)
	{
		base.Use(target);
		Debug.Log($"Using {itemName} on {target.name}");
		EntityStats entityStats = target.GetComponent<EntityStats>();
		if(entityStats != null) 
			effect.Apply(entityStats);
		//Add login to remove the item from inventory
	}
	public override void PickUp()
	{
		base.PickUp();
	}
}