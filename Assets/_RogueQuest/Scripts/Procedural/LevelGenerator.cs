using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEditor;
using System;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine.InputSystem.Controls;
using Unity.Mathematics;
using Unity.VisualScripting;

public class LevelGenerator : MonoBehaviour
{
	[Header("Room Prefabs")]
	public Room[] rooms;
	public Tilemap ground;
	public Tilemap background;
	public TileBase wallTile;

	[Header("Room Settings")]
	public int randomnessStrength = 10;

	[Header("Seed options")]
	public bool randomSeed = false;
	public uint seed;

	public Vector2Int from;
	public Vector2Int to;

	private static Dictionary<Direction, Vector2Int> directionVectors = new Dictionary<Direction, Vector2Int>
	{
		{Direction.Up, Vector2Int.up},
		{Direction.Down, Vector2Int.down},
		{Direction.Left, Vector2Int.left},
		{Direction.Right, Vector2Int.right}
	};


	private Dictionary<Vector2Int, Room> roomMap;
	private Dictionary<Vector2Int, int> weightMap;
	private List<GameObject> props = new List<GameObject>();
	private GameObject propContainer;
	private Unity.Mathematics.Random rng;

	[ContextMenu("Generate fully")]
	public void GenerateLevelSteps()
	{
		// Nettoyer les objets générés précédemment
		CleanUp();

		// Générer le niveau
		Path[] paths = GenerateInitialLayout();
		SetupRoomMap(paths);
		PlaceRooms();
	}


	[ContextMenu("Generate Layout")]
	private Path[] GenerateInitialLayout()
	{
		if (randomSeed) seed = (uint)UnityEngine.Random.Range(0, UInt32.MaxValue);
		rng = new Unity.Mathematics.Random(seed);
		to = new Vector2Int(rng.NextInt(), rng.NextInt());

		if (roomMap == null) roomMap = new Dictionary<Vector2Int, Room>();
		roomMap.Clear();

		if (weightMap == null) weightMap = new Dictionary<Vector2Int, int>();
		weightMap.Clear();
		weightMap[from] = int.MaxValue;
		weightMap[to] = int.MaxValue;

		if(propContainer==null) propContainer = new GameObject("Props");

		Path path = new Path();
		path.Append((BuildRandomPath(from + Vector2Int.right, to + Vector2Int.left)));
		path.position = to;
		Path end = new Path();
		end.position = from;
		path.Append(end);
		

		Path[] paths = new Path[1];
		paths[0] = path;
		return paths;
	}
	

	[ContextMenu("Place Rooms")]
	private void PlaceRooms()
	{
		foreach (var posAndRoom in roomMap)
		{
			Room room = posAndRoom.Value;
			Vector2Int pos = posAndRoom.Key;
			if (room != null)
			{
				// Placer la salle et obtenir son instance
				GameObject roomInstance = PlaceRoom(room.prefab, pos.x * room.size.x, pos.y * room.size.y);

				// Spawner les items dans la salle
				if (itemSpawner != null)
					itemSpawner.SpawnItems(room, roomInstance);

				// Spawner les ennemis dans la salle
				if (enemySpawner != null)
					enemySpawner.SpawnEnemies(room, roomInstance);
			}
		}
	}

	GameObject PlaceRoom(GameObject room, int x, int y)
	{
		/* Room prefab structure
		 room <TileMap>
			Props
				[0]
				[1]
			Mur <TileMap>
		 */
		Tilemap groundTileMap = room.GetComponent<Tilemap>();
		Tilemap backgroundTilemap = null;
		if(room.transform.childCount > 2)
			backgroundTilemap = room.transform.Find("Mur").GetComponent<Tilemap>();
		BoundsInt bounds = groundTileMap.cellBounds;
		TileBase[] groundTiles = groundTileMap.GetTilesBlock(bounds);
		TileBase[] backgroundTiles = null;
		if(backgroundTilemap)
			backgroundTiles = backgroundTilemap.GetTilesBlock(bounds);

		BoundsInt placementBounds = new BoundsInt(new Vector3Int(x, y, 0), bounds.size);
		ground.SetTilesBlock(placementBounds, groundTiles);
		if(backgroundTiles != null)
			background.SetTilesBlock(placementBounds, backgroundTiles);

		GameObject roomObject = new GameObject();
		roomObject.transform.position = new Vector3(x, y, 0);
		spawnedRooms.Add(roomObject);
		// TODO COPY PROPS
		if(room.transform.childCount < 1) return roomObject;
		GameObject prefabProps = room.transform.Find("Props").gameObject;
		for (int i = 0; i < prefabProps.transform.childCount; i++)
		{
			GameObject prop = prefabProps.transform.GetChild(i).gameObject;
			Vector3 propPosition = new Vector3(9 + prop.transform.position.x, 5 + prop.transform.position.y, 0);
			prop = GameObject.Instantiate(prop, roomObject.transform);
			prop.transform.localPosition = propPosition;
			props.Add(prop);
		}

		return roomObject;

	}

