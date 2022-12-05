using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text = null;

    public void DestroyText()
    {
        Destroy(gameObject);
    }

    public void SetValue(float amount)
    {
        text.text = string.Format("{0:0}", amount.ToString());
    }
}