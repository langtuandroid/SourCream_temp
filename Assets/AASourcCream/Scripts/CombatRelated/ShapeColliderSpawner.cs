using UnityEngine;

public class ShapeColliderSpawner : MonoBehaviour
{
    public GameObject shapePrefab; // Prefab or GameObject to spawn
    public float radius = 1f; // Radius of the cylinder
    public float height = 1f; // Height of the cylinder
    public int numSegments = 20; // Number of segments for the circular surfaces
    public float startAngle = 0f; // Starting angle for the quarter-circle

    void Start()
    {
        // Create the shape's mesh
        Mesh shapeMesh = CreateShapeMesh();

        // Spawn the game object with the mesh collider
        GameObject shapeObject = Instantiate(shapePrefab, transform.position, Quaternion.identity);
        MeshCollider meshCollider = shapeObject.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = shapeMesh;
    }

    // Function to create the shape's mesh
    Mesh CreateShapeMesh()
    {
        return null;
        // Mesh shapeMesh = new Mesh();

        // // Define the vertices, triangles, and other necessary data for the shape
        // int numVertices = (numSegments + 3) * 2; // Adjusted for two additional vertices
        // int numTriangles = numSegments * 6 + numSegments * 3; //Num segments is 20 so 120 + 60 = 180

        // Vector3[] vertices = new Vector3[numVertices];
        // int[] triangles = new int[numTriangles];

        // // Calculate the angle increment based on the number of segments
        // float angleIncrement = (Mathf.PI / 2f - startAngle) / numSegments;

        // // Set the positions of the vertices for the circular top and bottom surfaces
        // for (int i = 0; i <= numSegments; i++) {
        //     float angle = startAngle + i * angleIncrement;
        //     float x = radius * Mathf.Cos(angle);
        //     float z = radius * Mathf.Sin(angle);

        //     // Top surface vertices
        //     vertices[i] = new Vector3(x, height / 2f, z);

        //     // Bottom surface vertices
        //     vertices[i + numSegments + 3] = new Vector3(x, -height / 2f, z);
        // }

        // // Connect the ends of the shape
        // vertices[numSegments + 1] = vertices[numSegments]; // Top surface
        // vertices[numSegments * 2 + 2] = vertices[numSegments * 2 + 1]; // Bottom surface

        // // Set the indices of the triangles for the circular top and bottom surfaces
        // for (int i = 0; i < numSegments; i++) {
        //     // Top surface triangles
        //     triangles[i * 3] = i;
        //     triangles[i * 3 + 1] = i + 1;
        //     triangles[i * 3 + 2] = i + numSegments + 3;

        //     // Bottom surface triangles
        //     triangles[(numSegments + i) * 3] = i + numSegments + 3;
        //     triangles[(numSegments + i) * 3 + 1] = i + 1;
        //     triangles[(numSegments + i) * 3 + 2] = i + numSegments + 4;
        // }

        // // Connect the ends with triangles
        // for (int i = 0; i < numSegments; i++) {
        //     int triangleIndex = numSegments * 6 + i * 3;

        //     triangles[triangleIndex] = numSegments + 1; // Top surface
        //     triangles[triangleIndex + 1] = i;
        //     triangles[triangleIndex + 2] = i + 1;

        //     triangles[triangleIndex + numSegments * 3] = numSegments * 2 + 2; // Bottom surface
        //     triangles[triangleIndex + numSegments * 3 + 1] = i + numSegments + 3;
        //     triangles[triangleIndex + numSegments * 3 + 2] = i + numSegments + 2;
        // }

        // // Assign the vertices and triangles to the shape's mesh
        // shapeMesh.vertices = vertices;
        // shapeMesh.triangles = triangles;

        // shapeMesh.RecalculateNormals(); // Recalculate normals for correct shading

        // return shapeMesh;
    }
}