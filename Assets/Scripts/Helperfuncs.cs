using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.ComponentModel;
using System;

namespace HelperFuncs
{

    public static class ExtensionMethods
    {
        public static int RoundOff (this float i, int num)
        {
            return ((int)Math.Floor(i / num)) * num;
        }

        public static int RoundOff (this int i, int num)
        {
            return ((int)Math.Floor((float) (i / num))) * num;
        }
    }

    public class ModelHelper
    {
        static public ModelBoundary boundary;
        static public Transform bordersTran;

        public ModelHelper(Transform modelBor)
        {
            bordersTran = modelBor;
            ModelBounds();
        }

        public struct ModelBoundary
        {
            public float leftBound;
            public float rightBound;
            public float topBound;
            public float bottBound;

        }

        public static bool ClickInFrame(Vector3 mousePos)
        {
            float xPos = mousePos.x;
            float yPos = mousePos.y;

            if (xPos > boundary.leftBound && xPos < boundary.rightBound && yPos < boundary.topBound && yPos > boundary.bottBound) {
                return true;
            }

            return false;
        }

        public static void ModelBounds()
        {
            foreach (Transform b in bordersTran)
            {
                Collider2D bCol = b.gameObject.GetComponent<Collider2D>();
                Vector3 centre = bCol.bounds.center;
                string borderName = bCol.name.ToLower();
                string name = borderName.Substring(0, borderName.IndexOf("border"));

                switch (name)
                {
                    case "bott":
                        boundary.bottBound = centre.y + (bCol.bounds.size.y / 2);
                        break;
                    case "top":
                        boundary.topBound = centre.y - (bCol.bounds.size.y / 2);
                        break;
                    case "left":
                        boundary.leftBound = centre.x + (bCol.bounds.size.x / 2);
                        break;
                    case "right":
                        boundary.rightBound = centre.x - (bCol.bounds.size.x / 2);
                        break;
                    default:
                        throw new InvalidEnumArgumentException();
                }
            }
            
        }
    }

}