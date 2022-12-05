using GameDevTV.Utils;
using System;
using UnityEngine;

namespace Stats
{
    public class BaseStats : MonoBehaviour
    {
        [SerializeField] [Range(1, 99)] private int startingLevel = 1;
        [SerializeField] private CharacterClass characterClass;
        [SerializeField] private Progression progression;
        [SerializeField] private GameObject levelUpEffect;
        [SerializeField] private bool shouldUseModifiers = false;

        public event Action onLevelUp;

        private LazyValue<int> currentLevel;
        private Experience experience;


        private void Awake()
        {
            experience = GetComponent<Experience>();
            currentLevel = new LazyValue<int>(CalculateLevel);
        }

        private void Start()
        {
            currentLevel.ForceInit();
        }

        private void OnEnable()
        {
            if (experience != null)
            {
                experience.onExperienceGained += UpdateLevel;
            }
        }

        private void OnDisable()
        {
            if (experience != null)
            {
                experience.onExperienceGained -= UpdateLevel;
            }
        }

        private void UpdateLevel()
        {
            int newLevel = CalculateLevel();
            if (newLevel > currentLevel.value)
            {
                currentLevel.value = newLevel;
                LevelUpEffect();
                onLevelUp?.Invoke();
            }
        }

        private void LevelUpEffect()
        {
            Instantiate(levelUpEffect, transform);
        }

        private float GetAdditiveModifier(Stat stat)
        {

            if (!shouldUseModifiers) return 0;

            float total = 0;
            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach (float modifier in provider.GetAdditiveModifiers(stat))
                {
                    total += modifier;
                }
            }
            return total;
        }

        private float GetPercentageModifier(Stat stat)
        {
            if (!shouldUseModifiers) return 0;

            float total = 0;
            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach (float modifier in provider.GetAdditiveModifiers(stat))
                {
                    total += modifier;
                }
            }
            return total;
        }

        public float GetStat(Stat stat)
        {
            return (progression.GetStat(stat, characterClass, GetLevel()) + GetAdditiveModifier(stat)) * (1 + GetPercentageModifier(stat) / 100);
        }

        public int GetLevel()
        {
            if (currentLevel.value < 1)
            {
                currentLevel.value = CalculateLevel();
            }
            return currentLevel.value;
        }

        public int CalculateLevel()
        {
            Experience experience = GetComponent<Experience>();
            if (experience == null) return startingLevel;

            float currentXP = experience.GetExperiencePoints();
            int maxLevel = progression.GetLevels(Stat.ExperienceToLevelUp, characterClass);

            for (int i = 1; i <= maxLevel; i++)
            {
                if (currentXP < progression.GetStat(Stat.ExperienceToLevelUp, characterClass, i))
                {
                    return i;
                }
            }

            return maxLevel + 1;
        }
    }
}