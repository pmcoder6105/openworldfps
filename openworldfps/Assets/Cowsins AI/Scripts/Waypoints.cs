using System.Collections.Generic;
using UnityEngine;

public class Waypoints : MonoBehaviour
{
    [Range(0f, 2f)]
    [SerializeField] private float waypointSize = 1f;

    [Header("Path Settings")]
    [SerializeField] private bool canLoop = true;

    public List<Transform> waypoints = new List<Transform>(2);
    private void OnValidate()
    {
        if (waypoints.Count < 2)
        {
            Transform waypoint1 = new GameObject("1").transform;
            Transform waypoint2 = new GameObject("2").transform;

            waypoints.Add(waypoint1);
            waypoints.Add(waypoint2);

            var go = gameObject;
            waypoint1.transform.parent = go.transform;
            waypoint2.transform.parent = go.transform;
        }
    }

    private void OnDrawGizmos()
    {
        foreach (var waypoint in waypoints)
        {
            if (waypoint.gameObject == null)
            {
                waypoints.RemoveAt(waypoints.IndexOf(waypoint));
            }
        }

        foreach (Transform t in waypoints)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(t.position, waypointSize);
        }

        Gizmos.color = Color.red;
        for (int i = 0; i < waypoints.Count - 1; i++)
        {
            Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
        }

        if (canLoop)
        {
            if (waypoints.Count == 0)
            {
                return;
            }
            else
            {
                Gizmos.DrawLine(transform.GetChild(transform.childCount - 1).position, transform.GetChild(0).position);
            }
        }
    }
}