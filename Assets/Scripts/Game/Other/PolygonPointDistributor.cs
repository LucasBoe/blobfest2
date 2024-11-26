using System;
using System.Collections;
using AmplifyShaderEditor;
using Delaunay.Geo;
using NaughtyAttributes;
using Newtonsoft.Json.Linq;
using Unity.VisualScripting;
using UnityEngine;

[ExecuteAlways]
public class PolygonPointDistributor : MonoBehaviour
{
    [SerializeField] private Vector2[] polygon, draw, lerped;
    [SerializeField, Range(0, 8)] int subdivisions;
    [SerializeField, Range(1, 12)] int pointsToGenerate;

    private Vector2 center;

    [SerializeField] Transform toScore;
    private void Update()
    {
        polygon = CreatePolygonFromChilds();
        NewPolygonUtil.CalculateCenterPoint(polygon);

        var subdivided = NewPolygonUtil.SubdivideUntilMinPointCount(polygon, pointsToGenerate);
        draw = NewPolygonUtil.Retrace(subdivided);
        lerped = NewPolygonUtil.LerpToCenter(draw, center);
    }

    private Vector2[] CreatePolygonFromChilds()
    {
        Vector2[] childs = new Vector2[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
            childs[i] = transform.GetChild(i).position;
        
        return childs;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        for (int a = 0; a < polygon.Length; a++)
        {
            int b = (a == 0 ? polygon.Length : a) - 1;
            Gizmos.DrawLine(polygon[a], polygon[b]);
        }

        Gizmos.DrawWireSphere(center, .05f);



        //Vector2[] poly = polygon;
        //
        //while (poly.Length < pointsToGenerate * 3)
        //{
        //    poly = Subdivide(poly);
        //}

        foreach (var point in draw)
        {
            Gizmos.DrawLine(center, point);
        }

        int numberOfPoints = pointsToGenerate;

        Vector2[] pois = NewPolygonUtil.CalculateDynamicPointsInPolygon(polygon, numberOfPoints);

        Gizmos.color = Color.red;
        for (int i = 0; i < numberOfPoints; i++)
        {
            Gizmos.DrawWireSphere(pois[i], .1f);
        }
    }
}