using System;
using cowsins;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.AI;

public class CowsinsAICreator : EditorWindow
{
    public GameObject ObjectToSetup;
    public string AITag = "Untagged";
    public string AIName;

    public bool useRagdoll;
    
    private AIType _aiType;
    private enemyType _enemyType;
    private MovementType _movementType;
    private shooterType _shooterType;

    private Vector2 scrollPos;
    private Texture CowsinsAILogo;

    private TrailRenderer hitscanTrailRenderer;
    
    public enum MovementType {Waypoint = 0, Random = 1, Idle = 2}
    
    public enum AIType { Enemy, NPC }

    public enum enemyType { Shooter, Melee }
    
    public enum shooterType {Hitscan, Projectile}

    public LayerMask hitscanHitMask;
    public LayerMask enemyMask;

    private static float secs = 0;
    private static double startVal = 0;
    private static double progress = 0;

    private bool DisplayConfirmation = false;
    private static bool DontShowDisplayConfirmation = false;

    [MenuItem("COWSINS/AI/Cowsins AI Setup Handler", false, 0)]
    public static void ShowWindow()
    {
        EditorWindow editorWindow = EditorWindow.GetWindow(typeof(CowsinsAICreator), false, "Cowsins Setup Handler");
        editorWindow.minSize = new Vector2(300f, 250f);
    }

    private void OnEnable()
    {
        if (CowsinsAILogo == null) CowsinsAILogo = Resources.Load("CustomEditor/Cowsins AI Logo") as Texture;
    }

    private void OnGUI()
    {   
        GUILayout.Space(15);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginVertical("Box");
        
        var style = new GUIStyle(EditorStyles.boldLabel) { alignment = TextAnchor.MiddleCenter };
        EditorGUILayout.LabelField(new GUIContent(CowsinsAILogo), style, GUILayout.ExpandWidth(true),
            GUILayout.Height(64));
        EditorGUILayout.LabelField("Cowsins AI Setup Handler", style, GUILayout.ExpandWidth(true));
        EditorGUILayout.HelpBox(
            "The Cowsins AI Setup Handler applies all required settings and components to automatically create an AI on the given object. Be aware that after closing the Cowsins Setup Handler, you will lose all given references you've entered below. Make sure you select 'Setup AI' before closing if you'd like your changed to be applied.",
            MessageType.None, true);
        GUILayout.Space(4);
        
        var HelpButtonStyle = new GUIStyle(GUI.skin.button);
        HelpButtonStyle.normal.textColor = Color.white;
        HelpButtonStyle.fontStyle = FontStyle.Bold;

        GUI.backgroundColor = new Color(1f, 1, 0.25f, 0.25f);
        EditorGUILayout.LabelField("For a detailed tutorial on setting up an AI from start to finish, please see the Getting Started Tutorial below.", EditorStyles.helpBox);
        GUI.backgroundColor = new Color(90.6f, 32.9f, 50.2f, 0.5f);
        if (GUILayout.Button("See the Tutorial Playlist", HelpButtonStyle, GUILayout.Height(20)))
        {
            Application.OpenURL("https://www.youtube.com/playlist?list=PLFDlsE9sYjL-fpnT6RTmMoYVrf1GuZGFT");
        }
        GUI.backgroundColor = Color.white;
        
        GUILayout.Space(10);

        EditorGUILayout.EndVertical();
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(15);
        
        EditorGUILayout.BeginVertical();
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(25f);
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        EditorGUILayout.BeginVertical("Box");

        GUI.backgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.25f);
        EditorGUILayout.BeginVertical("Box");
        EditorGUILayout.LabelField("Setup Handler Settings", EditorStyles.boldLabel);
        GUI.backgroundColor = Color.white;
        EditorGUILayout.EndVertical();
        GUI.backgroundColor = Color.white;
        
        GUILayout.Space(15f);

