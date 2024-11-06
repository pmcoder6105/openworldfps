/// <summary>
/// This script belongs to cowsins™ as a part of the cowsins´ FPS Engine. All rights reserved. 
/// </summary>
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

namespace cowsins
{
    /// <summary>
    /// Super simple enemy script that allows any object with this component attached to receive damage,aim towards the player and shoot at it.
    /// This is not definitive and it will 100% be modified and re structured in future updates
    /// </summary>
    public class EnemyHealth : MonoBehaviour, IDamageable
    {
        [System.Serializable]
        public class Events
        {
            public UnityEvent OnSpawn, OnDamaged, OnDeath;
        }

        [Tooltip("Name of the enemy. This will appear on the killfeed"), SerializeField]
        protected string _name;

        [ReadOnly] public float health;

        [ReadOnly] public float shield;

        [Tooltip("initial enemy health "), SerializeField]
        protected float maxHealth;

        [Tooltip("initial enemy shield"), SerializeField]
        protected float maxShield;

        [Tooltip("When the object dies, decide if it should be destroyed or not."), SerializeField] private bool destroyOnDie;

        [SerializeField] private GameObject deathEffect;

        [Tooltip("display enemy status via UI"), SerializeField]
        public Slider healthSlider, shieldSlider;

        [Tooltip("If true, it will display the UI with the shield and health sliders."), SerializeField]
        private bool showUI;

        public bool showDamagePopUps;

        [Tooltip("If true, it will display the KillFeed UI."), SerializeField]
        protected bool showKillFeed;

        [Tooltip("Add a pop up showing the damage that has been dealt. Recommendation: use the already made pop up included in this package. "), SerializeField]
        private GameObject damagePopUp;

        [Tooltip("Horizontal randomness variation"), SerializeField]
        private float xVariation;

        [SerializeField]
        protected AudioClip dieSFX;

        [HideInInspector] protected Transform player;

        public Events events;

        protected bool isDead;

        public bool DestroyOnDie { get { return destroyOnDie; } }

        public bool ShowUI { get { return showUI; } }

        public Slider HealthSlider { get { return healthSlider; } }

        public Slider ShieldSlider { get { return shieldSlider; } }

        public GameObject DamagePopUp { get { return damagePopUp; } }

        public virtual void Start()
        {
            // Status initial settings
            health = maxHealth;
            shield = maxShield;

            // Spawn
            events.OnSpawn.Invoke();

            // Initial settings 
            player = GameObject.FindGameObjectWithTag("Player")?.transform;


            // UI 
            // Determine max values
            if (healthSlider != null) healthSlider.maxValue = maxHealth;
            if (shieldSlider != null) shieldSlider.maxValue = maxShield;
            if (!showUI) // Destroy the enemy UI if we do not want to display it
            {
                Destroy(healthSlider);
                Destroy(shieldSlider);
            }
        }

        // Update is called once per frame
        public virtual void Update()
        {
            //Handle UI 
            if (healthSlider != null) healthSlider.value = Mathf.Lerp(healthSlider.value, health, Time.deltaTime * 6);
            if (shieldSlider != null) shieldSlider.value = Mathf.Lerp(shieldSlider.value, shield, Time.deltaTime * 4);

            // Manage health
            if (health <= 0 && !isDead) Die();
        }
        /// <summary>
        /// Since it is IDamageable, it can take damage, if a shot is landed, damage the enemy
        /// </summary>
        public virtual void Damage(float _damage, bool isHeadshot)
        {
            float damage = Mathf.Abs(_damage);
            float oldDmg = damage;
            if (damage <= shield) // Shield will be damaged
            {
                shield -= damage;
            }
            else
            {
                damage = damage - shield;
                shield = 0;
                health -= damage;
            }

            // Custom event on damaged
            events.OnDamaged.Invoke();
            UIEvents.onEnemyHit?.Invoke(isHeadshot);
            // If you do not want to show a damage pop up, stop, do not continue
            if (!showDamagePopUps) return;
            GameObject popup = Instantiate(damagePopUp, transform.position, Quaternion.identity) as GameObject;
            if (oldDmg / Mathf.FloorToInt(oldDmg) == 1)
                popup.transform.GetChild(0).GetComponent<TMP_Text>().text = oldDmg.ToString("F0");
            else
                popup.transform.GetChild(0).GetComponent<TMP_Text>().text = oldDmg.ToString("F1");
            float xRand = Random.Range(-xVariation, xVariation);
            popup.transform.position = popup.transform.position + new Vector3(xRand, 0, 0);
        }
        public virtual void Die()
        {
            isDead = true;
            // Custom event on damaged
            events.OnDeath.Invoke();

            SoundManager.Instance.PlaySound(dieSFX, 0, 1, false, 0);
            // Does it display killfeed on death? 
            if (showKillFeed)
            {
                UIEvents.onEnemyKilled.Invoke(_name);
            }
            if (deathEffect) Instantiate(deathEffect, transform.position, Quaternion.identity);
            if (destroyOnDie) Destroy(this.gameObject);
        }

        public void PlayAnimation(string triggerIdentifier)
        {
            GetComponentInChildren<Animator>()?.SetTrigger(triggerIdentifier);
        }
    }
#if UNITY_EDITOR
    [System.Serializable]
    [CustomEditor(typeof(EnemyHealth))]
    public class EnemyEditor : Editor
    {
        private bool showIdentity = false;
        private bool showStats = false;
        private bool showUI = false;
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EnemyHealth myScript = (EnemyHealth)target;

