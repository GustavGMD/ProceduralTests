using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour {

    public MeshFilter meshTarget;

	// Use this for initialization
	void Start () {
        meshTarget.mesh = GenerateNewMesh();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public Mesh GenerateNewMesh()
    {
        Mesh newMesh = new Mesh();
        List<Vector3> newVertices = new List<Vector3>();
        List<int> newTriangles = new List<int>();
        List<Vector3> newNormals = new List<Vector3>();

        //temp setup parameters for testing
        int width = 10;
        int height = 10;
        int trianglesNum = (width - 1) * (height - 1) * 2;

        #region Vertices
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                newVertices.Add(new Vector3(j, i, 0));
            }
        }
        #endregion

        #region Triangles 
        //adding counter-clockwise for the faces to face the right direction       
        for (int i = 0; i < height-1; i++)
        {
            for (int j = 0; j < width-1; j++)
            {
                //first triangle
                newTriangles.Add(((i) * width) + (j));
                newTriangles.Add(((i + 1) * width) + (j));
                newTriangles.Add(((i) * width) + (j + 1));

                //second triangle
                newTriangles.Add(((i) * width) + (j + 1));
                newTriangles.Add(((i + 1) * width) + (j));
                newTriangles.Add(((i + 1) * width) + (j + 1));
            }
        }
        #endregion

        #region Normals
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                Vector3 __tempNormal;
                if (i < height - 1)
                {
                    if (j < width - 1)
                    {
                        __tempNormal = Vector3.Cross(
                            newVertices[((i) * width) + (j)]            -   newVertices[((i + 1) * width) + (j)], 
                            newVertices[((i) * width) + (j + 1)]        -   newVertices[((i + 1) * width) + (j)]
                            ).normalized;
                    }
                    else
                    {
                        __tempNormal = Vector3.Cross(
                            newVertices[((i) * width) + (j)]            -   newVertices[((i + 1) * width) + (j - 1)], 
                            newVertices[((i + 1) * width) + (j)]        -   newVertices[((i + 1) * width) + (j - 1)]
                            ).normalized;
                    }
                }
                else
                {
                    if (j < width - 1)
                    {
                        __tempNormal = Vector3.Cross(
                            newVertices[((i - 1) * width) + (j)]        -   newVertices[((i) * width) + (j)],
                            newVertices[((i) * width) + (j + 1)]        -   newVertices[((i) * width) + (j)]
                            ).normalized;
                    }
                    else
                    {
                        __tempNormal = Vector3.Cross(
                            newVertices[((i - 1) * width) + (j)]        -   newVertices[((i) * width) + (j - 1)],
                            newVertices[((i) * width) + (j)]            -   newVertices[((i) * width) + (j - 1)]
                            ).normalized;
                    }
                }
                newNormals.Add(__tempNormal);          
            }
        }
        #endregion

        newMesh.vertices = newVertices.ToArray();
        newMesh.triangles = newTriangles.ToArray();
        newMesh.normals = newNormals.ToArray();

        return newMesh;
    }
}
