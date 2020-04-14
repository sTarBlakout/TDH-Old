using TDH.Combat;
using TDH.Stats;
using UnityEngine;
using TDH.EnemyAI;

namespace TDH.Player
{
    public class PlayerAOESpellHit : MonoBehaviour
    {
        private float damage = 0f;
        public float selfDamage = 0f;
        private float hitPower = 0f;

        private GameObject player;
        private PlayerFighter fighter;
        private PlayerInventory inventory;
        private PlayerLightController lightController;
        private SphereCollider sphereCollider;

        private void Awake() 
        {
            player = GameObject.FindGameObjectWithTag("Player");
            lightController = player.GetComponent<PlayerLightController>();
            fighter = player.GetComponent<PlayerFighter>();
            inventory = player.GetComponent<PlayerInventory>();
            sphereCollider = player.transform.Find("AOEDamgeArea").GetComponent<SphereCollider>();

            inventory.OnSpellEquip += ChangeHitStats;
            fighter.OnCauseSpellDamage += ActivateCollider;
            fighter.OnCastFinished += DeactivateCollider;
        }

        private void Start() 
        {
            DeactivateCollider();
        }

        private void ChangeHitStats(Spell spell)
        {
            selfDamage = spell.GetSelfDamage();
            damage = spell.GetSpellDamage();
            hitPower = spell.GetHitPower();
            sphereCollider.radius = spell.GetRadius();
        }

        private void ActivateCollider()
        {
            sphereCollider.enabled = true;
            lightController.DealDamage(selfDamage);
        }

        private void DeactivateCollider()
        {
            sphereCollider.enabled = false;
        }

        private void OnTriggerEnter(Collider other) 
        {
            if (fighter.GetCurrentSpell().GetSpellType() != 1)
            {
                return;
            }
            if (other.gameObject.CompareTag("Enemy"))
            {              
                Vector3 dir = other.gameObject.transform.position - transform.position;
                other.gameObject.transform.GetComponent<IEnemy>().SetHitVelocity(dir.normalized, hitPower);
                other.gameObject.transform.GetComponent<Health>().DecreaseHealth(damage);
            }
        }
    }
}
