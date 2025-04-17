using UnityEngine;

namespace RogueQuest.LevelGeneration
{
	[CreateAssetMenu(fileName = "Room", menuName = "Level Generation/Room")]
	public class Room : ScriptableObject
	{
		public GameObject prefab;
		public Vector2Int size = new Vector2Int(16, 16);
		public Direction[] directions;

		public Room(Room other)
		{
			prefab = other.prefab;
			size = other.size;
			directions = other.directions;
		}
	}
}