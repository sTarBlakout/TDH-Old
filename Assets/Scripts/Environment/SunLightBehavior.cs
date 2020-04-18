using TDH.Player;
using TDH.UI;
using UnityEngine;

namespace TDH.Environment
{
    public class SunLightBehavior : MonoBehaviour
    {
        [SerializeField] float healStep = 0.1f;

        private bool healPlayer = false;

        private GameObject player;
        private PlayerLightController lightController;

        private UIManager managerUI = null;

        private void Awake() 
        {
            player = GameObject.FindGameObjectWithTag("Player");    
            lightController = player.GetComponent<PlayerLightController>();
            managerUI = GameObject.Find("UI").GetComponent<UIManager>();
        }

        private void FixedUpdate() 
        {
            if (healPlayer)
            {
                lightController.Heal(healStep);
            }
        }

        private void OnTriggerEnter(Collider other) 
        {
            if (other.gameObject.CompareTag("Player"))
            {
                healPlayer = true;
                managerUI.ActivateInventoryButton(true);
            }    
        }

        private void OnTriggerExit(Collider other) 
        {
            if (other.gameObject.CompareTag("Player"))
            {
                healPlayer = false;
                managerUI.ActivateInventoryButton(false);
            } 
        }
    }
}