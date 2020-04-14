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
    /// <summary>
    /// Easy UI Input standard button interface
    /// </summary>
    public interface IButton
    {
        /// <summary>
        /// Button name
        /// </summary>
        /// <returns></returns>
        string GetName();

        /// <summary>
        /// Called once when button pressed
        /// </summary>
        /// <returns></returns>
        bool OnPressed();

        /// <summary>
        /// Called every frame while button held
        /// </summary>
        /// <returns></returns>
        bool OnHeld();

        /// <summary>
        /// Called once when button released
        /// </summary>
        /// <returns></returns>
        bool OnReleased();

        /// <summary>
        /// Called once when will pass time long button pressed
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        bool OnLongPress(float time);
    }
}