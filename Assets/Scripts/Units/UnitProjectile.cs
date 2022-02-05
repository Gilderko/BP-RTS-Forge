using UnityEngine;

public class UnitProjectile : NetworkBehaviour
{
    [SerializeField] private Rigidbody rb = null;
    [SerializeField] private int damageToDeal = 20;
    [SerializeField] private float destroyAfterSeconds = 5f;
    [SerializeField] private float launchForce = 10f;

    public void Start()
    {
        rb.velocity = transform.forward * launchForce;

        if (IsServer)
        {
            Invoke(nameof(DestroySelf), destroyAfterSeconds);
        }
    }

    #region Server

#if UNITY_SERVER

    private void OnTriggerEnter(Collider other)
    {
        if (IsServer)
        {
            NetworkBehaviour otherObjectIHit;
            if (!other.TryGetComponent<NetworkBehaviour>(out otherObjectIHit))
            {
                return;
            }

            if (otherObjectIHit.OwnerClientIntId == OwnerClientIntId)
            {
                return;
            }

            Health health;
            if (!other.TryGetComponent<Health>(out health))
            {
                return;
            }

            health.DealDamage(damageToDeal);

            DestroySelf();
        }
    }

#endif


    private void DestroySelf()
    {
        Destroy(gameObject);
    }

#endregion
}
