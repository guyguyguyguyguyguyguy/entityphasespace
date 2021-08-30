using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HelperFuncs;
using System;

namespace FutureTraj 
{
    /*
        Take current state of sytem and work out the trajectory of each agent for the next n steps, given all other agents stay the same
            -> Can then say that at this step, there is emergence even if nothing directly influenced by it

        Compare the size of the 'phase space' given by this trajectory to the whole canvas, if this is different, then emergence has happened -> in the model, the phase space that is different is highlighted
            -> Once more complicated phase spaces are introduced (not just 2d comprised of x and y pos), can still know what size of 'total' possible phase space is by defining it ourselfs


        Need:
            -> State of model at current step
            -> Fast method to calculate future trajectory of agent without direct simulation (possibly recreate model, without the visual part and run agents in this)
            -> Access to linerenderer of each agent
        
        Possible this should be a model script not an agent script
    */

    public struct Trajectory
    {
        public int id;
        public int nStepsForward;
        public Vector3[] trajectory;
    }

    public class CollisionModel
    {
        /*
            Class for working out future trajectories in a model in which agents only collide with one another and the phase space is their 2d coordinates
        */

        public struct Agent 
        {
            public Vector3 pos;
            public Vector2 velocity;

            public Agent(Vector3 p, Vector2 vel)
            {
                pos = p;
                velocity = vel;
            }
        }

        private float agentWidth;
        private float agentHeight;
        static public int nStepsForward;
        // Dictionary representing discritisation of model into agent-sized cells -> 1 represents there is an object in that cells, 0 it is empty
        // Coarse graining each agent from circle to square (should be okay for starts)
        public Dictionary<string, int> discreteModel = new Dictionary<string, int>(); 

        public CollisionModel(int nSteps, float aWidth, float aHeight, float mWidth, float left, float mHeight, float bott)
        {
            agentHeight = aHeight;
            agentWidth = aWidth;
            nStepsForward = nSteps;
            GenerateModelGrid(agentWidth, agentHeight, mWidth, left, mHeight, bott);
        }

        public Tuple<Trajectory[], float[]> FutureTrajectories(Vector3[] stateOfModel, int[] ids, Vector2[] agentVels)
        {

            (Dictionary<string, int> modelAbstract, Dictionary<int, string> inverseModelAbstract) = GenerateCurrentStepAbstraction(stateOfModel, ids);
            Trajectory[] futureTrajOfAgents = new Trajectory[stateOfModel.Length];
            float[] phaseSpaceAreas = new float[stateOfModel.Length];


            for (int i = 0; i < stateOfModel.Length; ++i)
            {
                // remove each agent from dict, and then follow its course for nsteps using detect next collision and collision

                Dictionary<string, int> modelWithoutAgent = StateOfModelWithoutAgent(modelAbstract, inverseModelAbstract, ids[i]);

                Trajectory agentTraj = new Trajectory();
                agentTraj.id = ids[i];
                agentTraj.nStepsForward = nStepsForward;
                Agent agent = new Agent(stateOfModel[i], agentVels[i]);
                agentTraj.trajectory = CalculateAgentTrajectory(modelWithoutAgent, agent);

                float phaseArea = PhaseSpaceAreaApproximation(agentTraj.trajectory);
                phaseSpaceAreas[i] = phaseArea;
            }
            
            return Tuple.Create(futureTrajOfAgents, phaseSpaceAreas);

        }

        private void GenerateModelGrid(float agentWidth, float agentHeight, float modelWidth, float modelLeftPos, float modelHeight, float modelBottPos)
        {
            // Generate discrete model, with all possible cells added and set to -1
            // Each cell has dimensions of an agent and is normalised? and rounded for easy look-up for collision
            
            // TODO: Seems should be rounded to agentwidth/2 also in GenerateModelAbstraction but it doesn't work yet ahh!!!!!!!!!
            for (float i = modelLeftPos + (agentWidth/2); i < (modelLeftPos + modelWidth); i += agentWidth) 
            {
                for (float j = modelBottPos + (agentHeight/2); j < (modelBottPos + modelHeight); j += agentHeight)
                {
                    float xCoor = i.RoundOff((int) agentWidth);
                    float yCoor = j.RoundOff((int) agentHeight);
                    string key = xCoor.ToString() + ", " + yCoor.ToString();
                    discreteModel.Add(key, -1);
                }
            }
        }

