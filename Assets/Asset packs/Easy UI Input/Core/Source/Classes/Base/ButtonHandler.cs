/* =========================================================
   ---------------------------------------------------
   Project   :    Easy UI Input
   Publisher :    Infinite Dawn
   Author    :    Tamerlan Shakirov
   ---------------------------------------------------
   Copyright © Infinite Dawn 2018 All rights reserved.
   ========================================================== */

using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace EasyUIInput
{
    [AddComponentMenu("Easy Easy UI Input/Button Handler", 1)]
    public class ButtonHandler : MonoBehaviour, IButton, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private string buttonName;

        private bool isPressed;
        private bool isHeld;
        private bool isReleased;
        private bool save;
        private bool longPressCalled;
        public float cacheTime;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerDown(PointerEventData eventData)
        {
            isPressed = true;
            isHeld = true;
            StartCoroutine(ResetState());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerUp(PointerEventData eventData)
        {
            isReleased = true;
            isHeld = false;
            StartCoroutine(ResetState());
        }

        /// <summary>
        /// Button name
        /// </summary>
        /// <returns></returns>
        public string GetName()
        {
            return buttonName;
        }

        /// <summary>
        /// Called once when button pressed
        /// </summary>
        /// <returns></returns>
        public bool OnPressed()
        {
            return isPressed;
        }

        /// <summary>
        /// Called every frame while button held
        /// </summary>
        /// <returns></returns>
        public bool OnHeld()
        {
            return isHeld;
        }

        /// <summary>
        /// Called once when button released
        /// </summary>
        /// <returns></returns>
        public bool OnReleased()
        {
            return isReleased;
        }

        /// <summary>
        /// Called once when will pass time long button pressed
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public bool OnLongPress(float time)
        {
            if (!save)
            {
                cacheTime = time;
                save = true;
            }
            if (isHeld)
            {
                if (longPressCalled)
                    return false;

                cacheTime -= Time.deltaTime;
                if (cacheTime <= 0)
                {
                    longPressCalled = true;
                    return true;
                }
                else
                {
                    return false;
                }

            }
            else
            {
                save = false;
                longPressCalled = false;
                return false;
            }
        }

        /// <summary>
        /// Reset Pressed and Released state in false
        /// </summary>
        /// <returns></returns>
        public IEnumerator ResetState()
        {
            yield return new WaitForSeconds(0.0001f);
            isPressed = false;
            isReleased = false;
            yield break;
        }
    }
}