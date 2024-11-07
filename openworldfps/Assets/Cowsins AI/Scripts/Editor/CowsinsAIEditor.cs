using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

[CustomEditor(typeof(CowsinsAI))]
public class CowsinsAIEditor : Editor
{
    private string[] combatTabs = { "Variables", "Combat", "Debug" };
    private int combatTab;


    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        CowsinsAI cai = target as CowsinsAI;

        GUILayout.BeginHorizontal();
        Texture2D texture = Resources.Load<Texture2D>("CustomEditor/Cowsins AI Logo");
        GUILayout.Label(texture, GUILayout.Width(150), GUILayout.Height(150));
        GUILayout.EndHorizontal();

        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Cowsins AI", EditorStyles.whiteLargeLabel);
        EditorGUILayout.Space(5);
        combatTab = GUILayout.Toolbar(combatTab, combatTabs);
        EditorGUILayout.Space(10f);
        EditorGUILayout.EndVertical();

        if (combatTab >= 0 || combatTab < combatTabs.Length)
        {
            switch (combatTabs[combatTab])
            {
                case "Variables":
                    EditorGUILayout.Space(5);
                    EditorGUI.indentLevel++;
                    EditorGUILayout.LabelField("BASIC VARIABLES", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("aiState"));
                    EditorGUILayout.Space(5);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("aiType"));
                    EditorGUILayout.Space(5);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("useRagdoll"));

