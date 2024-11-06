using UnityEngine;

namespace cowsins
{
    public class TurretProjectile : MonoBehaviour
    {
        [HideInInspector] public Vector3 dir;

        [HideInInspector] public float damage, speed;

        void Update()
        {
            transform.Translate(dir * speed * Time.deltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player"))
            {
                Destroy(this.gameObject);
                return;
            }

            PlayerStats player = other.GetComponent<PlayerStats>();
            player.Damage(damage, false);
            Destroy(this.gameObject);

        }
    }

}