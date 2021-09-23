using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using HelperFuncs;
using FutureTraj;

public class Model : MonoBehaviour
{
    private Vector3[] phasePositions;
    private PhaseSpace phaseSpaceObj;
    private Transform modelBorders;
    private ModelHelper helper;
    private CollisionModel futureTraj;
    private float modelWidth;
    private float modelHeight;
    private bool paused = false;
    private int predSteps = 1000;

    public List<GameObject> Agents;
    public List<GameObject> Barriers;
    public GameObject Agent;
    public GameObject Barrier;
    public GameObject HullPrefab;
    public int numberOfAgents;

    private void Awake() 
    {
        modelBorders = GameObject.Find("Borders").transform;
        // activates static constructor, now all modules can use modelhelper with the correct attributes
        helper = new ModelHelper(modelBorders);

        // agent size
        SpriteRenderer agentRen = Agent.GetComponent<SpriteRenderer>();
        float agentWidth = agentRen.bounds.size.x;
        float agentHeight = agentRen.bounds.size.y;

        // model width and height
        modelWidth = ModelHelper.boundary.rightBound - ModelHelper.boundary.leftBound;
        modelHeight = ModelHelper.boundary.topBound - ModelHelper.boundary.bottBound;

        futureTraj = new CollisionModel(predSteps, agentWidth, agentHeight, modelWidth, ModelHelper.boundary.leftBound, modelHeight, ModelHelper.boundary.bottBound);
    }

    // Start is called before the first frame update
    private void Start()
    {

        phaseSpaceObj = (PhaseSpace)FindObjectOfType(typeof(PhaseSpace));
        phasePositions = phaseSpaceObj.gridPositions;

        // foreach (Transform a in this.Transform.GetChild(0))
        Transform startingAgents = this.transform.GetChild(0);
        for (int i = 0; i < Agents.Count; ++i)
        {
            GameObject agent = startingAgents.GetChild(i).gameObject;
            agent.GetComponent<AgentBehaviour>().id = numberOfAgents;
            agent.GetComponent<AgentBehaviour>().phasePosition = phasePositions[i];
        }
    }

    // Update is called once per frame
    private void Update()
    {
        PauseControl();
        CheckForMouseClick();
        numberOfAgents = Agents.Count;
    }

    private void FixedUpdate()
    {

    }

