#if UNITY_EDITOR
/// <summary>
/// This script belongs to cowsins™ as a part of the cowsins´ FPS Engine. All rights reserved. 
/// </summary>


using UnityEngine;
using UnityEditor;
namespace cowsins
{
    [System.Serializable]
    [CustomEditor(typeof(PlayerMovement))]
    public class PlayerMovementEditor : Editor
    {
        private string[] tabs = { "Assignables", "Movement", "Camera", "Sliding", "Jumping", "Aim assist", "Stamina", "Advanced Movement", "Others" };
        private int currentTab = 0;

        private bool showWallRun, showWallBounce, showDashing, showGrapplingHook, showClimbing;

        override public void OnInspectorGUI()
        {
            serializedObject.Update();
            PlayerMovement myScript = target as PlayerMovement;

            Texture2D myTexture = Resources.Load<Texture2D>("CustomEditor/playerMovement_CustomEditor") as Texture2D;
            GUILayout.Label(myTexture);


            EditorGUILayout.BeginVertical();
            currentTab = GUILayout.SelectionGrid(currentTab, tabs, 6);
            EditorGUILayout.Space(10f);
            EditorGUILayout.EndVertical();
            #region variables

            if (currentTab >= 0 || currentTab < tabs.Length)
            {
                switch (tabs[currentTab])
                {
                    case "Assignables":
                        EditorGUILayout.LabelField("ASSIGNABLES", EditorStyles.boldLabel);
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("playerCam"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("orientation"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("useSpeedLines"));
                        if (myScript.useSpeedLines)
                        {
                            EditorGUI.indentLevel++;
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("speedLines"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("minSpeedToUseSpeedLines"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("speedLinesAmount"));
                            EditorGUI.indentLevel--;
                        }
                        break;
                    case "Camera":
                        EditorGUILayout.LabelField("CAMERA LOOK", EditorStyles.boldLabel);
                        GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(8) });
                        if (myScript.maxCameraAngle != 89.7f) EditorGUILayout.LabelField("WARNING: The maximum camera angle is highly recommended to be set to the maximum value, 89.7", EditorStyles.helpBox);
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("maxCameraAngle"));

                        EditorGUILayout.Space(10f);
                        EditorGUILayout.HelpBox("WARNING: The Player Sensitivity (Mouse & Controller) values will be overridden if you open the scene from the Main Menu. Check GameSettingsManager.cs for more information.", MessageType.Warning);
                        EditorGUILayout.Space(5f);
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("sensitivityX"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("sensitivityY"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("controllerSensitivityX"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("controllerSensitivityY"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("aimingSensitivityMultiplier"));

                        GUILayout.Space(10);
                        EditorGUILayout.LabelField("CAMERA", EditorStyles.boldLabel);
                        GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(8) });
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("normalFOV"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("runningFOV"));
                        if (myScript.canWallRun)
                        {
                            EditorGUI.indentLevel++;
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("wallrunningFOV"));
                            EditorGUI.indentLevel--;
                        }
                        if (myScript.canDash)
                        {
                            EditorGUI.indentLevel++;
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("fovToAddOnDash"));
                            EditorGUI.indentLevel--;
                        }
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("fadeInFOVAmount"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("fadeOutFOVAmount"));
                        if (myScript.allowSliding)
                        {
                            EditorGUI.indentLevel++;
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("slidingCameraTiltAmount"));
                            EditorGUI.indentLevel--;
                        }
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("cameraTiltTransationSpeed"));
                        break;
                    case "Movement":
                        EditorGUILayout.LabelField("BASIC MOVEMENT INPUT SETTINGS", EditorStyles.boldLabel);
                        GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(8) });
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("autoRun"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("alternateSprint"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("alternateCrouch"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("canRunBackwards"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("canRunSideways"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("canRunWhileShooting"));
                        if (!myScript.canRunBackwards || !myScript.canRunWhileShooting)
                        {
                            EditorGUI.indentLevel++;
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("loseSpeedDeceleration"));
                            EditorGUI.indentLevel--;
                        }
                        GUILayout.Space(15);
                        EditorGUILayout.LabelField("BASIC MOVEMENT", EditorStyles.boldLabel);
                        GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(8) });
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("acceleration"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("runSpeed"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("walkSpeed"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("allowCrouch"));
                        if (myScript.allowCrouch)
                        {
                            EditorGUI.indentLevel++;
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("crouchCancelMethod"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("crouchSpeed"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("crouchTransitionSpeed"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("roofCheckDistance"));
                            EditorGUI.indentLevel--;
                        }
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("maxSpeedAllowed"));
                        if (myScript.maxSpeedAllowed < myScript.runSpeed) myScript.maxSpeedAllowed = myScript.runSpeed;
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("whatIsGround"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("groundCheckDistance"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("frictionForceAmount"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("maxSlopeAngle"));
                        break;
                    case "Sliding":
                        EditorGUILayout.LabelField("SLIDING", EditorStyles.boldLabel);
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("allowSliding"));
                        if (myScript.allowSliding)
                        {
                            EditorGUI.indentLevel++;
                            EditorGUILayout.LabelField("A new customizable variable has been unlocked in `CAMERA`.", EditorStyles.helpBox);
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("slideForce"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("slideCounterMovement"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("allowMoveWhileSliding"));
                            EditorGUI.indentLevel--;
                        }

                        break;
                    case "Jumping":

                        EditorGUILayout.LabelField("JUMPING", EditorStyles.boldLabel);
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("allowJump"));
                        if (myScript.allowJump)
                        {
                            EditorGUI.indentLevel++;
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("maxJumps"));
                            if (myScript.maxJumps > 1)
                            {
                                EditorGUI.indentLevel++;
                                EditorGUILayout.PropertyField(serializedObject.FindProperty("resetJumpsOnWallrun"));
                                if (myScript.canWallBounce)
                                {
                                    EditorGUI.indentLevel++;
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("resetJumpsOnWallBounce"));
                                    EditorGUI.indentLevel--;
                                }
                                if (myScript.GetComponent<PlayerStats>().takesFallDamage)
                                {
                                    EditorGUI.indentLevel++;
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("doubleJumpResetsFallDamage"));
                                    EditorGUI.indentLevel--;
                                }
                                else myScript.doubleJumpResetsFallDamage = false;
                                EditorGUI.indentLevel--;
                                EditorGUILayout.PropertyField(serializedObject.FindProperty("directionalJumpMethod"));

                                if (myScript.directionalJumpMethod != PlayerMovement.DirectionalJumpMethod.None)
                                {
                                    EditorGUI.indentLevel++;
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("directionalJumpForce"));
                                    EditorGUI.indentLevel--;
                                }
                            }
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("jumpForce"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("controlAirborne"));
                            if (myScript.allowCrouch)
                            {
                                EditorGUI.indentLevel++;
                                EditorGUILayout.PropertyField(serializedObject.FindProperty("allowCrouchWhileJumping"));
                                EditorGUILayout.PropertyField(serializedObject.FindProperty("canJumpWhileCrouching"));
                                EditorGUI.indentLevel--;
                            }
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("jumpCooldown"));
                            if (myScript.coyoteJumpTime == 0) EditorGUILayout.LabelField("Coyote Jump won´t be applied since the value is equal to 0", EditorStyles.helpBox);
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("coyoteJumpTime"));
                            EditorGUI.indentLevel--;
                        }
                        break;

                    case "Aim assist":

                        EditorGUILayout.LabelField("AIM ASSIST", EditorStyles.boldLabel);
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("applyAimAssist"));
                        if (myScript.applyAimAssist)
                        {
                            EditorGUI.indentLevel++;
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("maximumDistanceToAssistAim"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("aimAssistSpeed"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("aimAssistSensitivity"));
                            EditorGUI.indentLevel--;
                        }

                        break;
                    case "Stamina":
                        EditorGUILayout.LabelField("STAMINA", EditorStyles.boldLabel);
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("usesStamina"));
                        if (myScript.usesStamina)
                        {
                            EditorGUI.indentLevel++;
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("minStaminaRequiredToRun"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("maxStamina"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("staminaRegenMultiplier"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("LoseStaminaWalking"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("staminaLossOnJump"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("staminaLossOnSlide"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("staminaLossOnDash"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("staminaSlider"));
                            EditorGUI.indentLevel--;
                        }
                        break;
                    case "Advanced Movement":
                        EditorGUILayout.LabelField("ADVANCED MOVEMENT", EditorStyles.boldLabel);
                        EditorGUILayout.Space(5);

                        EditorGUI.indentLevel++;
                        EditorGUILayout.BeginVertical(GUI.skin.GetStyle("HelpBox"));
                        {
                            // Wall Run foldout
                            showWallRun = EditorGUILayout.Foldout(showWallRun, "WALL RUN", true);
                            if (showWallRun)
                            {
                                EditorGUI.indentLevel++;
                                EditorGUILayout.PropertyField(serializedObject.FindProperty("canWallRun"));
                                if (myScript.canWallRun)
                                {
                                    EditorGUI.indentLevel++;
                                    EditorGUILayout.LabelField("NEW FEATURE AVAILABLE UNDER ´CAMERA´ SETTINGS", EditorStyles.helpBox);
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("whatIsWallRunWall"));
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("useGravity"));
                                    if (myScript.useGravity)
                                    {
                                        EditorGUI.indentLevel++;
                                        EditorGUILayout.PropertyField(serializedObject.FindProperty("wallrunGravityCounterForce"));
                                        EditorGUI.indentLevel--;
                                    }
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("maxWallRunSpeed"));
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("normalWallJumpForce"));
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("upwardsWallJumpForce"));
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("stopWallRunningImpulse"));
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("wallMinimumHeight"));
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("wallrunCameraTiltAmount"));
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("cancelWallRunMethod"));
                                    if (myScript.cancelWallRunMethod == PlayerMovement.CancelWallRunMethod.Timer)
                                    {
                                        EditorGUI.indentLevel++;
                                        EditorGUILayout.PropertyField(serializedObject.FindProperty("wallRunDuration"));
                                        EditorGUI.indentLevel--;
                                    }

                                    EditorGUI.indentLevel--;
                                }
                                EditorGUI.indentLevel--;
                            }
                        }
                        EditorGUILayout.EndVertical();

                        EditorGUILayout.Space(5);
                        EditorGUILayout.BeginVertical(GUI.skin.GetStyle("HelpBox"));
                        {
                            // Wall Bounce foldout
                            showWallBounce = EditorGUILayout.Foldout(showWallBounce, "WALL BOUNCE", true);
                            if (showWallBounce)
                            {
                                EditorGUI.indentLevel++;
                                EditorGUILayout.PropertyField(serializedObject.FindProperty("canWallBounce"));
                                if (myScript.canWallBounce)
                                {
                                    EditorGUI.indentLevel++;
                                    if (myScript.maxJumps > 1) EditorGUILayout.LabelField("NEW FEATURE AVAILABLE UNDER `Jumping` SETTINGS ", EditorStyles.helpBox);
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("wallBounceForce"));
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("wallBounceUpwardsForce"));
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("oppositeWallDetectionDistance"));
                                    EditorGUI.indentLevel--;
                                }
                                EditorGUI.indentLevel--;
                            }
                        }
                        EditorGUILayout.EndVertical();

                        EditorGUILayout.Space(5);
                        EditorGUILayout.BeginVertical(GUI.skin.GetStyle("HelpBox"));
                        {
                            // Dashing foldout
                            showDashing = EditorGUILayout.Foldout(showDashing, "DASHING", true);
                            if (showDashing)
                            {
                                EditorGUI.indentLevel++;
                                if (myScript.canDash && !myScript.infiniteDashes) EditorGUILayout.LabelField("NEW FEATURE AVAILABLE UNDER ´ASSIGNABLES´ SETTINGS", EditorStyles.helpBox);
                                EditorGUILayout.PropertyField(serializedObject.FindProperty("canDash"));
                                if (myScript.canDash)
                                {
                                    EditorGUI.indentLevel++;
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("dashMethod"));
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("infiniteDashes"));
                                    if (!myScript.infiniteDashes)
                                    {
                                        EditorGUI.indentLevel++;
                                        EditorGUILayout.PropertyField(serializedObject.FindProperty("amountOfDashes"));
                                        EditorGUILayout.PropertyField(serializedObject.FindProperty("dashCooldown"));
                                        EditorGUI.indentLevel--;
                                    }
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("damageProtectionWhileDashing"));
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("dashForce"));
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("dashDuration"));
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("canShootWhileDashing"));
                                    EditorGUI.indentLevel--;
                                }

                                EditorGUI.indentLevel--;
                            }
                        }
                        EditorGUILayout.EndVertical();

                        EditorGUILayout.Space(5);
                        EditorGUILayout.BeginVertical(GUI.skin.GetStyle("HelpBox"));
                        {
                            // Grappling Hook foldout
                            showGrapplingHook = EditorGUILayout.Foldout(showGrapplingHook, "GRAPPLING HOOK", true);
                            if (showGrapplingHook)
                            {
                                EditorGUI.indentLevel++;
                                EditorGUILayout.PropertyField(serializedObject.FindProperty("allowGrapple"));
                                if (myScript.allowGrapple)
                                {
                                    EditorGUI.indentLevel++;
                                    EditorGUILayout.LabelField("NEW SOUNDS AVAILABLE UNDER ´Others´ SETTINGS", EditorStyles.helpBox);
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("maxGrappleDistance"));
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("grappleCooldown"));
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("distanceToBreakGrapple"));
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("grappleForce"));
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("grappleSpringForce"));
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("grappleDamper"));
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("drawDuration"));
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("ropeResolution"));
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("waveAmplitude"));
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("waveAmplitudeMitigation"));

                                    EditorGUI.indentLevel--;
                                }

                                EditorGUI.indentLevel--;
                            }
                        }
                        EditorGUILayout.EndVertical();


                        EditorGUILayout.Space(5);
                        EditorGUILayout.BeginVertical(GUI.skin.GetStyle("HelpBox"));
                        {
                            // Climbing foldout
                            showClimbing = EditorGUILayout.Foldout(showClimbing, "CLIMBING LADDERS", true);
                            if (showClimbing)
                            {
                                EditorGUI.indentLevel++;
                                EditorGUILayout.PropertyField(serializedObject.FindProperty("canClimb"));
                                if (myScript.canClimb)
                                {
                                    EditorGUI.indentLevel++;
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("maxLadderDetectionDistance"));
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("climbSpeed"));
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("topReachedUpperForce"));
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("topReachedForwardForce"));
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("allowVerticalLookWhileClimbing"));
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("hideWeaponWhileClimbing"));
                                    EditorGUI.indentLevel--;
                                }
                                EditorGUI.indentLevel--;
                            }
                        }
                        EditorGUILayout.EndVertical();
                        EditorGUI.indentLevel--;

                        break;

                    case "Others":
                        EditorGUILayout.LabelField("OTHERS", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField("FOOTSTEPS", EditorStyles.boldLabel);
                        GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(8) });
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("sounds"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("footstepVolume"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("footstepSpeed"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("footsteps"));
                        GUILayout.Space(5);
                        if (myScript.allowGrapple)
                        {
                            EditorGUI.indentLevel++;
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("grappleSounds"));
                            EditorGUI.indentLevel--;
                        }
                        GUILayout.Space(5);
                        EditorGUILayout.LabelField("EVENTS", EditorStyles.boldLabel);
                        GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(8) });
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("events"));
                        break;

                }
            }

            #endregion
            EditorGUILayout.Space(10f);
            serializedObject.ApplyModifiedProperties();

        }
    }
}
#endif