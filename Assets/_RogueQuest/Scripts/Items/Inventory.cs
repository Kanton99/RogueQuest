using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
	[Header("Inventory Settings")]
	public int maxConsumables = 2;
	public GameObject dropItemPrefab;
	public Collider2D itemCollider;

	public Weapon weapon { get; private set; }

	Consumable[] consumables;
	int lastConsumableSlotUsed = 0;
	List<Equipment> equipment = new List<Equipment>();

	private void Start()
	{
		consumables = new Consumable[maxConsumables]; // Example size, adjust as needed
	}

	public void AddItem(Item item)
	{
		switch (item)
		{
			case Weapon weapon:
				SwitchWeapon(weapon);
				break;
			case Consumable consumable:
				AddConsumable(consumable);
				break;
			case Equipment equipment:
				AddEquipment(equipment);
				break;
			default:
				Debug.LogWarning($"Item type {item.GetType()} not supported.");
				break;
		}
	}

	private void AddEquipment(Equipment equipment)
	{
		this.equipment.Add(equipment);
		gameObject.GetComponent<EntityStats>().ApplyEffect(equipment.effect);
	}

	private void AddConsumable(Consumable consumable)
	{
		if(consumables[lastConsumableSlotUsed] != null){
			Consumable consumable1 = consumables[lastConsumableSlotUsed];
			DropItem(consumable1);
		}

		consumables[lastConsumableSlotUsed] = consumable;
		lastConsumableSlotUsed = (lastConsumableSlotUsed + 1) % maxConsumables;
	}

	private void SwitchWeapon(Weapon weapon)
	{
		DropItem(this.weapon);
		this.weapon = weapon;
	}

	private void DropItem(Item item)
	{
		if(dropItemPrefab != null && this.weapon != null)
		{
			GameObject dropItem = Instantiate(dropItemPrefab, transform.position, Quaternion.identity);
			//dropItem.GetComponents<SpriteRenderer>()[0].sprite = this.weapon.sprite;
			dropItem.GetComponent<DroppedItem>().item = this.weapon;
		}
	}

    public void PickUpItem()
    {
        Debug.Log("Picking up item");
        if (itemCollider == null)
        {
            Debug.LogWarning("Item collider is not assigned.");
            return;
        }

        ContactFilter2D contactFilter = new ContactFilter2D();
        List<Collider2D> collisions = new List<Collider2D>();

        if (itemCollider.Overlap(contactFilter, collisions) == 0)
        {
            Debug.LogWarning("No items to pick up.");
            return;
        }

        Collider2D closest = collisions[0];
        float closestDistance = float.MaxValue;
        foreach (Collider2D collider in collisions)
        {
            float distance = (collider.transform.position - transform.position).magnitude;
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = collider;
            }
        }

        DroppedItem droppedItem = closest.transform.GetComponent<DroppedItem>();
        if (droppedItem == null)
        {
            Debug.LogWarning("No DroppedItem component found on the closest collider.");
            return;
        }

        AddItem(droppedItem.item);
        Destroy(closest.transform.gameObject);
    }
}