using System;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace TDH.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/New Weapon", order = 0)]
    public class Weapon : ScriptableObject 
    {
        [SerializeField] GameObject weaponPrefab = null;
        [SerializeField] GameObject weaponBackPrefab = null;
        [SerializeField] GameObject weaponBackCoverPrefab = null;
        [SerializeField] GameObject lootPointViewPref;
        [SerializeField] AnimatorOverrideController animatorOverride = null;
        [SerializeField] bool hasTrail = false;
        [SerializeField] WeaponType weaponType;
        [SerializeField] float damage = 0f;
        [SerializeField] float attackSpeed = 1f;
        [SerializeField] float hitPower = 0f;
        [SerializeField] float rangeX = 1f, rangeZ = 1f;
        [SerializeField] AttackSeries[] attackSeries;
        [Header("Powerfull Attack")]
        [SerializeField] bool hasPowerfullAttack = true;
        [SerializeField] int chargesAmount = 1;
        [SerializeField] GameObject damageAreaPrefab;
        [SerializeField] NoiseSettings cameraPowerNoisePreset;
        [SerializeField] GameObject moveParticlePref;
        [SerializeField] GameObject chargingParticlePref;
        [SerializeField] GameObject splashParticlePref;
        [SerializeField] GameObject effectOvertimePrefab;
        [SerializeField] float chargeTime;
        [SerializeField] float timeOffsetBeforeMove;
        [SerializeField] float powerfullDamage;
        [SerializeField] float powerfullHit;
        [SerializeField] float powerfullCooldown;
        [SerializeField] float moveForwardDist;
        [Header("Camera Shaking")]
        [SerializeField] NoiseSettings cameraNoisePreset;
        [SerializeField] float shackingAmp = 0f;
        [SerializeField] float shackingFreq = 0f;
        [SerializeField] float distChanging = 0f;

        public float MoveForwardDist { get => moveForwardDist; }

        public GameObject LootPointViewPref { get => lootPointViewPref; }

        public GameObject Equip(Transform handTransform, Animator animator, BoxCollider attackArea, 
            Dictionary<int, Dictionary<int, int>> attackSer)
        {
            foreach (AttackSeries atkSer in attackSeries)
            {
                int i = 0;
                Dictionary<int, int> temp = new  Dictionary<int, int>();
                foreach (int attackID in atkSer.attackSequence)
                {
                    i++;
                    temp.Add(i, attackID);
                }
                attackSer.Add(atkSer.id, temp);
            }

            attackArea.size = new Vector3(rangeX, 1f, rangeZ);
        
            if (weaponPrefab != null)
            {
                GameObject weapon =  Instantiate(weaponPrefab, handTransform);
                weapon.name = weaponPrefab.name;
                return weapon;
            }
            else
                return null;
        }

        public float GetPowerfullDamage()
        {
            return powerfullDamage;
        }

        public int GetChargesAmount()
        {
            return chargesAmount;
        }

        public bool HasTrail()
        {
            return hasTrail;
        }
        
        public float GetPowerfullHit()
        {
            return powerfullHit;
        }

        public GameObject GetDamageAreaPrefab()
        {
            return damageAreaPrefab; 
        }

        public GameObject GetWeaponPrefab()
        {
            return weaponPrefab;
        }

        public float GetPowChargeTime()
        {
            return chargeTime;
        }
        
        public float GetPowTimeOffset()
        {
            return timeOffsetBeforeMove;
        }

        public AnimatorOverrideController GetAnimatorController()
        {
            return animatorOverride;
        }

        public float GetAmplitudeVC()
        {
            return shackingAmp;
        }

        public float GetFrequencyVC()
        {
            return shackingFreq;
        }

        public float GetDistanceVC()
        {
            return distChanging;
        }

        public float GetWeaponDamage()
        {
            return damage;
        }

        public float GetAttackSpeed()
        {
            return attackSpeed;
        }

        public float GetHitPower()
        {
            return hitPower;
        }

        public bool HavePrefab()
        {
            if (weaponPrefab == null)
                return false;
            else
                return true;
        }

        public GameObject GetBackPrefab()
        {
            return weaponBackPrefab;
        }

        public GameObject GetBackCoverPrefab()
        {
            return weaponBackCoverPrefab;
        }

        public NoiseSettings GetNoisePreset()
        {
            return cameraNoisePreset;
        }

        public NoiseSettings GetPowerNoisePreset()
        {
            return cameraPowerNoisePreset;
        }

        public int GetWeaponType()
        {
            return (int)weaponType;
        }

        public GameObject GetMovePowParticle()
        {
            return moveParticlePref;
        }

        public GameObject GetChargePowParticle()
        {
            return chargingParticlePref;
        }

        public GameObject GetSplashPowParticle()
        {
            return splashParticlePref;
        }

        public GameObject GetEffectOvertime()
        {
            return effectOvertimePrefab;
        }

        public float GetPowerfullCooldown()
        {
            return powerfullCooldown;
        }

        public bool HasPowerfullAttack()
        {
            return hasPowerfullAttack;
        }
    }

    [Serializable]
    class AttackSeries
    {
        public int id;
        public int[] attackSequence;
    }
}