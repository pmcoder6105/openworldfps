using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Presets;
using UnityEditor.Animations;
namespace cowsins
{

    public class WeaponCreatorAssistant : EditorWindow
    {
        private string weaponName = "NewWeapon";
        private Color titleBackgroundColor = new Color(0.2f, 0.2f, 0.2f);
        private Preset weaponPreset;
        private GameObject weaponModel;

        private AnimationClip idleClip, shootClip, reloadClip, unholsterClip, walkClip, runClip, startInspectClip, loopInspectClip, endInspectClip;

        // Used to controls which foldouts are enabled or disabled
        private bool showName = true;
        private bool showPreset = false;
        private bool showModel = false;
        private bool showAnims = false;

        private Vector2 scrollPos;

        [MenuItem("Cowsins/Create/Weapon ( Weapon Creation Assistant )")]
        public static void DisplayConfirmationDialog()
        {
            WeaponCreatorAssistant window = EditorWindow.GetWindow<WeaponCreatorAssistant>("Create Weapon");
            // Limit the size of the window
            window.minSize = new Vector2(450, 550);
            window.maxSize = window.minSize;
            window.Show();
        }

        private void OnGUI()
        {
            DrawTitleBackground("Create New Weapon | FPS Engine by Cowsins");
            GUILayout.Space(10);

            EditorGUILayout.HelpBox("This feature is experimental and still a work in progress. We welcome your feedback.", MessageType.Warning);

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);

            GUILayout.BeginVertical();
            GUILayout.Space(10);
            // FPS Engine Logo
            GUILayout.Label(AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Cowsins/UI/Logo/FPS_Engine_Logo_White.png"), GUILayout.Width(375), GUILayout.Height(30));
            GUILayout.Space(5);

            // Weapon Name Foldout
            EditorGUILayout.BeginVertical(GUI.skin.GetStyle("HelpBox"));
            {
                showName = EditorGUILayout.Foldout(showName, "Name", true);
                if (showName)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.LabelField("Enter a name for the new weapon:");
                    weaponName = EditorGUILayout.TextField(weaponName);
                    EditorGUI.indentLevel--;
                }
            }
            EditorGUILayout.EndVertical();
            GUILayout.Space(10);

            // Weapon Preset Foldout
            EditorGUILayout.BeginVertical(GUI.skin.GetStyle("HelpBox"));
            {
                showPreset = EditorGUILayout.Foldout(showPreset, "Preset", true);
                if (showPreset)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.LabelField("(Recommended) ");
                    weaponPreset = (Preset)EditorGUILayout.ObjectField("Select Preset:", weaponPreset, typeof(Preset), false);

                    if (weaponPreset && !(weaponPreset.GetTargetTypeName() is "Weapon_SO"))
                    {
                        EditorGUILayout.HelpBox("Weapon Preset is not a valid type. Please ensure Weapon Preset is of type Weapon_SO", MessageType.Error);
                    }
                    EditorGUI.indentLevel--;
                }
            }
            EditorGUILayout.EndVertical();
            GUILayout.Space(10);

            // Weapon Model Foldout
            EditorGUILayout.BeginVertical(GUI.skin.GetStyle("HelpBox"));
            {
                showModel = EditorGUILayout.Foldout(showModel, "Model", true);
                if (showModel)
                {
                    EditorGUI.indentLevel++;
                    weaponModel = (GameObject)EditorGUILayout.ObjectField("Select Weapon Model:", weaponModel, typeof(GameObject), false);
                    if (weaponModel == null)
                    {
                        EditorGUILayout.HelpBox("Weapon Model is null. Please assign a 3D Viewmodel Weapon model to proceed.", MessageType.Error);
                    }
                    else
                    {
                        // Model Preview 
                        GUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                        Rect previewRect = GUILayoutUtility.GetRect(50, 50, GUILayout.Width(150), GUILayout.Height(150));
                        if (AssetPreview.GetAssetPreview(weaponModel))
                            EditorGUI.DrawPreviewTexture(previewRect, AssetPreview.GetAssetPreview(weaponModel));
                        else
                            EditorGUILayout.HelpBox("Preview Not Available", MessageType.Warning);
                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();
                    }
                    EditorGUI.indentLevel--;
                }
            }
            EditorGUILayout.EndVertical();
            GUILayout.Space(10);

