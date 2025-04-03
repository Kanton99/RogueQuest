using UnityEngine;

[CreateAssetMenu(fileName = "Room", menuName = "Level Generation/Room")]
public class Room : ScriptableObject
{
	public GameObject prefab;
	public Vector2Int size;
	public Direction[] direction;
}
	