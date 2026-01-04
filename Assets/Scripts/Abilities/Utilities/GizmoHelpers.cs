using UnityEngine;

namespace TidesEnd.Abilities
{
    /// <summary>
    /// Static utility class for drawing debug gizmos for abilities.
    /// Provides helper methods for common shapes used in ability visualization.
    /// </summary>
    public static class GizmoHelpers
    {
        /// <summary>
        /// Draw a wire circle on the XZ plane (ground plane).
        /// </summary>
        public static void DrawWireCircle(Vector3 center, float radius, Color color, int segments = 32)
        {

            Gizmos.color = color;

            float angleStep = 360f / segments;
            Vector3 prevPoint = center + new Vector3(radius, 0, 0);

            for (int i = 1; i <= segments; i++)
            {
                float angle = angleStep * i * Mathf.Deg2Rad;
                Vector3 newPoint = center + new Vector3(
                    Mathf.Cos(angle) * radius,
                    0,
                    Mathf.Sin(angle) * radius
                );

                Gizmos.DrawLine(prevPoint, newPoint);
                prevPoint = newPoint;
            }

        }

        /// <summary>
        /// Draw a filled circle on the XZ plane using lines.
        /// </summary>
        public static void DrawFilledCircle(Vector3 center, float radius, Color color, int segments = 32)
        {
            Gizmos.color = color;

            float angleStep = 360f / segments;

            for (int i = 0; i < segments; i++)
            {
                float angle1 = angleStep * i * Mathf.Deg2Rad;
                float angle2 = angleStep * (i + 1) * Mathf.Deg2Rad;

                Vector3 point1 = center + new Vector3(
                    Mathf.Cos(angle1) * radius,
                    0,
                    Mathf.Sin(angle1) * radius
                );

                Vector3 point2 = center + new Vector3(
                    Mathf.Cos(angle2) * radius,
                    0,
                    Mathf.Sin(angle2) * radius
                );

                // Draw line from center to edge and edge to next point
                Gizmos.DrawLine(center, point1);
                Gizmos.DrawLine(point1, point2);
            }

 
        }

        /// <summary>
        /// Draw a wire arc on the XZ plane.
        /// </summary>
        /// <param name="center">Center point of the arc</param>
        /// <param name="radius">Radius of the arc</param>
        /// <param name="direction">Forward direction (0 degrees)</param>
        /// <param name="arcAngle">Total angle of the arc in degrees</param>
        /// <param name="color">Color to draw</param>
        /// <param name="segments">Number of line segments</param>
        public static void DrawWireArc(Vector3 center, float radius, Vector3 direction, float arcAngle, Color color, int segments = 32)
        {

            Gizmos.color = color;

            // Normalize direction and project to XZ plane
            direction.y = 0;
            direction.Normalize();

            // Calculate start angle from direction
            float startAngle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;
            float halfArc = arcAngle / 2f;

            // Draw arc
            float angleStep = arcAngle / segments;
            Vector3 prevPoint = GetArcPoint(center, radius, startAngle - halfArc);

            for (int i = 1; i <= segments; i++)
            {
                float angle = startAngle - halfArc + (angleStep * i);
                Vector3 newPoint = GetArcPoint(center, radius, angle);
                Gizmos.DrawLine(prevPoint, newPoint);
                prevPoint = newPoint;
            }

            // Draw lines from center to arc endpoints
            Gizmos.DrawLine(center, GetArcPoint(center, radius, startAngle - halfArc));
            Gizmos.DrawLine(center, GetArcPoint(center, radius, startAngle + halfArc));

 
        }

        /// <summary>
        /// Draw a wire cylinder (useful for area effects with height).
        /// </summary>
        public static void DrawWireCylinder(Vector3 center, float radius, float height, Color color, int segments = 32)
        {

            Gizmos.color = color;

            Vector3 topCenter = center + Vector3.up * height;

            // Draw top and bottom circles
            DrawWireCircle(center, radius, color, segments);
            DrawWireCircle(topCenter, radius, color, segments);

            // Draw vertical lines connecting top and bottom
            float angleStep = 360f / 4; // 4 vertical lines
            for (int i = 0; i < 4; i++)
            {
                float angle = angleStep * i * Mathf.Deg2Rad;
                Vector3 offset = new Vector3(
                    Mathf.Cos(angle) * radius,
                    0,
                    Mathf.Sin(angle) * radius
                );

                Gizmos.DrawLine(center + offset, topCenter + offset);
            }


        }

