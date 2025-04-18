using UnityEngine;
using Cainos.PixelArtPlatformer_Dungeon;

public class PlayerInteractionChest : MonoBehaviour
{
    private Chest currentChest; // Référence au coffre proche

    void Update()
    {
        // Vérifie si le joueur appuie sur E et qu'un coffre est proche
        if (currentChest != null && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Interaction avec le coffre détectée."); // Débogage
            currentChest.IsOpened = !currentChest.IsOpened; // Alterne entre ouvrir et fermer le coffre
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Vérifie si l'objet avec lequel le joueur entre en collision est un coffre
        Chest chest = collision.GetComponent<Chest>();
        if (chest != null)
        {
            Debug.Log("Coffre détecté : " + chest.name); // Débogage
            currentChest = chest; // Stocke la référence au coffre
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Supprime la référence au coffre lorsque le joueur quitte la zone
        Chest chest = collision.GetComponent<Chest>();
        if (chest != null && currentChest == chest)
        {
            Debug.Log("Coffre quitté : " + chest.name); // Débogage
            currentChest = null;
        }
    }
}
