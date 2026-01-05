using UnityEngine;

namespace Game.PizzeriaSimulator.Customers.WaypointField
{
    class StepByStepWaypointsField : CustomersWaypointFieldBase
    {
        [SerializeField] Transform[] waypoints;
        int currentWayPoint;
        public override Transform GetWayPoint()
        {
            if (waypoints.Length < 1)
            {
                UnityEngine.Debug.LogError("Waypoints list is empty");
                return transform;
            }
            if (++currentWayPoint > waypoints.Length - 1) currentWayPoint = 0;
            return waypoints[currentWayPoint];
        }
    }
}
