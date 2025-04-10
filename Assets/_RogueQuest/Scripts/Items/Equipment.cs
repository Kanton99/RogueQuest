using UnityEngine;

[CreateAssetMenu(fileName = "Equipment", menuName = "Items/Equipment")]
class Equipment : Item
{
	[Header("Equipment Properties")]
	[SerializeField]
	public Effect effect;

	public override void Use()
	{
		EntityStats entityStats = GameObject.FindGameObjectWithTag("Player").GetComponent<EntityStats>();
		if (entityStats != null)
			effect.Apply(entityStats);
		else
			Debug.LogWarning($"Player does not have EntityStats component.");
	}
}