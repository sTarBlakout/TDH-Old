/* =========================================================
   ---------------------------------------------------
   Project   :    Easy UI Input
   Publisher :    Infinite Dawn
   Author    :    Tamerlan Shakirov
   ---------------------------------------------------
   Copyright © Infinite Dawn 2018 All rights reserved.
   ========================================================== */

using UnityEngine;
using UnityEditor;

namespace EasyUIInput.Editor
{
    public class MenuItems
    {
        public const string DOC_URL = "https://docs.google.com/document/d/1NCsqweteGMF4ABj_VipU1g7_8h0wQtMG2uWPV6Dy3n4/edit?usp=sharing";

        [MenuItem("Easy UI Input/About", false, 0)]
        private static void About()
        {
            AboutWindow.Open();
        }

        [MenuItem("Easy UI Input/Documentation", false, 1)]
        private static void Documentation()
        {
            Application.OpenURL(DOC_URL);
        }


    }
}