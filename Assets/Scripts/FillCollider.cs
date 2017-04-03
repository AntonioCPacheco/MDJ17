using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[ExecuteInEditMode]
public class FillCollider : MonoBehaviour
{
    PolygonCollider2D pc2;
    void Update()
    {
        pc2 = gameObject.GetComponent<PolygonCollider2D>();
        if (pc2.pathCount > 1)
        {
            throw new Exception("Why does this collider have 2 paths? Source: " + this.gameObject.name);
        }
        Vector2[] pointsAux = pc2.points;
        for (int i = 0; i < pointsAux.Length; i++)
        {
            pc2.points[pointsAux.Length - 1 - i] = pointsAux[i];
        }
        //Render thing
        int pointCount = 0;
        pointCount = pc2.GetTotalPointCount();
        MeshFilter mf = GetComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        Vector2[] points = pc2.points;
        Vector3[] vertices = new Vector3[pointCount];
        Vector2[] uv = new Vector2[pointCount];
        for (int j = 0; j < pointCount; j++)
        {
            Vector2 actual = points[j];
            vertices[j] = new Vector3(actual.x, actual.y, 0);
            uv[j] = actual;
        }
        Triangulator tr = new Triangulator(points);
        int[] triangles = tr.Triangulate();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;
        mf.mesh = mesh;
        //Render thing
        
    }
}