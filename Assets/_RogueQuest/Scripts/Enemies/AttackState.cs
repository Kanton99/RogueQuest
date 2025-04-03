using UnityEngine;

// �tat d'attaque
public class AttackState : EnemyState
{
    public AttackState(EnemyAI enemyAI) : base(enemyAI) { }

    // M�thode appel�e lors de l'entr�e dans l'�tat d'attaque
    public override void Enter()
    {
        Debug.Log("Entering Attack State");
    }

    // M�thode appel�e � chaque frame pour ex�cuter la logique d'attaque
    public override void Execute()
    {
        if (enemyAI == null) return;

        // Logique d'attaque
        Debug.Log("Attacking...");

        // Attaquer le joueur
        AttackPlayer();
    }

    // M�thode appel�e lors de la sortie de l'�tat d'attaque
    public override void Exit()
    {
        Debug.Log("Exiting Attack State");
    }

    // Logique pour infliger des d�g�ts au joueur
    private void AttackPlayer()
    {
        Debug.Log("Player attacked!");
    }
}
