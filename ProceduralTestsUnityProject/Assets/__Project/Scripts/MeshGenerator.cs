using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{

    public MeshFilter meshTarget;

    //bezier public test curve variables
    public bool enableCurveTesting = false;
    public int samplingRate = 100;
    public GameObject[] controlPoints;

    // Use this for initialization
    void Start()
    {
        //meshTarget.mesh = GenerateNewMesh();
        Vector3[] __controlPoints = new Vector3[controlPoints.Length];
        for (int i = 0; i < __controlPoints.Length; i++)
        {
            __controlPoints[i] = controlPoints[i].transform.position;
        }
        meshTarget.mesh = GenerateEvenlySpacedCurveCenteredBasedRectangularMesh(__controlPoints, 30);
    }

    // Update is called once per frame
    void Update()
    {
        if (enableCurveTesting)
        {
            //bezier curve tests
            Vector3[] __controlPoints = new Vector3[controlPoints.Length];
            for (int i = 0; i < __controlPoints.Length; i++)
            {
                __controlPoints[i] = controlPoints[i].transform.position;
            }
            Vector3[] __points = new Vector3[samplingRate];
            for (int i = 0; i < samplingRate; i++)
            {
                __points[i] = GetBezierCurvePoint(__controlPoints, (float)i / samplingRate);
            }
            GetComponent<LineRenderer>().numPositions = samplingRate;
            GetComponent<LineRenderer>().SetPositions(__points);

            meshTarget.mesh = GenerateEvenlySpacedCurveCenteredBasedRectangularMesh(__controlPoints, 30);
        }
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
                newVertices.Add(new Vector3(j - ((float)(width - 1) / 2), 0, i));
            }
        }
        #endregion

        #region Triangles 
        //adding counter-clockwise for the faces to face the right direction       
        for (int i = 0; i < height - 1; i++)
        {
            for (int j = 0; j < width - 1; j++)
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
                            newVertices[((i) * width) + (j + 1)] - newVertices[((i + 1) * width) + (j)],
                            newVertices[((i) * width) + (j)] - newVertices[((i + 1) * width) + (j)]
                            ).normalized;
                    }
                    else
                    {
                        __tempNormal = Vector3.Cross(
                            newVertices[((i + 1) * width) + (j)] - newVertices[((i + 1) * width) + (j - 1)],
                            newVertices[((i) * width) + (j)] - newVertices[((i + 1) * width) + (j - 1)]
                            ).normalized;
                    }
                }
                else
                {
                    if (j < width - 1)
                    {
                        __tempNormal = Vector3.Cross(
                            newVertices[((i) * width) + (j + 1)] - newVertices[((i) * width) + (j)],
                            newVertices[((i - 1) * width) + (j)] - newVertices[((i) * width) + (j)]

                            ).normalized;
                    }
                    else
                    {
                        __tempNormal = Vector3.Cross(
                            newVertices[((i) * width) + (j)] - newVertices[((i) * width) + (j - 1)],
                            newVertices[((i - 1) * width) + (j)] - newVertices[((i) * width) + (j - 1)]
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

    public Mesh GenerateCurveCenteredBasedRectangularMesh(Vector3[] p_curveControlPoints)
    {
        Mesh newMesh = new Mesh();
        List<Vector3> newVertices = new List<Vector3>();
        List<int> newTriangles = new List<int>();
        List<Vector3> newNormals = new List<Vector3>();

        //temp setup parameters for testing
        int width = 11;
        int height = 22;
        int trianglesNum = (width - 1) * (height - 1) * 2;
        //one extra point to calculate the angle for the last point we are using
        Vector3[] curvePoints = new Vector3[height + 1];

        #region Initialize Curve Points
        for (int i = 0; i < curvePoints.Length; i++)
        {
            curvePoints[i] = GetBezierCurvePoint(p_curveControlPoints, (float)i / (curvePoints.Length - 1));
        }
        #endregion

        #region Vertices
        for (int i = 0; i < height; i++)
        {
            Vector3 __direction = (curvePoints[i + 1] - curvePoints[i]);
            float __angle = Mathf.Atan2(__direction.z, __direction.x) - (Mathf.PI / 2); //rad
            Debug.Log(__angle * Mathf.Rad2Deg);
            //j is the position in the X axis, while i is the position in the Z axis
            for (int j = 0; j < width; j++)
            {
                float __radius = j - Mathf.Ceil((float)width / 2);
                newVertices.Add(new Vector3(curvePoints[i].x + (Mathf.Cos(__angle) * __radius), 0, curvePoints[i].z + (Mathf.Sin(__angle) * __radius)));
            }
        }
        #endregion

        #region Triangles 
        //adding counter-clockwise for the faces to face the right direction       
        for (int i = 0; i < height - 1; i++)
        {
            for (int j = 0; j < width - 1; j++)
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
                            newVertices[((i) * width) + (j + 1)] - newVertices[((i + 1) * width) + (j)],
                            newVertices[((i) * width) + (j)] - newVertices[((i + 1) * width) + (j)]
                            ).normalized;
                    }
                    else
                    {
                        __tempNormal = Vector3.Cross(
                            newVertices[((i + 1) * width) + (j)] - newVertices[((i + 1) * width) + (j - 1)],
                            newVertices[((i) * width) + (j)] - newVertices[((i + 1) * width) + (j - 1)]
                            ).normalized;
                    }
                }
                else
                {
                    if (j < width - 1)
                    {
                        __tempNormal = Vector3.Cross(
                            newVertices[((i) * width) + (j + 1)] - newVertices[((i) * width) + (j)],
                            newVertices[((i - 1) * width) + (j)] - newVertices[((i) * width) + (j)]

                            ).normalized;
                    }
                    else
                    {
                        __tempNormal = Vector3.Cross(
                            newVertices[((i) * width) + (j)] - newVertices[((i) * width) + (j - 1)],
                            newVertices[((i - 1) * width) + (j)] - newVertices[((i) * width) + (j - 1)]
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

    public Mesh GenerateEvenlySpacedCurveCenteredBasedRectangularMesh(Vector3[] p_curveControlPoints, int p_height)
    {
        Mesh newMesh = new Mesh();
        List<Vector3> newVertices = new List<Vector3>();
        List<int> newTriangles = new List<int>();
        List<Vector3> newNormals = new List<Vector3>();

        //temp setup parameters for testing
        int width = 11;
        int height = p_height;
        int trianglesNum = (width - 1) * (height - 1) * 2;
        //one extra point to calculate the angle for the last point we are using
        Vector3[] curvePoints = GetEvenlySpacedBezerCurvePoints(p_curveControlPoints, height + 1, 0, 1);

        #region Vertices
        for (int i = 0; i < height; i++)
        {
            Vector3 __direction = (curvePoints[i + 1] - curvePoints[i]);
            float __angle = Mathf.Atan2(__direction.z, __direction.x) - (Mathf.PI / 2); //rad
            Debug.Log(__angle * Mathf.Rad2Deg);
            //j is the position in the X axis, while i is the position in the Z axis
            for (int j = 0; j < width; j++)
            {
                float __radius = j - Mathf.Ceil((float)width / 2);
                newVertices.Add(new Vector3(curvePoints[i].x + (Mathf.Cos(__angle) * __radius), 0, curvePoints[i].z + (Mathf.Sin(__angle) * __radius)));
            }
        }
        #endregion

        #region Triangles 
        //adding counter-clockwise for the faces to face the right direction       
        for (int i = 0; i < height - 1; i++)
        {
            for (int j = 0; j < width - 1; j++)
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
                            newVertices[((i) * width) + (j + 1)] - newVertices[((i + 1) * width) + (j)],
                            newVertices[((i) * width) + (j)] - newVertices[((i + 1) * width) + (j)]
                            ).normalized;
                    }
                    else
                    {
                        __tempNormal = Vector3.Cross(
                            newVertices[((i + 1) * width) + (j)] - newVertices[((i + 1) * width) + (j - 1)],
                            newVertices[((i) * width) + (j)] - newVertices[((i + 1) * width) + (j - 1)]
                            ).normalized;
                    }
                }
                else
                {
                    if (j < width - 1)
                    {
                        __tempNormal = Vector3.Cross(
                            newVertices[((i) * width) + (j + 1)] - newVertices[((i) * width) + (j)],
                            newVertices[((i - 1) * width) + (j)] - newVertices[((i) * width) + (j)]

                            ).normalized;
                    }
                    else
                    {
                        __tempNormal = Vector3.Cross(
                            newVertices[((i) * width) + (j)] - newVertices[((i) * width) + (j - 1)],
                            newVertices[((i - 1) * width) + (j)] - newVertices[((i) * width) + (j - 1)]
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

    public Vector3[] GetEvenlySpacedBezerCurvePoints(Vector3[] p_controlPoints, int p_amountOfPoints, float p_minT, float p_maxT)
    {
        int __samplingRate = 1000;
        Vector3[] __sampledCurvePoints = new Vector3[__samplingRate];
        float __curveLenght = 0;
        float __curveLenghtPerPoint = 0;
        List<Vector3> __evenlySpacedPoints = new List<Vector3>();
        float __sum = 0;

        //Get the sample points
        for (int i = 0; i < __samplingRate; i++)
        {
            __sampledCurvePoints[i] = GetBezierCurvePoint(p_controlPoints, p_minT + (((float)i / __samplingRate) * p_maxT - p_minT));
        }

        //calculate the curve lenght based on sample points
        for (int i = 0; i < __samplingRate - 1; i++)
        {
            __curveLenght += (__sampledCurvePoints[i + 1] - __sampledCurvePoints[i]).magnitude;
        }

        __curveLenghtPerPoint = __curveLenght / p_amountOfPoints;

        __evenlySpacedPoints.Add(__sampledCurvePoints[0]);
        for (int i = 0; i < __samplingRate - 1; i++)
        {
            if (__sum >= __curveLenghtPerPoint)
            {
                __evenlySpacedPoints.Add(__sampledCurvePoints[i]);
                __sum = 0;
            }
            __sum += (__sampledCurvePoints[i + 1] - __sampledCurvePoints[i]).magnitude;
        }
        if (__sum >= __curveLenghtPerPoint * 0.5)
        {
            __evenlySpacedPoints.Add(__sampledCurvePoints[__sampledCurvePoints.Length - 1]);
        }

        return __evenlySpacedPoints.ToArray();
    }

    public Vector3 GetBezierCurvePoint(Vector3[] p_controlPoints, float p_t)
    {
        Vector3[] __tempPoints = p_controlPoints;
        while (__tempPoints.Length > 1)
        {
            __tempPoints = GetMidPointsFromArray(__tempPoints, p_t);
        }
        return __tempPoints[0];
    }

    /// <summary>
    /// This method is used in the Bezier Curve Generation Method
    /// </summary>
    /// <param name="p_points"></param>
    /// <param name="p_t"></param>
    /// <returns></returns>
    public Vector3[] GetMidPointsFromArray(Vector3[] p_points, float p_t)
    {
        Vector3[] __tempArray = new Vector3[p_points.Length - 1];

        for (int i = 0; i < __tempArray.Length; i++)
        {
            __tempArray[i] = Vector3.Lerp(p_points[i], p_points[i + 1], p_t);
        }

        return __tempArray;
    }
}
