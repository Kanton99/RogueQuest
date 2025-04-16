using UnityEngine;

[CreateAssetMenu(fileName = "ItemSpawner", menuName = "Level Generation/Item Spawner")]
public class ItemSpawner : ScriptableObject
{
    [Header("Item Spawn Settings")]
    public GameObject[] possibleItems; // Liste des items pouvant �tre spawn�s
    public int minItems = 1; // Nombre minimum d'items � spawn
    public int maxItems = 3; // Nombre maximum d'items � spawn

    /// <summary>
    /// Spawns items in the given room at random positions.
    /// </summary>
    /// <param name="room">La salle o� les items seront spawn�s.</param>
    public void SpawnItems(Room room, GameObject roomInstance)
    {
        if (possibleItems == null || possibleItems.Length == 0)
        {
            Debug.LogWarning("No items configured for spawning.");
            return;
        }

        int itemCount = Random.Range(minItems, maxItems + 1);
        for (int i = 0; i < itemCount; i++)
        {
            // Choisir un item al�atoire
            GameObject itemPrefab = possibleItems[Random.Range(0, possibleItems.Length)];

            // Calculer une position al�atoire dans la salle
            Vector2 randomPosition = new Vector2(
                Random.Range(1, room.size.x)+roomInstance.transform.position.x+0.5f,
                Random.Range(1, room.size.y)+roomInstance.transform.position.y+0.5f
            );

            // Instancier l'item dans la salle
            GameObject spawnedItem = Instantiate(itemPrefab, randomPosition, Quaternion.identity);

            // Attacher l'item � l'instance de la salle dans la sc�ne
            spawnedItem.transform.parent = roomInstance.transform;

            // Ajouter l'item g�n�r� � la liste
            LevelGenerator generator = Object.FindAnyObjectByType<LevelGenerator>();
            generator.spawnedItems.Add(spawnedItem);
        }
    }
}
