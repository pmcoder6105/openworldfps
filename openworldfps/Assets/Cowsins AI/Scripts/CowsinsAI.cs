using cowsins;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(EnemyAI))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class CowsinsAI : MonoBehaviour
{
    public bool wandering;
    public float waitTimeBetweenWander = 5f;
    public float wanderRadius;

    #region FOV

    [Range(0, 360)] public float searchAngle;
    [Range(0, 360)] public float attackSearchAngle;

    public float searchRadius;

    public LayerMask obstructionMasks;
    public LayerMask targetMask;

    #endregion

    public float attackRange;
    public bool increaseSightOnAttack;

    private bool attackedMelee;
    public float timeBetweenAttacks;
    private bool inRange;

    private Vector3 destination;
    public bool canSeePlayer;
    private float wanderWaitTimer;

    #region Shooting Variables
    public Transform firePoint;
    public float spreadAmount;
    public TrailRenderer bulletTrail;
    public float damageAmount;
    public float timeBetweenShots;
    private bool attackedShooter;
    public GameObject projectile;
    public AudioClip fireClip;

    private ObjectSoundManager objectSoundManager;

    [Serializable]
    public class Effects
    {
        public GameObject grassImpact, metalImpact, mudImpact, woodImpact, enemyImpact;
    }

    [Serializable]
    public class ImpactEffects
    {
        public GameObject defaultImpact, grassImpact, metalImpact, mudImpact, woodImpact, enemyImpact;
    }

    public Effects effects;

    public ImpactEffects impactEffects;

    #endregion

    private NavMeshAgent agent;
    private Animator animator;

    [HideInInspector] public GameObject player;

    public bool useRagdoll;

    public bool useDeathAnimation;
    public bool destroyAfterTimer;
    public float destroyTimer;
    
    public bool dumbAI;

    public Waypoints _waypoints;

    public float waypointWaitTime = 1f;

    private float waypointWaitTimer = 0f;

    private int currentWaypoint;

    private Vector3 idleStartPos;
    private Quaternion idleStartRot;

    private bool searchTimerStarted = false;
    public float searchTimer = 5;
    private float currentSearchTime;

    #region Enum References

    public State aiState;

    public MoveMode moveMode;

    public ShooterType shooterType;

    public AIType aiType;

    public EnemyType enemyType;

    #endregion

    #region Enum Logic

    public enum State
    {
        Idle = 0,
        Attack = 1,
        Search = 2
    }

    public enum MoveMode
    {
        Waypoints = 0,
        Random = 1,
        Idle = 2
    }

    public enum ShooterType
    {
        Projectile = 0,
        Hitscan = 1
    }

    public enum AIType
    {
        Enemy = 0,
        NPC = 1
    }

    public enum EnemyType
    {
        Shooter = 0,
        Melee = 1
    }

    #endregion

    public bool searchRadiusDebug;
    public bool attackRangeDebug;
    public bool canSeePlayerDebug;

    public SaveSettingSO saveSO;

    private void Awake()
    {
        aiState = State.Idle;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        wanderWaitTimer = waitTimeBetweenWander;
        player = GameObject.FindGameObjectWithTag("Player");

        StartCoroutine(FOVRoutine());

        if(moveMode == MoveMode.Idle)
        {
            idleStartPos = transform.position;
            idleStartRot = transform.rotation;
            agent.enabled = false;
        }

        objectSoundManager = gameObject.AddComponent<ObjectSoundManager>();
    }

    #region FOV

    private IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);

        while (true)
        {
            yield return wait;
            FOVCheck();
        }
    }

    private void FOVCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, searchRadius, targetMask);

        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < searchAngle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMasks))
                {
                    canSeePlayer = true;
                }
                else
                {
                    canSeePlayer = false;
                }
            }
            else
            {
                canSeePlayer = false;
            }
        }
        else if (canSeePlayer)
        {
            canSeePlayer = false;
        }
    }

    #endregion

    private void Update()
    {
        if(aiType == AIType.NPC)
        {
            IdleNPCState();
        }

        if(aiType == AIType.Enemy)
        {
            switch (aiState)
            {
                case State.Idle:
                    EnemyIdleState();
                    break;

                case State.Search:
                    EnemySearchState();
                    break;

                case State.Attack:
                    EnemyAttackState();
                    break;
            }

            if (aiState == State.Attack && !canSeePlayer)
                aiState = State.Search;
        }
    }

    void IdleNPCState()
    {
        switch (moveMode)
        {
            case MoveMode.Waypoints:
                WaypointsFunction();
                break;

            case MoveMode.Random:
                RandomMove();
                break;

            case MoveMode.Idle:
                Idle();
                break;
        }
    }

    #region AIType Functions

    void EnemyIdleState()
    {
        if (!dumbAI)
        {
            agent.isStopped = false;

            switch (moveMode)
            {
                case MoveMode.Waypoints:
                    WaypointsFunction();
                    break;

                case MoveMode.Random:
                    RandomMove();
                    break;
            }

            FOVCheck();

            if (canSeePlayer) aiState = State.Attack;

            if (enemyType == EnemyType.Shooter || enemyType == EnemyType.Melee)
            {
                EnemyIdleStateVelocityCheck();
            }
        }
    }

    void EnemySearchState()
    {
        agent.isStopped = false;

        RandomMove();

        if (!searchTimerStarted)
        {
            currentSearchTime = searchTimer;
            searchTimerStarted = true;
        }

        EnemySearchStateVelocityCheck();

        currentSearchTime -= Time.deltaTime;

        if (currentSearchTime <= 0)
        {
            aiState = State.Idle;
            if (HasParameter("aimedWalking")) animator.SetBool("aimedWalking", false);
            if (HasParameter("aimedIdle")) animator.SetBool("aimedIdle", false);

            if(moveMode == MoveMode.Idle)
            {
                agent.destination = idleStartPos;
                transform.rotation = idleStartRot;
            }
        }

        if (canSeePlayer) aiState = State.Attack;

        if (increaseSightOnAttack) searchAngle = attackSearchAngle;
    }

    void EnemyAttackState()
    {
        switch (enemyType)
        {
            case EnemyType.Shooter:
                ShooterAttack();
                break;

            case EnemyType.Melee:
                MeleeAttack();
                break;
        }

        if (!canSeePlayer) aiState = State.Search;
        if (increaseSightOnAttack) searchAngle = attackSearchAngle;
    }

    #endregion

    #region Velocity Checks

    void EnemyIdleStateVelocityCheck()
    {
        if (agent.velocity.magnitude >= 0.1f)
        {
            animator.SetBool("isMoving", true);

            if (HasParameter("aimedWalking")) animator.SetBool("aimedWalking", false);
        }
        else
        {
            animator.SetBool("isMoving", false);
        }
    }

    void EnemySearchStateVelocityCheck()
    {
        if(agent.velocity.magnitude >= 0.1f)
        {
            if (HasParameter("aimedWalking")) animator.SetBool("aimedWalking", true);
            if (HasParameter("aimedIdle")) animator.SetBool("aimedIdle", false);

            animator.SetBool("isMoving", false);
        }
        else
        {
            if (HasParameter("aimedWalking")) animator.SetBool("aimedWalking", false);
            if (HasParameter("aimedIdle")) animator.SetBool("aimedIdle", true);
        }
    }

    #endregion

    #region Attack Functions

    void MeleeAttack()
    {
        float distanceToPlayer = Vector3.Distance(player.transform.position, agent.transform.position);

        agent.destination = player.transform.position;

        if(distanceToPlayer <= attackRange)
        {
            agent.isStopped = true;

            if(agent.velocity.magnitude >= 0.1f)
            {
                inRange = true;
                animator.SetBool("isMoving", true);
            }
            else
            {
                inRange = false;
                animator.SetBool("isMoving", false);
            }

            CheckMeleeBool();
        }
        else
        {
            agent.isStopped = false;

            animator.SetBool("attacking", false);
        }
    }

    public void MeleeDamage()
    {
        if (enemyType == EnemyType.Melee && inRange)
        {
            player.transform.gameObject.GetComponent<PlayerStats>().Damage(damageAmount, false);
        }
    }

    void ShooterAttack()
    {
        agent.destination = player.transform.position;

        transform.LookAt(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z));

        float distanceToPlayer = Vector3.Distance(player.transform.position, agent.transform.position);

        if(distanceToPlayer <= attackRange)
        {
            animator.SetBool("firing", true);
            animator.SetBool("aimedWalking", false);
            CheckShooterBool();
            agent.isStopped = true;
        }
        else
        {
            animator.SetBool("firing", false);
            animator.SetBool("aimedWalking", true);
            agent.isStopped = false;
        }
    }

    void Projectile()
    {
        Rigidbody rb = Instantiate(projectile, firePoint.transform.position, Quaternion.identity)
            .GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * 32f, ForceMode.Impulse);
        rb.AddForce(transform.up * 2f, ForceMode.Impulse);
    }

    void Hitscan()
    {
        Vector3 direction = CalculateSpread(transform.forward, spreadAmount);

        Ray ray = new Ray(firePoint.position, direction);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            TrailRenderer trail = Instantiate(bulletTrail, firePoint.position, Quaternion.identity);
            StartCoroutine(SpawnTrail(trail, hit));

            CheckLayer(hit.collider.gameObject.layer, hit);

            if (hit.transform.gameObject.CompareTag("Player"))
            {
                hit.transform.gameObject.GetComponent<PlayerStats>().Damage(damageAmount, false);
            }
        }
        else
        {
            if (hit.collider != null)
            {
                TrailRenderer trail = Instantiate(bulletTrail, firePoint.position, Quaternion.identity);
                
                StartCoroutine(SpawnTrail(trail, ray.GetPoint(100)));
            }
        }

        objectSoundManager.PlaySound(fireClip, 0, 0, true, 0);
    }

    void CheckLayer(LayerMask layer, RaycastHit hit)
    {
        GameObject impact = null, impactBullet = null;

        switch (layer)
        {
            case int l when layer == LayerMask.NameToLayer("Default"):
                impactBullet = Instantiate(impactEffects.defaultImpact, hit.point, Quaternion.identity);
                break;
            case int l when layer == LayerMask.NameToLayer("Grass"):
                impact = Instantiate(effects.grassImpact, hit.point, Quaternion.identity);
                impact.transform.rotation = Quaternion.LookRotation(hit.normal);
                impactBullet = Instantiate(impactEffects.grassImpact, hit.point, Quaternion.identity);
                break;
            case int l when layer == LayerMask.NameToLayer("Metal"):
                impact = Instantiate(effects.metalImpact, hit.point, Quaternion.identity); // Metal
                impact.transform.rotation = Quaternion.LookRotation(hit.normal);
                impactBullet = Instantiate(impactEffects.metalImpact, hit.point, Quaternion.identity);
                Debug.Log(true);
                break;
            case int l when layer == LayerMask.NameToLayer("Mud"):
                impact = Instantiate(effects.mudImpact, hit.point, Quaternion.identity); // Mud
                impact.transform.rotation = Quaternion.LookRotation(hit.normal);
                impactBullet = Instantiate(impactEffects.mudImpact, hit.point, Quaternion.identity);
                break;
            case int l when layer == LayerMask.NameToLayer("Wood"):
                impact = Instantiate(effects.woodImpact, hit.point, Quaternion.identity); // Wood
                impact.transform.rotation = Quaternion.LookRotation(hit.normal);
                impactBullet = Instantiate(impactEffects.woodImpact, hit.point, Quaternion.identity);
                break;
            case int l when layer == LayerMask.NameToLayer("Enemy"):
                impact = Instantiate(effects.enemyImpact, hit.point, Quaternion.identity); // Enemy
                impact.transform.rotation = Quaternion.LookRotation(hit.normal);
                impactBullet = Instantiate(impactEffects.enemyImpact, hit.point, Quaternion.identity);
                break;
        }

        if(hit.collider != null && impactBullet != null)
        {
            impactBullet.transform.rotation = Quaternion.LookRotation(hit.normal);
            impactBullet.transform.SetParent(hit.collider.transform);
        }
    }

    IEnumerator SpawnTrail(TrailRenderer trail, RaycastHit hit)
    {
        float time = 0;
        Vector3 startPosition = trail.transform.position;

        while (time < 1)
        {
            trail.transform.position = Vector3.Lerp(startPosition, hit.point, time);
            time += Time.deltaTime / trail.time;

            yield return null;
        }

        trail.transform.position = hit.point;

        Destroy(trail.gameObject, trail.time);
    }

    private IEnumerator SpawnTrail(TrailRenderer trail, Vector3 endPoint)
    {
        float distance = Vector3.Distance(trail.transform.position, endPoint);
        float remainingDistance = distance;

        while (remainingDistance > 0)
        {
            trail.transform.position = Vector3.Lerp(trail.transform.position, endPoint, Time.deltaTime / distance);
            remainingDistance = Vector3.Distance(trail.transform.position, endPoint);
            yield return null;
        }

        // Optionally, destroy the trail after it reaches the endpoint
        Destroy(trail.gameObject, trail.time);
    }

    private Vector3 CalculateSpread(Vector3 direction, float angle)
    {
        // Calculate random angles within the spread range
        float spreadX = Random.Range(-angle, angle);
        float spreadY = Random.Range(-angle, angle);

        // Apply the spread to the direction
        direction = Quaternion.Euler(spreadX, spreadY, 0) * direction;

        return direction;
    }

    void ResetAttack()
    {
        switch (enemyType)
        {
            case EnemyType.Shooter:
                attackedShooter = false;
                break;

            case EnemyType.Melee:
                attackedMelee = false;
                break;
        }
    }

    #endregion

    #region Enemy Type Checks

    void CheckShooterBool()
    {
        if (!attackedShooter)
        {
            switch (shooterType)
            {
                case ShooterType.Hitscan:
                    Hitscan();
                    attackedShooter = true;
                    Invoke(nameof(ResetAttack), timeBetweenShots);
                    break;
                case ShooterType.Projectile:
                    Projectile();
                    attackedShooter = true;
                    Invoke(nameof(ResetAttack), timeBetweenShots);
                    break;
            }
        }
    }

    void CheckMeleeBool()
    {
        if (!attackedMelee)
        {
            attackedMelee = true;

            animator.SetBool("attacking", true);
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    #endregion


    #region Move Functions

    void RandomMove()
    {
        if (!wandering)
        {
            wanderWaitTimer -= Time.deltaTime;
            
            if (wanderWaitTimer <= 0f)
            {
                destination = RandomNavSphere(transform.position, wanderRadius, -1);
                agent.SetDestination(destination);
                wanderWaitTimer = waitTimeBetweenWander;
            }
        }

        if (agent.destination == destination)
        {
            wandering = false;
        }
    }

    private Vector3 RandomNavSphere(Vector3 origin, float dist, int layerMask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;
        wandering = true;

        NavMeshHit navHit;
        NavMesh.SamplePosition(randDirection, out navHit, dist, layerMask);

        return navHit.position;
    }

    void WaypointsFunction()
    {
        if (agent.remainingDistance < 0.5f)
        {
            waypointWaitTimer += Time.deltaTime;
            if (waypointWaitTimer >= waypointWaitTime)
            {
                currentWaypoint++;

                if (currentWaypoint >= _waypoints.waypoints.Count)
                {
                    currentWaypoint = 0;
                }

                agent.destination = _waypoints.waypoints[currentWaypoint].position;
                waypointWaitTimer = 0f;
            }
        }
    }

    void Idle()
    {
        agent.destination = idleStartPos;
        transform.rotation = idleStartRot;
    }

    #endregion

    #region Misc Functions

    bool HasParameter(string paramName)
    {
        foreach(AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == paramName) return true;
        }
        return false;
    }

    #endregion
}