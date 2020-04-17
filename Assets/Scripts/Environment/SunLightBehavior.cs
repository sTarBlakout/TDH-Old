using TDH.Player;
using UnityEngine;

namespace TDH.Environment
{
    public class SunLightBehavior : MonoBehaviour
    {
        [SerializeField] float healStep = 0.1f;

        private bool healPlayer = false;

        private GameObject player;
        private PlayerLightController lightController;

        private void Awake() 
        {
            player = GameObject.FindGameObjectWithTag("Player");    
            lightController = player.GetComponent<PlayerLightController>();
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
            }    
        }

        private void OnTriggerExit(Collider other) 
        {
            if (other.gameObject.CompareTag("Player"))
            {
                healPlayer = false;
            } 
        }
    }
}