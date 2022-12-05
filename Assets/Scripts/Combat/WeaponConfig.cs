using Attributes;
using UnityEngine;

namespace Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapon/Make new weapon", order = 0)]
    public class WeaponConfig : ScriptableObject
    {
        [SerializeField] AnimatorOverrideController animatorOverride = null;
        [SerializeField] Weapon weaponPrefab = null;
        [SerializeField] private float attackRange = 2f;
        [SerializeField] private float damage = 25f;
        [SerializeField] private float percentageBonus = 10;
        [SerializeField] private bool isRightHanded;
        [SerializeField] private Projectile projectile = null;
        private const string weaponName = "Weapon";

        public Weapon Spawn(Transform rightHandTransform, Transform leftHandTransform, Animator animator)
        {
            DestroyOldWeapon(rightHandTransform, leftHandTransform);

            Transform handTransform = GetTransform(rightHandTransform, leftHandTransform);

            Weapon weapon = null;
            if (weaponPrefab)
            {
                weapon = Instantiate(weaponPrefab, handTransform);
                weapon.gameObject.name = weaponName;
            }

            var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
            if (animatorOverride)
            {
                animator.runtimeAnimatorController = animatorOverride;
            }
            else if (overrideController)
            {
                animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
            }

            return weapon;
        }

        private void DestroyOldWeapon(Transform rightHand, Transform leftHand)
        {
            var oldweapon = rightHand.Find(weaponName);

            if (oldweapon == null)
            {
                oldweapon = leftHand.Find(weaponName);
            }

            if (oldweapon == null) return;

            oldweapon.name = "Destroying";
            Destroy(oldweapon.gameObject);
        }

        private Transform GetTransform(Transform rightHandTransform, Transform leftHandTransform)
        {
            Transform handTransform = null;

            if (isRightHanded) handTransform = rightHandTransform;
            else if (!isRightHanded) handTransform = leftHandTransform;
            return handTransform;
        }

        public bool hasProjectile()
        {
            return projectile != null;
        }

        public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target, GameObject instigator,
            float damage)
        {
            Projectile projectileInstance =
                Instantiate(projectile, GetTransform(rightHand, leftHand).position, Quaternion.identity);
            projectileInstance.SetTarget(target, damage, instigator);
        }

        public float GetDamage()
        {
            return damage;
        }

        public float GetPercentageBonus()
        {
            return percentageBonus;
        }

        public float GetAttackRange()
        {
            return attackRange;
        }
    }
}