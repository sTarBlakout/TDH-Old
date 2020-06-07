using Cinemachine;
using UnityEngine;
using System.Collections;

namespace TDH.Player
{
    public class PlayerCinemachineCamera : MonoBehaviour
    {
        [SerializeField] CinemachineVirtualCamera virtualCamera;
        [SerializeField] NoiseSettings defaultNoiseSetting;
        [SerializeField] float stepShackingAmp = 0f;
        [SerializeField] float stepShackingFreq = 0f;
        [SerializeField] float stepDistChanging = 0f;
        [SerializeField] float maxShackingAmp = 0f;
        [SerializeField] float maxShackingFreq = 0f;
        [SerializeField] float minDistChanging = 0f;
        [SerializeField] float defShackingAmp = 0f;
        [SerializeField] float defShackingFreq = 0f;
        [SerializeField] float defDistChanging = 0f;

        private bool isAmpDefault = true;
        private bool isFreqDefault = true;
        private bool isCamDistDefault = true;
        private bool isCameraDefaultState = true;
        private bool isChangingCameraDist = false;

        private CinemachineBasicMultiChannelPerlin noise;
        private CinemachineFramingTransposer dist;

        private void Awake() 
        {
            noise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            dist = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();    
        }

        public void ChangeVirtualCameraStat(float value, byte operationID, byte statID)
        {
            if (operationID == 1) //=
            {
                switch (statID)
                {
                    case 1:
                    {
                        noise.m_AmplitudeGain = value;
                        break;
                    }
                    case 2:
                    {
                        noise.m_FrequencyGain = value;
                        break;
                    }
                    case 3:
                    {
                        dist.m_CameraDistance = value;
                        break;
                    }
                    default:
                    {
                        Debug.Log("Wrong stat ID for Virtual Camera!");
                        break;
                    }
                }
            }
            else if (operationID == 2) //+
            {
                switch (statID)
                {
                    case 1:
                    {
                        noise.m_AmplitudeGain += value;
                        break;
                    }
                    case 2:
                    {
                        noise.m_FrequencyGain += value;
                        break;
                    }
                    case 3:
                    {
                        dist.m_CameraDistance += value;
                        break;
                    }
                    default:
                    {
                        Debug.Log("Wrong stat ID for Virtual Camera!");
                        break;
                    }
                }
            }
            else if (operationID == 3) //-
            {   
                switch (statID)
                {
                    case 1:
                    {
                        noise.m_AmplitudeGain -= value;
                        break;
                    }
                    case 2:
                    {
                        noise.m_FrequencyGain -= value;
                        break;
                    }
                    case 3:
                    {
                        dist.m_CameraDistance -= value;
                        break;
                    }
                    default:
                    {
                        Debug.Log("Wrong stat ID for Virtual Camera!");
                        break;
                    }
                }
            }
            else
            {
                Debug.Log("Wrong operation ID for Virtual Camera!");
                return;
            }

            FalseVCameraParameters();
        }

        public void ChangeVirtCamStatsForAttack(float stepAmp, float stepFreq, float stepDist)
        {
            if (noise.m_AmplitudeGain < maxShackingAmp)
                ChangeVirtualCameraStat(stepAmp, 2, 1);
            if (noise.m_FrequencyGain < maxShackingFreq)
                ChangeVirtualCameraStat(stepFreq, 2, 2);
            if (dist.m_CameraDistance > minDistChanging)
                ChangeVirtualCameraStat(stepDist, 3, 3);
        }

        public void SmoothlyResetCamStats(float amplitude, float frequency, float camDist)
        {
            if (!isAmpDefault)
            {
                if (noise.m_AmplitudeGain > defShackingAmp + 0.1f)
                    noise.m_AmplitudeGain -= amplitude;
                else if (noise.m_AmplitudeGain < defShackingAmp)
                    noise.m_AmplitudeGain += amplitude;
                else
                    isAmpDefault = true;
            }
            if(!isFreqDefault)
            {
                if (noise.m_FrequencyGain > defShackingFreq + 0.1f)
                    noise.m_FrequencyGain -= frequency;
                else if (noise.m_FrequencyGain < defShackingFreq)
                    noise.m_FrequencyGain += frequency;
                else
                    isFreqDefault = true;
            }
            if (!isCamDistDefault)
            {
                if (dist.m_CameraDistance < defDistChanging - 0.5f)
                    dist.m_CameraDistance += camDist;
                else if (dist.m_CameraDistance > defDistChanging)
                    dist.m_CameraDistance -= camDist;
                else
                    isCamDistDefault = true;
            }

            if (isAmpDefault && isFreqDefault && isCamDistDefault)
            {
                isCameraDefaultState = true;
            }
        }

        public void SetCameraNoice(NoiseSettings preset, float amp, float freq, float time)
        {
            noise.m_NoiseProfile = preset;
            noise.m_AmplitudeGain = amp;
            noise.m_FrequencyGain = freq;

            StartCoroutine(StartResetCamInSeconds(time));
        }

        private IEnumerator StartResetCamInSeconds(float delay)
        {
            yield return new WaitForSeconds(delay);
            FalseVCameraParameters();
        }

        private void FalseVCameraParameters()
        {
            isAmpDefault = false;
            isFreqDefault = false;
            isCamDistDefault = false;
            isCameraDefaultState = false;
        }

        public float GetDefAmp()
        {
            return defShackingAmp;
        }

        public float GetDefFreq()
        {
            return defShackingFreq;
        }

        public float GetDefDist()
        {
            return defDistChanging;
        }

        public bool IsCameraDefaultState()
        {
            return isCameraDefaultState;
        }

        public bool IsChangingCameraDist()
        {
            return isChangingCameraDist;
        }

        public void SetIsChangingCameraDist(bool isChanging)
        {
            isChangingCameraDist = isChanging;
        }

        public float GetCameraDistanceFromPlayer()
        {
            return dist.m_CameraDistance;
        }

        public void SetDefaultNoiseSettings()
        {
            noise.m_NoiseProfile = defaultNoiseSetting;
        }

        public void SetCustomNoiseSettings(NoiseSettings settings)
        {
            noise.m_NoiseProfile = settings;
        }

        public NoiseSettings GetDefaultNoiseSettings()
        {
            return defaultNoiseSetting;
        }
    }
}