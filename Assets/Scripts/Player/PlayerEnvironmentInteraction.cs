using TDH.Environment;
using TDH.UI;
using UnityEngine;

namespace TDH.Player
{
    public class PlayerEnvironmentInteraction : MonoBehaviour
    {
        private UIManager managerUI = null;
        private PlayerLightController lightController = null;
        private PlayerMover mover = null;

        private Animator animator = null;

        private SunLightBehavior sunLight = null;

        private void Awake() 
        {
            managerUI = GameObject.Find("UI").GetComponent<UIManager>();
            lightController = transform.GetComponent<PlayerLightController>();
            mover = transform.GetComponent<PlayerMover>();
            animator = transform.GetComponent<Animator>();
        }

        private void OnTriggerEnter(Collider other) 
        {
            if (other.gameObject.CompareTag("SunShine"))
            {
                sunLight = other.gameObject.GetComponent<SunLightBehavior>();
                managerUI.ActivateButtonControlPanel(4, true);
            }    
        }

        private void OnTriggerExit(Collider other) 
        {
            if (other.gameObject.CompareTag("SunShine"))
            {
                managerUI.ActivateButtonControlPanel(4, false);
                sunLight = null;
            } 
        }

        public void StartMeditation()
        {
            mover.RestrictMovement();
            animator.SetTrigger("StartSunMeditation");
            if (sunLight != null)
            {
                sunLight.ActivateVirtCamera(true);
            }
            managerUI.ActivatePanel(1, false);
            managerUI.ActivatePanel(2, true);
            lightController.HealFull();
        }
    }
}