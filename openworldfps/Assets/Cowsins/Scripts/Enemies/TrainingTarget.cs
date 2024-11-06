using UnityEngine;
namespace cowsins
{
    public class TrainingTarget : EnemyHealth
    {
        [SerializeField] private float timeToRevive;

        public override void Damage(float damage, bool isHeadshot)
        {
            if (isDead) return;
            base.Damage(damage, isHeadshot);
            GetComponent<Animator>().Play("Target_Hit");
        }
        public override void Die()
        {
            if (isDead) return;
            isDead = true;
            events.OnDeath?.Invoke();
            Invoke("Revive", timeToRevive);

            if (shieldSlider != null) shieldSlider.gameObject.SetActive(false);
            if (healthSlider != null) healthSlider.gameObject.SetActive(false);

            if (showKillFeed) UIEvents.onEnemyKilled.Invoke(_name);

            if (transform.parent.GetComponent<CompassElement>() != null) transform.parent.GetComponent<CompassElement>().Remove();

            GetComponent<Animator>().Play("Target_Die");


            SoundManager.Instance.PlaySound(dieSFX, 0, 0, false, 0);
        }
        private void Revive()
        {
            isDead = false;
            GetComponent<Animator>().Play("Target_Revive");
            health = maxHealth;
            shield = maxShield;

            if (shieldSlider != null) shieldSlider.gameObject.SetActive(true);
            if (healthSlider != null) healthSlider.gameObject.SetActive(true);

            if (transform.parent.GetComponent<CompassElement>() != null) transform.parent.GetComponent<CompassElement>().Add();
        }
    }
}