using UnityEngine;

// �tat d'inactivit�
public class IdleState : EnemyState
{
    public IdleState(EnemyAI enemyAI) : base(enemyAI) { }

    // M�thode appel�e lors de l'entr�e dans l'�tat d'inactivit�
    public override void Enter()
    {
        Debug.Log("Entering Idle State");
    }

    // M�thode appel�e � chaque frame pour ex�cuter la logique d'inactivit�
    public override void Execute()
    {
        if (enemyAI == null) return;

        // Logique d'inactivit� (par exemple, attendre ou regarder autour)
        Debug.Log("Idling...");
    }

    // M�thode appel�e lors de la sortie de l'�tat d'inactivit�
    public override void Exit()
    {
        Debug.Log("Exiting Idle State");
    }
}

