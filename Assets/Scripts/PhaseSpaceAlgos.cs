using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using FutureTraj;
using HelperFuncs;


namespace PhaseSpcaeAlgorithms
{
  public class ChansAlgoClass
  {
    private static bool Ccw(Vector3 a, Vector3 b, Vector3 c)
    {
      return ((b.x - a.x) * (c.y - a.y)) > ((b.y - a.y) * (c.x - a.x));
    }

    private static void sortLeftOrBottomMostVector3(ref Vector3[] points, string axis)
    {
      if (axis == "x") {
        Array.Sort(points, (p1, p2) => p1.x.CompareTo(p2.x));
      } else if (axis == "y")
      {
        Array.Sort(points, (p1, p2) => p1.y.CompareTo(p2.y));
      }
    }

    private static List<Vector3> GrahamScan(Vector3[] points)
    {
      List<Vector3> convexHull = new List<Vector3>();
        
      if (points.Length < 4)
      {
              convexHull.AddRange(points);
                    return convexHull;
                        
      }

      sortLeftOrBottomMostVector3(ref points, "x");

      foreach (Vector3 pt in points)
      {
        while (convexHull.Count >= 2 && !Ccw(convexHull[convexHull.Count - 2], convexHull[convexHull.Count - 1], pt)) {
                  convexHull.RemoveAt(convexHull.Count - 1);
        }
        convexHull.Add(pt);
      }

      int lowerHull = convexHull.Count + 1;
      for (int i = points.Length - 1; i >= 0; --i)
      {
        Vector3 pt = points[i];
        while (convexHull.Count >= lowerHull && !Ccw(convexHull[convexHull.Count - 2], convexHull[convexHull.Count -1], pt)) {
                  convexHull.RemoveAt(convexHull.Count - 1);
        }
        convexHull.Add(pt);
      }
      convexHull.RemoveAt(convexHull.Count - 1);
      return convexHull;
    }

    private static double orientationNew(Vector3 p, Vector3 q, Vector3 r)
    {
      double val = Math.Atan2(q.y - p.y, q.x - p.x) - Math.Atan2(r.y - p.y, r.x - p.x);
      
      // bit of a hack -> but works
      val = (val < - Math.PI) ? val += (2*Math.PI) : val;
      val = (Math.Abs(val) == Math.PI/2 ) ? Math.PI/2 : val;

      return val;
    }

    private static Vector3 AltJarvisMarch(Vector3 point, Vector3[] points, Vector3 prevVector3)
    {
      double angle = int.MinValue;
      Vector3 nextP = new Vector3();
      double dist = 0;
      
      // hack 
      Func<Vector3, Vector3, float> euclDistNoSqrt = (a, b) => (a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y);

      foreach (Vector3 p in points)
      {
        // Bit of a hack
        if (p != point && p != prevVector3)
        {
          double newA = orientationNew(point, p, prevVector3);
          if (newA > angle) {
            angle = newA;
            nextP = p;
            dist = euclDistNoSqrt(point, p);
            
          } 
          // if two points at same angle, take one further away
          else if (newA == angle) {
            double newDist = euclDistNoSqrt(point, p);
            if (dist < newDist){
              dist = newDist;
              nextP = p;
            }
          }
        }    
      }

      return nextP;
    }

    private static (List<List<Vector3>>, Vector3[], List<Vector3>, bool) ChansAlgo(Vector3[] points, int m)
    {
      List<Vector3> chunkConvexHull = new List<Vector3>();
      List<List<Vector3>> sepChunkConvexHull = new List<List<Vector3>>();
      Vector3[] chunk;
      
      if (m > points.Length) {
        m = points.Length;
      }

      // Console.WriteLine("m is currently: " + m);

      for (int i = 0; i < points.Length; i+=m)
      {
        if (i + m < points.Length) {
          chunk = new Vector3[m];
          Array.Copy(points, i, chunk, 0, m);
        } else {
          int n = points.Length - i;
          chunk = new Vector3[n];
          Array.Copy(points, i, chunk, 0, n);
        }

        List<Vector3> convexChunk = GrahamScan(chunk);
        sepChunkConvexHull.Add(convexChunk);
        chunkConvexHull.AddRange(convexChunk);
      }

      Vector3[] miniConvexHull = chunkConvexHull.ToArray();

      // if (m == points.Length)
      // {
      //   return (sepChunkConvexHull, miniConvexHull, miniConvexHull.ToList(), true);
      // }

      Vector3[] leftOverVector3s = (Vector3[])miniConvexHull.Clone();

      List<Vector3> totalConvexHull = new List<Vector3>();
      Vector3 currVector3 = new Vector3(int.MaxValue, 0);
      // Vector3 rightMost = new Vector3(int.MaxValue, 0);
      foreach (Vector3 p in chunkConvexHull) 
      {
        if (p.x < currVector3.x)
          currVector3 = p;
        // if (p.x > currVector3.x)
        //  rightMost = p;
      }

      Vector3 prevVector3 = new Vector3(int.MinValue, 0);
      totalConvexHull.Add(currVector3);
      // leftOverVector3s = leftOverVector3s.Except(new Vector3[]{currVector3}).ToArray();

      for (int i = 0; i < m; ++i)
      {
        Vector3 convexHullVector3 = AltJarvisMarch(currVector3, leftOverVector3s, prevVector3);
        prevVector3 = currVector3;
        currVector3 = convexHullVector3;
        // if (currVector3 == rightMost)
        // {
        //   switchSide = true;
        // }
        if (currVector3 == totalConvexHull[0]) {
          return (sepChunkConvexHull, miniConvexHull, totalConvexHull, true);
        } else {}
        totalConvexHull.Add(currVector3);
        leftOverVector3s = leftOverVector3s.Except(new Vector3[]{currVector3}).ToArray();

      }

      if (m == points.Length)
      {
        return (sepChunkConvexHull, miniConvexHull, totalConvexHull, true);
      }

      return (sepChunkConvexHull, miniConvexHull, totalConvexHull, false);
    }

    public static Vector3[] ConvexHull (Vector3[] points, int m = 2)
    {
      List<Vector3> totalConvexHull = new List<Vector3>();
      Vector3[] miniConvexHull;
      List<List<Vector3>> miniConvexHulls = new List<List<Vector3>>();
      bool isDone = false;
      int n = points.Length;

      do {
        (miniConvexHulls, miniConvexHull, totalConvexHull, isDone) = ChansAlgo(points, m);
        m *= m;
      } while (!isDone);

      return totalConvexHull.ToArray();
    }
  }
}