using System;
using cowsins;
using UnityEditor;
using UnityEngine;

public class CowsinsAnimatorController : EditorWindow
{
    //Shooter Clips
    private AnimationClip idleShooterClip;
    private AnimationClip walkShooterClip;
    private AnimationClip walkingAimedClip;
    private AnimationClip idleAimedClip;
    private AnimationClip firingClip;

    //Melee Clips
    private AnimationClip idleMeleeClip;
    private AnimationClip walkMeleeClip;
    private AnimationClip attackMeleeClip;
    
    //NPC Clips
    private AnimationClip idleNPCClip;
    private AnimationClip walkNPCClip;

    private ControllerType controllerType;

    private string animatorName;
    private string saveLocation;

    private bool hasClipsMelee = false;
    private bool hasClipsShooter = false;
    private bool hasClipsNPC = false;

    private Texture CowsinsAILogo;
    private Vector2 scrollPos;
    
    private enum ControllerType
    {
        Shooter,
        Melee,
        NPC
    }

    [MenuItem("COWSINS/AI/Animator Controller Creator", false, 1)]
    public static void ShowWindow()
    {
        EditorWindow editorWindow = EditorWindow.GetWindow(typeof(CowsinsAnimatorController), false, "Cowsins Animator Handler");
        editorWindow.minSize = new Vector2(300f, 250f);
    }

    private void OnEnable()
    {
        if(CowsinsAILogo == null) CowsinsAILogo = Resources.Load("CustomEditor/Cowsins AI Logo") as Texture;
    }

    private void OnGUI()
    {
        GUILayout.Space(15);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginVertical("Box");

        var style = new GUIStyle(EditorStyles.boldLabel) {alignment = TextAnchor.MiddleCenter};
        EditorGUILayout.LabelField(new GUIContent(CowsinsAILogo), style, GUILayout.ExpandWidth(true),
            GUILayout.Height(64));
        EditorGUILayout.LabelField("Cowsins AI Animator Handler", style, GUILayout.ExpandWidth(true));
        EditorGUILayout.HelpBox(
            "The Cowsins AI Animator Handler creates and handles the process of creating Animator Controllers that are crucial in the Cowsins AI Component working properly",
            MessageType.None, true);
        GUILayout.Space(4);

        var HelpButtonStyle = new GUIStyle(GUI.skin.button);
        HelpButtonStyle.normal.textColor = Color.white;
        HelpButtonStyle.fontStyle = FontStyle.Bold;

        GUI.backgroundColor = new Color(1f, 1f, 0.25f, 0.25f);
        EditorGUILayout.LabelField(
            "For a detailed tutorial on creating an Animator Controller, please see the Getting Started Tutorial Button below",
            EditorStyles.helpBox);
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
        EditorGUILayout.HelpBox("The AI Animation Controller Type", MessageType.None, true);
        GUI.backgroundColor = Color.white;
        controllerType = (ControllerType)EditorGUILayout.EnumPopup("Animation Controller Type", controllerType);
        EditorGUILayout.Space(10);

        if (controllerType == ControllerType.Shooter)
        {
            EditorGUI.indentLevel++;
            
            idleMeleeClip = null;
            walkMeleeClip = null;
            attackMeleeClip = null;
            walkNPCClip = null;
            idleNPCClip = null;

            GUI.backgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.19f);
            EditorGUILayout.HelpBox("The Animation Clip used for Shooter Idle Animation", MessageType.None, true);
            GUI.backgroundColor = Color.white;
            idleShooterClip = (AnimationClip) EditorGUILayout.ObjectField("Shooter Idle Animation", idleShooterClip,
                typeof(AnimationClip), true);
            EditorGUILayout.Space(10);
            
            GUI.backgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.19f);
            EditorGUILayout.HelpBox("The Animation Clip used for Shooter Walk Animation", MessageType.None, true);
            GUI.backgroundColor = Color.white;
            walkShooterClip = (AnimationClip) EditorGUILayout.ObjectField("Shooter Walk Animation", walkShooterClip,
                typeof(AnimationClip), true);
            EditorGUILayout.Space(10);
            
