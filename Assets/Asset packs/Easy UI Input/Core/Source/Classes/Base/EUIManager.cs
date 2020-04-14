/* =========================================================
   ---------------------------------------------------
   Project   :    Easy UI Input
   Publisher :    Infinite Dawn
   Author    :    Tamerlan Shakirov
   ---------------------------------------------------
   Copyright © Infinite Dawn 2018 All rights reserved.
   ========================================================== */

using EasyUIInput.Utility;
using System.Collections.Generic;
using UnityEngine;

namespace EasyUIInput
{
    /// <summary>
    /// Base Easy UI Input runtime manager
    /// </summary>
    public class EUIManager : Singleton<EUIManager>
    {
        [SerializeField] private List<AxisHandler> axis = new List<AxisHandler>();
        [SerializeField] private List<ButtonHandler> buttons = new List<ButtonHandler>();

        /// <summary>
        /// Return all registered axis on the scene
        /// </summary>
        /// <returns></returns>
        public List<AxisHandler> GetAxis()
        {
            return axis;
        }

        /// <summary>
        /// Return all registered buttons on the scene
        /// </summary>
        /// <returns></returns>
        public List<ButtonHandler> GetButtons()
        {
            return buttons;
        }
    }
}