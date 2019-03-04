using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshDeformer : MonoBehaviour
{
    public float springForce = 20f; // force like spring that wil pull the vertices back after applying diformation
    public float damping = 5f; // to decrease velocity of spring

    private Mesh deformingMesh; // mesh to apply effect on it
    private Vector3[] originalVertices, displacedVertices; // before and after to lerp between thrm
    private Vector3[] vertexVelocities; // velocity o each vertex when deformed

    private float uniformScale = 1f; // to apply the sme force to difrent sized objects

    private void Start()
    {
        deformingMesh = GetComponent<MeshFilter>().mesh;
        originalVertices = deformingMesh.vertices;
        displacedVertices = new Vector3[originalVertices.Length];

        for (int i = 0; i < originalVertices.Length; i++)
        {
            displacedVertices[i] = originalVertices[i];
        }

        vertexVelocities = new Vector3[originalVertices.Length];
    }

    private void Update()
    {
        if (MeshDeformerInput.pressed)
        {
            // if the scalliing is dynamic change force scale
            uniformScale = transform.localScale.x;

            for (int i = 0; i < displacedVertices.Length; i++)
            {
                UpdateVertices(i);
            }
            deformingMesh.vertices = displacedVertices;
            deformingMesh.RecalculateNormals();
        }
    }

    private void UpdateVertices(int i)
    {
        Vector3 velocity = vertexVelocities[i];
        // make spring force
        Vector3 displacment = displacedVertices[i] - originalVertices[i];
        displacment *= uniformScale;
        velocity -= displacment * springForce * Time.deltaTime;
        velocity *= 1f - damping * Time.deltaTime; // add dambing to spring velocitty
        vertexVelocities[i] = velocity;

        displacedVertices[i] += velocity * (Time.deltaTime / uniformScale);
    }

    // point is destination of rayCast
    public void AddDeformingForce(Vector3 point, float force)
    {
        //Debug.DrawLine(Camera.main.transform.position, point);

        //converting the deforming force position from world space to local space.
        point = transform.InverseTransformPoint(point);
        // apply deforming force to each vertex individually
        for (int i = 0; i < displacedVertices.Length; i++)
        {
            AddForceToVertex(i, point, force);
        }
    }

    private void AddForceToVertex(int i, Vector3 point, float force)
    {
        //we need to know both the direction and the distance of the deforming force per vertex
        Vector3 pointToVertex = displacedVertices[i] - point;
        pointToVertex *= uniformScale;

        // the attenuatedForce is found by inverse square law
        // we add one to grantee that te force will be at full strenght when distance equal zero
        float attenuatedForce = force / (1f + pointToVertex.sqrMagnitude);
        
        // velocity = aaccelaratiob.time , accelaration = force/mass
        // will ignore the mass as if it were one for each vertex
        float velocity = attenuatedForce * Time.deltaTime;
        // make direction
        vertexVelocities[i] += pointToVertex.normalized * velocity;
      
    }
}
