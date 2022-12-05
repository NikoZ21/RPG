using TMPro;
using UnityEngine;

namespace Stats
{
    public class XPDisplay : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI text;
        Experience experience;

        private void Awake()
        {
            experience = GameObject.FindWithTag("Player").GetComponent<Experience>();
        }

        private void Update()
        {
            text.text = string.Format("{0:0}", experience.GetExperiencePoints());
        }
    }
}