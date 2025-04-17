using UnityEngine;

namespace RogueQuest.Items
{
	[CreateAssetMenu(fileName = "Consumable", menuName = "Items/Consumable")]
	public class Consumable : Item
	{
		[Header("Consumable Properties")]
		public Effect effect;

		public override void Use()
		{
			EntityStats entityStats = GameObject.FindGameObjectWithTag("Player").GetComponent<EntityStats>();
			if (entityStats != null)
				effect.Apply(entityStats);
			//Add login to remove the item from inventory
		}
		public override void PickUp()
		{
		}
	}
}