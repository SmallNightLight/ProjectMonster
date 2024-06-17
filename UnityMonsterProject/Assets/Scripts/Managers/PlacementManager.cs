using ScriptableArchitecture.Data;
using UnityEngine;
using System.Linq;

public class PlacementManager : MonoBehaviour, IUpdateManager
{
    [SerializeField] private PlacementReference _placements;

    public void UpdateManager()
    {
        //var orderedPlacements = _placements.RuntimeSet
        //    .OrderBy(p => p.Lap)
        //    .ThenBy(p => p.Spline)
        //    .ThenBy(p => p.Step)
        //    .ToList();

        //_placements.RuntimeSet.Clear();

        //for(int i = 0; i < orderedPlacements.Count; i++)
        //{
        //    _placements.Add(orderedPlacements[i]);
        //}
    }
}