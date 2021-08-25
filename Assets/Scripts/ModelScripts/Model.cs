using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HelperFuncs;

public class Model : MonoBehaviour
{
    private Vector3[] phasePositions;
    private PhaseSpace phaseSpaceObj;
    private Transform modelBorders;
    private ModelHelper helper;
    private bool paused = false;

    public List<GameObject> Agents;
    public List<GameObject> Barriers;
    public GameObject Agent;
    public GameObject Barrier;
    public int NumberOfAgents;

    private void Awake() 
    {
        modelBorders = GameObject.Find("Borders").transform;
        // activates static constructor, now all modules can use modelhelper with the correct attributes
        helper = new ModelHelper(modelBorders);
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
            agent.GetComponent<AgentBehaviour>().id = NumberOfAgents;
            agent.GetComponent<AgentBehaviour>().phasePosition = phasePositions[i];
        }
    }

    // Update is called once per frame
    private void Update()
    {
        PauseControl();
        CheckForMouseClick();
        NumberOfAgents = Agents.Count;
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
        newAgent.GetComponent<AgentBehaviour>().id = NumberOfAgents;
        newAgent.GetComponent<AgentBehaviour>().phasePosition = phasePositions[NumberOfAgents];


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
        return true;
    }

    private bool ResumeModel() 
    {
        Time.timeScale = 1;
        return false;
    }

}
