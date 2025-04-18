using UnityEngine;
using Cainos.PixelArtPlatformer_Dungeon;

namespace RogueQuest
{
    public class PlayerDoorInteraction : MonoBehaviour
    {
        private Door currentDoor; // Référence à la porte proche

        void Update()
        {
            // Vérifie si le joueur appuie sur E et qu'une porte est proche
            if (currentDoor != null && Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("Interaction avec la porte détectée."); // Débogage
                currentDoor.Toggle(); // Alterne entre ouvrir et fermer la porte
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // Vérifie si l'objet avec lequel le joueur entre en collision est une porte
            Door door = collision.GetComponent<Door>();
            if (door != null)
            {
                Debug.Log("Porte détectée : " + door.name); // Débogage
                currentDoor = door; // Stocke la référence à la porte
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            // Supprime la référence à la porte lorsque le joueur quitte la zone
            Door door = collision.GetComponent<Door>();
            if (door != null && currentDoor == door)
            {
                Debug.Log("Porte quittée : " + door.name); // Débogage
                currentDoor = null;
            }
        }
    }
}
