using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using TDH.UI;
using TDH.Stats;
using TDH.EnemyAI;

namespace TDH.Player
{
    public class PlayerLightController : MonoBehaviour
    {
        [SerializeField] float initialLightHealth = 100f;
        [SerializeField] float decreasingStepLight = 0.1f;
        [SerializeField] float minLightRange;
        [SerializeField] float lightsOutThreshold = 1f;
        private float currentLightHealth;
        private float targetLighthHealth;

        private float healthChangeStepDecrease = 1;
        private float meditationIncreasingStep = 1;

        [SerializeField] Light[] lightsArea, lightsCharacter;
        [SerializeField] ParticleSystem[] particlesToStopWhenDie;
        [SerializeField] Vector2 minMaxIntensityArea, minMaxIntensityCharacter;
        [SerializeField] Vector2 minMaxTimeThreshold;
        
        private float maxLightRange;

        private float lastTimeIntensityChanged = 0f;
        private float timeToNextChange = 0f;

        private float rangeTarget;
        private float damageHealthRatio;

        private bool isDead = false;
        private bool isHealingFullMeditation = false;

        private UIManager managerUI = null;
        private PlayerCinemachineCamera cinemachineCam = null;

        private void Awake() 
        {
            managerUI = GameObject.Find("UI").GetComponent<UIManager>(); 
            cinemachineCam = this.transform.GetComponent<PlayerCinemachineCamera>();
        }

        private void Start() 
        {
            currentLightHealth = initialLightHealth;   
            targetLighthHealth = initialLightHealth;  
            managerUI.SetInitialHealthValue(initialLightHealth);
            rangeTarget = lightsArea[0].range;
            maxLightRange = rangeTarget;
        }

        void Update()
        {
            if (Time.time > lastTimeIntensityChanged + timeToNextChange)    
            {
                lastTimeIntensityChanged = Time.time;
                timeToNextChange = Random.Range(minMaxTimeThreshold.x, minMaxTimeThreshold.y);
                float randomIntensity = Random.Range(minMaxIntensityArea.x, minMaxIntensityArea.y);
                foreach (Light light in lightsArea)
                {
                    light.intensity = randomIntensity;
                }
                randomIntensity = Random.Range(minMaxIntensityCharacter.x, minMaxIntensityCharacter.y);
                foreach (Light light in lightsCharacter)
                {
                    light.intensity = randomIntensity;
                }
            }
        }

        private void FixedUpdate() 
        {
            if (isHealingFullMeditation)
            {   
                if (targetLighthHealth < initialLightHealth)
                {
                    targetLighthHealth = Mathf.Clamp(targetLighthHealth + meditationIncreasingStep, 0f, initialLightHealth);
                    managerUI.UpgradeTargetHealthBar(targetLighthHealth);
                }
                else
                {
                    isHealingFullMeditation = false;
                }
            }
            if (currentLightHealth != targetLighthHealth)
            {
                if (currentLightHealth < targetLighthHealth)
                {
                    currentLightHealth = targetLighthHealth;
                }
                else 
                {
                    currentLightHealth -= healthChangeStepDecrease;
                }
                managerUI.UpgradeCurrentHealthBar(currentLightHealth);
            }
            if (lightsArea[0].range > rangeTarget)
            {
                foreach(Light light in lightsArea)
                {
                    light.range -= decreasingStepLight;
                }
                foreach (Light light in lightsCharacter)
                {
                    light.range -= decreasingStepLight;
                }
            }    
            if (isDead)
            {
                cinemachineCam.SmoothlyResetCamStats(0.1f, 0.1f, 0.1f);
            }
        }

        public void HealFullMeditation()
        {
            if (isDead) return;
            if (targetLighthHealth == initialLightHealth) return;
            meditationIncreasingStep = (initialLightHealth - targetLighthHealth) * 0.01f;
            isHealingFullMeditation = true;
        }

        public void DealDamage(float damage)
        {   
            if (isDead) return;
            if (damage >= targetLighthHealth)
            {
                targetLighthHealth = 0;
                Die();
            }
            else
            {
                targetLighthHealth -= damage;
                rangeTarget = currentLightHealth / initialLightHealth * (maxLightRange - minLightRange) + minLightRange;
            }
            healthChangeStepDecrease = (currentLightHealth - targetLighthHealth) * 0.05f;
            managerUI.UpgradeTargetHealthBar(targetLighthHealth);
        }

        private void Die()
        {
            if (isDead) return;
            isDead = true;
            this.transform.GetComponent<NavMeshAgent>().enabled = false;
            Collider[] enemiesInShieldRange = Physics.OverlapSphere(this.transform.position, 2);
            foreach(Collider enemy in enemiesInShieldRange)
            {
                if (enemy.gameObject.CompareTag("Enemy"))
                {
                    enemy.gameObject.GetComponent<EnemyBehaviorAI>().SetHitVelocity(-enemy.transform.forward, 4);
                    enemy.gameObject.GetComponent<Health>().DecreaseHealth(0f);
                }
            }
            this.transform.GetComponent<PlayerFighter>().enabled = false;
            this.transform.GetComponent<PlayerMover>().enabled = false;
            this.transform.GetComponent<Animator>().SetTrigger("Die");
            IEnumerable<IEnemy> enemies = FindObjectsOfType<MonoBehaviour>().OfType<IEnemy>();
            foreach (IEnemy enemy in enemies)
            {
                enemy.PlayerDie();
                StartCoroutine(LightsOutThreshold());
            }
        }

        private IEnumerator LightsOutThreshold()
        {
            yield return new WaitForSeconds(lightsOutThreshold);
            foreach(ParticleSystem system in particlesToStopWhenDie)
            {
                system.Stop();
            }
            rangeTarget = 0f;
        }
    }
}