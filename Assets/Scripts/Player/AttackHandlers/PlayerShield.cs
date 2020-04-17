using TDH.Stats;
using TDH.EnemyAI;
using System.Collections;
using UnityEngine;

namespace TDH.Player
{
    public class PlayerShield : MonoBehaviour
    {
        [SerializeField] GameObject hitParticle = null;
        [SerializeField] float damage;
        [SerializeField] float hitPower;
        [SerializeField] float barrierPower;

        private void OnTriggerStay(Collider other) 
        {
            if (other.gameObject.CompareTag("Enemy"))
            {
                other.gameObject.GetComponent<EnemyBehaviorAI>().SetHitVelocity(-other.transform.forward, 1);
                other.gameObject.GetComponent<Health>().DecreaseHealth(0f);
            }    
        }

        public GameObject GetHitParticle()
        {
            return hitParticle;
        }

        public float GetHitPower()
        {
            return hitPower;
        }

        public float GetDamage()
        {
            return damage;
        }
    }
}