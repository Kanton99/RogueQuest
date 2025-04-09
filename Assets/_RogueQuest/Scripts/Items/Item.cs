using UnityEngine;

public abstract class Item : ScriptableObject
{
    [Header("Item Properties")]
	public Sprite sprite;
    public string itemName;
    public string description;

    public virtual void Use(GameObject target){
        Debug.Log($"Using {itemName}");
    }

    public virtual void PickUp()
    {
		Debug.Log($"Picking up {itemName}");
	}
}
