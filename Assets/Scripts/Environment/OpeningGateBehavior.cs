using System.Collections;
using UnityEngine;
using TDH.Player;
using Cinemachine;

namespace TDH.Environment
{
    public class OpeningGateBehavior : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] Transform standingPoint = null;
        [SerializeField] Transform firstGateTransformPoint = null, secondGateTransformPoint = null;
        [SerializeField] ParticleSystem[] paticlesOnOpening = null;
        [SerializeField] NoiseSettings camNoise = null;

        [Header("Values")]
        [SerializeField] int openKeyID;
        [SerializeField] float openingSpeed = 0.1f;
        [SerializeField] float openingTime = 2f;
        [SerializeField] float delayAfterActivating = 1f;
        [SerializeField] float delayBeforeStartMove = 1f;

        [Header("UI")]
        [SerializeField] GameObject openButton = null;

        private PlayerMover playerMover = null;
        private PlayerInventory playerInventory = null;
        private PlayerCinemachineCamera playerCam = null;

        private float timeToStopOpening = 0.0f;

        private bool isOpening = false;

        private void Awake() 
        {
            playerMover = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMover>();
            playerInventory = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>();
            playerCam = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCinemachineCamera>();
        }

        private void Start() 
        {
            openButton.SetActive(false);
            OpeningParticlePlay(false);
        }

        private void FixedUpdate() 
        {
            if (isOpening)
                OpeningProdcedure();
        }

        private void OpeningProdcedure()
        {
            firstGateTransformPoint.Translate(-transform.right * openingSpeed, Space.World);
            secondGateTransformPoint.Translate(transform.right * openingSpeed, Space.World);
            if (Time.time > timeToStopOpening)
            {
                isOpening = false;
                OpeningParticlePlay(false);
                playerCam.SetCameraNoice(playerCam.GetDefaultNoiseSettings(), 0f, 0f, 0f);
            }
        }

        private void OpeningParticlePlay(bool play)
        {
            foreach (ParticleSystem particle in paticlesOnOpening)
            {
                if (play)
                    particle.Play();
                else
                    particle.Stop();
            }
        }

        public void StartOpeningSequence()
        {
            Item key = playerInventory.GetKey(openKeyID);
            if (key == null) return;
            playerInventory.RemoveItem(key);
            transform.GetComponent<BoxCollider>().enabled = false;
            openButton.SetActive(false);
            if (playerMover != null)
                playerMover.ForceMove(standingPoint.position, standingPoint.rotation, "Activate");
            StartCoroutine(StartOpeningCor());
        }

        private IEnumerator StartOpeningCor()
        {
            yield return new WaitForSeconds(delayAfterActivating);
            playerCam.SetCameraNoice(camNoise, 1f, 1f, openingTime);
            timeToStopOpening = Time.time + openingTime;
            isOpening = true;
            OpeningParticlePlay(true);
            yield return new WaitForSeconds(delayBeforeStartMove);
            playerMover.AllowMove();
        }

        private void OnTriggerEnter(Collider other) 
        {
            if (other.gameObject.CompareTag("Player"))    
            {
                openButton.transform.LookAt(Camera.main.transform);
                openButton.SetActive(true);
            }
        }

        private void OnTriggerExit(Collider other) 
        {
            if (other.gameObject.CompareTag("Player"))    
            {
                openButton.SetActive(false);
            }   
        }
    }
}