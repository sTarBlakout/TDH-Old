using UnityEngine;

public class InitializeGame : MonoBehaviour
{
    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 30;
    }
}
