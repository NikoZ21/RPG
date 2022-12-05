using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;

namespace Attributes
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private Health health;
        [SerializeField] private RectTransform healthScale;
        [SerializeField] private Canvas healthBarCanvas;

        private void Update()
        {
            var scale = health.GetFraction();
            healthScale.localScale = new Vector3(scale, 1, 1);
            if (ShouldDisable(scale))
            {
                EnableCanvas(false);
                return;
            }

            EnableCanvas(true);
        }

        private bool ShouldDisable(float scale)
        {
            return Mathf.Approximately(scale, 0) || Mathf.Approximately(scale, 1);
        }

        private void EnableCanvas(bool state)
        {
            healthBarCanvas.enabled = state;
        }
    }
}