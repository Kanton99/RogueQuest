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

        // Appliquer un boost de défense temporaire
        target.ApplyDefenseBoost(3f, 7f); // Augmente la défense de 3 pendant 7 secondes

        // Appliquer un boost de santé temporaire
        target.ApplyHealthBoost(20f, 15f); // Augmente la santé maximale de 20 pendant 15 secondes

        // Appliquer un modificateur permanent
        target.ApplyPermanentModifier(StatType.Attack, 10f); // Augmente l'attaque de 10 de manière permanente
    }
}


