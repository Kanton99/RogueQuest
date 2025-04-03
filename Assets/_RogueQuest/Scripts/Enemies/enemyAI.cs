using UnityEngine;

// Classe de base pour les états de l'ennemi
public abstract class EnemyState
{
    protected EnemyAI enemyAI;

    public EnemyState(EnemyAI enemyAI)
    {
        this.enemyAI = enemyAI;
    }

    public abstract void Enter();
    public abstract void Execute();
    public abstract void Exit();
}

// État de patrouille
public class PatrolState : EnemyState
{
    public PatrolState(EnemyAI enemyAI) : base(enemyAI) { }

    public override void Enter()
    {
        Debug.Log("Entering Patrol State");
    }

    public override void Execute()
    {
        // Logique de patrouille
        Debug.Log("Patrolling...");

        // Détection du joueur
        if (DetectPlayer())
        {
            enemyAI.ChangeState(new ChaseState(enemyAI));
        }
    }

    public override void Exit()
    {
        Debug.Log("Exiting Patrol State");
    }

    private bool DetectPlayer()
    {
        RaycastHit hit;
        if (Physics.Raycast(enemyAI.transform.position, enemyAI.transform.forward, out hit, enemyAI.DetectionRange))
        {
            if (hit.collider.CompareTag("Player"))
            {
                return true;
            }
        }
        return false;
    }
}

// État de poursuite
public class ChaseState : EnemyState
{
    public ChaseState(EnemyAI enemyAI) : base(enemyAI) { }

    public override void Enter()
    {
        Debug.Log("Entering Chase State");
    }

    public override void Execute()
    {
        // Logique de poursuite
        Debug.Log("Chasing...");

        // Détection des obstacles
        if (DetectObstacle())
        {
            // Logique pour éviter l'obstacle
            Debug.Log("Obstacle detected!");
        }

        // Détection du joueur à portée d'attaque
        if (IsPlayerInRange())
        {
            enemyAI.ChangeState(new AttackState(enemyAI));
        }
    }

    public override void Exit()
    {
        Debug.Log("Exiting Chase State");
    }

    private bool DetectObstacle()
    {
        RaycastHit hit;
        if (Physics.Raycast(enemyAI.transform.position, enemyAI.transform.forward, out hit, enemyAI.DetectionRange))
        {
            if (hit.collider.CompareTag("Obstacle"))
            {
                return true;
            }
        }
        return false;
    }

    private bool IsPlayerInRange()
    {
        RaycastHit hit;
        if (Physics.Raycast(enemyAI.transform.position, enemyAI.transform.forward, out hit, enemyAI.AttackRange))
        {
            if (hit.collider.CompareTag("Player"))
            {
                return true;
            }
        }
        return false;
    }
}

// État d'attaque
public class AttackState : EnemyState
{
    public AttackState(EnemyAI enemyAI) : base(enemyAI) { }

    public override void Enter()
    {
        Debug.Log("Entering Attack State");
    }

    public override void Execute()
    {
        // Logique d'attaque
        Debug.Log("Attacking...");

        // Attaquer le joueur
        AttackPlayer();
    }

    public override void Exit()
    {
        Debug.Log("Exiting Attack State");
    }

    private void AttackPlayer()
    {
        // Logique pour infliger des dégâts au joueur
        Debug.Log("Player attacked!");
    }
}

// Classe de gestion de l'IA de l'ennemi
public class EnemyAI : MonoBehaviour
{
    private EnemyState currentState;

    [SerializeField] private float patrolSpeed = 2.0f;
    [SerializeField] private float chaseSpeed = 4.0f;
    [SerializeField] private float detectionRange = 5.0f;
    [SerializeField] private float attackRange = 1.5f;

    void Start()
    {
        // Initialiser avec l'état de patrouille
        ChangeState(new PatrolState(this));
    }

    void Update()
    {
        if (currentState != null)
        {
            currentState.Execute();
        }
    }

    public void ChangeState(EnemyState newState)
    {
        if (currentState != null)
        {
            currentState.Exit();
        }

        currentState = newState;

        if (currentState != null)
        {
            currentState.Enter();
        }
    }

    public float PatrolSpeed => patrolSpeed;
    public float ChaseSpeed => chaseSpeed;
    public float DetectionRange => detectionRange;
    public float AttackRange => attackRange;
}