                    if (!cai.useRagdoll)
                    {
                        EditorGUILayout.Space(5);
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("useDeathAnimation"));
                        EditorGUILayout.Space(5);
                        if (cai.useDeathAnimation)
                        {
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("destroyAfterTimer"));
                            EditorGUILayout.Space(5);
                            if (cai.destroyAfterTimer)
                            {
                                EditorGUILayout.PropertyField(serializedObject.FindProperty("destroyTimer"));
                            }
                        }
                    }
                    else
                    {
                        cai.useDeathAnimation = false;
                    }

                    if (cai.useDeathAnimation)
                    {
                        cai.useRagdoll = false;
                    }

                    EditorGUILayout.Space(5);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("dumbAI"));
                    EditorGUILayout.Space(5);

                    EditorGUI.indentLevel++;
                    if (!cai.dumbAI)
                    {
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("moveMode"));
                        EditorGUILayout.Space(5);

                        if (cai.moveMode == CowsinsAI.MoveMode.Waypoints)
                        {
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("_waypoints"));
                            EditorGUILayout.Space(5);
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("waypointWaitTime"));
                            EditorGUILayout.Space(5);
                        }
                        else if (cai.moveMode == CowsinsAI.MoveMode.Random)
                        {
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("wanderRadius"));
                            EditorGUILayout.Space(5);
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("waitTimeBetweenWander"));
                            EditorGUILayout.Space(5);
                        }
                    }
                    else
                    {
                        return;
                    }

                    EditorGUI.indentLevel--;

                    if (cai.aiType == CowsinsAI.AIType.Enemy)
                    {
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("searchRadius"));
                        EditorGUILayout.Space(5);
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("increaseSightOnAttack"));
                        EditorGUILayout.Space(5);

                        if (cai.increaseSightOnAttack)
                        {
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("attackSearchAngle"));
                            EditorGUILayout.Space(5);
                        }

                        EditorGUILayout.PropertyField(serializedObject.FindProperty("searchAngle"));
                        EditorGUILayout.Space(5);
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("targetMask"));
                        EditorGUILayout.Space(5);
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("obstructionMasks"));
                        EditorGUILayout.Space(5);
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("searchTimer"));
                        EditorGUILayout.Space(5);
                    }

                    EditorGUI.indentLevel--;

                    EditorGUILayout.Space(10);
                    GUI.backgroundColor = Color.green;
                    if (GUILayout.Button("Save Settings"))
                    {
                        SaveSettings();
                    }

                    EditorGUILayout.Space(10);
                    GUI.backgroundColor = Color.green;
                    if (GUILayout.Button("Load Settings"))
                    {
                        LoadSettings();
                    }

                    EditorGUILayout.Space(10);
                    GUI.backgroundColor = Color.white;
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("saveSO"));

                    break;

                case "Combat":
                    if (cai.aiType == CowsinsAI.AIType.Enemy)
                    {
                        EditorGUILayout.LabelField("COMBAT OPTIONS", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField("Note: Both Shooter AND Mele Cannot be Enabled at the Same Time or There Will be an Error!", EditorStyles.helpBox);
                        EditorGUILayout.Space(10);
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("enemyType"));
                        if (cai.enemyType == CowsinsAI.EnemyType.Shooter)
                        {
                            EditorGUILayout.LabelField("SHOOTER OPTIONS", EditorStyles.boldLabel);
                            EditorGUI.indentLevel++;
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("shooterType"));
                            EditorGUILayout.Space(5);
                            if (cai.shooterType == CowsinsAI.ShooterType.Projectile)
                            {
                                EditorGUILayout.PropertyField(serializedObject.FindProperty("projectile"));
                                EditorGUILayout.Space(10);
                            }
                            else if (cai.shooterType == CowsinsAI.ShooterType.Hitscan)
                            {
                                EditorGUILayout.PropertyField(serializedObject.FindProperty("bulletTrail"));
                                EditorGUILayout.Space(5);
                                EditorGUILayout.PropertyField(serializedObject.FindProperty("spreadAmount"));
                                EditorGUILayout.Space(5);
                                EditorGUILayout.Space(5);
                                EditorGUILayout.PropertyField(serializedObject.FindProperty("effects"));
                                EditorGUILayout.Space(5);
                                EditorGUILayout.Space(5);
                                EditorGUILayout.PropertyField(serializedObject.FindProperty("impactEffects"));
                                EditorGUILayout.Space(5);
                            }

                            EditorGUILayout.LabelField("UNIVERSAL COMBAT VARIABLES", EditorStyles.boldLabel);
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("fireClip"));
                            EditorGUILayout.Space(5);
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("firePoint"));
                            EditorGUILayout.Space(5);
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("timeBetweenShots"));
                            EditorGUILayout.Space(5);
                            EditorGUI.indentLevel--;
                        }

                        if (cai.enemyType == CowsinsAI.EnemyType.Melee)
                        {
                            EditorGUILayout.Space(5);
                            EditorGUILayout.LabelField("MELEE OPTIONS", EditorStyles.boldLabel);
                            EditorGUI.indentLevel++;
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("timeBetweenAttacks"));
                            EditorGUILayout.Space(5);
                            EditorGUI.indentLevel--;
                        }

                        EditorGUILayout.PropertyField(serializedObject.FindProperty("attackRange"));
                        EditorGUILayout.Space(5);
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("damageAmount"));
                        EditorGUILayout.Space(10);
                    }
                    else
                    {
                        GUI.backgroundColor = new Color(10f, 0.0f, 0.0f, 0.25f);
                        EditorGUILayout.LabelField("Shooter Tab Disabled Due to AI Type Settings!", EditorStyles.helpBox);
                    }

                    EditorGUILayout.Space(10);
                    GUI.backgroundColor = Color.green;
                    if (GUILayout.Button("Save Settings"))
                    {
                        SaveSettings();
                    }

                    EditorGUILayout.Space(10);
                    GUI.backgroundColor = Color.green;
                    if (GUILayout.Button("Load Settings"))
                    {
                        LoadSettings();
                    }

                    EditorGUILayout.Space(10);
                    GUI.backgroundColor = Color.white;
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("saveSO"));

                    break;

                case "Debug":
                    EditorGUILayout.LabelField("DEBUG VARIABLES", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("searchRadiusDebug"));
                    EditorGUILayout.Space(5);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("attackRangeDebug"));
                    EditorGUILayout.Space(15);

                    EditorGUILayout.LabelField("RUNTIME DEBUG VARIABLES", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("canSeePlayer"));

                    EditorGUILayout.Space(10);
                    GUI.backgroundColor = Color.green;
                    if(GUILayout.Button("Save Settings"))
                    {
                        SaveSettings();
                    }
                    
                    EditorGUILayout.Space(10);
                    GUI.backgroundColor = Color.green;
                    if (GUILayout.Button("Load Settings"))
                    {
                        LoadSettings();
                    }

                    EditorGUILayout.Space(10);
                    GUI.backgroundColor = Color.white;
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("saveSO"));

                    break;
            }
        }
        serializedObject.ApplyModifiedProperties();

        void SaveSettings()
        {
            SaveSettingSO saveSO = CreateInstance<SaveSettingSO>();

            saveSO.waitTimeBetweenWander = cai.waitTimeBetweenWander;
            saveSO.wanderRadius = cai.wanderRadius;
            saveSO.searchRadius = cai.searchRadius;
            saveSO.searchAngle = cai.searchAngle;

            saveSO.targetMask = cai.targetMask.value;
            saveSO.obstructionMasks = cai.obstructionMasks.value;

            saveSO.attackRange = cai.attackRange;
            saveSO.increaseSightOnAttack = cai.increaseSightOnAttack;
            saveSO.timeBetweenAttacks = cai.timeBetweenAttacks;
            saveSO.spreadAmount = cai.spreadAmount;
            saveSO.bulletTrail = cai.bulletTrail;
            saveSO.damageAmount = cai.damageAmount;
            saveSO.fireClip = cai.fireClip;
            saveSO.timeBetweenShots = cai.timeBetweenShots;
            saveSO.projectile = cai.projectile;
            saveSO.useRagdoll = cai.useRagdoll;
            saveSO.useDeathAnimation = cai.useDeathAnimation;
            saveSO.destroyAfterTimer = cai.destroyAfterTimer;
            saveSO.destroyTimer = cai.destroyTimer;
            saveSO.dumbAI = cai.dumbAI;
            saveSO.waypointWaitTime = cai.waypointWaitTime;
            saveSO.searchTimer = cai.searchTimer;

            saveSO.aiState = (int)cai.aiState;
            saveSO.moveMode = (int)cai.moveMode;
            saveSO.shooterType = (int)cai.shooterType;
            saveSO.aiType = (int)cai.aiType;
            saveSO.enemyType = (int)cai.enemyType;

            saveSO.searchRadiusDebug = cai.searchRadiusDebug;
            saveSO.attackRangeDebug = cai.attackRangeDebug;
            saveSO.canSeePlayerDebug = cai.canSeePlayerDebug;

            AssetDatabase.CreateAsset(saveSO, "Assets/CowsinsAISaveData.asset");
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = saveSO;
        }

        void LoadSettings()
        {
            SaveSettingSO saveSO = cai.saveSO;

            cai.waitTimeBetweenWander = saveSO.waitTimeBetweenWander;
            cai.wanderRadius = saveSO.wanderRadius;
            cai.searchRadius = saveSO.searchRadius;
            cai.searchAngle = saveSO.searchAngle;

            cai.obstructionMasks = saveSO.obstructionMasks;
            cai.targetMask = saveSO.targetMask;

            cai.attackRange = saveSO.attackRange;
            cai.increaseSightOnAttack = saveSO.increaseSightOnAttack;
            cai.timeBetweenAttacks = saveSO.timeBetweenAttacks;
            cai.spreadAmount = saveSO.spreadAmount;
            cai.bulletTrail = saveSO.bulletTrail;
            cai.damageAmount = saveSO.damageAmount;
            cai.fireClip = saveSO.fireClip;
            cai.timeBetweenShots = saveSO.timeBetweenShots;
            cai.projectile = saveSO.projectile;
            cai.useRagdoll = saveSO.useRagdoll;
            cai.useDeathAnimation = saveSO.useDeathAnimation;
            cai.destroyAfterTimer = saveSO.destroyAfterTimer;
            cai.destroyTimer = saveSO.destroyTimer;
            cai.dumbAI = saveSO.dumbAI;
            cai.waypointWaitTime = saveSO.waypointWaitTime;
            cai.searchTimer = saveSO.searchTimer;

            cai.aiState = (CowsinsAI.State)saveSO.aiState;
            cai.moveMode = (CowsinsAI.MoveMode)saveSO.moveMode;
            cai.shooterType = (CowsinsAI.ShooterType)saveSO.shooterType;
            cai.aiType = (CowsinsAI.AIType)saveSO.aiType;
            cai.enemyType = (CowsinsAI.EnemyType)saveSO.enemyType;

            cai.searchRadiusDebug = saveSO.searchRadiusDebug;
            cai.attackRangeDebug = saveSO.attackRangeDebug;
            cai.canSeePlayerDebug = saveSO.canSeePlayerDebug;

            cai.saveSO = null;

            EditorUtility.DisplayDialog("Loaded Save Settings", "Save Settings Successfully Loaded, if Using 'Shooter' Enemy Type, Firepoint Requires Manual Allocation.", "Continue");
        }
    }

    private void OnSceneGUI()
    {
        CowsinsAI cai = target as CowsinsAI;

        if (cai.aiType != CowsinsAI.AIType.Enemy) return;

        if (cai.searchRadiusDebug)
        {
            Handles.color = Color.white;
            Handles.DrawWireArc(cai.transform.position, Vector3.up, Vector3.forward, 360, cai.searchRadius);

            Vector3 searchViewAngle01 = DirectionFromAngle(cai.transform.eulerAngles.y, -cai.searchAngle / 2);
            Vector3 searchViewAngle02 = DirectionFromAngle(cai.transform.eulerAngles.y, cai.searchAngle / 2);

            Handles.color = Color.yellow;
            Handles.DrawLine(cai.transform.position, cai.transform.position + searchViewAngle01 * cai.searchRadius);
            Handles.DrawLine(cai.transform.position, cai.transform.position + searchViewAngle02 * cai.searchRadius);
        }

        if (cai.attackRangeDebug)
        {
            Handles.color = Color.red;
            Handles.DrawWireArc(cai.transform.position, Vector3.up, Vector3.forward, 360, cai.attackRange);
        }

    }

    Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
    {
        angleInDegrees += eulerY;
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}

#endif