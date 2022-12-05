using TMPro;
using UnityEngine;

namespace Attributes
{
    public class HealthDisplay : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI text;
        Health health;

        private void Awake()
        {
            health = GameObject.FindWithTag("Player").GetComponent<Health>();
        }

        private void Update()
        {
            text.text = string.Format("{0:0}/{1:0}", health.GetCurrentHealth(), health.GetMaxHealth());
        }

    }
}