            GUI.backgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.19f);
            EditorGUILayout.HelpBox("The Animation Clip used for Shooter Walking Aimed Animation", MessageType.None, true);
            GUI.backgroundColor = Color.white;
            walkingAimedClip = (AnimationClip) EditorGUILayout.ObjectField("Shooter Walking Aimed Animation", walkingAimedClip,
                typeof(AnimationClip), true);
            EditorGUILayout.Space(10);
            
            GUI.backgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.19f);
            EditorGUILayout.HelpBox("The Animation Clip used for Shooter Idle Aimed Animation", MessageType.None, true);
            GUI.backgroundColor = Color.white;
            idleAimedClip = (AnimationClip) EditorGUILayout.ObjectField("Shooter Idle Aimed Animation", idleAimedClip,
                typeof(AnimationClip), true);
            EditorGUILayout.Space(10);
            
            GUI.backgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.19f);
            EditorGUILayout.HelpBox("The Animation Clip used for Shooter Firing Animation", MessageType.None, true);
            GUI.backgroundColor = Color.white;
            firingClip = (AnimationClip) EditorGUILayout.ObjectField("Shooter Firing Animation", firingClip,
                typeof(AnimationClip), true);
            EditorGUILayout.Space(10);

            EditorGUI.indentLevel--;
        }

        if (controllerType == ControllerType.Melee)
        {
            EditorGUI.indentLevel++;
            
            idleShooterClip = null;
            walkShooterClip = null;
            walkingAimedClip = null;
            idleAimedClip = null;
            firingClip = null;
            walkNPCClip = null;
            idleNPCClip = null;
            
            GUI.backgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.19f);
            EditorGUILayout.HelpBox("The Animation Clip used for Melee Idle Animation", MessageType.None, true);
            GUI.backgroundColor = Color.white;
            idleMeleeClip = (AnimationClip) EditorGUILayout.ObjectField("Melee Idle Animation", idleMeleeClip,
                typeof(AnimationClip), true);
            EditorGUILayout.Space(10);
            
            GUI.backgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.19f);
            EditorGUILayout.HelpBox("The Animation Clip used for Melee Walk Animation", MessageType.None, true);
            GUI.backgroundColor = Color.white;
            walkMeleeClip = (AnimationClip) EditorGUILayout.ObjectField("Melee Walk Animation", walkMeleeClip,
                typeof(AnimationClip), true);
            EditorGUILayout.Space(10);
            
            GUI.backgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.19f);
            EditorGUILayout.HelpBox("The Animation Clip used for Melee Attack Animation", MessageType.None, true);
            GUI.backgroundColor = Color.white;
            attackMeleeClip = (AnimationClip) EditorGUILayout.ObjectField("Melee Attack Animation", attackMeleeClip,
                typeof(AnimationClip), true);
            EditorGUILayout.Space(10);
            
            EditorGUI.indentLevel--;
        }

        if (controllerType == ControllerType.NPC)
        {
            EditorGUI.indentLevel++;
            
            idleShooterClip = null;
            walkShooterClip = null;
            walkingAimedClip = null;
            idleAimedClip = null;
            firingClip = null;
            idleMeleeClip = null;
            walkMeleeClip = null;
            attackMeleeClip = null;
            
            GUI.backgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.19f);
            EditorGUILayout.HelpBox("The Animation Clip used for NPC Idle Animation", MessageType.None, true);
            GUI.backgroundColor = Color.white;
            idleNPCClip = (AnimationClip) EditorGUILayout.ObjectField("NPC Idle Animation", idleNPCClip,
                typeof(AnimationClip), true);
            EditorGUILayout.Space(10);
            
            GUI.backgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.19f);
            EditorGUILayout.HelpBox("The Animation Clip used for NPC Walk Animation", MessageType.None, true);
            GUI.backgroundColor = Color.white;
            walkNPCClip = (AnimationClip) EditorGUILayout.ObjectField("NPC Walk Animation", walkNPCClip,
                typeof(AnimationClip), true);
            EditorGUILayout.Space(10);
            
            EditorGUI.indentLevel--;
        }
        
        GUI.backgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.19f);
        EditorGUILayout.HelpBox("The Name used for the Animator Controller", MessageType.None, true);
        GUI.backgroundColor = Color.white;
        animatorName = EditorGUILayout.TextField("Animator Controller Name", animatorName);
        EditorGUILayout.Space(10);
        
        GUI.backgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.19f);
        EditorGUILayout.HelpBox("The Save Location used for the Animator Controller", MessageType.None, true);
        GUI.backgroundColor = Color.white;
        saveLocation = EditorGUILayout.TextField("Animator Controller Save Location", saveLocation);
        EditorGUILayout.Space(10);
        
        EditorGUILayout.Space(2f);
        if (GUILayout.Button("GENERATE ANIMATION CONTROLLER"))
        {
            if(controllerType == ControllerType.Shooter)
            {
                if (idleShooterClip == null)
                {
                    Debug.LogError("Idle Clip Empty");
                    hasClipsShooter = false;
                    return;
                }
                if (walkShooterClip == null)
                {
                    Debug.LogError("Walk Clip Empty");
                    hasClipsShooter = false;
                    return;
                }
                if (walkingAimedClip == null)
                {
                    Debug.LogError("Walking Aimed Clip Empty");
                    hasClipsShooter = false;
                    return;
                }
                if (idleAimedClip == null)
                {
                    Debug.LogError("Idle Aimed Clip Empty");
                    hasClipsShooter = false;
                    return;
                }
                if (firingClip == null)
                {
                    Debug.LogError("Firing Clip Empty");
                    hasClipsShooter = false;
                    return;
                }

                GenerateShooterController();
                hasClipsShooter = true;
            }

            if (hasClipsShooter == true)
            {
                GUI.backgroundColor = new Color(0.1f, 1f, 0.1f, 0.5f);
                EditorGUILayout.LabelField("Successfully Created Shooter Animator Controller", EditorStyles.helpBox);
            }
            if (hasClipsShooter == false)
            {
                GUI.backgroundColor = new Color(10f, 0.0f, 0.0f, 0.25f);
                EditorGUILayout.LabelField("Missing Crucial Animator Clips, Refer To Console For More Info", EditorStyles.helpBox);
            }

            if (controllerType == ControllerType.Melee)
            {
                if(idleMeleeClip == null)
                {
                    Debug.LogError("Melee Idle Clip Empty");
                    hasClipsMelee = false;
                    return;
                }
                if(walkMeleeClip == null)
                {
                    Debug.LogError("Melee Walk Clip Empty");
                    hasClipsMelee = false;
                    return;
                }
                if(attackMeleeClip == null)
                {
                    Debug.LogError("Melee Attack Clip Empty");
                    hasClipsMelee = false;
                    return;
                }

                GenerateMeleeController();
                hasClipsMelee = true;
            }
            
            if(hasClipsMelee == true)
            {
                GUI.backgroundColor = new Color(0.1f, 1f, 0.1f, 0.5f);
                EditorGUILayout.LabelField("Successfully Created Melee Animator Controller", EditorStyles.helpBox);
            }
            if(hasClipsMelee == false)
            {
                GUI.backgroundColor = new Color(10f, 0.0f, 0.0f, 0.25f);
                EditorGUILayout.LabelField("Missing Crucial Animator Clips, Refer To Console For More Info", EditorStyles.helpBox);
            }
            
            if (controllerType == ControllerType.NPC)
            {
                if (idleNPCClip == null)
                {
                    Debug.LogError("NPC Idle Clip Empty!");
                    hasClipsNPC = false;
                    return;
                }

                if (walkNPCClip == null)
                {
                    Debug.LogError("NPC Walk Clip Empty!");
                    hasClipsNPC = false;
                    return;
                }

                GenerateNPCController();
                hasClipsNPC = true;
            }
        
            if(hasClipsNPC == true)
            {
                GUI.backgroundColor = new Color(0.1f, 1f, 0.1f, 0.5f);
                EditorGUILayout.LabelField("Successfully Created NPC Animator Controller", EditorStyles.helpBox);
            }
            if(hasClipsNPC == false)
            {
                GUI.backgroundColor = new Color(10f, 0.0f, 0.0f, 0.25f);
                EditorGUILayout.LabelField("Missing Crucial Animator Clips, Refer To Console For More Info", EditorStyles.helpBox);
            }
        }
        GUILayout.Space(25);
        EditorGUI.EndDisabledGroup();
        
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView();
        GUILayout.Space(25);
        GUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
    }

    void GenerateNPCController()
    {
        var controller =
            UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath(saveLocation + animatorName +
                ".controller");
        
        //Parameters
        controller.AddParameter("isMoving", AnimatorControllerParameterType.Bool);

        var root = controller.layers[0].stateMachine;
        
        //States
        var idleState = root.AddState("Idle");
        var walkState = root.AddState("Walk");
        
        //Add Animations To States
        idleState.motion = idleNPCClip;
        walkState.motion = walkNPCClip;
        
        //Add Transitions
        var walkTransitionTo = idleState.AddTransition(walkState);
        walkTransitionTo.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, "isMoving");
        walkTransitionTo.duration = 0;
        
        var walkTransitionFrom = walkState.AddTransition(idleState);
        walkTransitionFrom.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0, "isMoving");
        walkTransitionFrom.duration = 0;

        Selection.activeObject = controller;
    }

    void GenerateMeleeController()
    {
        var controller = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath(saveLocation + animatorName + ".controller");
        
        //Parameters
        controller.AddParameter("isMoving", AnimatorControllerParameterType.Bool);
        controller.AddParameter("attacking", AnimatorControllerParameterType.Bool);
    
        var root = controller.layers[0].stateMachine;

        //States
        var idleState = root.AddState("Idle");
        var walkState = root.AddState("Walk");
        var attackState = root.AddState("Attack");

        //Add Animations To States
        idleState.motion = idleMeleeClip;
        walkState.motion = walkMeleeClip;
        attackState.motion = attackMeleeClip;

        //Add Transitions
        var walkTransitionTo = idleState.AddTransition(walkState);
        walkTransitionTo.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, "isMoving");
        walkTransitionTo.duration = 0;

        var walkTransitionFrom = walkState.AddTransition(idleState);
        walkTransitionFrom.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0, "isMoving");
        walkTransitionFrom.duration = 0;

        var attackTransitionTo = walkState.AddTransition(attackState);
        attackTransitionTo.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, "attacking");
        attackTransitionTo.duration = 0;

        var attackTransitionFrom = attackState.AddTransition(walkState);
        attackTransitionFrom.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0, "attacking");
        attackTransitionFrom.duration = 0;
        
        Selection.activeObject = controller;
    }

    void GenerateShooterController()
    {
        var controller = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath(saveLocation + animatorName + ".controller");

        //Parameters
        controller.AddParameter("isMoving", AnimatorControllerParameterType.Bool);
        controller.AddParameter("aimedWalking", AnimatorControllerParameterType.Bool);
        controller.AddParameter("aimedIdle", AnimatorControllerParameterType.Bool);
        controller.AddParameter("firing", AnimatorControllerParameterType.Bool);

        var root = controller.layers[0].stateMachine;

        //States
        var idleState = root.AddState("Idle");
        var walkState = root.AddState("Walk");
        var walkingAimedState = root.AddState("WalkingAimed");
        var firingState = root.AddState("Firing");
        var aimedIdleState = root.AddState("AimedIdle");

        //Add Animations To States
        idleState.motion = idleShooterClip;
        walkState.motion = walkShooterClip;
        walkingAimedState.motion = walkingAimedClip;
        aimedIdleState.motion = idleAimedClip;
        firingState.motion = firingClip;

        //Add Transitions

        /// Walk
        var walkTransitionTo = idleState.AddTransition(walkState);
        walkTransitionTo.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, "isMoving");
        walkTransitionTo.duration = 0;

        var walkTransitionFrom = walkState.AddTransition(idleState);
        walkTransitionFrom.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0, "isMoving");
        walkTransitionFrom.duration = 0;

        /// Combat Idle
        var idleAimedTransitionToWalkingAimed = root.AddAnyStateTransition(aimedIdleState);
        idleAimedTransitionToWalkingAimed.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, "aimedIdle");
        idleAimedTransitionToWalkingAimed.duration = 0;

        var idleAimedTransitionToIdle = aimedIdleState.AddTransition(idleState);
        idleAimedTransitionToIdle.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, "isMoving");
        idleAimedTransitionToIdle.duration = 0;

        /// Firing
        var firingTransitionFromRoot = root.AddAnyStateTransition(firingState);
        firingTransitionFromRoot.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, "firing");
        firingTransitionFromRoot.duration = 0;

        var firingTransitionToWalkingAimed = firingState.AddTransition(walkingAimedState);
        firingTransitionToWalkingAimed.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0, "firing");
        firingTransitionToWalkingAimed.duration = 0;

        var firingTransitionToIdle = firingState.AddTransition(idleState);
        firingTransitionToIdle.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, "isMoving");
        firingTransitionToIdle.duration = 0;

        /// Combat Walk
        var walkAimedTransitionToFromIdle = idleState.AddTransition(walkingAimedState);
        walkAimedTransitionToFromIdle.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, "aimedWalking");
        walkAimedTransitionToFromIdle.duration = 0;

        var walkAimedTransitionToFromAimedIdle = aimedIdleState.AddTransition(walkingAimedState);
        walkAimedTransitionToFromAimedIdle.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, "aimedWalking");
        walkAimedTransitionToFromAimedIdle.duration = 0;

        var walkAimedTransitionToIdle = walkingAimedState.AddTransition(idleState);
        walkAimedTransitionToIdle.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0, "aimedWalking");
        walkAimedTransitionToIdle.duration = 0;
        
        Selection.activeObject = controller;
    }
}
