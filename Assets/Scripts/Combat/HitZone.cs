using UnityEngine;

namespace TidesEnd.Combat
{
    /// <summary>
    /// Defines a hit zone on a character for location-based damage multipliers.
    /// Attach to colliders on head, body, limbs, etc.
    /// </summary>
    public class HitZone : MonoBehaviour
    {
        public enum ZoneType
        {
            Head,    // 2x damage (default)
            Body,    // 1x damage (default)
            Limb     // 0.75x damage (default)
        }

        [Header("Hit Zone Configuration")]
        [SerializeField] private ZoneType zoneType = ZoneType.Body;
        [SerializeField] private float customMultiplier = 0f; // 0 = use default for zone type

        [Header("Owner Reference")]
        [SerializeField] private GameObject ownerObject;

        private IDamageable cachedDamageable;

        /// <summary>
        /// The type of hit zone (Head, Body, Limb)
        /// </summary>
        public ZoneType Type => zoneType;

        /// <summary>
        /// The damage multiplier for this zone
        /// </summary>
        public float Multiplier => customMultiplier > 0f ? customMultiplier : GetDefaultMultiplier();

        /// <summary>
        /// The IDamageable component that should receive damage
        /// </summary>
        public IDamageable Owner
        {
            get
            {
                if (cachedDamageable == null)
                {
                    FindOwner();
                }
                return cachedDamageable;
            }
        }

        /// <summary>
        /// The GameObject that owns this hit zone
        /// </summary>
        public GameObject OwnerGameObject => ownerObject != null ? ownerObject : gameObject;

        private void Awake()
        {
            FindOwner();

            // Ensure this collider is set to trigger
            Collider col = GetComponent<Collider>();
            if (col != null && !col.isTrigger)
            {
                Debug.LogWarning($"HitZone on {name} has a collider that is not a trigger. Setting to trigger.", this);
                col.isTrigger = true;
            }
        }

        private void FindOwner()
        {
            // Try to find owner in specified object
            if (ownerObject != null)
            {
                cachedDamageable = ownerObject.GetComponent<IDamageable>();
                if (cachedDamageable != null)
                    return;
            }

            // Try to find in parent hierarchy
            cachedDamageable = GetComponentInParent<IDamageable>();

            if (cachedDamageable != null)
            {
                ownerObject = cachedDamageable.GameObject;
            }
            else
            {
                Debug.LogWarning($"HitZone on {name} could not find an IDamageable owner!", this);
            }
        }

        private float GetDefaultMultiplier()
        {
            switch (zoneType)
            {
                case ZoneType.Head:
                    return 2.0f;
                case ZoneType.Body:
                    return 1.0f;
                case ZoneType.Limb:
                    return 0.75f;
                default:
                    return 1.0f;
            }
        }

        /// <summary>
        /// Manually set the owner if auto-detection fails
        /// </summary>
        public void SetOwner(IDamageable owner)
        {
            cachedDamageable = owner;
            if (owner != null)
            {
                ownerObject = owner.GameObject;
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            // Draw gizmo to visualize hit zones in editor
            Collider col = GetComponent<Collider>();
            if (col == null) return;

            Color gizmoColor = zoneType switch
            {
                ZoneType.Head => Color.red,
                ZoneType.Body => Color.yellow,
                ZoneType.Limb => Color.blue,
                _ => Color.white
            };

            gizmoColor.a = 0.3f;
            Gizmos.color = gizmoColor;

            if (col is SphereCollider sphere)
            {
                Gizmos.DrawSphere(transform.position, sphere.radius * transform.lossyScale.x);
            }
            else if (col is BoxCollider box)
            {
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawCube(box.center, box.size);
            }
            else if (col is CapsuleCollider capsule)
            {
                Gizmos.DrawSphere(transform.position, capsule.radius * transform.lossyScale.x);
            }
        }
#endif
    }
}