            Texture2D myTexture = Resources.Load<Texture2D>("CustomEditor/enemyHealth_CustomEditor") as Texture2D;
            GUILayout.Label(myTexture);
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginVertical(GUI.skin.GetStyle("HelpBox"));
            {
                // Identity foldout
                showIdentity = EditorGUILayout.Foldout(showIdentity, "IDENTITY", true);
                if (showIdentity)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("_name"));
                    EditorGUILayout.Space(6);
                    EditorGUI.indentLevel--;
                }

            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(6);
            EditorGUILayout.BeginVertical(GUI.skin.GetStyle("HelpBox"));
            {
                // Stats foldout
                showStats = EditorGUILayout.Foldout(showStats, "STATISTICS", true);
                if (showStats)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("maxHealth"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("maxShield"));

                    if (!myScript.DestroyOnDie)
                    {
                        EditorGUILayout.HelpBox("DestroyOnDie is set to false, your object won’t be destroyed once you kill it.", MessageType.Warning);
                    }

                    EditorGUILayout.PropertyField(serializedObject.FindProperty("destroyOnDie"));
                    if (myScript.DestroyOnDie)
                    {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("deathEffect"));
                        EditorGUI.indentLevel--;
                    }
                    EditorGUILayout.Space(6);
                    EditorGUI.indentLevel--;
                }

            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(6);
            EditorGUILayout.BeginVertical(GUI.skin.GetStyle("HelpBox"));
            {
                showUI = EditorGUILayout.Foldout(showUI, "UI", true);
                if (showUI)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("showUI"));
                    if (myScript.ShowUI)
                    {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("healthSlider"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("shieldSlider"));
                        if (!myScript.HealthSlider && !myScript.ShieldSlider)
                        {
                            EditorGUILayout.Space(10);
                            EditorGUILayout.HelpBox("Your Enemy UI is null, create a new custom Canvas or Create the Default UI", MessageType.Error);
                            EditorGUILayout.Space(5);
                            if (GUILayout.Button("Create Default UI"))
                            {
                                CreateDefaultUI(myScript.transform);
                            }
                            EditorGUILayout.Space(10);
                        }
                        EditorGUI.indentLevel--;
                    }

                    EditorGUILayout.PropertyField(serializedObject.FindProperty("showDamagePopUps"));
                    if (myScript.showDamagePopUps)
                    {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("damagePopUp"));
                        if (!myScript.DamagePopUp)
                        {
                            EditorGUILayout.Space(10);
                            EditorGUILayout.HelpBox("Damage Pop Up is null. Do you want to automatically assign the default Prefab?", MessageType.Error);
                            EditorGUILayout.Space(5);
                            if (GUILayout.Button("Assign Default Damage Pop Up"))
                            {
                                AssignDefaultDamagePopUp();
                            }
                            EditorGUILayout.Space(10);
                        }
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("xVariation"));
                        EditorGUI.indentLevel--;
                    }

                    EditorGUILayout.PropertyField(serializedObject.FindProperty("showKillFeed"));
                    EditorGUILayout.Space(6);
                    EditorGUI.indentLevel--;
                }


            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(6);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("events"));
            EditorGUI.indentLevel--;


            serializedObject.ApplyModifiedProperties();
        }



        private void CreateDefaultUI(Transform myTransform)
        {
            // Path to EnemyStatusSlider prefab
            string prefabPath = "Assets/Cowsins/Prefabs/Others/UI/EnemyStatusSlider.prefab";

            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

            if (prefab != null)
            {
                // Instantiate the prefab 
                GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, myTransform);
                instance.transform.localPosition += new Vector3(0, 5, 0);
                Undo.RegisterCreatedObjectUndo(instance, "Create Default UI");

                SerializedProperty healthSliderProperty = serializedObject.FindProperty("healthSlider");
                SerializedProperty shieldSliderProperty = serializedObject.FindProperty("shieldSlider");

                healthSliderProperty.objectReferenceValue = instance.transform.Find("HealthSlider").GetComponent<Slider>();
                shieldSliderProperty.objectReferenceValue = instance.transform.Find("ShieldSlider").GetComponent<Slider>();
                serializedObject.ApplyModifiedProperties();

                EditorUtility.SetDirty(target);
            }
            else
            {
                Debug.LogError($"Prefab not found at path: { prefabPath }. Did you move, rename or delete EnemyStatusSlider.prefab? ");
            }
        }

        private void AssignDefaultDamagePopUp()
        {
            // Path to DMGPopUp prefab
            string prefabPath = "Assets/Cowsins/Prefabs/Others/UI/DMGPopUp.prefab";

            // Load prefab
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (prefab != null)
            {
                SerializedProperty damagePopUpProperty = serializedObject.FindProperty("damagePopUp");

                // Assign the prefab to the property
                damagePopUpProperty.objectReferenceValue = prefab;
                serializedObject.ApplyModifiedProperties();
                // Ensure changes are saved
                EditorUtility.SetDirty(target);
            }
            else
            {
                Debug.LogError($"Prefab at path {prefabPath} could not be found. Did you move, rename or delete DMPopUp.prefab? ");
            }
        }

    }
#endif
}