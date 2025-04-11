using System.Collections;
using UnityEngine;

public class DroppedItem : MonoBehaviour
{
	public Item item;


	private void Start()
	{
		SpriteRenderer sprite = gameObject.GetComponent<SpriteRenderer>();
		if (sprite)
			sprite.sprite = item.sprite;
	}
}