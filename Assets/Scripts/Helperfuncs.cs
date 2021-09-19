using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.ComponentModel;
using System;
using System.Linq;

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


        public static Vector3 GetLeftMost(Vector3[] vs)
        {
            Vector3 leftMost = Vector3.positiveInfinity;

            foreach(Vector3 v in vs)
            {
                if (v.x < leftMost.x)
                    leftMost = v;
            }

            return leftMost;
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

        public static double PolygonArea(Vector3[] points)
        {
            double area = 0;
            int j = points.Length - 1;

            float[] xs = (float[]) points.Select(p => p.x).ToArray();
            float[] ys = (float[]) points.Select(p => p.y).ToArray();

            for (int i = 0; i < points.Length; ++i)
            {
                area += (xs[j] + xs[i]) * (ys[j] - ys[i]);
                j = i;
            }

            return area / 2;
        }
    }
}