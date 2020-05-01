using UnityEngine;

public class OpeningGateBehavior : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] Transform standingPoint = null;
    [SerializeField] Transform firstGateTransformPoint = null, secondGateTransformPoint = null;
    [Header("Values")]
    [SerializeField] float openingSpeed = 0.1f;
    [SerializeField] float openingTime = 2f;
    [Header("UI")]
    [SerializeField] GameObject openButton = null;

    private float timeToStopOpening = 0.0f;

    private bool isOpening = false;

    private void Start() 
    {
        openButton.SetActive(false);
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
            isOpening = false;
    }

    public void StartOpening()
    {
        transform.GetComponent<BoxCollider>().enabled = false;
        openButton.SetActive(false);
        timeToStopOpening = Time.time + openingTime;
        isOpening = true;
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
