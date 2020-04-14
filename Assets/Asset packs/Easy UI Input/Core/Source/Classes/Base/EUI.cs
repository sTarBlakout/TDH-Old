/* =========================================================
   ---------------------------------------------------
   Project   :    Easy UI Input
   Publisher :    Infinite Dawn
   Author    :    Tamerlan Shakirov
   ---------------------------------------------------
   Copyright © Infinite Dawn 2018 All rights reserved.
   ========================================================== */

namespace EasyUIInput
{
    public class EUI
    {
        public static EUIManager inputManager = EUIManager.Instance;

        /// <summary>
        /// Get axis float value
        /// </summary>
        /// <param name="name"></param>
        /// <param name="axisType"></param>
        /// <returns>Axis value</returns>
        public static float GetAxis(string name, AxisType axisType)
        {
            if (inputManager == null || inputManager.GetAxis().Count == 0)
                return 0;

            for (int i = 0; i < inputManager.GetAxis().Count; i++)
            {
                if (name == inputManager.GetAxis()[i].GetName())
                {
                    return inputManager.GetAxis()[i].OnAxis(axisType);
                }
            }
            return 0;
        }

        /// <summary>
        /// Get axis int value
        /// </summary>
        /// <param name="name"></param>
        /// <param name="axisType"></param>
        /// <returns>Axis value</returns>
        public static int GetAxisRaw(string name, AxisType axisType)
        {
            if (inputManager == null || inputManager.GetAxis().Count == 0)
                return 0;

            for (int i = 0; i < inputManager.GetAxis().Count; i++)
            {
                if (name == inputManager.GetAxis()[i].GetName())
                {
                    return inputManager.GetAxis()[i].OnAxisRaw(axisType);
                }
            }
            return 0;
        }

        /// <summary>
        /// Called once when button pressed
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Button state</returns>
        public static bool GetButtonDown(string name)
        {
            if (inputManager == null || inputManager.GetButtons().Count == 0)
                return false;

            for (int i = 0; i < inputManager.GetButtons().Count; i++)
            {
                if (name == inputManager.GetButtons()[i].GetName())
                {
                    return inputManager.GetButtons()[i].OnPressed();
                }
            }
            return false;
        }

        /// <summary>
        /// Called every frame while button held
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Button state</returns>
        public static bool GetButton(string name)
        {
            if (inputManager == null || inputManager.GetButtons().Count == 0)
                return false;

            for (int i = 0; i < inputManager.GetButtons().Count; i++)
            {
                if(name == inputManager.GetButtons()[i].GetName())
                {
                    return inputManager.GetButtons()[i].OnHeld();
                }
            }
            return false;
        }

        /// <summary>
        /// Called once when button released
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Button state</returns>
        public static bool GetButtonUp(string name)
        {
            if (inputManager == null || inputManager.GetButtons().Count == 0)
                return false;

            for (int i = 0; i < inputManager.GetButtons().Count; i++)
            {
                if (name == inputManager.GetButtons()[i].GetName())
                {
                    return inputManager.GetButtons()[i].OnReleased();
                }
            }
            return false;
        }

        /// <summary>
        /// Called once when will pass time long button pressed
        /// </summary>
        /// <param name="name"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public static bool GetButtonLongPress(string name, float time)
        {
            if (inputManager == null || inputManager.GetButtons().Count == 0)
                return false;

            for (int i = 0; i < inputManager.GetButtons().Count; i++)
            {
                if (name == inputManager.GetButtons()[i].GetName())
                {
                    return inputManager.GetButtons()[i].OnLongPress(time);
                }
            }
            return false;
        }
    }
}