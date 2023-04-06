using System;
using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using Attributes;
using Combat;
using Core;
using GameDevTV.Utils;
using Movement;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

namespace Control
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] private bool isGizmosToggled = false;
        [SerializeField] private float chaseDistance = 5f;
        [SerializeField] private float suspiciousTime = 3f;
        [SerializeField] private float agrooCooldownTime = 3f;
        [SerializeField] private PatrolPath patrolPath;
        [SerializeField] private float wayPointTolerance = 1f;
        [SerializeField] private float dwellTime = 4f;
        [SerializeField] private float patrolSpeed = 3f;
        [SerializeField] private float chaseSpeed = 6f;
        [SerializeField] private float shoutDistance = 10;

        private Fighter _fighter;
        private GameObject _player;
        private Health _health;
        private NavMeshAgent _navMeshAgent;
        private Mover _mover;
        private ActionScheduler _actionScheduler;

        private LazyValue<Vector3> guardPosition;
        private float timeSinceLastSawPlayer = Mathf.Infinity;
        private float timeSinceTheLastWaypoint = Mathf.Infinity;
        private float timeSinceAggrevated = Mathf.Infinity;
        private int currentWayPointIndex = 0;



        private void Awake()
        {
            _player = GameObject.FindWithTag("Player");
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _health = GetComponent<Health>();
            _fighter = GetComponent<Fighter>();
            _mover = GetComponent<Mover>();
            _actionScheduler = GetComponent<ActionScheduler>();

            guardPosition = new LazyValue<Vector3>(GetGuardPosition);
        }

        private Vector3 GetGuardPosition()
        {
            return transform.position;
        }

        private void Start()
        {
            guardPosition.ForceInit();
        }

        private void Update()
        {
            if (_health.IsDead)
            {
                _navMeshAgent.enabled = !_health.IsDead;
                return;
            }


            if (IsAggrevated(_player) && _fighter.CanAttack(_player))
            {
                AttackState();
            }
            else if (timeSinceLastSawPlayer < suspiciousTime)
            {
                SuspicionState();
            }
            else
            {
                SetSpeed(patrolSpeed);
                PatrolState();
            }

            UpdateTimers();
        }

        private void UpdateTimers()
        {
            timeSinceLastSawPlayer += Time.deltaTime;
            timeSinceTheLastWaypoint += Time.deltaTime;
            timeSinceAggrevated += Time.deltaTime;
        }

        private void SuspicionState()
        {
            _actionScheduler.CancelCurrentAction();
        }

        private void AttackState()
        {
            timeSinceLastSawPlayer = 0;
            _fighter.Attack(_player);

            AggrevaiteNearByEnemies();

        }

        private void AggrevaiteNearByEnemies()
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, shoutDistance, Vector3.up, 0);

            foreach (var hit in hits)
            {
                var enemy = hit.transform.GetComponent<AIController>();
                if (!enemy) continue;

                enemy.Aggrevate();
            }
        }

        private void PatrolState()
        {
            Vector3 nextPosition = guardPosition.value;

            if (patrolPath)
            {
                if (AtWaypoint())
                {
                    timeSinceTheLastWaypoint = 0;
                    CycleWaypoints();
                }

                nextPosition = GetCurrentWayPoint();
            }

            if (timeSinceTheLastWaypoint > dwellTime)
            {
                _mover.MoveTo(nextPosition);
            }
        }

        private bool AtWaypoint()
        {
            float distanceWToWayPoint = Mathf.Abs(Vector3.Distance(transform.position, GetCurrentWayPoint()));
            return distanceWToWayPoint <= wayPointTolerance;
        }

        private Vector3 GetCurrentWayPoint()
        {
            return patrolPath.GetWaypoint(currentWayPointIndex);
        }

        private void CycleWaypoints()
        {
            timeSinceTheLastWaypoint = 0;
            currentWayPointIndex = patrolPath.GetNextIndex(currentWayPointIndex);
        }


        private bool IsAggrevated(GameObject player)
        {
            float distanceToPlayer = Mathf.Abs(Vector3.Distance(transform.position, player.transform.position));
            return distanceToPlayer <= chaseDistance || timeSinceAggrevated < agrooCooldownTime;
        }

        private void OnDrawGizmos()
        {
            if (isGizmosToggled)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(transform.position, chaseDistance);
                Gizmos.DrawWireSphere(transform.position + Vector3.up * 1f, shoutDistance);
            }
        }

        private void SetSpeed(float speed)
        {
            _navMeshAgent.speed = speed;
        }

        public void Aggrevate()
        {
            SetSpeed(chaseSpeed);
            timeSinceAggrevated = 0;
        }
    }
}