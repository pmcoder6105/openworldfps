using UnityEngine;
using cowsins;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.AI;
using TMPro;
using System.Collections;
using UnityEditor;

public class EnemyAI : MonoBehaviour, IDamageable
{
    [System.Serializable]
    public class Events
    {
        public UnityEvent OnSpawn, OnShoot, OnDamaged, OnDeath;
    }

    [Tooltip("Name of the Enemy. This will Appear in the Killfeed"), SerializeField]
    public string name;

    public float health;

    [Tooltip("Initial Enemy Health"), SerializeField]
    public float maxHealth;

    public float shield;

    [Tooltip("Initial Enemy Shield"), SerializeField]
    public float maxShield;

    [Tooltip("Display Enemy Status via UI"), SerializeField]
    protected Slider healthSlider, shieldSlider;

    [Tooltip(
        "If True, it Will Display the UI With the Shield and Health Sliders, Disabling this Will Disable Pop Ups."), 
        SerializeField]
    public bool showUI;

    [Tooltip("If true, it will display the KillFeed UI.")] public bool showKillFeed;

    [Tooltip(
        "Add a Pop Up Showing the Damage that has Been Dealt. Recommendation: Use the Already Made Pop Up Included in Cowsins FPS Engine. "),
        SerializeField]
    private GameObject damagePopUp;

    [Tooltip("Colour for the Specific Status to be Displayed in the Slider"), SerializeField]
    private Color shieldColor, healthColor;

    [Tooltip("Horizontal Randomness Variation"), SerializeField]
    private float xVariation;

    [HideInInspector] public Transform player;
    protected Transform UI;

    public Events events;

    private CowsinsAI cai;
    private Animator animator;
    private NavMeshAgent agent;

    public virtual void Start()
    {
        health = maxHealth;
        shield = maxShield;

        events.OnSpawn.Invoke();

        player = GameObject.FindGameObjectWithTag("Player").transform;
        cai = GetComponent<CowsinsAI>();
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        if (healthSlider != null) healthSlider.maxValue = maxHealth;
        if (shieldSlider != null) shieldSlider.maxValue = maxShield;

        if (!showUI)
        {
            Destroy(healthSlider);
            Destroy(shieldSlider);
        }

        if (cai.useRagdoll)
        {
            Rigidbody[] rigidBodies = gameObject.GetComponentsInChildren<Rigidbody>();

            foreach (Rigidbody rigidBody in rigidBodies) rigidBody.isKinematic = true;
        }
    }

    public virtual void Update()
    {
        if (healthSlider != null) healthSlider.value = Mathf.Lerp(healthSlider.value, health, Time.deltaTime * 6);
        if (shieldSlider != null) shieldSlider.value = Mathf.Lerp(shieldSlider.value, shield, Time.deltaTime * 4);

        if (health <= 0) Die();
    }

    public virtual void Damage(float dmg, bool isHeadshot)
    {
        float damage = Mathf.Abs(dmg);
        float oldDmg = damage;

        if (damage <= shield)
        {
            shield -= damage;
            if (shieldSlider != null) shieldSlider.GetComponent<Animator>().Play("UIDamageShieldEnemy");
        }
        else
        {
            damage = damage - shield;
            shield = 0;
            health -= damage;

            if (shieldSlider != null && health >= 0) shieldSlider.GetComponent<Animator>().Play("UIDamageShieldEnemy");
            if (healthSlider != null && health >= 0) healthSlider.GetComponent<Animator>().Play("UIDamageEnemy");
        }

        events.OnDamaged.Invoke();
        UIEvents.onEnemyHit?.Invoke(isHeadshot);

        if (!showUI) return;
        GameObject popup = Instantiate(damagePopUp, transform.position, Quaternion.identity);
        if (oldDmg / Mathf.FloorToInt(oldDmg) == 1)
            popup.transform.GetChild(0).GetComponent<TMP_Text>().text = oldDmg.ToString("F0");
        else
            popup.transform.GetChild(0).GetComponent<TMP_Text>().text = oldDmg.ToString("F1");
        float xRand = Random.Range(-xVariation, xVariation);
        popup.transform.position = popup.transform.position + new Vector3(xRand, 0, 0);
    }

    public virtual void Die()
    {
        events.OnDeath.Invoke();

        if (showKillFeed) UIEvents.onEnemyKilled.Invoke(name);

        if (cai.useRagdoll) RagdollDeath();

        if (cai.useDeathAnimation) AnimationDeath();

        if (!cai.useRagdoll && !cai.useDeathAnimation) Destroy(gameObject);
    }

    void RagdollDeath()
    {
        Rigidbody[] rigidbodies = gameObject.GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody rigidbody in rigidbodies)
        {
            rigidbody.isKinematic = false;
        }

        cai.enabled = false;

        animator.enabled = false;

        agent.enabled = false;

        healthSlider.gameObject.SetActive(false);
        shieldSlider.gameObject.SetActive(false);
        
        Destroy(gameObject, 10f);
    }

    void AnimationDeath()
    {
        cai.enabled = false;
        agent.enabled = false;

        if (cai.enemyType == CowsinsAI.EnemyType.Melee)
        {
            if (showUI)
            {
                healthSlider.gameObject.SetActive(false);
                shieldSlider.gameObject.SetActive(false);
            }
            GetComponent<Collider>().enabled = false;

            animator.SetBool("isWalking", false);
            animator.SetBool("attacking", false);
            animator.SetBool("dead", true);
        }

        if (cai.enemyType == CowsinsAI.EnemyType.Shooter)
        {
            if (showUI)
            {
                healthSlider.gameObject.SetActive(false);
                shieldSlider.gameObject.SetActive(false);
            }

            GetComponent<Collider>().enabled = false;
            animator.SetBool("isWalking", false);
            animator.SetBool("combatWalk", false);
            animator.SetBool("combatIdle", false);
            animator.SetBool("firing", false);
            animator.SetTrigger("dead");
        }
    }
    void AnimationDeathDeactivateAnimator()
    {
        animator.enabled = false;

        if (cai.destroyAfterTimer)
        {
            StartCoroutine(DestroyPlayer(cai.destroyTimer));
        }
    }

    IEnumerator DestroyPlayer(float destroyAfterTime)
    {
        float timer = destroyAfterTime;

        while (timer > 0f)
        {
            yield return new WaitForSeconds(1f);
            timer--;
        }

        Destroy(gameObject);
    }
}

#if UNITY_EDITOR
[System.Serializable]
[CustomEditor(typeof(EnemyAI))]
public class EnemyEditorAI : Editor
{

    override public void OnInspectorGUI()
    {
        serializedObject.Update();
        EnemyAI myScript = target as EnemyAI;

        EditorGUILayout.LabelField("ENEMY AI HEALTH", EditorStyles.boldLabel);
        EditorGUILayout.Space(15);

        EditorGUILayout.LabelField("IDENTITY", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("name"));

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("STATS", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("maxHealth"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("maxShield"));

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("UI", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("showUI"));
        if (myScript.showUI)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("healthSlider"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("shieldSlider"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("healthColor"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("shieldColor"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("damagePopUp"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("xVariation"));
        }

        EditorGUILayout.PropertyField(serializedObject.FindProperty("showKillFeed"));
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("EVENTS", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("events"));

        serializedObject.ApplyModifiedProperties();

    }
}
#endif