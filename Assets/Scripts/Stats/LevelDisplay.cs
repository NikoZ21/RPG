using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


namespace Stats
{
    public class LevelDisplay : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI text;
        BaseStats _baseStats;

        private void Awake()
        {
            _baseStats = GameObject.FindWithTag("Player").GetComponent<BaseStats>();
        }

        private void Update()
        {
            text.text = string.Format("{0:0}", _baseStats.GetLevel());
        }
    }
}


