using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEditor;
using System;

public class Generator : MonoBehaviour
{
	public Room[] rooms;
	public GameObject map;

	[Header("Room Settings")]
	public Vector2Int ROOM_SIZE = new Vector2Int(16, 16);
	public int roomCount = 10;

	private static Dictionary<Direction, Direction> oppositeDirection = new Dictionary<Direction, Direction>
	{
		{Direction.Up, Direction.Down},
		{Direction.Down, Direction.Up},
		{Direction.Left, Direction.Right},
		{Direction.Right, Direction.Left}
	};

	private static Dictionary<Direction, Vector2Int> directionVectors = new Dictionary<Direction, Vector2Int>
	{
		{Direction.Up, Vector2Int.up},
		{Direction.Down, Vector2Int.down},
		{Direction.Left, Vector2Int.left},
		{Direction.Right, Vector2Int.right}
	};


	private	Dictionary<Vector2Int, Room> roomMap;
	private	List<Vector2Int> positionsToPlace = new List<Vector2Int>();
	private List<Vector2Int> positionsUsed = new List<Vector2Int>();

	[ContextMenu("Generate")]
	[CustomEditor(typeof(Generator))]
	private void Generate()
	{
		map.GetComponent<Tilemap>().ClearAllTiles();
		roomMap = new Dictionary<Vector2Int, Room>();
		//initialize the roomMap with null values of size roomCount x roomCount
		positionsToPlace.Clear();
		positionsUsed.Clear();


		//Generate a 2D array of rooms
		positionsToPlace.Add(new Vector2Int(0, 0));

		for(int i = 0; i<roomCount; i++){
			if(positionsToPlace.Count == 0) break;

			int nextPosIter = UnityEngine.Random.Range(0, positionsToPlace.Count >= 5 ? 5 : positionsToPlace.Count);

			var newPos = positionsToPlace[nextPosIter];
			positionsToPlace.RemoveAt(0);
			if(positionsUsed.Contains(newPos)) {
				i--;
				continue;	
			}
			positionsUsed.Add(newPos);
			Room room = SelectRoom(newPos);
			if(room == null) continue;
			roomMap.Add(newPos, room);
			Direction[] directions = room.direction;
			foreach (Direction direction in directions)
			{
				Vector2Int positionToBeAdded = newPos + directionVectors[direction];
				if(positionsUsed.Contains(positionToBeAdded) || positionsToPlace.Contains(positionToBeAdded)) continue;
				positionsToPlace.Add(positionToBeAdded);
			}
		}

		//from the 2D array, place the rooms in the map
		foreach(var posAndRoom in roomMap)
		{
			Room room = posAndRoom.Value;
			if (room != null)
			{
				PlaceRoom(room.prefab, posAndRoom.Key.x*posAndRoom.Value.size.x, posAndRoom.Key.y*posAndRoom.Value.size.y);
			}
		}
	}

	private bool IsValidPosition(Vector2Int positionToBeAdded)
	{
		throw new NotImplementedException();
	}

	private Room SelectRoom(Vector2Int newPos)
	{
		//Get rooms around the position in the 2D array
		List<Direction> neededDirections = new List<Direction>();
		//loop through the directionVector dictionary
		foreach (var direction in directionVectors)
		{
			Vector2Int neighborPos = newPos + direction.Value;
			Room room = roomMap.GetValueOrDefault(neighborPos,null);
			if (room != null)
			{
				neededDirections.Add(oppositeDirection[direction.Key]);
			}
		}

		//Get a random room from the list of rooms that has the needed directions
		List<Room> possibleRooms = new List<Room>();
		foreach (Room room in rooms)
		{
			bool hasAllDirections = true;
			foreach (Direction direction in neededDirections)
			{
				if (!Array.Exists(room.direction, d => d==direction))
				{
					hasAllDirections = false;
					break;
				}
			}
			if (hasAllDirections)
			{
				possibleRooms.Add(room);
			}
		}

		//return a random room from the list of possible rooms
		if (possibleRooms.Count == 0)
		{
			return null;
		}
		int randomIndex = UnityEngine.Random.Range(0, possibleRooms.Count);
		return possibleRooms[randomIndex];
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
}
public enum Direction
{
	Up,
	Down,
	Left,
	Right
}
