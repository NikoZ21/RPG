using Attributes;
using Control;
using UnityEngine;

namespace Combat
{
    [RequireComponent(typeof(Health))]
    public class CombatTarget : MonoBehaviour, IRayCastable
    {
        public CursorType GetCursorType()
        {
            return CursorType.Combat;
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if (!callingController.GetComponent<Fighter>().CanAttack(gameObject))
            {
                return false;
            }

            if (Input.GetMouseButton(1))
            {
                callingController.GetComponent<Fighter>().Attack(gameObject);
            }

            return true;
        }
    }
}