using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public struct Registredrrows
{
    public Arrow real;
    public Arrow hidden;
}

public class ShootTrajectory : MonoBehaviour
{
    public static bool charging;

    public GameObject arrow;
    public Transform refrenceArrow;

    private Scene mainScene;
    private Scene physicsScene;

    public GameObject marker;
    private List<GameObject> markers = new List<GameObject>();
    private Dictionary<string, Registredrrows> allArows = new Dictionary<string, Registredrrows>();

    public GameObject objectsToSpawn;

    private void Start()
    {
        Physics.autoSimulation = false;

        mainScene = SceneManager.GetActiveScene();
        physicsScene = SceneManager.CreateScene("PhysicsScene", new CreateSceneParameters(LocalPhysicsMode.Physics3D));

        PreparePhysicsScene();
    }

    private void FixedUpdate()
    {
        if (Input.GetMouseButton(0))
        {
            ShowTrajectory();
        }

        mainScene.GetPhysicsScene().Simulate(Time.fixedDeltaTime);
    }

    public void RegisteredArrow(Arrow arrow)
    {
        if (!allArows.ContainsKey(arrow.gameObject.name))
        {
            allArows[arrow.gameObject.name] = new Registredrrows();
        }

        var arrows = allArows[arrow.gameObject.name];
        if (string.Compare(arrow.gameObject.scene.name,physicsScene.name) == 0)
        {
            arrows.hidden = arrow;
        }
        else
        {
            arrows.real = arrow;
        }

        allArows[arrow.gameObject.name] = arrows;
    }

    public void PreparePhysicsScene()
    {
        SceneManager.SetActiveScene(physicsScene);

        GameObject g = GameObject.Instantiate(objectsToSpawn);
        g.transform.name = "RefrenceArrow";
        g.GetComponent<Arrow>().isRefrence = true;
        Destroy(g.GetComponent<MeshRenderer>());

        SceneManager.SetActiveScene(mainScene);
    }

    public void CreateMovmentMarkers()
    {
        foreach (var arrowtype in allArows)
        {
            var arrows = arrowtype.Value;
            Arrow hidden = arrows.hidden;

            GameObject g = GameObject.Instantiate(marker, hidden.transform.position, Quaternion.identity);
            g.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            markers.Add(g);
        }
    }

    public void ShowTrajectory()
    {
        SyncArrows();
        ClearTrajecory();

        allArows["RefrenceArrow"].hidden.transform.rotation = refrenceArrow.transform.rotation;
        allArows["RefrenceArrow"].hidden.GetComponent<Rigidbody>().velocity = refrenceArrow.transform.TransformDirection(Vector3.up * 15f);
        allArows["RefrenceArrow"].hidden.GetComponent<Rigidbody>().useGravity = true;

        int steps = (int)(2f / Time.fixedDeltaTime);
        for (int i = 0; i < steps; i++)
        {
            physicsScene.GetPhysicsScene().Simulate(Time.fixedDeltaTime);
            CreateMovmentMarkers();
        }
    }

    public void SyncArrows()
    {
        foreach (var arrowtype in allArows)
        {
            var arrows = arrowtype.Value;

            Arrow visual = arrows.real;
            Arrow hidden = arrows.hidden;
            var rb = hidden.GetComponent<Rigidbody>();

            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            hidden.transform.position = visual.transform.position;
            hidden.transform.rotation = visual.transform.rotation;
        }
    }

    public void ClearTrajecory()
    {
        foreach (var go in markers)
        {
            Destroy(go);
        }
        markers.Clear();
    }
}
