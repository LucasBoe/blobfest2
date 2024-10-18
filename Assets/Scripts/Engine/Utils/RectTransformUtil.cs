using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Engine
{
    public static class RectTransformUtil
    {
        public static Vector2Int CalcItemCount(this GridLayoutGroup grid)
        {
            int itemsCount = grid.transform.childCount;
            Vector2Int size = Vector2Int.zero;

            if (itemsCount == 0)
                return size;

            switch (grid.constraint)
            {
                case GridLayoutGroup.Constraint.FixedColumnCount:
                    size.x = grid.constraintCount;
                    size.y = GetOtherAxisCount(itemsCount, size.x);
                    break;

                case GridLayoutGroup.Constraint.FixedRowCount:
                    size.y = grid.constraintCount;
                    size.x = GetOtherAxisCount(itemsCount, size.y);
                    break;

                case GridLayoutGroup.Constraint.Flexible:
                    size = GetFlexibleCount(grid);
                    break;

                default:
                    throw new ArgumentOutOfRangeException($"Unexpected constraint: {grid.constraint}");
            }

            return size;
        }

        private static Vector2Int GetFlexibleCount(this GridLayoutGroup grid)
        {
            int itemsCount = grid.transform.childCount;
            float prevX = float.NegativeInfinity;
            int xCount = 0;

            for (int i = 0; i < itemsCount; i++)
            {
                var child = grid.transform.GetChild(i) as RectTransform;

                if (!child.gameObject.activeSelf)
                    continue;

                Vector2 pos = child.anchoredPosition;

                if (pos.x <= prevX)
                   break;

                prevX = pos.x;
                xCount++;
            }

            int yCount = GetOtherAxisCount(itemsCount, xCount);
            return new Vector2Int(xCount, yCount);
        }

        private static int GetOtherAxisCount(int totalCount, int axisCount)
        {
            return totalCount / axisCount + Mathf.Min(1, totalCount % axisCount);
        }
    }
}
