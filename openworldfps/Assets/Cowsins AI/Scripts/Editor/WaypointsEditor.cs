using System;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

[CustomEditor(typeof(Waypoints))]
public class WaypointsEditor : Editor
{
    private GameObject waypointObj;
    
    override public void OnInspectorGUI()
    {
        serializedObject.Update();
        Waypoints waypoints = target as Waypoints;
        
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Waypoints Editor", EditorStyles.whiteLargeLabel);
        EditorGUILayout.Space(10);
        EditorGUILayout.EndVertical();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("waypointSize"));
        EditorGUILayout.Space(5);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("canLoop"));
        EditorGUILayout.Space(5);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("waypoints"));
        EditorGUILayout.Space(15);

        if (GUILayout.Button("Add Waypoint"))
        {
            waypointObj = new GameObject("Waypoint");
            
            waypointObj.transform.SetParent(waypoints.transform);
            waypoints.waypoints.Add(waypointObj.transform);
        }
        
        serializedObject.ApplyModifiedProperties();
    }
}
#endif