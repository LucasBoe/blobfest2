using UnityEngine;

namespace VoronoiMap
{
    public class VoronoiMapGizmoDrawer : MonoBehaviour
    {
        [SerializeField] private VoronoiMapData mapData;
        public VoronoiMapData Map { get => mapData; set => mapData = value; }

        [SerializeField] private bool drawBounds = true;
        [SerializeField] private bool drawCells = true;
        [SerializeField] private bool drawFails = true;

        [SerializeField, Range(0, 10)] int failStepsToDraw = 10;

        private void OnDrawGizmos()
        {
            if (mapData == null)
                return;

            if (drawCells)
            {
                for (int i = 0; i < mapData.Cells.Count; i++)
                {
                    VoronoiCellData cell = mapData.Cells[i];
                    Gizmos.color = Color.HSVToRGB((float)i / mapData.Cells.Count, 1, 1);
                    for (int j = 1; j < cell.Edges.Length; j++)
                    {
                        Gizmos.DrawLine(cell.Edges[j - 1], cell.Edges[j]);
                    }
                }
            }

            if (drawBounds)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(new Vector3(0, 0, 0), new Vector3(0, 0, mapData.Size.y));
                Gizmos.DrawLine(new Vector3(0, 0, 0), new Vector3(mapData.Size.x, 0, 0));
                Gizmos.DrawLine(new Vector3(mapData.Size.x, 0, 0), new Vector3(mapData.Size.x, 0, mapData.Size.y));
                Gizmos.DrawLine(new Vector3(0, 0, mapData.Size.y), new Vector3(mapData.Size.x, 0, mapData.Size.y));
            }

            if (drawFails)
            {
                foreach (var item in mapData.Fails)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireSphere((item.Center), .2f);
                    int max = Mathf.Min(item.OwnEdgePoints.Count, failStepsToDraw);

                    for (int j = 1; j < max; j++)
                    {
                        Gizmos.color = Color.Lerp(Color.yellow, Color.red, (float)j / max);
                        Gizmos.DrawLine((item.OwnEdgePoints[j - 1]), (item.OwnEdgePoints[j]));
                    }
                }
            }
        }
    }
}