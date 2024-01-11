using UnityEngine;

public class CircleMeshFilter : MonoBehaviour
{
    public float radius = 1.0f;
    public int segments = 64;

    private MeshFilter meshFilter;

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            meshFilter = gameObject.AddComponent<MeshFilter>();
        }

        // 원 메시 생성 및 설정.
        Mesh circleMesh = new Mesh();
        circleMesh.name = "CircleMesh";

        Vector3[] vertices = new Vector3[segments + 1];
        int[] triangles = new int[segments * 3];

        vertices[0] = Vector3.zero;

        for (int i = 1; i <= segments; i++)
        {
            float angle = 2 * Mathf.PI * i / segments;
            vertices[i] = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);

            if (i < segments)
            {
                triangles[(i - 1) * 3] = 0;
                triangles[(i - 1) * 3 + 1] = i;
                triangles[(i - 1) * 3 + 2] = i + 1;
            }
            else
            {
                triangles[(i - 1) * 3] = 0;
                triangles[(i - 1) * 3 + 1] = i;
                triangles[(i - 1) * 3 + 2] = 1;
            }
        }

        circleMesh.vertices = vertices;
        circleMesh.triangles = triangles;

        // 메시 필터에 메시 설정.
        meshFilter.mesh = circleMesh;

        GetComponent<MeshRenderer>().material.renderQueue = 3100;   // 제일 마지막에 렌더링해서 다른 오브젝트 렌더링한테 안먹히게 하려고
    }

    

}
