using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyAi : MonoBehaviour
{
    [SerializeField] GameObject SeePlayer;
    [SerializeField] GameObject TeleportActivation;
    [SerializeField] GameObject unlockPlanetScreen;

    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIdGround, whatIsPlayer;
    [SerializeField] TrainerController trainer;

    public static float health;

    public float Health { get { return health; } }

    //Patrolling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    public GameObject projectile;

    //States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        //check to see if the player is within sight or attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange) Patroling();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInSightRange && playerInAttackRange) AttackPlayer();
    }

    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if(walkPointSet)
        {
            agent.SetDestination(walkPoint);
            agent.updateRotation = true;
        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //walkpoint reached
        if(distanceToWalkPoint.magnitude < 1f)
        {
            walkPointSet = false;
        }
    }

    private void SearchWalkPoint()
    {
        SeePlayer.SetActive(false);

        //calculate a random point range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if(Physics.Raycast(walkPoint, -transform.up, 2f, whatIdGround))
        {
            walkPointSet = true;
        }
    }

    private void ChasePlayer()
    {
        trainer.TriggerTrainerBattle();

        SeePlayer.SetActive(true);

        agent.SetDestination(player.position);
        agent.updateRotation = true;
    }

    private void AttackPlayer()
    {
        //make sure the enemy dosnt move on attack
        agent.SetDestination(transform.position);
        agent.updateRotation = true;

        //look at player
        transform.LookAt(player);

        if(!alreadyAttacked)
        {
            //attack code here
            GameController.Instance.StartTrainerBattle(trainer);
            
            //has the enemy attacked
            alreadyAttacked = true;
        }
    }

    private void OnDrawGizmosSelected()
    {
        //this will visualize the attack and sight range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
