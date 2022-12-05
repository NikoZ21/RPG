using Attributes;
using UnityEngine;
using UnityEngine.Events;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float projectielSpeed = 10f;
    [SerializeField] private bool isHomingmissle = false;
    [SerializeField] private ParticleSystem hitEffect;
    [SerializeField] private float maxLifeTime = 10;
    [SerializeField] private UnityEvent onHit;
    private Health target;
    private GameObject instigator;
    private float damage = 0;

    private void Start()
    {
        transform.LookAt(GetAimLocation());
    }

    void Update()
    {
        if (isHomingmissle && !target.IsDead)
        {
            transform.LookAt(GetAimLocation());
        }

        transform.Translate(Vector3.forward * Time.deltaTime * projectielSpeed);
    }

    public Vector3 GetAimLocation()
    {
        CapsuleCollider targetCapsule = target.GetComponent<CapsuleCollider>();
        if (!targetCapsule)
        {
            return target.transform.position;
        }

        return target.transform.position + Vector3.up * targetCapsule.height / 2;
    }

    public void SetTarget(Health target, float damage, GameObject instigator)
    {
        this.target = target;
        this.damage = damage;
        this.instigator = instigator;

        Destroy(gameObject, maxLifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        var enemy = other.GetComponent<Health>();
        if (enemy != target) return;

        DisplayHitEffect();
        onHit?.Invoke();
        enemy.GetComponent<Health>().TakeDamage(instigator, damage);
        Destroy(gameObject);
    }

    private void DisplayHitEffect()
    {
        if (!hitEffect) return;
        Instantiate(hitEffect, GetAimLocation(), transform.rotation);
    }
}