using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DamageTextSpawner : MonoBehaviour
{
    [SerializeField] private DamageText damageText;

    public void Spawn(float damage)
    {
        DamageText instance = Instantiate<DamageText>(damageText, transform);
        instance.SetValue(damage);
    }
}