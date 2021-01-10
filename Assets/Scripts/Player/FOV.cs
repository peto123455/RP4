using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOV : MonoBehaviour
{
    /* Deklarácia premenných */
    [SerializeField] private LayerMask layerMask;
    private Mesh mesh;
    [SerializeField] private float fov;
    [SerializeField] private float viewDistance = 15f;
    [SerializeField] int rayCount = 250;
    private Vector3 position;
    private float startingAngle;

    private void Start()
    {
        /* Vytvorenie a pridelenie mesha componentu */
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        //fov = 145f; //Nastavuje veľkosť (širku) videnia hráča
        //viewDistance = 15f; //Nastavuje vzdialenosť videnia hráča
    }

    private void LateUpdate() //Update, ktorý sa vykonáva po Update
    {
        float fovP = fov + GlobalValues.fov * 4;
        if(fovP > 360f) fovP = 360f;

        float angle = startingAngle; //Počiatočný uhol
        float angleIncrease = fovP / rayCount; //Slúži na rovnomerné rozloženie rayov

        /* POLIA */
        Vector3[] vertices = new Vector3[rayCount + 1 + 1];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[rayCount * 3]; //Na každý jeden ray pripadne jeden trojuholník

        vertices[0] = position;

        /* Nastavenie indexov na počiatočnú hodnotu */
        int vertexIndex = 1;
        int triangleIndex = 0;
        for (int i = 0; i <= rayCount; i++)
        {
            Vector3 vertex;
            RaycastHit2D raycastHit2D = Physics2D.Raycast(position, MathFunctions.AngleToVector(angle), viewDistance, layerMask); //Vyslanie raya, ktorý bude detekovať kolízie s objektami v zadanej vrstve
            if (raycastHit2D.collider == null) vertex = position + MathFunctions.AngleToVector(angle) * viewDistance;
            else vertex = raycastHit2D.point;
            vertices[vertexIndex] = vertex;

            if (i > 0) //Vynechá v prvom loope, pretože nemožno vytvoriť trojiholník z 2 bodov
            {
                /* Vloženie bodov do arraya vrcholov, ktoré tvoria jeden trojuholník */
                triangles[triangleIndex] = 0;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = vertexIndex;

                triangleIndex += 3; // Navýšenie indexu pre další loop
            }

            vertexIndex++;
            angle -= angleIncrease;
        }


        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.bounds = new Bounds(position, Vector3.one * 1000f);
    }

    public void SetPosition(Vector3 position) //Funkcia slúžiaca na nastavenie pozície FOV
    {
        this.position = position;
    }

    public void SetDirection(float aimDirection) //Funkcia slúžiaca na nastavenie rotácie FOV
    {
        startingAngle = -aimDirection + fov / 2f;
    }
}
