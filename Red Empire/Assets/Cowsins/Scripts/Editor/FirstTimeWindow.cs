using UnityEditor;
using UnityEngine;

namespace cowsins
{

    public class FirstTimeWindow : EditorWindow
    {

        private GUIStyle selectedStyle;

        #region initLogic

        [MenuItem("Cowsins/FPS Engine Startup")]
        public static void OpenWindow()
        {
            FirstTimeWindow window = GetWindow<FirstTimeWindow>();
            window.titleContent = new GUIContent("FPS Engine Startup");
            window.minSize = new Vector2(420, 600);
            window.maxSize = new Vector2(420, 600);
            window.Show();
        }

        #endregion

        #region content
        private Vector2 scrollPosition = Vector2.zero;

        private void OnGUI()
        {
            DefineSelectedStyle();

            float scrollViewWidth = 420f; // Width of the scroll view
            GUILayout.BeginArea(new Rect((Screen.width - scrollViewWidth) / 2, 20, scrollViewWidth, Screen.height - 40));
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(scrollViewWidth));


            GUILayout.BeginVertical();

            // FPS Engine Logo
            GUILayout.Label(AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Cowsins/UI/Logo/FPS_Engine_Logo_White.png"), GUILayout.Width(400), GUILayout.Height(100));

            // Header Label
            GUIStyle headerStyle = new GUIStyle(GUI.skin.label);
            headerStyle.fontSize = 20;
            headerStyle.fontStyle = FontStyle.Bold;
            GUIStyle subHeaderStyle = new GUIStyle(GUI.skin.label);
            subHeaderStyle.fontSize = 16;
            subHeaderStyle.fontStyle = FontStyle.Bold;
            GUILayout.Label("Welcome to FPS Engine!", headerStyle, GUILayout.Width(400));

            // Description Label
            GUIStyle descriptionStyle = new GUIStyle(GUI.skin.label);
            descriptionStyle.wordWrap = true; // Allow wrapping for the description
            GUILayout.Label("Hey! Thanks for purchasing the FPS Engine! At Cowsins, we love helping game developers make their dreams come true, so we can't wait to see the masterpiece you create with our tool!", descriptionStyle, GUILayout.Width(400));
            GUILayout.Space(10);
            GUILayout.Label("Additionally, please consider leaving a review on the Unity Asset Store to help us provide Free Support and Free Updates to FPS Engine!", descriptionStyle, GUILayout.Width(400));
            GUILayout.Space(5);
            if (GUILayout.Button("Leave a Review"))
            {
                Application.OpenURL("https://assetstore.unity.com/packages/templates/systems/fps-engine-218594");
            }
            GUILayout.Space(20);
            GUILayout.Label("Tutorials, Support & Documentation", headerStyle, GUILayout.Width(400));
            GUILayout.Space(5);
            GUILayout.Label("Documentation & Tutorials", subHeaderStyle, GUILayout.Width(400));
            GUILayout.Label("FPS Engine documentation and tutorials can be found at: ", descriptionStyle, GUILayout.Width(400));

            if (GUILayout.Button("Documentation"))
            {
                Application.OpenURL("https://cowsinss-organization.gitbook.io/fps-engine-documentation");
            }
            GUILayout.Space(5);
            GUILayout.Label("Support", subHeaderStyle, GUILayout.Width(400));
            GUILayout.Label("Get access to support by Cowsins on our Discord Server!", descriptionStyle, GUILayout.Width(400));
            GUILayout.Space(5);
            if (GUILayout.Button("Join Discord Server"))
            {
                Application.OpenURL("https://discord.gg/759gSeTT9m");
            }

            GUILayout.Space(20);
            GUILayout.Label("First Steps", headerStyle, GUILayout.Width(400));
            GUILayout.Space(5);
            GUILayout.Label("Make sure to include all dependencies when importing FPS Engine, as well as enabling the New Input System. ", descriptionStyle, GUILayout.Width(400));
            GUILayout.Space(5);
            GUILayout.Label("We highly recommend you to navigate to Cowsins/Demo, where you will find the showrooms, which are perfect to get familiar with the engine. You will also find the blank scene, which works the best to kickstart the development of your game!", descriptionStyle, GUILayout.Width(400));
            GUILayout.Label("Feel free to reach out on Discord if you need help!", descriptionStyle, GUILayout.Width(400));
            GUILayout.Space(50);
            GUILayout.EndVertical();

            GUILayout.EndScrollView();
            GUILayout.EndArea();

            // Footer
            Rect footerRect = new Rect(0, position.height - 20, position.width, 50);

            EditorGUI.DrawRect(footerRect, new Color(0.15f, 0.15f, 0.15f));
            GUILayout.BeginArea(footerRect);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            // Draggable Button visibility toggle
            bool newToggleValue = GUILayout.Toggle(DraggableButtonInSceneView.showDraggableButton, "Show Draggable Button in Scene View");
            if (newToggleValue != DraggableButtonInSceneView.showDraggableButton)
            {
                DraggableButtonInSceneView.SetShowDraggableButton(newToggleValue);
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        #endregion

        #region styles
        private void DefineSelectedStyle()
        {
            selectedStyle = new GUIStyle(GUI.skin.button);
            selectedStyle.normal.background = MakeTex(2, 2, new Color(0.2f, 0.2f, 0.2f, 1f));
            selectedStyle.active.background = MakeTex(2, 2, new Color(0.4f, 0.4f, 0.4f, 1f));
        }

        private Texture2D MakeTex(int width, int height, Color color)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; ++i)
            {
                pix[i] = color;
            }
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }
        #endregion
    }

}