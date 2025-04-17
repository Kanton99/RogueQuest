using System.Collections;
using UnityEngine;

namespace RogueQuest.Items
{
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
}