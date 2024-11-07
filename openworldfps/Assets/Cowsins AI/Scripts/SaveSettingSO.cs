using UnityEditor;
using UnityEngine;

public class SaveSettingSO : ScriptableObject
{
    public float waitTimeBetweenWander;
    public float wanderRadius;
    public float searchRadius;
    public float searchAngle;

    public LayerMask obstructionMasks;
    public LayerMask targetMask;

    public float attackRange;
    public bool increaseSightOnAttack;

    public float timeBetweenAttacks;

    public float spreadAmount;
    public TrailRenderer bulletTrail;
    public float damageAmount;
    public float timeBetweenShots;
    public GameObject projectile;

    public AudioClip fireClip;

    public bool useRagdoll;

    public bool useDeathAnimation;
    public bool destroyAfterTimer;
    public float destroyTimer;

    public bool dumbAI;

    public float waypointWaitTime;
    public float searchTimer;

    public int aiState;
    public int moveMode;
    public int shooterType;
    public int aiType;
    public int enemyType;

    public bool searchRadiusDebug;
    public bool attackRangeDebug;
    public bool canSeePlayerDebug;
}

#if UNITY_EDITOR
[System.Serializable]
[CustomEditor(typeof(SaveSettingSO))]
public class SaveSettingSOEditor : Editor
{

    override public void OnInspectorGUI()
    {
        serializedObject.Update();
        SaveSettingSO script = target as SaveSettingSO;

        GUI.backgroundColor = Color.red;
        EditorGUILayout.LabelField("ALL SETTINGS ARE NOT INTENDED TO BE READ IN THIS FORM", EditorStyles.helpBox);

        GUI.backgroundColor = Color.white;
        DrawDefaultInspector();

        serializedObject.ApplyModifiedProperties();

    }
}
#endif