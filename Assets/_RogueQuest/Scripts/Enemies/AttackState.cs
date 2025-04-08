using UnityEngine;

// État d'attaque
public class AttackState : EnemyState
{
    public AttackState(EnemyAI enemyAI) : base(enemyAI) { }

    // Méthode appelée lors de l'entrée dans l'état d'attaque
    public override void Enter()
    {
        Debug.Log("Entering Attack State");
    }

    // Méthode appelée à chaque frame pour exécuter la logique d'attaque
    public override void Execute()
    {
        if (enemyAI == null) return;

        // Logique d'attaque
        Debug.Log("Attacking...");

        // Attaquer le joueur
        AttackPlayer();
    }

    // Méthode appelée lors de la sortie de l'état d'attaque
    public override void Exit()
    {
        Debug.Log("Exiting Attack State");
    }

    // Logique pour infliger des dégâts au joueur
    private void AttackPlayer()
    {
        Debug.Log("Player attacked!");
    }
}
