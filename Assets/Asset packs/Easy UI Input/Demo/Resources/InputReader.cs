/* =========================================================
   ---------------------------------------------------
   Project   :    Next-Gen FPS
   Publisher :    Infinite Dawn
   Author    :    Tamerlan Shakirov
   ---------------------------------------------------
   Copyright © Infinite Dawn 2017 - 2018 All rights reserved.
   ========================================================== */

using UnityEngine;
using UnityEngine.UI;

namespace EasyUIInput
{
    public class InputReader : MonoBehaviour
    {
        [SerializeField] private Text axis;
        [SerializeField] private Text axisRaw;
        [SerializeField] private Text buttonState;
        [SerializeField] private Text button;

        private void Update()
        {
            UITextHandler();
        }

        private void UITextHandler()
        {
            axis.text = string.Format("Axis: [Vertical: {0}], [Horizontal: {1}]", EUI.GetAxis("DemoAxis", AxisType.Vertical).ToString("0.00"), EUI.GetAxis("DemoAxis", AxisType.Horizontal).ToString("0.00"));
            axisRaw.text = string.Format("Axis Raw: [Vertical: {0}], [Horizontal: {1}]", EUI.GetAxisRaw("DemoAxis", AxisType.Vertical), EUI.GetAxisRaw("DemoAxis", AxisType.Horizontal));
            button.text = string.Format("Button Held: {0}", EUI.GetButton("DemoButton"));
            
            if (EUI.GetButtonDown("DemoButton")) { buttonState.text = "Button Down Detected (Last State)"; }
            if (EUI.GetButtonUp("DemoButton")) { buttonState.text = "Button Up Detected (Last State)"; }
            if (EUI.GetButtonLongPress("DemoButton", 1.0f)) { buttonState.text = "Button Long Press Detected (1.0 sec) (Last State)"; }

            if (EUI.GetButtonDown("DemoButton2")) { buttonState.text = "Button2 Down Detected (Last State)"; }
            if (EUI.GetButtonUp("DemoButton2")) { buttonState.text = "Button2 Up Detected (Last State)"; }
            if (EUI.GetButtonLongPress("DemoButton2", 1.0f)) { buttonState.text = "Button2 Long Press Detected (1.0 sec) (Last State)"; }
        }
    }
}