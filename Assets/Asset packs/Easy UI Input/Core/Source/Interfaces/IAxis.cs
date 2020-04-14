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
    /// Easy UI Input axis interface
    /// </summary>
    public interface IAxis
    {
        /// <summary>
        /// Axis name
        /// </summary>
        /// <returns></returns>
        string GetName();

        /// <summary>
        /// Called every frame while axis touched
        /// </summary>
        /// <param name="type"></param>
        /// <returns>Axis float value</returns>
        float OnAxis(AxisType axis);

        /// <summary>
        /// Called every frame while axis touched
        /// </summary>
        /// <param name="type"></param>
        /// <returns>Axis int value</returns>
        int OnAxisRaw(AxisType axis);
    }
}