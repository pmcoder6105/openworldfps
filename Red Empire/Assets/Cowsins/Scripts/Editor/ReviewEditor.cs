#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace cowsins
{
    // REVIEW EDITOR LICENSE BY FIRST GEAR GAMES.
    /* BSD 2-Clause License
    This software was designed for Fish-Networking on Unity https://github.com/FirstGearGames/FishNet

    Redistribution and use in source and binary forms, with or without
    modification, are permitted provided that the following conditions are met:

    1. Redistributions of source code must retain the above copyright notice, this
    list of conditions and the following disclaimer.

    2. Redistributions in binary form must reproduce the above copyright notice,
    this list of conditions and the following disclaimer in the documentation
    and/or other materials provided with the distribution.

    THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
    AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
    IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
    DISCLAIMED.IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
    FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
    DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
    SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
    CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
    OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
    OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.*/
    /// <summary>
    /// Contributed by FirstGearGames! Thank you!
    /// </summary>
    public class ReviewEditor : EditorWindow
    {
        private Texture2D logo;
        private GUIStyle labelStyle, reviewButtonStyle;

        private const string DATETIME_REMINDED = "ReviewDateTimeReminded";
        private const string CHECK_REMIND_COUNT = "CheckRemindCount";
        private const string IS_ENABLED = "ReminderEnabled";

        private static ReviewEditor window;

        internal static void CheckRemindToReview()
        {
            bool reminderEnabled = EditorPrefs.GetBool(IS_ENABLED, true);
            if (!reminderEnabled)
                return;

            /* Require at least two opens and 10 days
             * to be passed before reminding. */
            int checkRemindCount = (EditorPrefs.GetInt(CHECK_REMIND_COUNT, 0) + 1);
            EditorPrefs.SetInt(CHECK_REMIND_COUNT, checkRemindCount);

            //Not enough checks.
            if (checkRemindCount < 2)
                return;

            string dtStr = EditorPrefs.GetString(DATETIME_REMINDED, string.Empty);
            //Somehow got cleared. Reset.
            if (string.IsNullOrWhiteSpace(dtStr))
            {
                ResetDateTimeReminded();
                return;
            }
            long binary;
            //Failed to parse.
            if (!long.TryParse(dtStr, out binary))
            {
                ResetDateTimeReminded();
                return;
            }
            //Not enough time passed.
            DateTime dt = DateTime.FromBinary(binary);
            if ((DateTime.Now - dt).TotalDays < 10)
                return;

            //If here then the reminder can be shown.
            EditorPrefs.SetInt(CHECK_REMIND_COUNT, 0);

            ShowReminder();
        }

        internal static void ResetDateTimeReminded()
        {
            EditorPrefs.SetString(DATETIME_REMINDED, DateTime.Now.ToBinary().ToString());
        }

        private static void ShowReminder()
        {
            InitializeWindow();
        }

        static void InitializeWindow()
        {
            if (window != null)
                return;
            window = (ReviewEditor)EditorWindow.GetWindow(typeof(ReviewEditor));
            window.position = new Rect(0f, 0f, 320f, 300f);
            Rect mainPos;
#if UNITY_2020_1_OR_NEWER
            mainPos = EditorGUIUtility.GetMainWindowPosition();
#else
            mainPos = new Rect(Vector2.zero, Vector2.zero);
#endif
            var pos = window.position;
            float w = (mainPos.width - pos.width) * 0.5f;
            float h = (mainPos.height - pos.height) * 0.5f;
            pos.x = mainPos.x + w;
            pos.y = mainPos.y + h;
            window.position = pos;
        }

        static void StyleWindow()
        {
            InitializeWindow();
            window.logo = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Cowsins/UI/Logo/FPS_Engine_Logo_White.png", typeof(Texture));
            window.labelStyle = new GUIStyle("label");
            window.labelStyle.fontSize = 24;
            window.labelStyle.wordWrap = true;
            window.labelStyle.normal.textColor = new Color32(154, 188, 230, 255);

            window.reviewButtonStyle = new GUIStyle("button");
            window.reviewButtonStyle.fontSize = 18;
            window.reviewButtonStyle.fontStyle = FontStyle.Bold;
            window.reviewButtonStyle.alignment = TextAnchor.MiddleCenter;
            window.reviewButtonStyle.normal.textColor = new Color(1, 1, 1, 1);

        }

        void OnGUI()
        {
            float thisWidth = this.position.width;
            StyleWindow();
            GUILayout.Box(logo, GUILayout.Width(this.position.width), GUILayout.Height(80f));
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(8f);
            GUILayout.Label("Help us by leaving a review!", labelStyle, GUILayout.Width(thisWidth * 0.95f));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Don't Ask Again", GUILayout.Width(this.position.width)))
            {
                this.Close();
                EditorPrefs.SetBool(IS_ENABLED, false);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Ask Later", GUILayout.Width(this.position.width)))
            {
                this.Close();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Leave A Review", GUILayout.Width(this.position.width)))
            {
                EditorPrefs.SetBool(IS_ENABLED, false);
                Application.OpenURL("https://assetstore.unity.com/packages/templates/systems/fps-engine-218594");
                this.Close();
            }
            EditorGUILayout.EndHorizontal();

        }
    }

}
#endif