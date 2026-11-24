using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace NewPowerSupply.UI
{
    public static class UIPositionUtility
    {
        public static Vector2 GetMousePosition()
        {
            Vector2 mousePosition = Input.mousePosition;
            mousePosition.y = Screen.height - mousePosition.y;
            return mousePosition;
        }

        public static VisualElement GetPickElement(VisualElement root, Vector2? position = null)
        {
            position ??= GetMousePosition();

            return root.panel.Pick(position.Value);
        }

        public static Vector2 GetPositionFromScroller(Vector2 position, ScrollView scroller)
        {
            Rect contentViewportRect = scroller.contentViewport.worldBound;

            Vector2 range = new Vector2(
                  scroller.contentContainer.worldBound.width - contentViewportRect.width
                , scroller.contentContainer.worldBound.height - contentViewportRect.height);

            Vector2 scrollFactor = new Vector2(
                  range.x > 0 ? scroller.scrollOffset.x / range.x : 0f
                , range.y > 0 ? scroller.scrollOffset.y / range.y : 0f);

            position += new Vector2(
                  (scroller.worldBound.x - contentViewportRect.width) * scrollFactor.x - contentViewportRect.x // ?
                , (scroller.worldBound.y - contentViewportRect.height) * scrollFactor.y - contentViewportRect.y);

            return position;
        }

        public static Vector2 SnapPositionToGrid(Vector2 position, Vector2 gridSize)
        {
            position -= new Vector2(
                  position.x - MathF.Round(position.x / gridSize.x) * gridSize.x
                , position.y - MathF.Round(position.y / gridSize.y) * gridSize.y);

            return position;
        }

        public static Vector2 ClampPosition(Vector2 position, Vector2 min, Vector2 max)
        {
            position = new Vector2(
                  Mathf.Clamp(position.x, min.x, max.x)
                , Mathf.Clamp(position.y, min.x, max.y));

            return position;
        }
    }
}
