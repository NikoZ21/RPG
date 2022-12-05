using Attributes;
using Core;
using GameDevTV.Utils;
using Movement;
using Saving;
using Stats;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


namespace Combat
{
    public class Fighter : MonoBehaviour, IAction, ISaveable, IModifierProvider
    {
        [SerializeField] private float timeBetweenAttacks = 1.3f;
        [SerializeField] private Transform rightHand;
        [SerializeField] private Transform leftHand;
        [SerializeField] private WeaponConfig defaultWeaponConfig;

        private WeaponConfig currentWeaponConfig;
        private LazyValue<Weapon> currentWeapon;
        private Health _target;
        private Mover _mover;
        private ActionScheduler _actionScheduler;
        private Animator _animator;
        private float timeSinceLastAttack = 0;

        private void Awake()
        {
            _actionScheduler = GetComponent<ActionScheduler>();
            _mover = GetComponent<Mover>();
            _animator = GetComponent<Animator>();
            currentWeaponConfig = defaultWeaponConfig;
            currentWeapon = new LazyValue<Weapon>(SetDefaultWeapon);
        }

        private Weapon SetDefaultWeapon()
        {
            return AttachWeapon(defaultWeaponConfig);
        }

        private void Start()
        {
            currentWeapon.ForceInit();
        }


        private void Update()
        {
            timeSinceLastAttack += Time.deltaTime;
            if (!_target) return;
            if (_target.IsDead) return;

            if (!GetIsInRange(_target.transform))
            {
                _mover.MoveTo(_target.transform.position);
            }
            else
            {
                _mover.Stop();
                AttackBehaviour();
            }
        }

        private void AttackBehaviour()
        {
            if (timeSinceLastAttack >= timeBetweenAttacks)
            {
                transform.LookAt(_target.transform.position);
                timeSinceLastAttack = 0;
                TriggerAttack();
            }
        }

        private void TriggerAttack()
        {
            _animator.ResetTrigger("stopAttack");
            _animator.SetTrigger("attack");
        }

        private bool GetIsInRange(Transform targetTransform)
        {
            return Mathf.Abs(Vector3.Distance(transform.position, targetTransform.transform.position)) <=
                   currentWeaponConfig.GetAttackRange();
        }

        private void StopAttack()
        {
            _animator.ResetTrigger("attack");
            _animator.SetTrigger("stopAttack");
        }

        public void EquipWeapon(WeaponConfig weaponConfig)
        {
            currentWeaponConfig = weaponConfig;
            currentWeapon.value = AttachWeapon(weaponConfig);
        }

        private Weapon AttachWeapon(WeaponConfig weaponConfig)
        {
            return weaponConfig.Spawn(rightHand, leftHand, _animator);
        }

        public void Attack(GameObject target)
        {
            _actionScheduler.StartAction(this);
            _target = target.GetOrAddComponent<Health>();
        }

        public void Cancel()
        {
            StopAttack();
            _target = null;
            _mover.Cancel();
        }

        public void Hit()
        {
            if (!_target) return;

            if (currentWeapon.value != null)
            {
                currentWeapon.value.Hit();
            }

            float damage = GetComponent<BaseStats>().GetStat(Stat.Damage);

            if (currentWeaponConfig.hasProjectile())
            {
                currentWeaponConfig.LaunchProjectile(rightHand, leftHand, _target, gameObject, damage);
            }
            else
            {
                _target.TakeDamage(gameObject, damage);
            }
        }

        public void Shoot()
        {
            Hit();
        }

        public bool CanAttack(GameObject combatTarget)
        {
            if (!combatTarget) return false;
            if (!_mover.CanMoveTo(combatTarget.transform.position) && !GetIsInRange(combatTarget.transform))
                return false;
            var health = combatTarget.GetComponent<Health>();
            return !health || !health.IsDead;
        }

        public object CaptureState()
        {
            return currentWeaponConfig.name;
        }

        public void RestoreState(object state)
        {
            var savedWeapon = (string)state;
            WeaponConfig weaponConfig = Resources.Load<WeaponConfig>(savedWeapon);
            EquipWeapon(weaponConfig);
        }

        public Health GetHealth()
        {
            if (_target == null)
            {
                return null;
            }

            return _target.GetComponent<Health>();
        }

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return currentWeaponConfig.GetDamage();
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return currentWeaponConfig.GetPercentageBonus();
            }
        }
    }
}