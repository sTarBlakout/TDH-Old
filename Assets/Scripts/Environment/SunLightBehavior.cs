using TDH.Player;
using TDH.UI;
using UnityEngine;
using Cinemachine;

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

        public void ActivateVirtCamera(bool activate)
        {
            virtualCameraGO.SetActive(activate);
            isCameraActive = true;
        }
    }
}