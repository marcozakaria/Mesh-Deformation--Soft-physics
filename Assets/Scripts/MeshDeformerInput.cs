using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshDeformerInput : MonoBehaviour
{
    public float force = 10f;
    // offset of the raycast to grantee that the vertices are always pushed into the surface
    public float forceOffset = 0.1f;

    public static bool pressed = false;

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            HandleInput();
            pressed = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            StartCoroutine(StopSimulation());
            Debug.Log("up");
        }
    }

    private void HandleInput()
    {
        Ray inputRay = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(inputRay,out hit))
        {
            MeshDeformer deformer = hit.collider.GetComponent<MeshDeformer>();
            if (deformer)
            {
                Vector3 point = hit.point;
                point += hit.normal * forceOffset;
                deformer.AddDeformingForce(point, force);
            }
        }
    }

    private WaitForSeconds twoSeconds = new WaitForSeconds(2.5f);
    private IEnumerator StopSimulation()
    {
        yield return twoSeconds;
        pressed = false;
    }
}