            // Weapon Animations Foldout
            EditorGUILayout.BeginVertical(GUI.skin.GetStyle("HelpBox"));
            {
                showAnims = EditorGUILayout.Foldout(showAnims, "Animations", true);
                if (showAnims)
                {
                    EditorGUI.indentLevel++;

                    idleClip = (AnimationClip)EditorGUILayout.ObjectField("Idle:", idleClip, typeof(AnimationClip), false);
                    shootClip = (AnimationClip)EditorGUILayout.ObjectField("Shoot:", shootClip, typeof(AnimationClip), false);
                    reloadClip = (AnimationClip)EditorGUILayout.ObjectField("Reload:", reloadClip, typeof(AnimationClip), false);
                    unholsterClip = (AnimationClip)EditorGUILayout.ObjectField("Unholster:", unholsterClip, typeof(AnimationClip), false);
                    walkClip = (AnimationClip)EditorGUILayout.ObjectField("Walk:", walkClip, typeof(AnimationClip), false);
                    runClip = (AnimationClip)EditorGUILayout.ObjectField("Run:", runClip, typeof(AnimationClip), false);
                    startInspectClip = (AnimationClip)EditorGUILayout.ObjectField("Start Inspect:", startInspectClip, typeof(AnimationClip), false);
                    loopInspectClip = (AnimationClip)EditorGUILayout.ObjectField("Loop Inspect:", loopInspectClip, typeof(AnimationClip), false);
                    endInspectClip = (AnimationClip)EditorGUILayout.ObjectField("End Inspect:", endInspectClip, typeof(AnimationClip), false);

                    EditorGUI.indentLevel--;
                }
            }
            EditorGUILayout.EndVertical();
            GUILayout.Space(10);
            if (GUILayout.Button("Create"))
            {
                if (weaponPreset && !(weaponPreset.GetTargetTypeName() is "Weapon_SO"))
                {
                    EditorUtility.DisplayDialog("Weapon Creation Failed",
                                                              "Weapon Preset is not a valid type. Please ensure Weapon Preset is of type Weapon_SO",
                                                              "Ok");
                    return;
                }
                if (weaponModel == null)
                {
                    EditorUtility.DisplayDialog("Weapon Model is null",
                                                             "Weapon Model Reference is null. Please attach your 3D Viewmodel Weapon model. \n\n"
                                                             , "OK");
                    return;
                }

                // Display confirmation dialog before creating the weapon
                bool confirmed = EditorUtility.DisplayDialog("Create Weapon",
                                                              "Are you sure you want to create a new weapon with the name \"" + weaponName + "\"?\n\n" +
                                                              "This action will create a new ScriptableObject and a corresponding WeaponObject prefab in the project.\n\n" +
                                                              "This WeaponObject will be assigned to your new ScriptableObject automatically.\n\n" +
                                                              "This ScriptableObject will be automatically assigned to the weapon slot in the WeaponIdentification component of the WeaponObject prefab.",
                                                              "Yes", "No");
                if (confirmed)
                {
                    CreateWeapon();
                    this.Close();
                }

            }

            GUILayout.Space(10);
            GUIStyle hyperlinkStyle = new GUIStyle(GUI.skin.label);
            hyperlinkStyle.normal.textColor = new Color(0.1f, 0.5f, 1.0f);
            hyperlinkStyle.hover.textColor = new Color(0.1f, 0.7f, 1.0f);
            if (GUILayout.Button("Need Help? Join our Discord server", hyperlinkStyle))
            {
                Application.OpenURL("https://discord.gg/759gSeTT9m");
            }
            GUILayout.EndVertical();

