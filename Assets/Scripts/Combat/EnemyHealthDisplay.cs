using Attributes;
using Combat;
using TMPro;
using UnityEngine;

namespace Combat
{
    public class EnemyHealthDisplay : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI text;
        GameObject player;

        private void Awake()
        {
            player = GameObject.FindWithTag("Player");
        }

        private void Update()
        {
            Health targetHealth = player.GetComponent<Fighter>().GetHealth();
            if (targetHealth == null)
            {
                text.text = "N/A";
                return;
            }
            text.text = string.Format("{0:0}/{1:0}", targetHealth.GetCurrentHealth(), targetHealth.GetMaxHealth());
        }

    }
}
