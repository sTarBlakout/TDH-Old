using UnityEngine;
using Cinemachine;

namespace TDH.Combat
{
    [CreateAssetMenu(fileName = "Spell", menuName = "Spells/New Spell", order = 0)]
    public class Spell : ScriptableObject 
    {
        [Header("Required Fields")]
        [SerializeField] GameObject spellPrefab; 
        [SerializeField] GameObject lootPointViewPref;
        [SerializeField] AnimatorOverrideController animatorOverride = null;
        [SerializeField] float timeToInstantiateSpell;
        [SerializeField] float damage;
        [SerializeField] float selfDamage;
        [SerializeField] float hitPower;
        [SerializeField] float cooldown;
        [SerializeField] SpellType spellType;
        [Header("Camera Shaking")]
        [SerializeField] NoiseSettings cameraNoisePreset;
        [SerializeField] float amplitude;
        [SerializeField] float frequency;
        [Header("FlameThrower")]
        [SerializeField] float damageRate;
        [SerializeField] float maxTimeUsing;
        [Header("AOE")]
        [SerializeField] float radius = 1f;

        public GameObject LootPointViewPref { get => lootPointViewPref; }

        public GameObject Cast(Transform transform) 
        {   
            return Instantiate(spellPrefab, transform);
        }

        public AnimatorOverrideController GetAnimatorController()
        {
            return animatorOverride;
        }

        public float GetTime()
        {
            return timeToInstantiateSpell;
        }

        public float GetRadius()
        {
            return radius;
        }

        public float GetSpellDamage()
        {
            return damage;
        }

        public float GetHitPower()
        {
            return hitPower;
        }

        public float GetDamageRate()
        {
            return damageRate;
        }

        public int GetSpellType()
        {
            return (int)spellType;
        }

        public NoiseSettings GetNoisePreset()
        {
            return cameraNoisePreset;
        }

        public float GetCamAmplitude()
        {
            return amplitude;
        }

        public float GetCamFrequency()
        {
            return frequency;
        }

        public float GetCooldown()
        {
            return cooldown;
        }
        
        public float GetMaxTimeUsing()
        {
            return maxTimeUsing;
        }

        public float GetSelfDamage()
        {
            return selfDamage;
        }
    }
}