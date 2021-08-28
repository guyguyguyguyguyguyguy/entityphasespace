using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HelperFuncs;

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
          public float[] position;
          public Vector2 velocity;
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

        private void GenerateModelGrid(float agentWidth, float agentHeight, float modelWidth, float modelLeftPos, float modelHeight, float modelBottPos)
        {
            // Generate discrete model, with all possible cells added and set to 0
            // Each cell has dimensions of an agent and is normalised? and rounded for easy look-up for collision
            


            // TODO: Seems should be rounded to agentwidth/2 also in GenerateModelAbstraction but it doesn't work yet ahh!!!!!!!!!
            for (float i = modelLeftPos + (agentWidth/2); i < (modelLeftPos + modelWidth); i += agentWidth) 
            {
                for (float j = modelBottPos + (agentHeight/2); j < (modelBottPos + modelHeight); j += agentHeight)
                {
                    float xCoor = i.RoundOff((int) agentWidth);
                    float yCoor = j.RoundOff((int) agentHeight);
                    string key = xCoor.ToString() + ", " + yCoor.ToString();
                    discreteModel.Add(key, 0);
                }
            }
        }

        public Trajectory[] FutureTrajectory() 
        {
            
            return new Trajectory[1];

        }

        public void GenerateCurrentStepAbstraction(Vector3[] stateOfModel)
        {
            // Set cells with agent inside as 1


            //TODO: all agents apart from the agent we are looking at, need to change input and take agent to detect next colllision and track how many steps have passed between collisions so ensure all are carried out for same num of steps


            Dictionary<string, int> newDiscreteModel = new Dictionary<string, int>(discreteModel);
           
            // TODO: Seems should be rounded to agentwidth/2 also in GenerateModelGrid but it doesn't work yet ahh!!!!!!!!!
            foreach(Vector3 pos in stateOfModel)
            {
                string agentGridPos = pos[0].RoundOff((int) agentWidth).ToString() + ", " + pos[1].RoundOff((int) agentHeight).ToString();
                newDiscreteModel[agentGridPos] = 1;
            }
        }
        
        private void DetectNextCollision()
        {
           /* 
            * Here we 'skip' steps in the simulation and skip until next collision event:
            *  -> dont need to include positions/steps in which no collision occurs, just the position at end of last collision and start of next
            */   


        }

        private Vector2 Collision(Agent a, float[] environment) 
        {
           /* Collision of agent with an element in model abstraction grid:
            *  -> returns agent velocity post-collision
            */

            return Vector2.zero;

        }

        private float PhaseSpaceAreaApproximation()
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