    private void CheckForMouseClick()
    {
        if (Input.GetMouseButtonDown(0)) {
            // check which section mouse press is in
        }

        if (Input.GetMouseButtonUp(0)) {

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (ModelHelper.ClickInFrame(mousePos)) {
                GameObject newAgent = AddAgent(mousePos);
                AddToAgents(newAgent);
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            GameObject newBarrier = AddBarrier();
            AddToBarriers(newBarrier);
        }

    }

    private GameObject AddAgent(Vector3 pos) 
    {
        Vector3 agentPos = pos;
        agentPos.z = -1;
        GameObject newAgent = Instantiate(Agent, agentPos, Quaternion.identity) as GameObject;
        int numAgents = Agents.Count; 
        newAgent.name = "Agent " + (++numAgents).ToString();
        newAgent.GetComponent<AgentBehaviour>().id = numberOfAgents;
        newAgent.GetComponent<AgentBehaviour>().phasePosition = phasePositions[numberOfAgents];


        return newAgent;
    }
    
    private void AddToAgents(GameObject a)
    {
        Transform potentialAgentParent = this.transform.GetChild(0);
        a.transform.parent = potentialAgentParent.name == "Agents" ? potentialAgentParent : this.transform;
        Agents.Add(a);
    }

    private GameObject AddBarrier() 
    {
        GameObject newBarrier = Instantiate(Barrier, Vector3.zero, Quaternion.identity) as GameObject;
        int numBarriers = Barriers.Count; 
        newBarrier.name = "Barrier " + (++numBarriers).ToString();
        newBarrier.GetComponent<BarrierDrawer>().justInstantitaed = true;

        return newBarrier;
    }

    private void AddToBarriers(GameObject a)
    {
        Transform potentialBarrierParent = this.transform.GetChild(1);
        a.transform.parent = potentialBarrierParent.name == "Barriers" ? potentialBarrierParent : this.transform;
        Barriers.Add(a);
    }

    private void PauseControl() 
    {
        if (Input.GetKeyDown("space")) {
            paused = paused ? ResumeModel() : PauseModel();
        }
    }

    private bool PauseModel() 
    {
        Time.timeScale = 0;
        // ProduceFutureTraj();
        EmergenceDetection();
        return true;
    }

    private bool ResumeModel() 
    {
        Time.timeScale = 1;
        return false;
    }


    // Future trajectory stuff -> Get each agents future trajectory, give it to them and render in agentbehaviour
    // Want a way to compare projected future trajectory vs the actual trajectory
    private (FutureTraj.Trajectory[], Vector3[][]) ProduceFutureTraj()
    {
        Vector3[] currentState = new Vector3[numberOfAgents];
        int[] ids = new int[numberOfAgents];
        Vector2[] agentVels = new Vector2[numberOfAgents];

        for(int i = 0; i < numberOfAgents; ++i)
        {
            GameObject a = Agents[i];
            currentState[i] = a.transform.position;
            ids[i] = a.GetComponent<AgentBehaviour>().id;

            Rigidbody2D agentRb = a.GetComponent<Rigidbody2D>();
            Vector2 agentVel = agentRb.velocity; 
            agentVels[i] = agentVel;
        }

        (FutureTraj.Trajectory[] agentTrajectories, Vector3[][] futureVects) = futureTraj.FutureTrajectories(currentState, ids, agentVels);


        return (agentTrajectories, futureVects);
    }


    private void RenderTrajInSpace(Vector3[][] futureVects)
    {
        for (int i = 0; i < numberOfAgents; ++i)
        {
            GameObject a = Agents[i];
            Vector3[] vers = new Vector3[predSteps];

            a.GetComponent<AgentBehaviour>().lineRend.positionCount = 0;
            a.GetComponent<AgentBehaviour>().lineRend.positionCount = predSteps;

            for (int j = 0; j < predSteps; ++j)
            {
                Vector3 v = futureVects[i][j];

                // This is for putting trajectory in the phase space box
                v.x = v.x * 0.092f;
                v.y = v.y * 0.11f;
                v.z = v.z * 0.092f;

                v += a.GetComponent<AgentBehaviour>().phasePosition;

                a.GetComponent<AgentBehaviour>().lineRend.SetPosition(j, v);

            }   
        }
    }

    private void EmergenceDetection() 
    {
        (FutureTraj.Trajectory[] agentTrajectories, Vector3[][] agentFutureVecs) = ProduceFutureTraj();

        // RenderTrajInSpace(agentFutureVecs);

        using(System.IO.StreamWriter file = new System.IO.StreamWriter("text.csv"))
        {
            for (int i = 0; i < numberOfAgents; ++i)
            {
                Vector3[] agentConvexHull = PhaseSpcaeAlgorithms.ChansAlgorithm.ConvexHull(agentFutureVecs[i]);
                double convexHullArea = ModelHelper.PolygonArea(agentConvexHull);
                RenderConvexHull(agentConvexHull, i);
                EmergenceTest(convexHullArea, i);

                file.Write(string.Join("\n", agentConvexHull.Select(p => (p.x, p.y))));
                file.Write("\n, \n");
                file.Write(string.Join("\n", agentFutureVecs[i].Select(p => (p.x, p.y))));
                file.Write("\n, \n");
            }
        }
    }

    private void EmergenceTest(double area, int id)
    {

    }

    private void RenderConvexHull(Vector3[] vs, int id)
    {
        GameObject hull = GameObject.Find("ConvexHull " + id.ToString()); 
        MeshRenderer r;
        MeshFilter f;
        Mesh m;

        if (hull == null) {
            GameObject newHull = Instantiate(HullPrefab, Vector3.zero, Quaternion.identity) as GameObject;
            newHull.name = "ConvexHull " + id.ToString();
            newHull.transform.parent = this.transform.GetChild(4);

            r = newHull.GetComponent<MeshRenderer>();
            f = newHull.GetComponent<MeshFilter>();
            m = newHull.GetComponent<ConvexHullRend>().m;
        } else {
            r = hull.GetComponent<MeshRenderer>();
            f = hull.GetComponent<MeshFilter>();
            m = hull.GetComponent<ConvexHullRend>().m;
            m.Clear();
        }

        Vector3[] conVs = new Vector3[vs.Length];
        Vector3 v;
        Vector3 phasePos = phasePositions[id];

        for (int i = 0; i < vs.Length; ++i)
        {
            v = vs[i];

            v.x = v.x * 0.092f;
            v.y = v.y * 0.11f;
            v.z = v.z * 0.092f;

            v += phasePos; 
            conVs[i] = v;
        }

        var triangulator = new Triangulator(conVs);
        var indices = triangulator.Triangulate();
        var colors = Enumerable.Repeat(new Color(0f, 0.8f, 0.8f, 0.5f), conVs.Length).ToArray();

        m.vertices = conVs;
        m.triangles = indices;
        m.colors = colors;

        r.material = new Material(Shader.Find("Sprites/Default"));
        f.mesh = m;
    }
}
