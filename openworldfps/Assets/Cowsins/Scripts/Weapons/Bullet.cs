/// <summary>
/// This script belongs to cowsins™ as a part of the cowsins´ FPS Engine. All rights reserved. 
/// </summary>using UnityEngine;
using UnityEngine;

namespace cowsins
{
    public class Bullet : MonoBehaviour
    {
        [HideInInspector] public float speed;
        [HideInInspector] public float damage;
        [HideInInspector] public Vector3 destination;
        [HideInInspector] public bool gravity;
        [HideInInspector] public Transform player;
        [HideInInspector] public bool hurtsPlayer;
        [HideInInspector] public bool explosionOnHit;
        [HideInInspector] public GameObject explosionVFX;
        [HideInInspector] public float explosionRadius;
        [HideInInspector] public float explosionForce;
        [HideInInspector] public float criticalMultiplier;
        [HideInInspector] public float duration;

        private bool projectileHasAlreadyHit = false; // Prevent from double hitting issues

        private void Start()
        {
            transform.LookAt(destination);
            Invoke(nameof(DestroyProjectile), duration);
        }

        private void Update()
        {
            transform.Translate(0.0f, 0.0f, speed * Time.deltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (projectileHasAlreadyHit) return;

            if (other.CompareTag("Critical"))
            {
                DamageTarget(other.transform, damage * criticalMultiplier, true);
            }
            else if (other.CompareTag("BodyShot"))
            {
                DamageTarget(other.transform, damage, false);
            }
            else if (other.GetComponent<IDamageable>() != null && !other.CompareTag("Player"))
            {
                DamageTarget(other.transform, damage, false);
            }
            else if (IsGroundOrObstacleLayer(other.gameObject.layer))
            {
                DestroyProjectile();
            }
        }

        private void DamageTarget(Transform target, float dmg, bool isCritical)
        {
            var damageable = CowsinsUtilities.GatherDamageableParent(target);
            if (damageable != null)
            {
                damageable.Damage(dmg, isCritical);
                projectileHasAlreadyHit = true;
                DestroyProjectile();
            }
        }

        private bool IsGroundOrObstacleLayer(int layer)
        {
            return layer == LayerMask.NameToLayer("Ground") || layer == LayerMask.NameToLayer("Object")
                || layer == LayerMask.NameToLayer("Grass") || layer == LayerMask.NameToLayer("Metal") ||
                layer == LayerMask.NameToLayer("Mud") || layer == LayerMask.NameToLayer("Wood") || layer == LayerMask.NameToLayer("Enemy");
        }

        private void DestroyProjectile()
        {
            if (explosionOnHit)
            {
                if (explosionVFX != null)
                {
                    var contact = GetComponent<Collider>().ClosestPoint(transform.position);
                    Instantiate(explosionVFX, contact, Quaternion.identity);
                }

                Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

                foreach (var collider in colliders)
                {
                    var damageable = collider.GetComponent<IDamageable>();
                    var playerMovement = collider.GetComponent<PlayerMovement>();
                    var rigidbody = collider.GetComponent<Rigidbody>();

                    if (damageable != null)
                    {
                        // Calculate the distance ratio and damage based on the explosion radius
                        float distanceRatio = 1 - Mathf.Clamp01(Vector3.Distance(collider.transform.position, transform.position) / explosionRadius);
                        float dmg = damage * distanceRatio;

                        // Apply damage if the collider is a player and the explosion should hurt the player
                        if (collider.CompareTag("Player") && hurtsPlayer)
                        {
                            damageable.Damage(dmg, false);
                        }
                        // Apply damage if the collider is not a player
                        else if (!collider.CompareTag("Player"))
                        {
                            damageable.Damage(dmg, false);
                        }
                    }

                    if (playerMovement != null)
                    {
                        CamShake.instance.ExplosionShake(Vector3.Distance(CamShake.instance.gameObject.transform.position, transform.position));
                    }

                    if (rigidbody != null && collider != this)
                    {
                        rigidbody.AddExplosionForce(explosionForce, transform.position, explosionRadius, 5, ForceMode.Force);
                    }
                }
            }

            Destroy(gameObject);
        }
    }
}
