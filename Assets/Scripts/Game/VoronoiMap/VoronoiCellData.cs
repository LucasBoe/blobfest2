using UnityEngine;

namespace VoronoiMap
{
    [System.Serializable]
    public class VoronoiCellData
    {
        public long GUID;
        public Vector2 Center;
        public Vector2[] Edges;
        public long[] NeightbourGUIDs;
        public VoronoiCellData() { }
        public VoronoiCellData(Vector2 point, Vector2[] edges)
        {
            this.GUID = GUIDHelper.GenerateNew();
            this.Center = point;
            this.Edges = edges;
        }
    }
}