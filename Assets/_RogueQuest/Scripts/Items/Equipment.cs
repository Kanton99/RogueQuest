using UnityEngine;

[CreateAssetMenu(fileName = "Equipment", menuName = "Items/Equipment")]
class Equipment : Item
{
	[Header("Equipment Properties")]
	[SerializeField]
	public Effect effect;
	public override void Use(GameObject target)
	{
		base.Use(target);
		Debug.Log($"Equipping {itemName} on {target.name}");
		EntityStats entityStats = target.GetComponent<EntityStats>();
		if (entityStats != null)
		{
			effect.Apply(entityStats);
		}
		else
		{
			Debug.LogWarning($"Target {target.name} does not have EntityStats component.");
		}
	}
}