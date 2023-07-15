using UnityEngine;

public class AbilityDecalSpawner : MonoBehaviour
{
    public GameObject decalPrefab;

    public void SpawnDecal(DecalShape shape, float size)
    {
        // Instantiate the decal prefab
        GameObject decal = Instantiate(decalPrefab, transform.position, Quaternion.identity);

        // Set the decal size based on the shape parameter
        switch (shape) {
            case DecalShape.Circle:
                decal.transform.localScale = new Vector3(size, 1f, size);
                break;
            case DecalShape.Triangle:
                decal.transform.localScale = new Vector3(size, size, size);
                break;
                // Add more cases for additional shapes if needed
        }

        // Position the decal on the ground (assuming the ground is at y=0)
        RaycastHit hit;
        Debug.DrawRay(transform.position, Vector3.down * 10f, Color.red, 5f);
        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, LayerMask.GetMask("Terrain"))) {
            decal.transform.position = hit.point;
            decal.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
        } else {

            Debug.LogWarning("Unable to spawn decal. No ground detected.");
            Destroy(decal);
            return;
        }

        // Add any additional logic or customization as needed (e.g., decal material, duration, etc.)
        // ...

        // Example: Destroy the decal after 5 seconds
        Destroy(decal, 5f);
    }
}

public enum DecalShape
{
    Circle,
    Triangle,
    // Add more shapes as needed
}