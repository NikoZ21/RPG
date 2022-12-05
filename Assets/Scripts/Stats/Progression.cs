using System.Collections.Generic;
using UnityEngine;

namespace Stats
{
    [CreateAssetMenu(fileName = "Progression", menuName = "Stats/New Progression", order = 0)]
    public class Progression : ScriptableObject
    {
        [SerializeField] ProgressionCharacterClass[] characterClasses;
        private Dictionary<CharacterClass, Dictionary<Stat, float[]>> lookUpTable = null;

        public float GetStat(Stat stat, CharacterClass characterClass, int level)
        {
            BuildLookUp();

            float[] levels = lookUpTable[characterClass][stat];
            if (levels.Length < level)
            {
                return 0;
            }

            return levels[level - 1];
        }

        public int GetLevels(Stat stat, CharacterClass characterClass)
        {
            BuildLookUp();

            var levels = lookUpTable[characterClass][stat];
            return levels.Length;
        }

        private void BuildLookUp()
        {
            if (lookUpTable != null) return;

            lookUpTable = new Dictionary<CharacterClass, Dictionary<Stat, float[]>>();

            foreach (var chrachterClass in characterClasses)
            {
                Dictionary<Stat, float[]> statLookUpTable = new Dictionary<Stat, float[]>();

                foreach (var progressionStat in chrachterClass.Stats)
                {
                    statLookUpTable[progressionStat.stat] = progressionStat.levels;
                }

                lookUpTable[chrachterClass.characterClass] = statLookUpTable;
            }
        }


        [System.Serializable]
        class ProgressionCharacterClass
        {
            public CharacterClass characterClass = CharacterClass.Grunt;

            public ProgressionStat[] Stats;
            //public float[] Health;
        }

        [System.Serializable]
        public class ProgressionStat
        {
            public Stat stat;
            public float[] levels;
        }
    }
}