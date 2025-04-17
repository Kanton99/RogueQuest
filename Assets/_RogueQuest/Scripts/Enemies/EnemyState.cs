using UnityEngine;

// Classe de base pour les �tats de l'ennemi
namespace RogueQuest
{
    public abstract class EnemyState
    {
        protected EnemyAI enemyAI;

        public EnemyState(EnemyAI enemyAI)
        {
            this.enemyAI = enemyAI;
        }

        // M�thode appel�e lors de l'entr�e dans l'�tat
        public abstract void Enter();

        // M�thode appel�e � chaque frame pour ex�cuter la logique de l'�tat
        public abstract void Execute();

        // M�thode appel�e lors de la sortie de l'�tat
        public abstract void Exit();
    }
}