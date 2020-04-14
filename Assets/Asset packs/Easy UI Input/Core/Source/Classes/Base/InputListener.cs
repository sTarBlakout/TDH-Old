/* =========================================================
   ---------------------------------------------------
   Project   :    Easy UI Input
   Publisher :    Infinite Dawn
   Author    :    Tamerlan Shakirov
   ---------------------------------------------------
   Copyright © Infinite Dawn 2018 All rights reserved.
   ========================================================== */

using UnityEngine;
using UnityEngine.Events;

namespace EasyUIInput
{
	public class InputListener : MonoBehaviour
	{
		[Header("Button")]
		[SerializeField] private ButtonHandler buttonHandler;
		[SerializeField] private float longPressInterval;
		[Header("Events")]
		[SerializeField] UnityEvent pressedEvent;
		[SerializeField] UnityEvent heldEvent;
		[SerializeField] UnityEvent releasedEvent;
		[SerializeField] UnityEvent longPressEvent;

		private void Update()
		{
			if (buttonHandler.OnPressed()) { pressedEvent.Invoke(); }
			if (buttonHandler.OnHeld()) { heldEvent.Invoke(); }
			if (buttonHandler.OnReleased()) { releasedEvent.Invoke(); }
			if (buttonHandler.OnLongPress(longPressInterval)) { longPressEvent.Invoke(); }
		}
	}
}