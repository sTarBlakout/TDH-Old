using TDH.Player;
using TDH.UI;
using UnityEngine;
using Cinemachine;
using System.Collections;

namespace TDH.Environment
{
    public class SunLightBehavior : MonoBehaviour
    {
        [SerializeField] float cameraMoveStep = 0.05f;

        private GameObject player;
        private PlayerLightController lightController;

        private UIManager managerUI = null;

        private GameObject virtualCameraGO = null;
        private CinemachineTrackedDolly virtualCameraTrackedDolly = null;
        private bool isCameraActive = false;

        private Transform lookAtPoint = null;
        private Transform playerStandPosition = null;

        private Coroutine activateCameraThreshold = null;

        private void Awake() 
        {
            player = GameObject.FindGameObjectWithTag("Player");    
            lightController = player.GetComponent<PlayerLightController>();
            managerUI = GameObject.Find("UI").GetComponent<UIManager>();
            virtualCameraGO = transform.Find("VirtualCamera").gameObject;
            if (virtualCameraGO != null)
            {
                virtualCameraTrackedDolly = virtualCameraGO.GetComponent<CinemachineVirtualCamera>()
                    .GetCinemachineComponent<CinemachineTrackedDolly>();
            }
            lookAtPoint = transform.Find("LookAtPoint");
            playerStandPosition = transform.Find("PlayerStandPosition");
        }

        private void Start() 
        {
            virtualCameraGO.SetActive(false);    
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
    }
}