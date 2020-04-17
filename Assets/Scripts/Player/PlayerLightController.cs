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
        [SerializeField] float decreasingStep = 0.1f;
        [SerializeField] float minLightRange;
        [SerializeField] float lightsOutThreshold = 1f;
        private float currentLightHealth;

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
            if (lightsArea[0].range > rangeTarget)
            {
                foreach(Light light in lightsArea)
                {
                    light.range -= decreasingStep;
                }
                foreach (Light light in lightsCharacter)
                {
                    light.range -= decreasingStep;
                }
            }    
            if (isDead)
            {
                cinemachineCam.SmoothlyResetCamStats(0.1f, 0.1f, 0.1f);
            }
        }

        public void Heal(float amount)
        {
            if (isDead) return;
            if (currentLightHealth == initialLightHealth) return;
            currentLightHealth = Mathf.Clamp(currentLightHealth + amount, 0f, initialLightHealth);
            managerUI.UpgradeHealthBar(currentLightHealth);
        }

        public void DealDamage(float damage)
        {   
            if (isDead) return;
            if (damage >= currentLightHealth)
            {
                currentLightHealth = 0;
                Die();
            }
            else
            {
                currentLightHealth -= damage;
                rangeTarget = currentLightHealth / initialLightHealth * (maxLightRange - minLightRange) + minLightRange;
            }
            managerUI.UpgradeHealthBar(currentLightHealth);
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