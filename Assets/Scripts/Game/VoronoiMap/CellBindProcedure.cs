using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VoronoiMap
{
    public enum ProcedureState
    {
        FAILURE = -1, PENDING = 0, SUCCESS = 1
    }
    [System.Serializable]
    public class CellBindProcedure
    {
        [SerializeField] private List<Vector2Pair> allEdges;
        [SerializeField] private Vector2 startingPoint;
        [SerializeField] Vector2 currentPoint, nextPoint;
        [SerializeField] List<Vector2> ownEdgePoints = new();
        [SerializeField] Vector2 center;
        [SerializeField] int tryCount = 0;
        [SerializeField] private ProcedureState state = ProcedureState.PENDING;
        public List<Vector2> OwnEdgePoints { get => ownEdgePoints; }
        public Vector2 Center { get => center; }
        public int TryCount { get => tryCount; }
        public ProcedureState State => state;
        public VoronoiCellData Cell { get; private set; }
        public CellBindProcedure(Vector2 center, List<Vector2Pair> allEdges)
        {
            this.center = center;
            this.allEdges = allEdges;
            this.startingPoint = allEdges.OrderBy(p => Vector2.Distance(center, p.A)).FirstOrDefault().A;
            this.ownEdgePoints = new List<Vector2> { startingPoint };
        }
        public bool TryExecuteFull()
        {
            while (state == ProcedureState.PENDING)
                TryExecuteSingleStep();

            return state == ProcedureState.SUCCESS;
        }

        public bool TryExecuteSingleStep()
        {
            if (TryCount > 100)
            {
                state = ProcedureState.FAILURE;
                return false;
            }    

            currentPoint = OwnEdgePoints.Last();

            //try to find next point
            if (!FindNext(currentPoint, allEdges, Center, ref ownEdgePoints, out var next))
            {
                state = ProcedureState.FAILURE;
                return false;
            }

            nextPoint = next;

            if (!SameDistance(nextPoint, startingPoint) && ContainsDistance(OwnEdgePoints, nextPoint))
            {
                state = ProcedureState.FAILURE;
                return false;
            }


            if (SameDistance(nextPoint, startingPoint))
            {
                Cell = new VoronoiCellData(Center, OwnEdgePoints.ToArray());
                state = ProcedureState.SUCCESS;
                return true;
            }

            OwnEdgePoints.Add(nextPoint);
            tryCount++;
            return true;
        }

        private bool FindNext(Vector2 current, List<Vector2Pair> edges, Vector2 center, ref List<Vector2> exclude, out Vector2 next)
        {
            var previous = exclude.ToArray();
            var filtered = CreatePoolFromDistanceComparison(current, edges, exclude);

            if (filtered.Count() == 0 || filtered.Count() < 2 && SameDistance(filtered[0], previous.First()))
            {
                next = Vector2.zero;
                return false;
            }

            next = filtered.OrderBy(point => AngleDelta(current, point, center)).Last();
            return true;
        }
        private Vector2[] CreatePoolFromDistanceComparison(Vector2 current, List<Vector2Pair> edges, List<Vector2> filter)
        {
            List<Vector2> pool = new List<Vector2>();

            foreach (var edge in edges)
            {
                var points = new Vector2[] { edge.A, edge.B };

                foreach (var point in points)
                {
                    if (Vector2.Distance(point, current) < 0.02f)
                        pool.Add(edge.Other(point));
                }
            }

            var first = filter.First();
            return pool.Where(p => SameDistance(p, first) || !ContainsDistance(filter.ToList(), p)).ToArray();
        }
        private bool ContainsDistance(List<Vector2> list, Vector2 item)
        {
            foreach (var point in list)
            {
                if (SameDistance(point, item))
                    return true;
            }
            return false;
        }
        private bool SameDistance(Vector2 a, Vector2 b)
        {
            return (Vector2.Distance(a, b) < 0.02f);
        }
        private float AngleDelta(Vector2 a, Vector2 b, Vector2 center)
        {
            float angleA = Mathf.Atan2(a.x - center.x, a.y - center.y) * Mathf.Rad2Deg;
            float angleB = Mathf.Atan2(b.x - center.x, b.y - center.y) * Mathf.Rad2Deg;

            return Mathf.DeltaAngle(angleA, angleB);
        }
    }
}
