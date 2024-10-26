using UnityEngine;
using System.Collections.Generic;

public static class PolygonUtil
{
    private class PointWithVertex
    {
        public Vector2 Point;
        public Vector2 ClosestVertex;

        public PointWithVertex(Vector2 point, Vector2 closestVertex)
        {
            Point = point;
            ClosestVertex = closestVertex;
        }
    }

    public static List<Vector2> GetDynamicRandomPointsInPolygon(Vector2[] polygon, int numberOfPoints, int smoothingIterations = 100)
    {
        List<PointWithVertex> points = new List<PointWithVertex>();

        // Step 1: Get the center of the polygon
        Vector2 center = GetPolygonCenter(polygon);

        // Step 2: Find the furthest angle that maximizes distance from the center
        float maxDistanceAngle = FindFurthestAngleFromCenter(center, polygon);

        // Step 3: Place the first point at this angle and the rest relative to it, and associate with the closest vertex
        float angleIncrement = 360f / numberOfPoints;
        for (int i = 0; i < numberOfPoints; i++)
        {
            float angle = (maxDistanceAngle + i * angleIncrement) * Mathf.Deg2Rad;
            Vector2 newPoint = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * 0.1f;

            // Move outward until the point is valid
            float distance = 0.1f;
            while (MapDataUtil.IsInPolygon(polygon, newPoint))
            {
                distance += 0.1f;
                newPoint = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * distance;
            }

            // Step 4: Find the closest vertex instead of the closest edge
            Vector2 closestVertex = FindClosestVertex(newPoint, polygon);

            // Store the point and its closest vertex
            points.Add(new PointWithVertex(newPoint, closestVertex));
        }

        // Step 5: Smoothing process
        for (int iteration = 0; iteration < smoothingIterations; iteration++)
        {
            for (int i = 0; i < points.Count; i++)
            {
                MovePointTowardsBestPosition(points, polygon, i);
            }
        }

        // Convert the list of PointWithVertex to a list of Vector2
        List<Vector2> resultPoints = new List<Vector2>();
        foreach (var pointWithVertex in points)
        {
            resultPoints.Add(pointWithVertex.Point);
        }

        return resultPoints;
    }

    // Finds the vertex (corner) of the polygon closest to the given point
    private static Vector2 FindClosestVertex(Vector2 point, Vector2[] polygon)
    {
        Vector2 closestVertex = Vector2.zero;
        float minDistance = float.MaxValue;

        foreach (var vertex in polygon)
        {
            float distance = Vector2.Distance(point, vertex);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestVertex = vertex;
            }
        }

        return closestVertex;
    }

    // Move a point towards the best position balancing between closest neighbors and its associated vertex
    private static void MovePointTowardsBestPosition(List<PointWithVertex> points, Vector2[] polygon, int pointIndex)
    {
        var currentPointWithVertex = points[pointIndex];
        Vector2 currentPoint = currentPointWithVertex.Point;

        // Find the two closest points
        (Vector2 closest1, Vector2 closest2) = FindClosestPoints(points, currentPoint, pointIndex);

        // Calculate the target position based on the midpoint between closest points and the associated vertex
        Vector2 targetPosition = CalculateTargetPosition(currentPoint, closest1, closest2, currentPointWithVertex.ClosestVertex);

        // Move slightly towards the target position
        float movementStep = 0.1f;
        Vector2 newPointPosition = Vector2.MoveTowards(currentPoint, targetPosition, movementStep);

        // Ensure the new position is within the polygon
        if (MapDataUtil.IsInPolygon(polygon, newPointPosition))
        {
            currentPointWithVertex.Point = newPointPosition;
        }
    }

    // Finds the two closest points to the current point, excluding itself
    private static (Vector2, Vector2) FindClosestPoints(List<PointWithVertex> points, Vector2 currentPoint, int excludeIndex)
    {
        Vector2 closest1 = Vector2.zero;
        Vector2 closest2 = Vector2.zero;
        float closestDistance1 = float.MaxValue;
        float closestDistance2 = float.MaxValue;

        for (int i = 0; i < points.Count; i++)
        {
            if (i == excludeIndex) continue;

            float distance = Vector2.Distance(currentPoint, points[i].Point);
            if (distance < closestDistance1)
            {
                closestDistance2 = closestDistance1;
                closest2 = closest1;

                closestDistance1 = distance;
                closest1 = points[i].Point;
            }
            else if (distance < closestDistance2)
            {
                closestDistance2 = distance;
                closest2 = points[i].Point;
            }
        }

        return (closest1, closest2);
    }

    // Calculates the target position based on the midpoint of the two closest points and the associated vertex
    private static Vector2 CalculateTargetPosition(Vector2 currentPoint, Vector2 closest1, Vector2 closest2, Vector2 vertex)
    {
        // Midpoint between the two closest points
        Vector2 midpoint = (closest1 + closest2) / 2;

        // Calculate the target position balancing between the midpoint and the vertex
        Vector2 targetPosition = (midpoint + vertex) / 2;

        return targetPosition;
    }

    // Finds the angle where the distance from the center to the polygon edge is maximized
    private static float FindFurthestAngleFromCenter(Vector2 center, Vector2[] polygon)
    {
        float maxDistance = 0f;
        float maxDistanceAngle = 0f;

        int samples = 360;
        for (int i = 0; i < samples; i++)
        {
            float angle = i * Mathf.Deg2Rad;
            Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            Vector2 testPoint = center + direction * 0.1f;

            float distance = 0f;
            while (MapDataUtil.IsInPolygon(polygon, testPoint))
            {
                distance += 0.1f;
                testPoint = center + direction * distance;
            }

            if (distance > maxDistance)
            {
                maxDistance = distance;
                maxDistanceAngle = i;
            }
        }

        return maxDistanceAngle;
    }

    // Get the center of the polygon by averaging its vertices
    private static Vector2 GetPolygonCenter(Vector2[] polygon)
    {
        Vector2 center = Vector2.zero;
        foreach (var point in polygon)
        {
            center += point;
        }
        return center / polygon.Length;
    }
}
