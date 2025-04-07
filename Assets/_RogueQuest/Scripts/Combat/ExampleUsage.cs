using UnityEngine;

public class ExampleUsage : MonoBehaviour
{
    public EntityStats target;

    private void Start()
    {
        // Appliquer un boost de vitesse temporaire
        target.ApplySpeedBoost(2f, 5f); // Augmente la vitesse de 2 pendant 5 secondes

        // Appliquer un boost d'attaque temporaire
        target.ApplyAttackBoost(5f, 10f); // Augmente l'attaque de 5 pendant 10 secondes

        // Appliquer un boost de d�fense temporaire
        target.ApplyDefenseBoost(3f, 7f); // Augmente la d�fense de 3 pendant 7 secondes

        // Appliquer un boost de sant� temporaire
        target.ApplyHealthBoost(20f, 15f); // Augmente la sant� maximale de 20 pendant 15 secondes

        // Appliquer un modificateur permanent
        target.ApplyPermanentModifier(StatType.Attack, 10f); // Augmente l'attaque de 10 de mani�re permanente
    }
}


