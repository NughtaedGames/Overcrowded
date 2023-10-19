using UnityEngine;
using UnityEditor;

namespace WS20.P3.Overcrowded
{
    [InitializeOnLoad()]
    public class WaypointEditor
    {
        #region Public Fields

        public float show;

        #endregion

        #region MonoBehaviour CallBacks

        [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected | GizmoType.Pickable)]
        public static void OnDrawSceneGizmo(Waypoint waypoint, GizmoType gizmoType)
        {
            if ((gizmoType & GizmoType.Selected) != 0)
            {
                Gizmos.color = Color.yellow;
                //Draw Radius
                Gizmos.DrawWireSphere(waypoint.transform.position, waypoint.radius);
            }
            else
            {
                Gizmos.color = Color.yellow * 0.7f;
            }

            if (waypoint.showGizmoAlways)
            {
                //Draw Radius for all
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(waypoint.transform.position, waypoint.radius);
            }

            Gizmos.DrawSphere(waypoint.transform.position, .1f);
            Gizmos.color = Color.white;
            
            Gizmos.DrawLine(waypoint.transform.position + (waypoint.transform.right * waypoint.width / 2f), 
                waypoint.transform.position - (waypoint.transform.right * waypoint.width / 2f));

            if (waypoint.previousWaypoint != null)
            {
                Gizmos.color = Color.red;
                Vector3 offset = waypoint.transform.right * waypoint.width / 2f;
                Vector3 offsetTo = waypoint.previousWaypoint.transform.right * waypoint.previousWaypoint.width / 2f;

                Gizmos.DrawLine(waypoint.transform.position + offset,
                    waypoint.previousWaypoint.transform.position + offsetTo);
            }

            if (waypoint.nextWaypoint != null)
            {
                Gizmos.color = Color.green;
                Vector3 offset = waypoint.transform.right * -waypoint.width / 2f;
                Vector3 offsetTo = waypoint.nextWaypoint.transform.right * -waypoint.nextWaypoint.width / 2f;

                Gizmos.DrawLine(waypoint.transform.position + offset,
                    waypoint.nextWaypoint.transform.position + offsetTo);
            }

            if (waypoint.branches != null)
            {
                foreach (Waypoint branch in waypoint.branches)
                {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawLine(waypoint.transform.position, branch.transform.position);
                }
            }
        }
        
        #endregion
    }
}