using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Attributes;
using Combat;
using Movement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

namespace Control
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] CursorMapping[] cursourMappings = null;
        [SerializeField] float maxNavMeshProjectionDistance = 1f;
        [SerializeField] float rayCastRadious = 0.5f;

        private Camera _mainCamera;
        private Mover _mover;
        private Fighter _fighter;
        private Health _health;
        private NavMeshAgent _navMeshAgent;


        [System.Serializable]
        public struct CursorMapping
        {
            public CursorType type;
            public Texture2D texture;
            public Vector2 hotSpot;
        }

        void Awake()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _health = GetComponent<Health>();
            _mover = GetComponent<Mover>();
            _mainCamera = Camera.main;
            _fighter = GetComponent<Fighter>();
        }

        void Update()
        {
            if (InterctWithUI()) return;

            if (_health.IsDead)
            {
                SetCoursor(CursorType.None);
                return;
            }

            if (InteractWithComponent()) return;
            if (IntercatWithMovement()) return;

            SetCoursor(CursorType.None);
        }

        private bool InterctWithUI()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                SetCoursor(CursorType.UI);
                return true;
            }

            return false;
        }

        private bool InteractWithComponent()
        {
            RaycastHit[] hits = RaycastAllSorted();
            foreach (var hit in hits)
            {
                IRayCastable[] rayCastables = hit.transform.GetComponents<IRayCastable>();
                foreach (IRayCastable rayCastable in rayCastables)
                {
                    if (rayCastable.HandleRaycast(this))
                    {
                        SetCoursor(rayCastable.GetCursorType());
                        return true;
                    }
                }
            }

            return false;
        }

        RaycastHit[] RaycastAllSorted()
        {
            RaycastHit[] hits = Physics.SphereCastAll(GetMouseRay(),rayCastRadious);
            float[] distances = new float[hits.Length];
            for (int i = 0; i < hits.Length; i++)
            {
                distances[i] = hits[i].distance;
            }

            Array.Sort(distances, hits);

            return hits;
        }

        private bool IntercatWithMovement()
        {
            Vector3 target;
            bool hasHit = RayCastNavmesh(out target);

            if (hasHit)
            {

                if (!_mover.CanMoveTo(target)) return false;

                if (Input.GetMouseButton(1))
                {
                    _mover.StartMoveAction(target);
                }

                SetCoursor(CursorType.Movement);
                return true;
            }

            return false;
        }

        private bool RayCastNavmesh(out Vector3 target)
        {
            target = new Vector3();

            RaycastHit hit;
            bool hashit = Physics.Raycast(GetMouseRay(), out hit);
            if (!hashit) return false;

            NavMeshHit navMeshHit;
            bool hasCastToNavMesh = NavMesh.SamplePosition(hit.point, out navMeshHit, maxNavMeshProjectionDistance,
                NavMesh.AllAreas);
            if (!hasCastToNavMesh) return false;

            target = navMeshHit.position;

            return true;
        }



        private void SetCoursor(CursorType cursorType)
        {
            CursorMapping mapping = GetCursorMapping(cursorType);
            Cursor.SetCursor(mapping.texture, mapping.hotSpot, CursorMode.Auto);
        }

        private CursorMapping GetCursorMapping(CursorType type)
        {
            foreach (var mapping in cursourMappings)
            {
                if (mapping.type != type) continue;

                return mapping;
            }

            return new CursorMapping();
        }

        private Ray GetMouseRay()
        {
            return _mainCamera.ScreenPointToRay(Input.mousePosition);
        }
    }
}