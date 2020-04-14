/* =========================================================
   ---------------------------------------------------
   Project   :    Easy UI Input
   Publisher :    Infinite Dawn
   Author    :    Tamerlan Shakirov
   ---------------------------------------------------
   Copyright © Infinite Dawn 2018 All rights reserved.
   ========================================================== */

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace EasyUIInput
{
    public enum AxisType { Vertical, Horizontal }

    [AddComponentMenu("Easy Easy UI Input/Axis Handler", 0)]
    public class AxisHandler : MonoBehaviour, IAxis, IDragHandler, IPointerUpHandler, IPointerDownHandler
    {
        #region Axis Property
        [SerializeField] private string axisName;
        [SerializeField] private Image background;
        [SerializeField] private Image stick;
        [SerializeField] private bool isResetStick;
        [SerializeField] private bool setStickPosOnPress;
        #endregion

        #region Axis
        private float vertical;
        private float horizontal;
        #endregion

        #region Additionals Fields
        Vector3 direction;
        #endregion

        public void OnDrag(PointerEventData eventData)
        {
            Vector2 position = Vector2.zero;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(background.rectTransform, eventData.position, eventData.pressEventCamera, out position))
            {
                position.x = (position.x / background.rectTransform.sizeDelta.x);
                position.y = (position.y / background.rectTransform.sizeDelta.y);

                float x = (background.rectTransform.pivot.x == 1) ? position.x * 2 + 1 : position.x * 2 - 1;
                float y = (background.rectTransform.pivot.y == 1) ? position.y * 2 + 1 : position.y * 2 - 1;

                direction = new Vector3(x, 0, y);
                direction = (direction.magnitude > 1) ? direction.normalized : direction;

                horizontal = direction.x;
                vertical = direction.z;

                stick.rectTransform.anchoredPosition = new Vector3(direction.x * (background.rectTransform.sizeDelta.x / 3), direction.z * (background.rectTransform.sizeDelta.y / 3));
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (setStickPosOnPress)
                OnDrag(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (isResetStick)
                ResetStick();
        }

        public void ResetStick()
        {
            direction = Vector3.zero;
            stick.rectTransform.anchoredPosition = Vector3.zero;
            vertical = 0;
            horizontal = 0;
        }

        public string GetName()
        {
            return axisName;
        }

        public float OnAxis(AxisType axis)
        {
            switch (axis)
            {
                case AxisType.Vertical:
                    return vertical;
                case AxisType.Horizontal:
                    return horizontal;
                default:
                    return 0;
            }
        }

        public int OnAxisRaw(AxisType axis)
        {
            switch (axis)
            {
                case AxisType.Vertical:
                    if (vertical > 0)
                        return 1;
                    else if (vertical < 0)
                        return -1;
                    else
                        return 0;
                case AxisType.Horizontal:
                    if (horizontal > 0)
                        return 1;
                    else if (horizontal < 0)
                        return -1;
                    else
                        return 0;
                default:
                    return 0;
            }
        }
    }
}