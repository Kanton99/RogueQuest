using UnityEngine;

[CreateAssetMenu(fileName = "EnemySpawner", menuName = "Level Generation/Enemy Spawner")]
public class EnemySpawner : ScriptableObject
{
    [Header("Enemy Spawn Settings")]
    public GameObject[] possibleEnemies; // Liste des ennemis pouvant �tre spawn�s
    public int minEnemies = 1; // Nombre minimum d'ennemis � spawn
    public int maxEnemies = 5; // Nombre maximum d'ennemis � spawn

    /// <summary>
    /// Spawns enemies in the given room at random positions.
    /// </summary>
    /// <param name="room">La salle o� les ennemis seront spawn�s.</param>
    public void SpawnEnemies(Room room, GameObject roomInstance)
    {
        if (possibleEnemies == null || possibleEnemies.Length == 0)
        {
            Debug.LogWarning("No enemies configured for spawning.");
            return;
        }

        int enemyCount = Random.Range(minEnemies, maxEnemies + 1);
        for (int i = 0; i < enemyCount; i++)
        {
            // Choisir un ennemi al�atoire
            GameObject enemyPrefab = possibleEnemies[Random.Range(0, possibleEnemies.Length)];

            // Calculer une position al�atoire dans la salle
            Vector2 randomPosition = new Vector2(
                Random.Range(1, room.size.x)+roomInstance.transform.position.x+0.5f,
                Random.Range(1, room.size.y)+roomInstance.transform.position.y+0.5f
            );

            // Instancier l'ennemi dans la salle
            GameObject spawnedEnemy = Instantiate(enemyPrefab, randomPosition, Quaternion.identity);

            // Attacher l'ennemi � l'instance de la salle dans la sc�ne
            spawnedEnemy.transform.parent = roomInstance.transform;

            // Ajouter l'ennemi g�n�r� � la liste
            LevelGenerator generator = Object.FindAnyObjectByType<LevelGenerator>();
            generator.spawnedEnemies.Add(spawnedEnemy);
        }
    }
}
