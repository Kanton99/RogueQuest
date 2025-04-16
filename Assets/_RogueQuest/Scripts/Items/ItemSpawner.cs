using UnityEngine;

[CreateAssetMenu(fileName = "ItemSpawner", menuName = "Level Generation/Item Spawner")]
public class ItemSpawner : ScriptableObject
{
    [Header("Item Spawn Settings")]
    public GameObject[] possibleItems; // Liste des items pouvant être spawnés
    public int minItems = 1; // Nombre minimum d'items à spawn
    public int maxItems = 3; // Nombre maximum d'items à spawn

    /// <summary>
    /// Spawns items in the given room at random positions.
    /// </summary>
    /// <param name="room">La salle où les items seront spawnés.</param>
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
            // Choisir un item aléatoire
            GameObject itemPrefab = possibleItems[Random.Range(0, possibleItems.Length)];

            // Calculer une position aléatoire dans la salle
            Vector2 randomPosition = new Vector2(
                Random.Range(1, room.size.x)+roomInstance.transform.position.x+0.5f,
                Random.Range(1, room.size.y)+roomInstance.transform.position.y+0.5f
            );

            // Instancier l'item dans la salle
            GameObject spawnedItem = Instantiate(itemPrefab, randomPosition, Quaternion.identity);

            // Attacher l'item à l'instance de la salle dans la scène
            spawnedItem.transform.parent = roomInstance.transform;

            // Ajouter l'item généré à la liste
            LevelGenerator generator = Object.FindAnyObjectByType<LevelGenerator>();
            generator.spawnedItems.Add(spawnedItem);
        }
    }
}