            GUILayout.Space(10);
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            EditorGUILayout.EndScrollView();
        }

        private void CreateWeapon()
        {

            string folderPath = "Assets/NewWeapons";

            // Check if the folder exists, create it if it doesnt
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                AssetDatabase.CreateFolder("Assets", "NewWeapons");
                AssetDatabase.Refresh();
            }

            folderPath = "Assets/NewWeapons/" + weaponName;

            // Check if the folder exists, create it if it doesnt, taking into account the name of the weapon
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                AssetDatabase.CreateFolder("Assets/NewWeapons", weaponName);
                AssetDatabase.Refresh();
            }
            else
            {
                EditorUtility.DisplayDialog("Folder Already Exists",
                                                "A folder with the name \"" + weaponName + "\" already exists. Please choose a different name.",
                                                "OK");
                return;
            }
            // CREATING A NEW SCRIPTABLE OBJECT INSTANCE
            Weapon_SO newWeapon = ScriptableObject.CreateInstance<Weapon_SO>();

            string assetPath = AssetDatabase.GenerateUniqueAssetPath(folderPath + "/" + weaponName + ".asset");

            AssetDatabase.CreateAsset(newWeapon, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.FocusProjectWindow();
            Selection.activeObject = newWeapon;

            // CREATING A NEW WEAPON PREFAB OBJECT
            string newPath = folderPath + "/" + weaponName + "_WeaponObject.prefab";

            GameObject originalPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Cowsins/Prefabs/Weapons/CowsinsBlankWeaponTemplate.prefab");

            if (originalPrefab == null)
            {
                Debug.LogError("Original prefab not found at specified path.");
                EditorUtility.DisplayDialog("Weapon Creation Failed.", "CowsinsBlankWeaponTemplate not found at path: Assets/Cowsins/Prefabs/Weapons/CowsinsBlankWeaponTemplate.prefab", "OK");
                return;
            }
            GameObject duplicatedPrefab = Instantiate(originalPrefab);
            Transform parent = duplicatedPrefab.transform.GetChild(0).GetChild(0).GetChild(0);

            if (parent == null)
            {
                Debug.LogError("Original prefab has been modified. `Meshes` inside CowsinsBlankWeaponTemplate/Weapon/Crouch could not be found.");
                EditorUtility.DisplayDialog("Weapon Creation Failed.", "CowsinsBlankWeaponTemplate has been modified.", "OK");
                return;
            }
            GameObject instantiatedWeaponModel = Instantiate(weaponModel, duplicatedPrefab.transform);
            instantiatedWeaponModel.transform.SetParent(parent);

            int weaponsLayer = LayerMask.NameToLayer("Weapons");
            SetLayerRecursively(instantiatedWeaponModel, weaponsLayer);

            // Remove Animator from the root of the prefab
            Animator rootAnimator = duplicatedPrefab.GetComponent<Animator>();
            if (rootAnimator != null)
            {
                DestroyImmediate(rootAnimator);
            }

            // Add Animator to the instantiatedWeaponModel
            Animator newAnimator = instantiatedWeaponModel.AddComponent<Animator>();

            AnimatorController originalController = AssetDatabase.LoadAssetAtPath<AnimatorController>("Assets/Cowsins/Animations/Weapons/BlankWeaponAnimatorTemplate.controller");

            if (originalController != null)
            {
                AnimatorController newController = UnityEditor.Animations.AnimatorController.Instantiate(originalController);
                newController.name = weaponName + "_AnimatorController";

                string controllerPath = folderPath + "/" + weaponName + "_AnimatorController.controller";
                AssetDatabase.CreateAsset(newController, controllerPath);
                AssetDatabase.SaveAssets();

                // Assign Animator Controller to the new Animator
                newAnimator.runtimeAnimatorController = newController;

                AssignAnimationClipsToStates(newController);
            }
            else
            {
                Debug.LogError("Original Animator Controller not found at specified path.");
                EditorUtility.DisplayDialog("Weapon Creation Failed.", "BlankWeaponAnimatorTemplate.controller not found at path: Assets/Cowsins/Animations/Weapons/BlankWeaponAnimatorTemplate.controller", "OK");
                return;
            }


            PrefabUtility.SaveAsPrefabAsset(duplicatedPrefab, newPath);
            DestroyImmediate(duplicatedPrefab);
            AssetDatabase.Refresh();

            WeaponIdentification weaponIdentification = AssetDatabase.LoadAssetAtPath<GameObject>(newPath).GetComponent<WeaponIdentification>();
            if (weaponIdentification == null)
            {
                Debug.LogError("WeaponIdentification component not found on the duplicated prefab.");
                EditorUtility.DisplayDialog("Weapon Creation Failed.", "WeaponIdentification component not found on the duplicated prefab.", "OK");
                return;
            }

            // Gather weapon
            weaponIdentification.weapon = newWeapon;

            if (weaponPreset)
                CowsinsUtilities.ApplyPreset(weaponPreset, newWeapon);

            // Assign the WeaponIdentification component to the weaponObject property of the newWeapon
            newWeapon.weaponObject = weaponIdentification;

            // Mark the Weapon_SO asset as dirty and save it
            EditorUtility.SetDirty(newWeapon);
            EditorUtility.SetDirty(weaponIdentification);
            AssetDatabase.SaveAssets();

            EditorUtility.DisplayDialog("Weapon created.", weaponName + " successfully created at path: " + newPath, "OK");
        }

        private void AssignAnimationClipsToStates(AnimatorController controller)
        {
            AnimatorState FindStateByName(ChildAnimatorState[] states, string stateName)
            {
                foreach (var state in states)
                {
                    if (state.state.name == stateName)
                    {
                        return state.state;
                    }
                }
                return null;
            }

            var states = controller.layers[0].stateMachine.states;

            // Finds the animation states and applies the clips
            AnimatorState idleState = FindStateByName(states, "Idle");
            AnimatorState shootState = FindStateByName(states, "shooting");
            AnimatorState reloadState = FindStateByName(states, "reloading");
            AnimatorState unholsterState = FindStateByName(states, "Unholster");
            AnimatorState walkState = FindStateByName(states, "Walk_Anim");
            AnimatorState runState = FindStateByName(states, "Run_Anim");
            AnimatorState startInspectState = FindStateByName(states, "StartInspection");
            AnimatorState loopInspectState = FindStateByName(states, "InspectLoop");
            AnimatorState endInspectState = FindStateByName(states, "StopInspection");
            Debug.Log(idleState);
            if (idleState != null && idleClip != null) idleState.motion = idleClip;
            if (shootState != null && shootClip != null) shootState.motion = shootClip;
            if (reloadState != null && reloadClip != null) reloadState.motion = reloadClip;
            if (unholsterState != null && unholsterClip != null) unholsterState.motion = unholsterClip;
            if (walkState != null && walkClip != null) walkState.motion = walkClip;
            if (runState != null && runClip != null) runState.motion = runClip;
            if (startInspectState != null && startInspectClip != null) startInspectState.motion = startInspectClip;
            if (loopInspectState != null && loopInspectClip != null) loopInspectState.motion = loopInspectClip;
            if (endInspectState != null && endInspectClip != null) endInspectState.motion = endInspectClip;
        }

        private void DrawTitleBackground(string title)
        {
            Rect rect = GUILayoutUtility.GetRect(new GUIContent(title), EditorStyles.boldLabel);
            EditorGUI.DrawRect(new Rect(0, rect.y, position.width, rect.height), titleBackgroundColor);
            GUI.Label(rect, title, EditorStyles.boldLabel);
        }

        // Loops through the children of obj to assign the selected layer
        private void SetLayerRecursively(GameObject obj, int newLayer)
        {
            if (null == obj) return;

            obj.layer = newLayer;

            foreach (Transform child in obj.transform)
            {
                if (null == child) continue;
                SetLayerRecursively(child.gameObject, newLayer);
            }
        }
    }

}

#endif
