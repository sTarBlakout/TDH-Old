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