        /// <summary>
        /// Draw a wire sphere (for target selection, blast radius).
        /// </summary>
        public static void DrawWireSphere(Vector3 center, float radius, Color color)
        {
        
            Gizmos.color = color;
            Gizmos.DrawWireSphere(center, radius);
       
        }

        /// <summary>
        /// Draw a directional arrow (for projectile direction, knockback, etc.).
        /// </summary>
        public static void DrawArrow(Vector3 start, Vector3 direction, float length, Color color, float arrowheadSize = 0.25f)
        {
     
            Gizmos.color = color;

            Vector3 end = start + direction.normalized * length;

            // Draw main line
            Gizmos.DrawLine(start, end);

            // Draw arrowhead
            Vector3 right = Vector3.Cross(direction, Vector3.up).normalized;
            Vector3 up = Vector3.Cross(direction, right).normalized;

            if (right == Vector3.zero)
            {
                right = Vector3.right;
                up = Vector3.Cross(direction, right).normalized;
            }

            float arrowheadLength = length * arrowheadSize;
            Vector3 arrowBase = end - direction.normalized * arrowheadLength;

            // Draw 4 arrowhead lines
            Gizmos.DrawLine(end, arrowBase + right * arrowheadLength * 0.5f);
            Gizmos.DrawLine(end, arrowBase - right * arrowheadLength * 0.5f);
            Gizmos.DrawLine(end, arrowBase + up * arrowheadLength * 0.5f);
            Gizmos.DrawLine(end, arrowBase - up * arrowheadLength * 0.5f);

 
        }

        /// <summary>
        /// Draw a rectangular box (for zone effects).
        /// </summary>
        public static void DrawWireBox(Vector3 center, Vector3 size, Color color)
        {
            Gizmos.color = color;
            Gizmos.DrawWireCube(center, size);
        }

        /// <summary>
        /// Draw a projectile trajectory arc (parabolic path).
        /// </summary>
        public static void DrawTrajectory(Vector3 start, Vector3 velocity, float duration, Color color, int segments = 20)
        {

            Gizmos.color = color;

            float timeStep = duration / segments;
            Vector3 prevPoint = start;

            for (int i = 1; i <= segments; i++)
            {
                float time = timeStep * i;
                Vector3 newPoint = start + velocity * time + Physics.gravity * (0.5f * time * time);
                Gizmos.DrawLine(prevPoint, newPoint);
                prevPoint = newPoint;
            }

 
        }

        /// <summary>
        /// Helper method to get a point on a circle at a given angle.
        /// </summary>
        private static Vector3 GetArcPoint(Vector3 center, float radius, float angleDegrees)
        {
            float angleRad = angleDegrees * Mathf.Deg2Rad;
            return center + new Vector3(
                Mathf.Cos(angleRad) * radius,
                0,
                Mathf.Sin(angleRad) * radius
            );
        }

        /// <summary>
        /// Draw a cone for cone-shaped abilities (useful for directional AOE).
        /// </summary>
        public static void DrawWireCone(Vector3 origin, Vector3 direction, float range, float angle, Color color, int segments = 16)
        {

            Gizmos.color = color;

            // Normalize direction and project to XZ plane
            direction.y = 0;
            direction.Normalize();

            float radius = Mathf.Tan(angle * 0.5f * Mathf.Deg2Rad) * range;
            Vector3 endCenter = origin + direction * range;

            // Draw cone circle at the end
            DrawWireCircle(endCenter, radius, color, segments);

            // Draw lines from origin to circle perimeter
            float angleStep = 360f / segments;
            float startAngle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;

            for (int i = 0; i < segments; i += segments / 4) // Draw 4 lines to circle edge
            {
                float currentAngle = (startAngle + angleStep * i) * Mathf.Deg2Rad;
                Vector3 circlePoint = endCenter + new Vector3(
                    Mathf.Cos(currentAngle) * radius,
                    0,
                    Mathf.Sin(currentAngle) * radius
                );
                Gizmos.DrawLine(origin, circlePoint);
            }

        }
    }
}
