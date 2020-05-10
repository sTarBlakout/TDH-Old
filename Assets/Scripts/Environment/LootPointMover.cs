using UnityEngine;

public class LootPointMover : MonoBehaviour
{
    private const float rotateStep = 360f;
    private const float upDownSpeed = 1.2f;

    [SerializeField] GameObject lootViewPrefab;
    [SerializeField] float rotateSpeed;
    [SerializeField] float height;

    private bool isLootTaken = false;
    private Transform lootPoint;

    private void Awake() 
    {
        lootPoint = this.transform.GetChild(1);
    }

    void Start()
    {
        Instantiate(lootViewPrefab, lootPoint);
        LeanTween.moveY(lootPoint.gameObject, height, upDownSpeed).setLoopPingPong();
        LeanTween.rotateAround(lootPoint.gameObject, Vector3.up, rotateStep, rotateSpeed).setLoopClamp();
    }
}
