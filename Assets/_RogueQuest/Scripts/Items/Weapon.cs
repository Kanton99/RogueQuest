using System.Collections;
using UnityEngine;

namespace RogueQuest.Items
{
	[CreateAssetMenu(fileName = "Weapon", menuName = "Items/Weapon")]
	public class Weapon : Item
	{
		[Header("Weapon Stats")]
		public int damage;
		public float attackSpeed;
		public float range;

		public override void Use()
		{
			Debug.Log($"Attacking with {itemName} for {damage} damage");
		}

		public override void PickUp()
		{
			base.PickUp();
			Debug.Log($"Picking up {itemName} with damage {damage}");
		}
	}
}