	[Header("Item Spawner")]
	public ItemSpawner itemSpawner;
	[Header("Enemy Spawner")]
	public EnemySpawner enemySpawner;
	private List<GameObject> spawnedRooms = new List<GameObject>();
	public List<GameObject> spawnedItems = new List<GameObject>();
	public List<GameObject> spawnedEnemies = new List<GameObject>();
	private struct RoomPath
	{
		public Vector2Int position;
		public Path path;
		public int score;
	}
	Path BuildRandomPath(Vector2Int from, Vector2Int to){
		List<RoomPath> toVisit = new List<RoomPath>();

		RoomPath start = new RoomPath();
		start.position = from;
		start.path = new Path();
		start.path.position = from;
		start.score = 0;
		toVisit.Add(start);

		for(int _ = 0; _ < 10000; _++){
			if (toVisit.Count == 0) return null;
			RoomPath current = toVisit[0];
			toVisit.RemoveAt(0);

			//Check if we reached the target
			if (current.position == to)
			{
				return current.path;
			}
			//get the neighbors of current
			List<RoomPath> neighbors = new List<RoomPath>();
			foreach (var direction in Enum.GetValues(typeof(Direction)).Cast<Direction>())
			{
				Vector2Int neighborPos = current.position + directionVectors[direction];
				if (!weightMap.ContainsKey(neighborPos))
					weightMap[neighborPos] = rng.NextInt(0, randomnessStrength);

				RoomPath neighbor = new RoomPath();
				neighbor.position = neighborPos;
				Path path = new Path();
				path.position = neighborPos;
				path.prev = current.path;
				current.path.next = path;
				neighbor.path = path;
				neighbor.score = current.path.Length() + weightMap[neighborPos] + 1;
				neighbors.Add(neighbor);
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
		}
		return null;

	}

	private void SetupRoomMap(Path[] paths){
		if (paths.Length == 0) return;
		Path path = paths[0];
		while (path != null)
		{
			roomMap[path.position] = rooms[0];
			path = path.prev;
		}

		IEnumerable<Vector2Int> roomPositions = roomMap.Keys;
		for(int i = 0; i<roomPositions.Count();  i++){
			Vector2Int roomPos = roomPositions.ElementAt(i);
			List<Direction> neighbors = new List<Direction>();
			if (roomMap.ContainsKey(roomPos + Vector2Int.left)) neighbors.Add(Direction.Left);
			if (roomMap.ContainsKey(roomPos + Vector2Int.right)) neighbors.Add(Direction.Right);
			if (roomMap.ContainsKey(roomPos + Vector2Int.up)) neighbors.Add(Direction.Up);
			if (roomMap.ContainsKey(roomPos + Vector2Int.down)) neighbors.Add(Direction.Down);
			neighbors.Sort();

			Room[] possibilities = Array.FindAll(rooms, r =>
			{
				Array.Sort(r.directions);
				return ArrayUtility.ArrayEquals(neighbors.ToArray(), r.directions);
			});


			Room r = possibilities.Length > 0 ? possibilities[UnityEngine.Random.Range(0, possibilities.Length)] : rooms[0];

			roomMap[roomPos] = r;
		}
	}
	private void CleanUp()
	{
		// Détruire toutes les salles générées
		foreach (GameObject room in spawnedRooms)
		{
			if (room != null)
				#if !UNITY_EDITOR
					Destroy(room); // En mode jeu
				#else
					DestroyImmediate(room); // En mode édition
				#endif
		}
		spawnedRooms.Clear();

		// Détruire tous les items générés
		foreach (GameObject item in spawnedItems)
		{
			if (item != null)
			#if !UNITY_EDITOR
					Destroy(item); // En mode jeu
			#else
					DestroyImmediate(item); // En mode édition
			#endif
		}
		spawnedItems.Clear();

		// Détruire tous les ennemis générés
		foreach (GameObject enemy in spawnedEnemies)
		{
			if (enemy != null)
			#if !UNITY_EDITOR
					Destroy(enemy); // En mode jeu
			#else
					DestroyImmediate(enemy); // En mode édition
			#endif
		}
		spawnedEnemies.Clear();

		// Nettoyer la tilemap
		if (ground)
			ground.ClearAllTiles();
		if (background)
			background.ClearAllTiles();
		if (props.Count > 0)
		{
			foreach(GameObject prop in props){
				if(prop)
				#if !UNITY_EDITOR
						Destroy(prop); // En mode jeu
				#else
						DestroyImmediate(prop); // En mode édition
				#endif
			}
		}
	}
	public void AddSpawnedItem(GameObject item)
	{
		spawnedItems.Add(item);
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

	public bool HasPosition(Vector2Int pos){
		return pos == position || prev.HasPosition(pos);
	}

	public void Append(Path path){
		if (prev != null)
			prev.Append(path);
		else{
			prev = path;
			path.next = this;
		}
	}
}
