using ScriptableArchitecture.Core;
using System.Collections.Generic;
using System.Linq;

namespace ScriptableArchitecture.Data
{
    [System.Serializable]
    public class Placement : IDataPoint
    {
        public Dictionary<int, Place> Places = new Dictionary<int, Place>();
        public List<int> PlayerPlacement;
        public Dictionary<int, int> Players = new Dictionary<int, int>();

        public void UpdatePlayer(int player, int lap, int spline, float step, bool lockPlayer = false)
        {
            if (!Places.ContainsKey(player))
                Places.Add(player, new Place());

            Place place = Places[player];
            place.Lap = lap;
            place.Spline = spline;
            place.Step = step;

            UpdatePlacements();

            if (lockPlayer)
            {
                place.AddedPlace = 100 - GetPlace(player);
            }
        }

        public void UpdatePlacements()
        {
            PlayerPlacement = Places.OrderBy(p => p.Value.Lap + p.Value.AddedPlace).ThenBy(p => p.Value.Spline).ThenBy(p => p.Value.Step).Select(p => p.Key).Reverse().ToList();
            Players.Clear();
            
            for (int i = 0; i < PlayerPlacement.Count; i++)
            {
                Players.Add(PlayerPlacement[i], i + 1);
            }
        }

        public int GetPlace(int player)
        {
            return Players[player];
        }
    }

    public class Place 
    {
        public int Lap;
        public int Spline;
        public float Step;
        public int AddedPlace;
    }
}