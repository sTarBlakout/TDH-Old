using TDH.Environment;
using TDH.UI;
using UnityEngine;
using System;

namespace TDH.Player
{
    public class PlayerEnvironmentInteraction : MonoBehaviour
    {
        [SerializeField] Transform leftHandTransform = null;
        [SerializeField] Cloth capeCloth = null;

        public Action OnMeditationStart = null, OnMeditationFinish = null;

        private UIManager managerUI = null;
        private PlayerLightController lightController = null;
        private PlayerMover mover = null;

        private Animator animator = null;

        private SunLightBehavior sunLight = null;

        private bool lookAtPointSunshineSet = false;
        private bool isMeditating = false;

        private Transform backWeaponHolder = null;

        private void Awake() 
        {
            managerUI = GameObject.Find("UI").GetComponent<UIManager>();
            lightController = transform.GetComponent<PlayerLightController>();
            mover = transform.GetComponent<PlayerMover>();
            animator = transform.GetComponent<Animator>();
            backWeaponHolder = transform.Find("Root/Hips/Spine_01/Spine_02/Spine_03/WeaponContainer");
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
                    lightController.HealFullMeditation();
                }
            }
        }

        private void OnTriggerEnter(Collider other) 
        {
            if (other.gameObject.CompareTag("SunShine"))
            {
                sunLight = other.gameObject.GetComponent<SunLightBehavior>();
                sunLight.OnMeditationStarted += StartMeditation;
            }    
        }

        private void OnTriggerExit(Collider other) 
        {
            if (other.gameObject.CompareTag("SunShine"))
            {
                sunLight = null;
                other.gameObject.GetComponent<SunLightBehavior>().OnMeditationStarted -= StartMeditation;
            } 
        }

        private void ResetEnemies()
        {
            foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
            {
                Destroy(enemy);
            }
        }

        public void StartMeditation()
        {
            if (isMeditating) return;
            isMeditating = true;
            OnMeditationStart();
            lookAtPointSunshineSet = false;
            backWeaponHolder.transform.Rotate(new Vector3(0, 0, 28), Space.Self);
            ResetEnemies();
            if (capeCloth != null)
            {
                capeCloth.externalAcceleration = new Vector3(0, 0, -10);
                capeCloth.randomAcceleration = new  Vector3(5, 0, 0);
            }
        }

        public void StopMeditation()
        {
            OnMeditationFinish();
            animator.SetTrigger("StopSunMeditation");
            backWeaponHolder.transform.Rotate(new Vector3(0, 0, -28), Space.Self);
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
            isMeditating = false;
        }
    }
}