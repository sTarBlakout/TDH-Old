using UnityEngine;
using TDH.Stats;
using TDH.Combat;
using TDH.EnemyAI;
using System.Collections.Generic;

namespace TDH.Player
{
    public class PlayerContinuousSpellHit : MonoBehaviour
    {
        private float selfDamage = 0.5f;
        private float damage = 2f;
        private float hitPower = 1f;
        private float damageRate = 1f;

        private float nextDamageTime = 0f;

        private GameObject player;
        private PlayerFighter fighter;
        private PlayerInventory inventory;
        private PlayerLightController lightController;
        private Collider boxCollider;

        private List<Collider> collList = new List<Collider>();

        private void Awake() 
        {
            player = GameObject.FindGameObjectWithTag("Player");
            lightController = player.GetComponent<PlayerLightController>();
            fighter = player.GetComponent<PlayerFighter>();
            inventory = player.GetComponent<PlayerInventory>();
            boxCollider = player.transform
                .Find("Root/Hips/Spine_01/Spine_02/Spine_03/Clavicle_L/Shoulder_L/Elbow_L/Hand_L/SpellArea")
                .GetComponent<BoxCollider>();

            inventory.OnSpellEquip += ChangeSpellStats;

            fighter.OnCastStarted += ActivateCollider;
            fighter.OnCastFinished += DeactivateCollider;
        }

        private void Start() 
        {
            DeactivateCollider();    
        }

        private void Update() 
        {
            if (fighter.GetCurrentSpell() == null) return;
            if (fighter.GetCurrentSpell().GetSpellType() != 0 || boxCollider.enabled == false)
            {
                return;
            }
            if (Time.time > nextDamageTime)
            {
                nextDamageTime = Time.time + damageRate;
                foreach (Collider other in collList)
                {
                    other.gameObject.transform.GetComponent<IEnemy>().SetHitVelocity(player.transform.forward, hitPower);
                    other.gameObject.transform.GetComponent<Health>().DecreaseHealth(damage);
                }
            }
        }

        private void FixedUpdate() 
        {
            if (fighter.GetCurrentSpell() == null) return;
            if (fighter.GetCurrentSpell().GetSpellType() != 0 || boxCollider.enabled == false)
            {
                return;
            }
            lightController.DealDamage(selfDamage);
        }

        private void OnTriggerEnter(Collider other) 
        {
            if (other.gameObject.CompareTag("Enemy"))
            {
                if (!collList.Contains(other)) 
                {   
                    collList.Add(other);   
                }
            }
        }

        private void OnTriggerExit(Collider other) 
        {
            if (collList.Contains(other)) 
            {   
                collList.Remove(other);   
            }
        }

        private void ChangeSpellStats(Spell spell)
        {
            selfDamage = spell.GetSelfDamage();
            damage = spell.GetSpellDamage();
            hitPower = spell.GetHitPower();
            damageRate = spell.GetDamageRate();
        }

        private void ActivateCollider()
        {
            collList.Clear();
            boxCollider.enabled = true;
        }

        private void DeactivateCollider()
        {
            collList.Clear();
            boxCollider.enabled = false;
        }
    }
}