        GUI.backgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.19f);
        EditorGUILayout.HelpBox("The Name of The AI (This will be applied to the Enemy AI Health Script).", MessageType.None, true);
        GUI.backgroundColor = Color.white;
        AIName = EditorGUILayout.TextField("Name of the AI", AIName);
        EditorGUILayout.Space(10);
        
        GUI.backgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.19f);
        EditorGUILayout.HelpBox("The Object that the Cowsins AI Component will be added to.", MessageType.None, true);
        GUI.backgroundColor = Color.white;
        if (ObjectToSetup == null)
        {
            GUI.backgroundColor = new Color(10f, 0.0f, 0.0f, 0.25f);
            EditorGUILayout.LabelField("This field CANNOT be left empty", EditorStyles.helpBox);
            GUI.backgroundColor = Color.white;
        }

        ObjectToSetup = (GameObject) EditorGUILayout.ObjectField("AI Object", ObjectToSetup, typeof(GameObject), true);
        GUILayout.Space(10);

        GUI.backgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.19f);
        EditorGUILayout.HelpBox("The Unity Tag that will be used to the AI. Note: Untagged will result in the AI not functioning.", MessageType.None, true);
        GUI.backgroundColor = Color.white;
        AITag = EditorGUILayout.TagField("Tag for the AI", AITag);
        EditorGUILayout.Space(10);
        
        GUI.backgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.19f);
        EditorGUILayout.HelpBox("The Unity Layer that will be used to the AI. Note: Default will result in the AI not functioning.", MessageType.None, true);
        GUI.backgroundColor = Color.white;
        enemyMask = (LayerMask)EditorGUILayout.LayerField("Layer for the AI", enemyMask);
        EditorGUILayout.Space(10);
        
        GUI.backgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.19f);
        EditorGUILayout.HelpBox("Whether or not the death method should be ragdoll or instant delete.", MessageType.None, true);
        GUI.backgroundColor = Color.white;
        useRagdoll = EditorGUILayout.Toggle("Use Ragdoll", useRagdoll);
        EditorGUILayout.Space(10);

        if (useRagdoll)
        {
            GUI.backgroundColor = new Color(10f, 10.0f, 0.0f, 0.25f);
            EditorGUILayout.HelpBox("Click the 'See the Tutorial Playlist' button above to view how to setup ragdoll.", MessageType.Warning, true);
        }
        
        GUI.backgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.19f);
        EditorGUILayout.HelpBox("The AI Type being used.", MessageType.None, true);
        GUI.backgroundColor = Color.white;
        _aiType = (AIType)EditorGUILayout.EnumPopup("AI Type", _aiType);
        EditorGUILayout.Space(10);

        if (_aiType == AIType.Enemy)
        {
            EditorGUI.indentLevel++;
            GUI.backgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.19f);
            EditorGUILayout.HelpBox("The Type of the Enemy", MessageType.None, true);
            GUI.backgroundColor = Color.white;
            _enemyType = (enemyType)EditorGUILayout.EnumPopup("Enemy Type", _enemyType);
            EditorGUILayout.Space(10);

            if (_enemyType == enemyType.Shooter)
            {
                EditorGUI.indentLevel++;
                
                GUI.backgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.19f);
                EditorGUILayout.HelpBox("The Shooter Method the AI should use.", MessageType.None, true);
                GUI.backgroundColor = Color.white;
                _shooterType = (shooterType)EditorGUILayout.EnumPopup("Shooter Type", _shooterType);
                EditorGUILayout.Space(10);

                if (_shooterType == shooterType.Hitscan)
                {
                    GUI.backgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.19f);
                    EditorGUILayout.HelpBox("The Bullet Trail the AI will use.", MessageType.None, true);
                    GUI.backgroundColor = Color.white;
                    hitscanTrailRenderer = (TrailRenderer)EditorGUILayout.ObjectField("Bullet Trail Object", hitscanTrailRenderer, typeof(TrailRenderer), false);
                    EditorGUILayout.Space(10);
                    
                    GUI.backgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.19f);
                    EditorGUILayout.HelpBox("What the AI will hit.", MessageType.None, true);
                    GUI.backgroundColor = Color.white;
                    hitscanHitMask = EditorGUILayout.LayerField("Hit Mask", hitscanHitMask);
                    EditorGUILayout.Space(10);
                    EditorGUI.indentLevel--;
                }

                EditorGUI.indentLevel--;
            }
            EditorGUI.indentLevel--;
        }
        
        GUI.backgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.19f);
        EditorGUILayout.HelpBox("The Movement Type Applied.", MessageType.None, true);
        GUI.backgroundColor = Color.white;
        _movementType = (MovementType) EditorGUILayout.EnumPopup("Movement Type", _movementType);
        EditorGUILayout.Space(10);

        if (ObjectToSetup == null)
        {
            GUI.backgroundColor = new Color(10f, 0.0f, 0.0f, 0.25f);
            EditorGUILayout.HelpBox("You must have a GameObject applied below before the Setup process can be completed.", MessageType.Error, true);
            GUI.backgroundColor = Color.white;
        }
        
        EditorGUI.BeginDisabledGroup(ObjectToSetup == null);
        if (GUILayout.Button("Setup AI"))
        {
            if (EditorUtility.DisplayDialog("Cowsins AI Setup Handler",
                    "Are you sure you'd like to setup an AI on this GameObject?", "Continue", "Cancel"))
            {
                #if UNITY_2018_3_OR_NEWER

                PrefabAssetType assetType = PrefabUtility.GetPrefabAssetType(ObjectToSetup);

                if (assetType != PrefabAssetType.NotAPrefab)
                {
                    PrefabUtility.UnpackPrefabInstance(ObjectToSetup, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
                }
                
                #else
                PrefabUtility.DisconnectPrefabInstance(ObjectToSetup);
                #endif
                AssignCowsinsAIComponents();
                startVal = EditorApplication.timeSinceStartup;
            }
        }
        GUILayout.Space(25);
        EditorGUI.EndDisabledGroup();
        
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView();
        GUILayout.Space(25);
        GUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
        
        GUILayout.Space(30);

        if (secs > 0)
        {
            progress = EditorApplication.timeSinceStartup - startVal;

            if (progress < secs)
            {
                EditorUtility.DisplayProgressBar("Cowsins AI Setup Manager", "Configuring AI...",
                    (float) (progress / secs));
            }
            else
            {
                EditorUtility.ClearProgressBar();

                if (DisplayConfirmation && !DontShowDisplayConfirmation)
                {
                    if (EditorUtility.DisplayDialog("Cowsins AI Setup Manager - Success",
                            "Your AI has been successfully created!", "Okay", "Okay, Don't Show Again"))
                    {
                        DisplayConfirmation = false;
                    }
                    else
                    {
                        DisplayConfirmation = false;
                        DontShowDisplayConfirmation = true;
                    }
                }
            }
        }

    }

    void AssignCowsinsAIComponents()
    {
        if (ObjectToSetup != null && ObjectToSetup.GetComponent<CowsinsAI>() == null &&
            ObjectToSetup.GetComponent<Animator>() != null)
        {
            secs = 1.5f;

            ObjectToSetup.AddComponent<CowsinsAI>();

            ObjectToSetup.tag = AITag;
            ObjectToSetup.layer = enemyMask;

            foreach (Transform gameObject in ObjectToSetup.transform)
            {
                gameObject.gameObject.layer = enemyMask;
            }
            
            EnemyAI enemyAI = ObjectToSetup.AddComponent<EnemyAI>();

            enemyAI.name = AIName;
            
            ObjectToSetup.AddComponent<NavMeshAgent>();

            CowsinsAI cowsinsAI = ObjectToSetup.GetComponent<CowsinsAI>();

            cowsinsAI.moveMode = (CowsinsAI.MoveMode) _movementType;
            cowsinsAI.aiType = (CowsinsAI.AIType) _aiType;
            cowsinsAI.enemyType = (CowsinsAI.EnemyType) _enemyType;
            cowsinsAI.shooterType = (CowsinsAI.ShooterType)_shooterType;
            cowsinsAI.useRagdoll = useRagdoll;
        }
        else if (ObjectToSetup == null)
        {
            EditorUtility.DisplayDialog("Cowsins AI Setup Manager - Oops!", "There is no object assigned to the AI Object slot. Please assign one and try again.", "Okay");
            return;
        }
        else if (ObjectToSetup.GetComponent<CowsinsAI>() != null)
        {
            EditorUtility.DisplayDialog("Cowsins AI Setup Manager - Oops!", "There is already an Cowsins AI component on this object. Please choose another object to apply an Cowsins AI component to.", "Okay");
            return;
        }
        else if (ObjectToSetup.GetComponent<Animator>() == null)
        {
            EditorUtility.DisplayDialog("Cowsins AI Setup Manager - Oops!", "There is no Animator component attached to your AI. Please assign one and try again.", "Okay");
            return;
        }
    }
}