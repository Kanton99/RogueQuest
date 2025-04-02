using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEditor;

public class Generator : MonoBehaviour
{
	public List<GameObject> rooms = new List<GameObject>();
	public GameObject map;

	[Header("Room Settings")]
	public Vector2Int ROOM_SIZE = new Vector2Int(16, 16);
	public Vector2Int MAP_SIZE = new Vector2Int(2, 1);

	private Dictionary<Direction, Direction> oppositeDirection = new Dictionary<Direction, Direction>
	{
		{Direction.Up, Direction.Down},
		{Direction.Down, Direction.Up},
		{Direction.Left, Direction.Right},
		{Direction.Right, Direction.Left}
	};

	[ContextMenu("Generate")]
	[CustomEditor(typeof(Generator))]
	private void Generate()
	{
		map.GetComponent<Tilemap>().ClearAllTiles();
		//Generate a 2D array of rooms
		Room[,] roomMap = new Room[MAP_SIZE.x, MAP_SIZE.y];
		List<Vector2Int> positionsToPlace = new List<Vector2Int>();

		//from the 2D array, place the rooms in the map
	}

	void PlaceRoom(GameObject room, int x, int y)
	{
		Tilemap tilemap = room.GetComponent<Tilemap>();
		BoundsInt bounds = tilemap.cellBounds;
		var size = tilemap.size;
		TileBase[] roomTiles = tilemap.GetTilesBlock(bounds);

		for (int xi = 0; xi < bounds.size.x; xi++)
		{
			for (int yi = 0; yi < bounds.size.y; yi++)
			{
				TileBase tile = roomTiles[xi + yi * bounds.size.x];
				if (tile != null)
				{
					Vector3Int position = new Vector3Int(xi - (bounds.size.x / 2)+x, yi - (bounds.size.y / 2)+y, 0);
					Tilemap mapTilemap = map.GetComponent<Tilemap>();
					mapTilemap.SetTile(position, tile);
				}
			}
		}
	}

	GameObject SelectRoom(GameObject prevRoom, Vector2Int prevPosition, Vector2Int newPosition)
	{
		if(prevRoom == null)
		{
			return rooms[Random.Range(0, rooms.Count)];
		}
		var directions = FindConnections(prevRoom);
		List<GameObject> validRooms = new List<GameObject>();
		foreach (var room in rooms)
		{
			var roomDirections = FindConnections(room);
			foreach (var direction in directions)
			{
				if (System.Array.Exists(roomDirections, d => d == oppositeDirection[direction]))
				{
					validRooms.Add(room);
					break;
				}
			}
		}
		if (validRooms.Count == 0)
		{
			//return rooms[Random.Range(0, rooms.Count)];
		}
		return validRooms[Random.Range(0, validRooms.Count)];
	}

	Direction[] FindConnections(GameObject room)
	{
		//check the room for holes in the outer walls
		Tilemap tilemap = room.GetComponent<Tilemap>();
		BoundsInt bounds = tilemap.cellBounds;
		var size = tilemap.size;
		TileBase[] roomTiles = tilemap.GetTilesBlock(bounds);

		List<Direction> connections = new List<Direction>();
		for (int x = 0; x < bounds.size.x; x++)
		{
			if(roomTiles[x] == null && roomTiles[x+1] == null)
			{
				connections.Add(Direction.Up);
			}
			if (roomTiles[x + (bounds.size.y - 1) * bounds.size.x] == null &&
				roomTiles[(x+1) + (bounds.size.y - 1) * bounds.size.x] == null)
			{
				connections.Add(Direction.Down);
			}
		}

		for (int y = 0; y < bounds.size.y; y++)
		{
			if (roomTiles[y * bounds.size.x] == null &&
				roomTiles[(y+1) * bounds.size.x] == null)
			{
				connections.Add(Direction.Left);
			}
			if (roomTiles[(bounds.size.x - 1) + y * bounds.size.x] == null && roomTiles[(bounds.size.x - 1) + y * bounds.size.x] == null)
			{
				connections.Add(Direction.Right);
			}
		}

		return connections.ToArray();
	}


	private enum Direction
	{
		Up,
		Down,
		Left,
		Right
	}

	private struct Room
	{
		public GameObject room;
		public Direction[] accesses;
	}
}
