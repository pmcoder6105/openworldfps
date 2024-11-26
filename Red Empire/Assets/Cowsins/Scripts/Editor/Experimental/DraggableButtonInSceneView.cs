#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
namespace cowsins
{
    [InitializeOnLoad]
    public static class DraggableButtonInSceneView
    {
        public static int buttonCount = 4;


        private const string ShowDraggableButtonKey = "Cowsins_ShowDraggableButton";
        public static bool showDraggableButton;

        private static Vector2 buttonPosition = new Vector2(10, 10);
        private static bool isDragging = false;
        private static GUIStyle buttonStyle;
        private static Texture2D logoIcon;
        private static Vector2 buttonSize = new Vector2(40, 35);
        private static Vector2 imageSize = new Vector2(21, 21);
        private static bool showMenu = false;

        public delegate void AddButtonDelegate(Rect menuRect);
        public static event AddButtonDelegate OnAddExternalButtons;

        static DraggableButtonInSceneView()
        {
            // Gather Visibility State from Editor Prefs
            showDraggableButton = EditorPrefs.GetBool(ShowDraggableButtonKey, true);
            SceneView.duringSceneGui += OnSceneGUI;

            logoIcon = Resources.Load<Texture2D>("CustomEditor/LogoIcon");
        }

        private static void OnSceneGUI(SceneView sceneView)
        {
            if (!showDraggableButton)
                return;

            // Initialize button style
            if (buttonStyle == null)
            {
                buttonStyle = new GUIStyle(GUI.skin.button);
            }

            Handles.BeginGUI();

            Rect buttonRect = new Rect(buttonPosition.x, buttonPosition.y, buttonSize.x, buttonSize.y);

            // Handle button interaction
            if (Event.current.button != 0)
                HandleDragging(buttonRect, sceneView);

            // Toggle the Menu
            if (GUI.Button(buttonRect, GUIContent.none, buttonStyle) && Event.current.button == 0)
            {
                showMenu = !showMenu;
                Event.current.Use();
            }

            // Draw image 
            Rect imageRect = new Rect(
                buttonRect.x + (buttonSize.x - imageSize.x) / 2,
                buttonRect.y + (buttonSize.y - imageSize.y) / 2,
                imageSize.x,
                imageSize.y
            );

            GUI.DrawTexture(imageRect, logoIcon, ScaleMode.ScaleToFit);

            // Draw the menu if it should be visible
            if (showMenu && !isDragging)
            {
                // Determine menu position based on button position
                DrawMenu(new Rect(
                    buttonRect.x + (buttonPosition.x < 500 ? buttonSize.x : -40 * buttonCount),
                    buttonRect.y,
                    100,
                    buttonSize.y
                ), buttonPosition.x);
            }
            Handles.EndGUI();

            sceneView.Repaint();
        }

        private static void DrawMenu(Rect menuRect, float buttonPosition)
        {
            // Background
            GUI.Box(menuRect, GUIContent.none, GUI.skin.box);

            // Textures for different options
            Texture2D option1Image = Resources.Load<Texture2D>("CustomEditor/FolderWeapon");
            Texture2D option2Image = Resources.Load<Texture2D>("CustomEditor/FolderSO");
            Texture2D option3Image = Resources.Load<Texture2D>("CustomEditor/AddWeapon");
            Texture2D option4Image = Resources.Load<Texture2D>("CustomEditor/Tutorials");


            GUIContent option1Content = new GUIContent(option1Image);
            GUIContent option2Content = new GUIContent(option2Image);
            GUIContent option3Content = new GUIContent(option3Image);
            GUIContent option4Content = new GUIContent(option4Image);

            float itemWidth = menuRect.width / 3;
            float itemHeight = menuRect.height;

            // Button 1: Focus the Weapons folder
            if (GUI.Button(new Rect(GetButtonPosition(menuRect, 0), menuRect.y, itemWidth, itemHeight), option1Content))
            {
                string folderPath = "Assets/Cowsins/Prefabs/Weapons";

                Object folder = AssetDatabase.LoadAssetAtPath<Object>(folderPath);
                if (folder != null)
                {
                    EditorGUIUtility.PingObject(folder);
                    Selection.activeObject = folder;
                }
                else
                {
                    Debug.LogError($"Folder is empty or not found: {folderPath}. Did you remove or rename the Weapons Folder?");
                }
            }
            // Button 2: Focus the Weapon_SO Folder
            if (GUI.Button(new Rect(GetButtonPosition(menuRect, 1), menuRect.y, itemWidth, itemHeight), option2Content))
            {
                string folderPath = "Assets/Cowsins/ScriptableObjects/Weapons";
                Object folder = AssetDatabase.LoadAssetAtPath<Object>(folderPath);
                if (folder != null)
                {
                    EditorGUIUtility.PingObject(folder);
                    Selection.activeObject = folder;
                }
                else
                {
                    Debug.LogError($"Folder is empty or not found: {folderPath}. Did you remove or rename the ScriptableObjects Folder?");
                }

            }
            // Open the WeaponCreatorAssistant
            if (GUI.Button(new Rect(GetButtonPosition(menuRect, 2), menuRect.y, itemWidth, itemHeight), option3Content))
            {
                WeaponCreatorAssistant window = EditorWindow.GetWindow<WeaponCreatorAssistant>("Create Weapon");
                window.minSize = new Vector2(450, 550);
                window.maxSize = window.minSize;
                window.Show();
            }
            // Open Docs
            if (GUI.Button(new Rect(GetButtonPosition(menuRect, 3), menuRect.y, itemWidth, itemHeight), option4Content))
            {
                Application.OpenURL("https://www.cowsins.com/");
            }

            OnAddExternalButtons?.Invoke(menuRect);
        }

