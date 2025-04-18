﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace RogueQuest.Items
{
    public class Inventory : MonoBehaviour
    {
        [Header("Inventory Settings")]
        public int maxConsumables = 2;
        public GameObject dropItemPrefab;
        public Collider2D itemCollider;

        public Weapon weapon { get; private set; }

        public Consumable[] consumables; // Make this public to access from HUDManager
        int lastConsumableSlotUsed = 0;
        List<Equipment> equipment = new List<Equipment>();

        private HUDManager hudManager;

    private void Start()
    {
        // Initialize consumables array
        consumables = new Consumable[maxConsumables];
        hudManager = FindObjectOfType<HUDManager>();

        // Equip a default weapon if none is assigned
        if (weapon == null)
        {
            Weapon defaultWeapon = ScriptableObject.CreateInstance<Weapon>();
            defaultWeapon.itemName = "Enemy Sword";
            defaultWeapon.damage = 10;
            defaultWeapon.range = 1.5f;
            AddItem(defaultWeapon);
        }
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

            // Update quickslots in HUD
            if (hudManager != null)
            {
                hudManager.UpdateQuickSlots(this);
            }
        }

        private void AddEquipment(Equipment equipment)
        {
            this.equipment.Add(equipment);
            gameObject.GetComponent<EntityStats>().ApplyEffect(equipment.effect);
        }

        private void AddConsumable(Consumable consumable)
        {
            if (consumables[lastConsumableSlotUsed] != null)
            {
                Consumable consumable1 = consumables[lastConsumableSlotUsed];
                DropItem(consumable1);
            }

            consumables[lastConsumableSlotUsed] = consumable;
            Debug.Log($"Added consumable: {consumable.itemName} to slot {lastConsumableSlotUsed}");
            lastConsumableSlotUsed = (lastConsumableSlotUsed + 1) % maxConsumables;
        }

    private void SwitchWeapon(Weapon weapon)
    {
        if (this.weapon != null)
        {
            DropItem(this.weapon);
        }

        this.weapon = weapon;
        Debug.Log($"Equipped weapon: {weapon.itemName}");
    }

    private void DropItem(Item item)
    {
        if (dropItemPrefab != null && item != null)
        {
            GameObject dropItem = Instantiate(dropItemPrefab, transform.position, Quaternion.identity);
            dropItem.GetComponent<DroppedItem>().item = item;
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

            Collider2D closest = null;
            float closestDistance = float.MaxValue;

            foreach (Collider2D collider in collisions)
            {
                // Check if the collider has a DroppedItem component
                if (collider.GetComponent<DroppedItem>() == null)
                {
                    continue; // Skip colliders without DroppedItem
                }

                float distance = (collider.transform.position - transform.position).magnitude;
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closest = collider;
                }
            }

            if (closest == null)
            {
                Debug.LogWarning("No valid DroppedItem found in range.");
                return;
            }

            DroppedItem droppedItem = closest.GetComponent<DroppedItem>();
            if (droppedItem == null)
            {
                Debug.LogWarning("No DroppedItem component found on the closest collider.");
                return;
            }

            Debug.Log($"Picked up item: {droppedItem.item.itemName}");
            AddItem(droppedItem.item);
            Destroy(closest.gameObject);
        }
    }
}
