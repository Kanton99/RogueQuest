using UnityEngine;

// Classe de base pour les états de l'ennemi
namespace RogueQuest
{
    public abstract class EnemyState
    {
        protected EnemyAI enemyAI;

        public EnemyState(EnemyAI enemyAI)
        {
            this.enemyAI = enemyAI;
        }

        // Méthode appelée lors de l'entrée dans l'état
        public abstract void Enter();

        // Méthode appelée à chaque frame pour exécuter la logique de l'état
        public abstract void Execute();

        // Méthode appelée lors de la sortie de l'état
        public abstract void Exit();
    }
}