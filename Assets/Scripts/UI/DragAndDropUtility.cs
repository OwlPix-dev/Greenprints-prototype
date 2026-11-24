using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace NewPowerSupply.UI
{
    public static class DragAndDropUtility
    {
        //public static Vector2 CalculateDragElementPosition(
        //      VisualElement dragElement
        //    , Vector2? position = null
        //    , Vector2? offset = null
        //    , Vector2? gridSize = null
        //    , Vector2? clampMaxPosition = null)
        //{
        //    position ??= UIPositionUtility.GetMousePosition();
        //    gridSize ??= Vector2.zero;
        //    offset ??= Vector2.zero;
        //    clampMaxPosition ??= new Vector2(Screen.width, Screen.height);

        //    position -= offset;

        //    if (gridSize != Vector2.zero)
        //    {
        //        UIPositionUtility.SnapPositionToGrid(position.Value, gridSize.Value);
        //    }


        //    return position.Value;
        //}

        //public static Vector2 CalculateDragElementPositionFromScroller(
        //      ScrollView scroller
        //    , VisualElement canvas
        //    , VisualElement dragElement
        //    , Vector2? position = null
        //    , Vector2? offset = null
        //    , Vector2? gridSize = null)
        //{
        //    position ??= UIPositionUtility.GetMousePosition();

        //    position = CalculateDragElementPosition(dragElement, position, offset, gridSize, maxPosition);

        //    UIPositionUtility.GetPositionFromScroller(position.Value, scroller);

        //    Vector2 maxPosition = new Vector2(
        //          canvas.worldBound.width - dragElement.worldBound.width
        //        , canvas.worldBound.height - dragElement.worldBound.height);

        //    UIPositionUtility.ClampPosition(position.Value, Vector2.zero, clampMaxPosition.Value);

        //    return position.Value;
        //}




        public static Vector2 GetCanvasPosition(Vector2 position, Vector2 gridSize, ScrollView scroller, VisualElement canvas)
        {
            Rect contentViewportRect = scroller.contentViewport.worldBound;

            Vector2 range = new Vector2(
                  scroller.contentContainer.worldBound.width - contentViewportRect.width
                , scroller.contentContainer.worldBound.height - contentViewportRect.height);

            Vector2 scrollFactor = new Vector2(
                  range.x > 0 ? scroller.scrollOffset.x / range.x : 0f
                , range.y > 0 ? scroller.scrollOffset.y / range.y : 0f);

            position += new Vector2(
                  (canvas.worldBound.width - contentViewportRect.width) * scrollFactor.x - contentViewportRect.x
                , (canvas.worldBound.height - contentViewportRect.height) * scrollFactor.y - contentViewportRect.y);

            if (gridSize != Vector2.zero)
            {
                position -= new Vector2(
                      position.x - MathF.Round(position.x / gridSize.x) * gridSize.x
                    , position.y - MathF.Round(position.y / gridSize.y) * gridSize.y);
            }

            return position;
        }

        public static Vector2 GetDragElementPosition(
              Vector2 offset
            , Vector2 gridSize
            , VisualElement dragElement
            , ScrollView scroller
            , VisualElement canvas
            , Vector2? position = null)
        {
            position ??= UIPositionUtility.GetMousePosition();

            position -= offset;

            position = GetCanvasPosition(position.Value, gridSize, scroller, canvas);

            Vector2 maxPosition = new Vector2(
                  canvas.worldBound.width - dragElement.worldBound.width
                , canvas.worldBound.height - dragElement.worldBound.height);

            position = new Vector2(
                  Mathf.Clamp(position.Value.x, 0, maxPosition.x)
                , Mathf.Clamp(position.Value.y, 0, maxPosition.y));

            return position.Value;
        }
    }
}
