using TDH.Environment;
using TDH.UI;
using UnityEngine;

namespace TDH.Player
{
    public class PlayerEnvironmentInteraction : MonoBehaviour
    {
        [SerializeField] Transform leftHandTransform = null;
        [SerializeField] Cloth capeCloth = null;

        private UIManager managerUI = null;
        private PlayerLightController lightController = null;
        private PlayerMover mover = null;

        private Animator animator = null;

        private SunLightBehavior sunLight = null;

        private bool lookAtPointSunshineSet = false;

        private void Awake() 
        {
            managerUI = GameObject.Find("UI").GetComponent<UIManager>();
            lightController = transform.GetComponent<PlayerLightController>();
            mover = transform.GetComponent<PlayerMover>();
            animator = transform.GetComponent<Animator>();
        }

        private void Update() 
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("SunMeditation") && !lookAtPointSunshineSet)
            {
                lookAtPointSunshineSet = true;
                if (sunLight != null)
                {
                    sunLight.SetLookAtPointPosition(leftHandTransform.position);
                    sunLight.ActivateVirtCamera(true);
                    managerUI.ActivatePanel(2);
                    sunLight.ActivateParticlesMeditation(true);
                }
            }
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
            lookAtPointSunshineSet = false;
            if (sunLight != null)
            {
                Transform playerStandPos = sunLight.GetPlayerStandPosition();
                mover.ForceMove(playerStandPos.position, playerStandPos.rotation, "StartSunMeditation");
            }
            if (capeCloth != null)
            {
                capeCloth.externalAcceleration = new Vector3(0, 0, -10);
                capeCloth.randomAcceleration = new  Vector3(5, 0, 0);
            }
            lightController.HealFull();
        }

        public void StopMeditation()
        {
            animator.SetTrigger("StopSunMeditation");
            if (sunLight != null)
            {
                sunLight.ActivateVirtCamera(false);
                sunLight.ActivateParticlesMeditation(false);
            }
            if (capeCloth != null)
            {
                capeCloth.externalAcceleration = new Vector3(0, 0, 0);
                capeCloth.randomAcceleration = new  Vector3(5, 0, 3);
            }
            mover.AllowMove();
            managerUI.ActivatePanel(1);
        }
    }
}