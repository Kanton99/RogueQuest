using UnityEngine;

// État d'inactivité
public class IdleState : EnemyState
{
    public IdleState(EnemyAI enemyAI) : base(enemyAI) { }

    // Méthode appelée lors de l'entrée dans l'état d'inactivité
    public override void Enter()
    {
        Debug.Log("Entering Idle State");
    }

    // Méthode appelée à chaque frame pour exécuter la logique d'inactivité
    public override void Execute()
    {
        if (enemyAI == null) return;

        // Logique d'inactivité (par exemple, attendre ou regarder autour)
        Debug.Log("Idling...");
    }

    // Méthode appelée lors de la sortie de l'état d'inactivité
    public override void Exit()
    {
        Debug.Log("Exiting Idle State");
    }
}

