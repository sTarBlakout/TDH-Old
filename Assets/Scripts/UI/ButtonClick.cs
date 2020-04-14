using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TDH.UI
{
    public class ButtonClick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private bool pointerDown;
        private float pointerDownTimer;

        [SerializeField]
        private float requiredHoldTime;

        public UnityEvent onLongClick;
        public UnityEvent onButtonUp;
        public UnityEvent onButtonDown;

        public void OnPointerDown(PointerEventData eventData)
        {
            onButtonDown.Invoke();
            pointerDown = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            onButtonUp.Invoke();
            Reset();
        }

        private void Update()
        {
            if (pointerDown)
            {
                pointerDownTimer += Time.deltaTime;
                if (pointerDownTimer >= requiredHoldTime)
                {
                    if (onLongClick != null)
                        onLongClick.Invoke();

                    Reset();
                }
            }
        }

        private void Reset()
        {
            pointerDown = false;
            pointerDownTimer = 0;
        }

    }
}