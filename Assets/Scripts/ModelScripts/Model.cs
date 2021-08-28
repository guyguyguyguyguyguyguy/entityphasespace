﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    public List<GameObject> Agents;
    public List<GameObject> Barriers;
    public GameObject Agent;
    public GameObject Barrier;
    public int numberOfAgent;

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

        futureTraj = new CollisionModel(5000, agentWidth, agentHeight, modelWidth, ModelHelper.boundary.leftBound, modelHeight, ModelHelper.boundary.bottBound);
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
            agent.GetComponent<AgentBehaviour>().id = numberOfAgent;
            agent.GetComponent<AgentBehaviour>().phasePosition = phasePositions[i];
        }
    }

    // Update is called once per frame
    private void Update()
    {
        PauseControl();
        CheckForMouseClick();
        numberOfAgent = Agents.Count;
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
        newAgent.GetComponent<AgentBehaviour>().id = numberOfAgent;
        newAgent.GetComponent<AgentBehaviour>().phasePosition = phasePositions[numberOfAgent];


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
        ProduceFutureTraj();
        return true;
    }

    private bool ResumeModel() 
    {
        Time.timeScale = 1;
        return false;
    }


    // Future trajectory stuff -> Get each agents future trajectory, give it to them and render in agentbehaviour
    private void ProduceFutureTraj()
    {
        Vector3[] currentState = new Vector3[numberOfAgent];

        for(int i = 0; i < numberOfAgent; ++i)
        {
            GameObject a = Agents[i];
            currentState[i] = a.transform.position;
        }

        futureTraj.GenerateCurrentStepAbstraction(currentState);
    }

}
