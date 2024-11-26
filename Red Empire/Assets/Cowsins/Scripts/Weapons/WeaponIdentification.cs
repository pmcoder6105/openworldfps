/// <summary>
/// This script belongs to cowsins� as a part of the cowsins� FPS Engine. All rights reserved. 
/// </summary>
using UnityEngine;
using UnityEngine.Animations;
using System.Collections.Generic;
using UnityEditor;
namespace cowsins
{
    [System.Serializable]
    public class DefaultAttachment
    {
        public Attachment defaultBarrel,
            defaultScope,
            defaultStock,
            defaultGrip,
            defaultMagazine,
            defaultFlashlight,
            defaultLaser;
    }
    /// <summary>
    /// Attach this to your weapon object ( the one that goes in the weapon array of WeaponController )
    /// </summary>
    public class WeaponIdentification : MonoBehaviour
    {
        public Weapon_SO weapon;

        [Tooltip("Every weapon, excluding melee, must have a firePoint, which is the point where the bullet comes from." +
            "Just make an empty object, call it firePoint for organization purposes and attach it here. ")]
        public Transform[] FirePoint;

        public Transform aimPoint;

        [HideInInspector] public int totalMagazines, magazineSize, bulletsLeftInMagazine, totalBullets; // Internal use

        [Tooltip("Defines the default attachments for your weapon. The first time you pick it up, these attachments will be equipped.")] public DefaultAttachment defaultAttachments;

        [HideInInspector]
        public Attachment barrel,
            scope,
            stock,
            grip,
            magazine,
            flashlight,
            laser;


        [Tooltip("Defines all the attachments that can be equipped on your weapon.")] public CompatibleAttachments compatibleAttachments;

        [HideInInspector] public Vector3 originalAimPointPos, originalAimPointRot;

        [HideInInspector] public ParentConstraint constraint;

        [HideInInspector] public float heatRatio;

        private void OnEnable()
        {
            originalAimPointPos = aimPoint.localPosition;
            originalAimPointRot = aimPoint.localRotation.eulerAngles;
        }
        private void Start()
        {
            totalMagazines = weapon.totalMagazines;
            GetMagazineSize();
            GetComponentInChildren<Animator>().keepAnimatorStateOnDisable = true;
        }

        public void GetMagazineSize()
        {
            if (magazine == null) magazineSize = weapon.magazineSize;
            else magazineSize = weapon.magazineSize + magazine.GetComponent<Magazine>().magazineCapacityAdded;

            if (bulletsLeftInMagazine > magazineSize) bulletsLeftInMagazine = magazineSize;
        }

        public void SetConstraint(Transform obj)
        {

            ConstraintSource newConstraintSource = new ConstraintSource();

            newConstraintSource.sourceTransform = obj;

            if (constraint == null)
                constraint = GetComponentInChildren<ParentConstraint>();
            constraint.AddSource(newConstraintSource);
            constraint.SetSource(0, newConstraintSource);
        }

        public void ResetConstraint()
        {

            ConstraintSource newConstraintSource = new ConstraintSource();

            newConstraintSource.sourceTransform = aimPoint;

            if (constraint == null)
                constraint = GetComponentInChildren<ParentConstraint>();

            constraint.SetSource(0, newConstraintSource);
        }

        public (List<AttachmentIdentifier_SO>, int) GetDefaultAttachments()
        {
            List<AttachmentIdentifier_SO> attachments = new List<AttachmentIdentifier_SO>();

            if (defaultAttachments.defaultBarrel != null)
                attachments.Add(defaultAttachments.defaultBarrel.attachmentIdentifier);
            if (defaultAttachments.defaultScope != null)
                attachments.Add(defaultAttachments.defaultScope.attachmentIdentifier);
            if (defaultAttachments.defaultStock != null)
                attachments.Add(defaultAttachments.defaultStock.attachmentIdentifier);
            if (defaultAttachments.defaultGrip != null)
                attachments.Add(defaultAttachments.defaultGrip.attachmentIdentifier);
            if (defaultAttachments.defaultMagazine != null)
                attachments.Add(defaultAttachments.defaultMagazine.attachmentIdentifier);
            if (defaultAttachments.defaultFlashlight != null)
                attachments.Add(defaultAttachments.defaultFlashlight.attachmentIdentifier);
            if (defaultAttachments.defaultLaser != null)
                attachments.Add(defaultAttachments.defaultLaser.attachmentIdentifier);

            int magCapacityAdded = 0;
            if (defaultAttachments.defaultMagazine is Magazine magazine)
            {
                magCapacityAdded = magazine.magazineCapacityAdded;
            }

            return (attachments, magCapacityAdded);
        }
#if UNITY_EDITOR

        #region Gizmos

        // Draws additional Weapon Information on the editor view
        Vector3 boxSize = new Vector3(0.1841836f, 0.14f, 0.54f);
        Vector3 boxPosition = new Vector3(0,-.2f,.6f);

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying)
            {
                Gizmos.color = new Color(1, 0, 0, 0.3f);

                Gizmos.DrawWireCube(transform.position + boxPosition, boxSize);
                Handles.Label(transform.position + boxPosition + Vector3.up * (boxSize.y / 2 + 0.1f), "Approximate Weapon Location");

                if(aimPoint)
                {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawWireCube(aimPoint.position , Vector3.one * .02f);
                    Handles.Label(aimPoint.position + Vector3.up * .05f, "Aim Point");

                }

                for(int i = 0; i < FirePoint.Length; i++)
                {
                    if (FirePoint[i] != null)
                    {
                        Gizmos.color = Color.green;
                        Gizmos.DrawWireCube(FirePoint[i].position, Vector3.one * .02f);
                        Handles.Label(FirePoint[i].position + Vector3.up * .05f, "Fire Point " + (i + 1));

                    }
                }
            }
        }
        #endregion

#endif
    }
#if UNITY_EDITOR


    [CustomEditor(typeof(WeaponIdentification))]
    public class WeaponIdentificationInspector : Editor
    {

        private string[] tabs = { "Basic", "Attachments" };
        private int currentTab = 0;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            Texture2D myTexture = Resources.Load<Texture2D>("CustomEditor/weaponIdentification_CustomEditor") as Texture2D;
            GUILayout.Label(myTexture);


            currentTab = GUILayout.Toolbar(currentTab, tabs);

            if (currentTab >= 0 || currentTab < tabs.Length)
            {
                switch (tabs[currentTab])
                {
                    case "Basic":
                        EditorGUILayout.Space(20f);
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("weapon"));

                        EditorGUILayout.PropertyField(serializedObject.FindProperty("FirePoint"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("aimPoint"));
                        break;
                    case "Attachments":
                        EditorGUILayout.Space(5f);
                        if (GUILayout.Button("Attachments Tutorial", GUILayout.Height(20)))
                        {
                            Application.OpenURL("https://youtu.be/Q1saDyb4eDI");
                        }
                        EditorGUILayout.Space(20f);
                        GUILayout.Label("If you aren't using attachments on this particular weapon, make sure these references are null.", EditorStyles.wordWrappedLabel);
                        EditorGUILayout.Space(20f);
                        EditorGUILayout.LabelField("Assign the original or default attachments that your weapon is meant to have, even when removing other attachments. This could include items like iron sights or standard magazines.", EditorStyles.helpBox);
                        EditorGUILayout.Space(5f);
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("defaultAttachments"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("compatibleAttachments"));
                        break;
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }

#endif
}
