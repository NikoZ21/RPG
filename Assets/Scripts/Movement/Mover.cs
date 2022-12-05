using Combat;
using Core;
using Saving;
using UnityEngine;
using UnityEngine.AI;

namespace Movement
{
    public class Mover : MonoBehaviour, IAction, ISaveable
    {

        [SerializeField] private float maxNavmeshLength = 10;

        private NavMeshAgent _navMeshAgent;
        private Animator _animator;
        private ActionScheduler _actionScheduler;

        void Awake()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _animator = GetComponent<Animator>();
            _actionScheduler = GetComponent<ActionScheduler>();
        }

        void Update()
        {
            UpdateAnimator();
        }

        private void UpdateAnimator()
        {
            Vector3 velocity = _navMeshAgent.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            float speed = localVelocity.z;
            _animator.SetFloat("moveSpeed", speed);
        }

        public void StartMoveAction(Vector3 destination)
        {
            _actionScheduler.StartAction(this);
            MoveTo(destination);
        }

        public bool CanMoveTo(Vector3 destination)
        {
            NavMeshPath path = new NavMeshPath();
            var hasPath = NavMesh.CalculatePath(transform.position, destination, NavMesh.AllAreas, path);
            if (!hasPath) return false;
            if (path.status != NavMeshPathStatus.PathComplete) return false;
            if (GetPathLength(path) > maxNavmeshLength) return false;
            return true;
        }

        public void MoveTo(Vector3 destination)
        {
            _navMeshAgent.destination = destination;
            _navMeshAgent.isStopped = false;
        }

        public void Stop()
        {
            _navMeshAgent.isStopped = true;
        }

        public void Cancel()
        {
            _navMeshAgent.isStopped = true;
        }

        public object CaptureState()
        {
            SerializeableVector3 serializedbeforesave = new SerializeableVector3(transform.position);
            return serializedbeforesave;
        }

        public void RestoreState(object state)
        {
            SerializeableVector3 serializeableVector3 = (SerializeableVector3)state;

            _navMeshAgent.enabled = false;
            transform.position = serializeableVector3.GetVector();
            _navMeshAgent.enabled = true;
            _actionScheduler.CancelCurrentAction();
        }
        private float GetPathLength(NavMeshPath path)
        {
            Vector3[] vectors = path.corners;
            float totalLength = 0;

            if (vectors.Length < 2) return totalLength;

            for (int i = 0; i < vectors.Length - 1; i++)
            {
                totalLength += Vector3.Distance(vectors[i], vectors[i + 1]);
            }

            return totalLength;
        }
    }
}