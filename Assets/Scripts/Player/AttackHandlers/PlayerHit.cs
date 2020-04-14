using UnityEngine;
using TDH.Stats;
using TDH.Combat;
using TDH.EnemyAI;

namespace TDH.Player
{
    public class PlayerHit : MonoBehaviour
    {
        private float damage = 0f;
        private float hitPower = 5f;

        private GameObject player;
        private PlayerFighter fighter;
        private Collider boxCollider;

        private void Awake() 
        {
            player = GameObject.FindGameObjectWithTag("Player");
            fighter = player.GetComponent<PlayerFighter>();
            boxCollider = player.transform.Find("MeeleAttackArea").GetComponent<BoxCollider>();

            fighter.OnHit += ActivateCollider;
            fighter.OnAttackFinished += DeactivateCollider;
            fighter.OnWeaponChange += ChangeHitStats;
        }

        private void Start() 
        {
            DeactivateCollider();    
        }

        private void ChangeHitStats(Weapon weapon)
        {
            damage = weapon.GetWeaponDamage();
            hitPower = weapon.GetHitPower();
        }

        private void ActivateCollider()
        {
            boxCollider.enabled = true;
        }

        private void DeactivateCollider()
        {
            boxCollider.enabled = false;
        }

        private void OnTriggerEnter(Collider other) 
        {
            if (other.gameObject.CompareTag("Enemy"))
            {
                other.gameObject.transform.GetComponent<IEnemy>().SetHitVelocity(player.transform.forward, hitPower);
                other.gameObject.transform.GetComponent<Health>().DecreaseHealth(damage);
            }
        }

    }
}