        private static void HandleDragging(Rect buttonRect, SceneView sceneView)
        {
            // Right mouse button down events 
            if (Event.current.type == EventType.MouseDown && Event.current.button == 1 && buttonRect.Contains(Event.current.mousePosition))
            {
                isDragging = true;
                showMenu = false;
                Event.current.Use();
            }

            // Handle dragging
            if (isDragging && Event.current.type == EventType.MouseDrag)
            {
                buttonPosition += Event.current.delta;
                // Limit button position to the Scene Window
                ClampButtonPosition(sceneView);
                Event.current.Use();
            }

            // Handle Stop Dragging and snapping
            if (Event.current.type == EventType.MouseUp && Event.current.button == 1)
            {
                isDragging = false;
                SnapToNearestEdge(sceneView);
            }
        }

        private static void ClampButtonPosition(SceneView sceneView)
        {
            // Get SceneView window size
            Vector2 sceneViewSize = new Vector2(sceneView.position.width, sceneView.position.height);

            // Calculate limits
            float clampedX = Mathf.Clamp(buttonPosition.x, 10, sceneViewSize.x - buttonSize.x - 10);
            float clampedY = Mathf.Clamp(buttonPosition.y, 10, sceneViewSize.y - buttonSize.y - 10);

            buttonPosition = new Vector2(clampedX, clampedY);
        }

        private static void SnapToNearestEdge(SceneView sceneView)
        {
            // Get SceneView window size
            Vector2 sceneViewSize = new Vector2(sceneView.position.width, sceneView.position.height);

            // Calculate the distances to each edge
            float distanceToLeft = buttonPosition.x - 10;
            float distanceToRight = sceneViewSize.x - (buttonPosition.x + buttonSize.x + 10);
            float distanceToTop = buttonPosition.y - 10;
            float distanceToBottom = sceneViewSize.y - (buttonPosition.y + buttonSize.y + 10);

            // Find the nearest edge
            float minDistance = Mathf.Min(distanceToLeft, distanceToRight, distanceToTop, distanceToBottom);

            if (minDistance == distanceToLeft) // left edge
            {
                buttonPosition.x = 10;
            }
            else if (minDistance == distanceToRight) // right edge
            {
                buttonPosition.x = sceneViewSize.x - buttonSize.x - 10;
            }
            else if (minDistance == distanceToTop) // top edge
            {
                buttonPosition.y = 10;
            }
            else if (minDistance == distanceToBottom)//bottom edge
            {
                buttonPosition.y = sceneViewSize.y - buttonSize.y - 10;
            }
        }

        public static float GetButtonPosition(Rect menuRect, int index)
        {
            float spacing = 5;
            float itemWidth = menuRect.width / 3;
            float offsetX = 3 * itemWidth + 4 * spacing;
            float initialSpacing = buttonPosition.x < 500 ? 5 : -4;

            return menuRect.x + spacing * (index + 1) + itemWidth * index + initialSpacing;
        }

        public static void SetShowDraggableButton(bool value)
        {
            showDraggableButton = value;
            EditorPrefs.SetBool(ShowDraggableButtonKey, value); // Save the value to EditorPrefs
        }
    }
}

#endif