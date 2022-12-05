using Core;
using GameDevTV.Utils;
using Saving;
using Stats;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace Attributes
{
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] private float regenPercentage = 70;
        [SerializeField] private Animator _animator;
        [SerializeField] private ActionScheduler _actionScheduler;
        [SerializeField] private UnityEvent<float> onTakeDamage;
        [SerializeField] private UnityEvent onDie;

        public bool IsDead { get; private set; } = false;

        private LazyValue<float> healthPoints;
        private BaseStats _baseStats;


        private void Awake()
        {
            print("I am from awake");
            _baseStats = GetComponent<BaseStats>();
            healthPoints = new LazyValue<float>(GetInitialHealth);
        }

        private void Start()
        {
            print("I am from start");
            healthPoints.ForceInit();
        }

        private float GetInitialHealth()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }


        private void OnEnable()
        {
            _baseStats.onLevelUp += RegenerateHealth;
        }

        private void OnDisable()
        {
            _baseStats.onLevelUp -= RegenerateHealth;
        }

        private void RegenerateHealth()
        {
            float regenHealthPoints = _baseStats.GetStat(Stat.Health) * (regenPercentage / 100);
            healthPoints.value = MathF.Max(healthPoints.value, regenHealthPoints);
        }

        public void TakeDamage(GameObject instigator, float damage)
        {
            healthPoints.value -= damage;

            if (healthPoints.value <= 0)
            {
                Die();
                AwardExperience(instigator);
            }
            else
            {
                onTakeDamage?.Invoke(damage);
            }
        }

        private void AwardExperience(GameObject instigator)
        {
            if (instigator.tag == "Player")
            {
                instigator.GetComponent<Experience>()
                    .GainExperience(transform.GetComponent<BaseStats>().GetStat(Stat.ExperienceReward));
            }
        }

        private void Die()
        {
            if (IsDead) return;

            onDie?.Invoke();
            IsDead = true;
            GetComponent<CapsuleCollider>().enabled = false;
            _animator.SetTrigger("die");
            _actionScheduler.CancelCurrentAction();
        }

        public float GetFraction()
        {
            if (healthPoints.value <= 0) return 0;

            return healthPoints.value / _baseStats.GetStat(Stat.Health);
        }

        public float GetCurrentHealth()
        {
            if (healthPoints.value <= 0) return 0;
            return healthPoints.value;
        }

        public float GetMaxHealth()
        {
            var maxHealth = _baseStats.GetStat(Stat.Health);
            return maxHealth;
        }

        public object CaptureState()
        {
            return healthPoints.value;
        }

        public void RestoreState(object state)
        {
            print("I am from restore");
            healthPoints.value = (float)state;

            if (healthPoints.value <= 0)
            {
                Die();
            }
        }

        public void Heal(float heal)
        {
            healthPoints.value = Mathf.Min(healthPoints.value + heal, GetMaxHealth());
        }
    }
}