        public Tuple<Dictionary<string, int>, Dictionary<int, string>> GenerateCurrentStepAbstraction(Vector3[] stateOfModel, int[] ids)
        {

            Dictionary<string, int> newDiscreteModel = new Dictionary<string, int>(discreteModel);
            Dictionary<int, string> inverseNewDiscreteModel = new Dictionary<int, string>(); 
           
            // TODO: Seems should be rounded to agentwidth/2 also in GenerateModelGrid but it doesn't work yet ahh!!!!!!!!!
            for (int i = 0; i < stateOfModel.Length; ++i)
            {
                string agentGridPos = stateOfModel[i][0].RoundOff((int) agentWidth).ToString() + ", " + stateOfModel[i][1].RoundOff((int) agentHeight).ToString();
                newDiscreteModel[agentGridPos] = ids[i];
                inverseNewDiscreteModel[ids[i]] = agentGridPos;
            }

            return Tuple.Create(newDiscreteModel, inverseNewDiscreteModel);
        }

        private Dictionary<string, int> StateOfModelWithoutAgent(Dictionary<string, int> totalModel, Dictionary<int, string> inverseTotalModel, int id)
        {
            // remove current agent from abstractModel
            Dictionary<string, int> modelSinAgent = new Dictionary<string, int>(totalModel);

            modelSinAgent[inverseTotalModel[id]] = 0;

            return modelSinAgent;

        }
        
        private Vector3[] CalculateAgentTrajectory(Dictionary<string, int> modelWithoutAgent, Agent agent)
        {
            int steps = 0;
            Vector3[] trajectory = new Vector3[nStepsForward];

            do {
                agent.pos += ((Vector3) agent.velocity / (1.0f / Time.deltaTime));
                string currentAgentCell = agent.pos[0].RoundOff((int) agentWidth).ToString() + ", " + agent.pos[1].RoundOff((int) agentHeight).ToString();

                int dictValue;
                // returns 0 if key is not found
                modelWithoutAgent.TryGetValue(currentAgentCell, out dictValue);

                // Implicit way of detecting barrier collision??
                if (dictValue == 0) {
                    BarrierCollision(ref agent);
                    ++steps;
                } else if (dictValue > 0) {
                    AgentCollision(ref agent);
                    ++steps;
                }
                ++steps;
            } while (steps.RoundOff(5) != 10); // Roundoff is ugllyyy but needed as sometimes steps goes up by 1 and sometimes by 2 -> means cannot know what number it'll skip over

            return new Vector3[1];
        }

        private void BarrierCollision(ref Agent a)
        {
            Debug.Log("I am in barrier collision yeahhh");



        }

        private void AgentCollision(ref Agent a) 
        {
           /* Collision of agent with a circle at the position the agent is at -> a.pos
            *  -> alters reference to the agent
            */
            
            Debug.Log("i am in agent collision, madddd!");


        }

        private float PhaseSpaceAreaApproximation(Vector3[] agentTrak)
        {
           /*
            * Calculate approximate phase space area of the future trajectory:
            *   -> Take square approx of phase space area by taking the leftmost and rightmost x values, and topmost/bottommost y value
            *   -> float area = (rightmost - leftmost) * (bottommost - topmost) // As cartisian coordinates so y is flipped
            *
            *  This gives upper-bound of possible future phase space
            *
            * Used to compare against 'total' phase space (the whole model):
            *   -> If significance difference, then emergence appears to have occured
            */


            return 0.0f;

        }
    }

}
