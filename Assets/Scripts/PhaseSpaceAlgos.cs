using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using FutureTraj;
using HelperFuncs;


namespace PhaseSpcaeAlgorithms
{

    public static class ChansAlgorithm
    {

        public static Vector3[] ConvexHull(Vector3[] points)
        {
            Vector3[] totalConvexHull;
            bool isDone = false;
            int m = 2;

            do {
                (totalConvexHull, isDone) = ChanksAlgo(points, m);
                m *= m;
            } while(!isDone);

            return totalConvexHull;
        }

        private static Vector3[][] ChunkPoints(Vector3[] points, int m)
        {
            int k  = (int) points.Length/m;
            Vector3[][] chunks = new Vector3[k][];

            for (int i = 0; i < points.Length; i += m)
            {
                Vector3[] chunk;

                if (i + m < points.Length) {
                    chunk = new Vector3[m];
                    Array.Copy(points, i, chunk, 0, m);
                } else {
                    int n = points.Length - i;
                    chunk = new Vector3[n];
                    Array.Copy(points, i, chunk, 0, n);
                }

                chunks[i] = chunk;
            }

            return chunks;
        }

        private static Vector3[] ChunkConvexHull(Vector3[][] chunks)
        {
            List<Vector3> possibleConvexHull = new List<Vector3>();

            foreach (Vector3[] c in chunks)
            {
                List<Vector3> convexChunk = GrahamAlgorithm.GrahamScan(c);
                possibleConvexHull.AddRange(convexChunk);
            }

            return possibleConvexHull.ToArray();
        }

        private static (Vector3[], bool) ChanksAlgo(Vector3[] points, int m) 
        {
            Vector3[] leftOverPoints = (Vector3[]) points.Clone();
            List<Vector3> convexHull = new List<Vector3>();
            Vector3 prevPoint = Vector3.zero;
            prevPoint.x = int.MinValue;

            Vector3 currPoint = ExtensionMethods.GetLeftMost(points);
            convexHull.Add(currPoint);

            for (int i = 0; i < m; ++i)
            {
                Vector3 convexHullPoint = AltJarvisAlgorithm.JarvisMarch(currPoint, leftOverPoints, prevPoint);
                prevPoint = currPoint;
                currPoint = convexHullPoint;

                if (currPoint == convexHull[0]) {
                    return (convexHull.ToArray(), true);
                } else { }

                convexHull.Add(currPoint);
                leftOverPoints = leftOverPoints.Except(new Vector3[] {currPoint}).ToArray();
            }

            if (m == points.Length) {
                return (convexHull.ToArray(), true);
            }

            return (convexHull.ToArray(), false);
        }
    }

    static class AltJarvisAlgorithm
    {
        public static Vector3 JarvisMarch(Vector3 point , Vector3[] points, Vector3 prevPoint)
        {
            double angle = int.MinValue;
            Vector3 nextP = Vector3.zero;
            double dist = 0;

            Func<Vector3, Vector3, float> euclDistNoSqrt = (a, b) => (a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y);

            foreach (Vector3 p in points)
            {
                if (p != point && p != prevPoint)
                {
                    double newA = AngleCal(point, p, prevPoint);
                    if (newA > angle) {
                        angle = newA;
                        nextP = p;
                        dist = euclDistNoSqrt(point, p);
                    } else if (newA == angle) {
                        double newDist = euclDistNoSqrt(point, p);
                        if (dist < newDist) {
                            dist = newDist;
                            nextP = p;
                        }
                    }
                }
            }

            return nextP;
        }

        private static double AngleCal(Vector3 p, Vector3 q, Vector3 r)
        {
            double val = Math.Atan2(q.y - p.x, q.x - p.x) - Math.Atan2(r.y - p.y, r.x - p.x);
            
            val = (val < Math.PI) ? val += (2*Math.PI) : val;
            val = (Math.Abs(val) == Math.PI/2) ? Math.PI/2 : val;

            return val;
        }
    }

    static class GrahamAlgorithm
    {
        public static List<Vector3> GrahamScan(Vector3[] points)
        {
            List<Vector3> gConvexHull = new List<Vector3>();

            if (points.Length < 4) {
                gConvexHull.AddRange(points);
                return gConvexHull;
            }

            Vector3 leftMost = ExtensionMethods.GetLeftMost(points);
            
            foreach(Vector3 pt in points)
            {
                while (gConvexHull.Count >= 2 && !Ccw(gConvexHull[gConvexHull.Count - 2], gConvexHull[gConvexHull.Count - 1], pt)) {
                    gConvexHull.RemoveAt(gConvexHull.Count - 1);
                }
                gConvexHull.Add(pt);
            }

            int lowerHull = gConvexHull.Count + 1;
            for (int i = points.Length - 1; i >= 0; --i)
            {
                Vector3 pt = points[i];
                while (gConvexHull.Count >= lowerHull && !Ccw(gConvexHull[gConvexHull.Count - 2], gConvexHull[gConvexHull.Count -1], pt)) {
                    gConvexHull.RemoveAt(gConvexHull.Count - 1);
                }
                gConvexHull.Add(pt);
            }

            gConvexHull.RemoveAt(gConvexHull.Count - 1);

            return gConvexHull;
        }

        private static bool Ccw(Vector3 a, Vector3 b, Vector3 c)
        {
            return ((b.x - a.x) * (c.y - a.y)) > ((b.y - a.y) * (c.x - a.x));
        }
    }
}
