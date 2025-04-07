using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEditor;
using System;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine.InputSystem.Controls;
using Unity.Mathematics;

public class Generator : MonoBehaviour
{
	public Room[] rooms;
	public GameObject map;

	[Header("Room Settings")]
	public Vector2Int ROOM_SIZE = new Vector2Int(16, 16);
	public int roomCount = 10;
	public uint seed;

	public Vector2Int from;
	public Vector2Int to;

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


	private	Dictionary<Vector2Int, Room[]> roomMapPossibilities;
	private Dictionary<Vector2Int, Room> roomMap;
	private	List<Vector2Int> positionsToPlace = new List<Vector2Int>();
	private List<Vector2Int> positionsUsed = new List<Vector2Int>();

	[ContextMenu("Generate fully")]
	private void GenerateLevelSteps(){
		GenerateInitialLayout();
		//CleanUp();
		PlaceRooms();
	}


	[ContextMenu("Generate Layout")]
	private void GenerateInitialLayout()
	{
		Unity.Mathematics.Random rng = new Unity.Mathematics.Random(seed);
		if (roomMap == null) roomMap = new Dictionary<Vector2Int, Room>();
		roomMap.Clear();
		for(int x = -roomCount; x<=roomCount;x++){
			for(int y = -roomCount; y<=roomCount;y++){
				Vector2Int pos = new Vector2Int(x, y);
				roomMap[pos] =  new Room(rooms[0]);
				//apply random weight between 0-10 to the room

				roomMap[pos].weight = rng.NextInt(0, 100);
			}
		}

		Path path = BuildRandomPath(from, to);
		Debug.Log(path);
	}
	

	[ContextMenu("Place Rooms")]
	private void PlaceRooms(){
		map.GetComponent<Tilemap>().ClearAllTiles();
		foreach(var posAndRoom in roomMap)
		{
			Room room = posAndRoom.Value;
			Vector2Int pos = posAndRoom.Key;
			if (room != null)
			{
				PlaceRoom(room.prefab, pos.x*room.size.x, pos.y*room.size.y);
			}
		}
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

	private struct RoomPath
	{
		public Room room;
		public Vector2Int position;
		public Path path;
		public int score;
	}
	Path BuildRandomPath(Vector2Int from, Vector2Int to){
		int euclidianDistance = Mathf.Abs(from.x - to.x) + Mathf.Abs(from.y - to.y);

		List<RoomPath> toVisit = new List<RoomPath>();

		RoomPath start = new RoomPath();
		start.room = roomMap[from];
		start.position = from;
		start.path = new Path();
		start.path.position = from;
		start.score = 0;
		toVisit.Add(start);

		for(int _ = 0; _ < 10000; _++){
			RoomPath current = toVisit[0];
			toVisit.RemoveAt(0);

			//get the neighbors of current
			List<RoomPath> neighbors = new List<RoomPath>();
			foreach (var direction in Enum.GetValues(typeof(Direction)).Cast<Direction>())
			{
				Vector2Int neighborPos = current.position + directionVectors[direction];
				if (roomMap.ContainsKey(neighborPos))
				{
					RoomPath neighbor = new RoomPath();
					neighbor.room = roomMap[neighborPos];
					neighbor.position = neighborPos;
					Path path = new Path();
					path.position = neighborPos;
					path.prev = current.path;
					neighbor.path = path;
					neighbor.score = current.path.Length() + roomMap[neighborPos].weight + 1;
					neighbors.Add(neighbor);
				}
			}
			//toVisit.AddRange(neighbors);
			//Add neighbors to the toVisit list if not already present
			foreach (var neighbor in neighbors)
			{
				if (!toVisit.Any(x => x.position == neighbor.position))
				{
					toVisit.Add(neighbor);
				}
			}

			toVisit = toVisit.OrderBy(x => x.score).ToList();
			//Check if we reached the target
			if (current.position == to)
			{
				return current.path;
			}
			if (toVisit.Count == 0) return null;
		}
		return null;

	}
}
public enum Direction
{
	Up,
	Down,
	Left,
	Right
}

public class Path{
	public Path prev;
	public Path next;

	public Vector2Int position;

	public int Length(){
		return 1 + (prev != null ? prev.Length() : 0);
	}

	public override string ToString()
	{
		string str = position.ToString();
		if (prev != null)
		{
			str += " <- " + prev.ToString();
		}
		return str;
	}
}
