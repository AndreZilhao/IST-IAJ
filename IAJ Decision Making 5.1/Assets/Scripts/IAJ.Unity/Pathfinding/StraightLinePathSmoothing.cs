using UnityEngine;
using UnityEditor;
using Assets.Scripts.IAJ.Unity.Pathfinding.Path;

public static class StraightLinePathSmoothing
{
    public static GlobalPath SmoothPath(Vector3 position, GlobalPath currentSolution)
    {
        GlobalPath PathSmoothed = new GlobalPath();
        for (int j = 0; j < currentSolution.PathPositions.Count; j++)
        {
            PathSmoothed.PathPositions.Add(currentSolution.PathPositions[j]);
        }

        int i = 0;
        while (i < PathSmoothed.PathPositions.Count - 2)
        {
            RaycastHit hit;
            Vector3 origin = PathSmoothed.PathPositions[i];
            Vector3 direction = (PathSmoothed.PathPositions[i + 2] - PathSmoothed.PathPositions[i]).normalized;
            float distance = (PathSmoothed.PathPositions[i + 2] - PathSmoothed.PathPositions[i]).magnitude;
            if (!(Physics.Raycast(origin, direction, out hit, distance)))
            {
                PathSmoothed.PathPositions.Remove(PathSmoothed.PathPositions[i + 1]);
            }
            else
                i++;
        }
        return PathSmoothed;

    }
}