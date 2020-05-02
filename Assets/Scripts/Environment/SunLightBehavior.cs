using TDH.Player;
using TDH.UI;
using UnityEngine;
using Cinemachine;
using System.Collections;
using System;

namespace TDH.Environment
{
    public class SunLightBehavior : MonoBehaviour
    {
        [SerializeField] GameObject startMedButton = null;

        [SerializeField] float cameraMoveStep = 0.05f;

        [SerializeField] ParticleSystem[] particlesMeditationStarted;

        // private GameObject player;
        // private PlayerLightController lightController = null;
        private PlayerMover playerMover = null;

        private UIManager managerUI = null;

        private GameObject virtualCameraGO = null;
        private CinemachineTrackedDolly virtualCameraTrackedDolly = null;
        private bool isCameraActive = false;

        private Transform lookAtPoint = null;
        private Transform playerStandPosition = null;

        private Coroutine activateCameraThreshold = null;

        public Action OnMeditationStarted;

        private void Awake() 
        {
            managerUI = GameObject.Find("UI").GetComponent<UIManager>();
            virtualCameraGO = transform.Find("Camera").Find("VirtualCamera").gameObject;
            if (virtualCameraGO != null)
            {
                virtualCameraTrackedDolly = virtualCameraGO.GetComponent<CinemachineVirtualCamera>()
                    .GetCinemachineComponent<CinemachineTrackedDolly>();
            }
            lookAtPoint = transform.Find("LookAtPoint");
            playerStandPosition = transform.Find("PlayerStandPosition");
            playerMover = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMover>();
        }

        private void Start() 
        {
            startMedButton.SetActive(false);
            virtualCameraGO.SetActive(false);    
            ActivateParticlesMeditation(false);
        }

        private void FixedUpdate() 
        {
            if (isCameraActive)
            {
                virtualCameraTrackedDolly.m_PathPosition += cameraMoveStep;
            }
        }

        private IEnumerator ActivateCameraThreshold()
        {
            yield return new WaitForSeconds(2f);
            isCameraActive = true;
        }

        public void ActivateVirtCamera(bool activate)
        {
            virtualCameraGO.SetActive(activate);
            if (activate)
            {
                activateCameraThreshold = StartCoroutine(ActivateCameraThreshold());
            }
            else
            {   
                if (activateCameraThreshold != null)
                {
                    StopCoroutine(activateCameraThreshold);
                    activateCameraThreshold = null;
                }
                isCameraActive = false;
            }
        }

        public void SetLookAtPointPosition(Vector3 pos)
        {
            lookAtPoint.position = pos;
        }

        public Transform GetPlayerStandPosition()
        {
            return playerStandPosition;
        }

        public void ActivateParticlesMeditation(bool activate)
        {
            foreach (ParticleSystem particle in particlesMeditationStarted)
            {
                if (activate)
                    particle.Play();
                else
                    particle.Stop();
            }
        }

        public void StartMeditation()
        {
            startMedButton.SetActive(false);
            playerMover.ForceMove(playerStandPosition.position, playerStandPosition.rotation, "StartSunMeditation");
            if (OnMeditationStarted != null)
                OnMeditationStarted();
        }

        private void OnTriggerEnter(Collider other) 
        {
            if (other.gameObject.CompareTag("Player"))
            {
                startMedButton.transform.LookAt(Camera.main.transform);
                startMedButton.SetActive(true);
            }
        }

        private void OnTriggerExit(Collider other) 
        {
            if (other.gameObject.CompareTag("Player"))
            {
                startMedButton.SetActive(false);
            }
        }
    }
}