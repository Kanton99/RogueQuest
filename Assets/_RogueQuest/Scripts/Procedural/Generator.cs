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

public class Generator : MonoBehaviour
{
	[Header("Room Prefabs")]
	public Room[] rooms;
	public GameObject map;

	[Header("Room Settings")]
	public Vector2Int ROOM_SIZE = new Vector2Int(16, 16);
	public int levelRadius = 10;

	[Header("Seed options")]
	public bool randomSeed = true;
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
	private static Dictionary<Vector2Int, Direction> reversedDirectionVectors = ReverseDictionary(directionVectors);


	private Dictionary<Vector2Int, Room> roomMap;
	private Dictionary<Vector2Int, int> weightMap;

	[ContextMenu("Generate fully")]
    private void GenerateLevelSteps()
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
		Unity.Mathematics.Random rng = new Unity.Mathematics.Random(seed);
		if (roomMap == null) roomMap = new Dictionary<Vector2Int, Room>();
		roomMap.Clear();
		if (weightMap == null) weightMap = new Dictionary<Vector2Int, int>();
		weightMap.Clear();

		for(int x = -levelRadius; x<=levelRadius;x++){
			for(int y = -levelRadius; y<=levelRadius;y++){
				Vector2Int pos = new Vector2Int(x, y);
				weightMap[pos] = rng.NextInt(0, 100);
			}
		}

		Path path = BuildRandomPath(from, to);

		Path[] paths = new Path[1];
		paths[0] = path;
		return paths;
	}
	

	[ContextMenu("Place Rooms")]
    private void PlaceRooms()
    {
        map.GetComponent<Tilemap>().ClearAllTiles();
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
                {
                    itemSpawner.SpawnItems(room, roomInstance);
                }

                // Spawner les ennemis dans la salle
                if (enemySpawner != null)
                {
                    enemySpawner.SpawnEnemies(room, roomInstance);
                }
            }
        }
    }

    GameObject PlaceRoom(GameObject roomPrefab, int x, int y)
    {
        // Instancier la salle dans la scène
        GameObject roomInstance = Instantiate(roomPrefab, new Vector3(x, y, 0), Quaternion.identity);

        // Ajouter la salle générée à la liste
        spawnedRooms.Add(roomInstance);

        Tilemap tilemap = roomInstance.GetComponent<Tilemap>();
        BoundsInt bounds = tilemap.cellBounds;
        TileBase[] roomTiles = tilemap.GetTilesBlock(bounds);

        for (int xi = 0; xi < bounds.size.x; xi++)
        {
            for (int yi = 0; yi < bounds.size.y; yi++)
            {
                TileBase tile = roomTiles[xi + yi * bounds.size.x];
                if (tile != null)
                {
                    Vector3Int position = new Vector3Int(xi - (bounds.size.x / 2) + x, yi - (bounds.size.y / 2) + y, 0);
                    Tilemap mapTilemap = map.GetComponent<Tilemap>();
                    mapTilemap.SetTile(position, tile);
                }
            }
        }

        // Retourner l'instance de la salle
        return roomInstance;
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
		int euclidianDistance = Mathf.Abs(from.x - to.x) + Mathf.Abs(from.y - to.y);

		List<RoomPath> toVisit = new List<RoomPath>();

		RoomPath start = new RoomPath();
		start.position = from;
		start.path = new Path();
		start.path.position = from;
		start.score = 0;
		toVisit.Add(start);

		for(int _ = 0; _ < 1000000; _++){
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
				if (weightMap.ContainsKey(neighborPos))
				{
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

	Direction[] getConnectedDirections(Vector2Int pos, Path path){
		Vector2Int prevDir = path.prev != null ? path.prev.position - path.position : new Vector2Int();
		Vector2Int nextDir = path.next != null ? path.next.position - path.position : new Vector2Int();

		//Create an array of directions with only the ones that are not 0,0
		List<Direction> directions = new List<Direction>();

		if (prevDir != new Vector2Int(0, 0)) directions.Add(reversedDirectionVectors[prevDir]);
		if (nextDir != new Vector2Int(0, 0)) directions.Add(reversedDirectionVectors[nextDir]);

		return directions.ToArray();
	}

	private static Dictionary<TValue, TKey> ReverseDictionary<TKey, TValue>(Dictionary<TKey, TValue> original)
	{
		Dictionary<TValue, TKey> reversed = new Dictionary<TValue, TKey>();
		foreach (var kvp in original)
		{
			reversed[kvp.Value] = kvp.Key;
		}
		return reversed;
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
            {
                if (Application.isPlaying)
                {
                    Destroy(room); // En mode jeu
                }
                else
                {
                    DestroyImmediate(room); // En mode édition
                }
            }
        }
        spawnedRooms.Clear();

        // Détruire tous les items générés
        foreach (GameObject item in spawnedItems)
        {
            if (item != null)
            {
                if (Application.isPlaying)
                {
                    Destroy(item); // En mode jeu
                }
                else
                {
                    DestroyImmediate(item); // En mode édition
                }
            }
        }
        spawnedItems.Clear();

        // Détruire tous les ennemis générés
        foreach (GameObject enemy in spawnedEnemies)
        {
            if (enemy != null)
            {
                if (Application.isPlaying)
                {
                    Destroy(enemy); // En mode jeu
                }
                else
                {
                    DestroyImmediate(enemy); // En mode édition
                }
            }
        }
        spawnedEnemies.Clear();

        // Nettoyer la tilemap
        if (map != null)
        {
            Tilemap tilemap = map.GetComponent<Tilemap>();
            if (tilemap != null)
            {
                tilemap.ClearAllTiles();
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
}
