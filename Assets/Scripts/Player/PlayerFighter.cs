using UnityEngine;
using System;
using System.Collections;
using TDH.Combat;
using TDH.EnemyAI;
using TDH.Stats;
using TDH.UI;
using System.Collections.Generic;
using System.Linq;

namespace TDH.Player
{
    public class PlayerFighter : MonoBehaviour
    {
        private PlayerInventory inventory;
        private PlayerMover mover;
        private PlayerCinemachineCamera playerCamera;
        private PlayerEnvironmentInteraction playerEnvironment;
        private UIManager managerUI;
        private Animator animator;

        [Header("Attack Behavior")]
        [SerializeField] GameObject shieldParticlePrefab;
        [SerializeField] GameObject secondShieldParticlePrefab;
        [SerializeField] float attackVelocityBoost = 1f;
        [SerializeField] float timeToHideWeapon = 5f;
        [SerializeField] float timeToStopAttackSeries = 2f;
        [SerializeField] float shieldTimeLimit = 1f;
        [SerializeField] float shieldCooldown = 2f;
        private IEnumerator shieldTimerCoroutine;
        private GameObject shieldParticle;
        private GameObject secondShieldParticle;
        private ParticleSystem weaponTrailParticle = null, weaponChargeMedParticle = null;
        private int chargesLeft = 0;
        private BoxCollider attackArea;
        private bool isBlocking = false;
        private bool isShieldInCooldown = false;
        private bool isAttackGoing = false;
        private bool isPowerfullAtkInCooldown = false;
        private bool finishedAttackSequence = true;
        private bool calledFinishAttack = false;
        private int attackType = 1, lastAttackID = 0, lastAttackSeriesID = 1;
        private float remainingTimeToStopAttackSeries = 0f;
        private IEnumerator powAttackCoroutine;
        private Dictionary<int, Dictionary<int, int>> attackSeries = new Dictionary<int, Dictionary<int, int>>();
        public Action OnAttackStarted;
        public Action<bool> OnHit;
        public Action OnAttackFinished;
        public Action OnPowerfullAttackStarted;
        public Action OnBlockStarted;
        public Action OnBlockFinished;

        [Header("Weapon Stuff")]
        [SerializeField] Weapon currentWeapon = null;
        [SerializeField] Weapon backWeapon = null;
        [SerializeField] Transform rightHand = null;
        [SerializeField] Transform leftHand = null;
        [SerializeField] GameObject particleChangWepMed = null;
        private GameObject currentWeaponPrefab;
        private Weapon newWeaponToChange = null;
        private Transform backWeaponHolder;
        private Transform backWeaponCoverHolder;
        private bool isCalledToHideWeapon = false;
        [SerializeField] private bool isWeaponInHands = false;
        private bool isMeditating = false;
        private float remainingTimeToHideWeapon = 0f;
        public Action<Weapon> OnWeaponChange;

        [Header("Magic Stuff")]
        [SerializeField] Spell currentSpell;
        private GameObject runtimeSpell = null;
        private GameObject shieldCenter = null;
        private bool isCastGoing = false;
        private bool isSpellInCooldown = false;
        private IEnumerator spellCoroutine;
        private IEnumerator stopCastCoroutine;
        private GameObject chargeParticle;
        public Action OnCastStarted;
        public Action OnCastFinished;
        public Action OnCauseSpellDamage;


    #region Unity Methods

        private void Awake() 
        {
            animator = GetComponent<Animator>();
            mover = GetComponent<PlayerMover>();
            inventory = GetComponent<PlayerInventory>();
            managerUI = GameObject.Find("UI").GetComponent<UIManager>();
            playerCamera = GetComponent<PlayerCinemachineCamera>();
            playerEnvironment = GetComponent<PlayerEnvironmentInteraction>();
            attackArea = transform.Find("MeeleAttackArea").GetComponent<BoxCollider>();
            shieldCenter = transform.Find("ShieldCenter").gameObject;
            backWeaponHolder = transform.Find("Root/Hips/Spine_01/Spine_02/Spine_03/WeaponContainer/BackWeapon"); 
            backWeaponCoverHolder = transform.Find("Root/Hips/Spine_01/Spine_02/Spine_03/WeaponContainer/WeaponCover");

            inventory.OnWeaponEquip += ChangeBackWeapon;
            inventory.OnSpellEquip += ChangeSpell;
            playerEnvironment.OnMeditationStart += MeditationStartProcessing;
            playerEnvironment.OnMeditationFinish += MeditationFinishProcessing;

        }

        private void Start() 
        {
            EquipWeapon(currentWeapon);
            playerCamera.ChangeVirtualCameraStat(playerCamera.GetDefAmp(), 1, 1);
            playerCamera.ChangeVirtualCameraStat(playerCamera.GetDefFreq(), 1, 2);
            playerCamera.ChangeVirtualCameraStat(playerCamera.GetDefDist(), 1, 3);
            if (currentWeapon.name == "Unarmed")
            {
                isWeaponInHands = false;
            }
        }

        private void FixedUpdate()
        {
            //Just normal restoring camera state
            if (finishedAttackSequence && !playerCamera.IsCameraDefaultState() && !isCastGoing)
            {
                playerCamera.SmoothlyResetCamStats(0.02f, 0.02f, 0.1f);
            }
            //Decreasing camera distance for AOE spell
            if (playerCamera.IsChangingCameraDist() && playerCamera.GetCameraDistanceFromPlayer() > 5f)
            {
                if (isAttackGoing)
                {
                    playerCamera.ChangeVirtualCameraStat(currentWeapon.GetAmplitudeVC(), 3, 3);
                }
                else if (isCastGoing)
                {
                    playerCamera.ChangeVirtualCameraStat(0.4f, 3, 3);
                }
            }
        }

        private void Update() 
        {
            if (!finishedAttackSequence)
            {
                CheckForAttackInactivityAtkSeriesFinish();
            }

            if (!isCalledToHideWeapon && isWeaponInHands && !isBlocking)
            {
                CheckForAttackInactivityWeaponHide();
            }
        }

    #endregion

    #region Public Methods

        public float GetAtkVelBoost()
        {
            return attackVelocityBoost;
        }

        public void StartBlock()
        {
            if (isBlocking || isShieldInCooldown) return;
            isBlocking = true;
            OnBlockStarted();
            managerUI.ActivateActionSlider(shieldTimeLimit, SliderType.STATE_CASTING, true);
            shieldTimerCoroutine = ShieldCastTimer();
            StartCoroutine(shieldTimerCoroutine);
            IEnumerable<IEnemy> enemies = FindObjectsOfType<MonoBehaviour>().OfType<IEnemy>();
            foreach (IEnemy enemy in enemies)
            {
                enemy.ActivatedPlayerShield(true);
            }
            animator.SetBool("Block", true);
            Transform paticleTransform = this.transform.Find("ShieldCenter").transform;
            shieldCenter.GetComponent<SphereCollider>().enabled = true;
            shieldParticle = Instantiate(shieldParticlePrefab, paticleTransform);
            secondShieldParticle = Instantiate(secondShieldParticlePrefab, paticleTransform);
            Collider[] enemiesInShieldRange = Physics.OverlapSphere(this.transform.position, shieldParticle.transform.localScale.x);
            foreach(Collider enemy in enemiesInShieldRange)
            {
                if (enemy.gameObject.CompareTag("Enemy"))
                {
                    enemy.gameObject.GetComponent<EnemyBehaviorAI>().SetHitVelocity(-enemy.transform.forward, 5);
                    enemy.gameObject.GetComponent<Health>().DecreaseHealth(0f);
                }
            }
        }

        public void StopBlock()
        {
            if (!isBlocking) return;
            isBlocking = false;
            isShieldInCooldown = true;
            StartCoroutine(ShieldCooldown());
            if (shieldTimerCoroutine != null)
            {
                StopCoroutine(shieldTimerCoroutine);
                shieldTimerCoroutine = null;
            }
            managerUI.DeactivateActionSlider();
            IEnumerable<IEnemy> enemies = FindObjectsOfType<MonoBehaviour>().OfType<IEnemy>();
            foreach (IEnemy enemy in enemies)
            {
                enemy.ActivatedPlayerShield(false);
            }
            remainingTimeToHideWeapon = Time.time + timeToHideWeapon;
            animator.SetBool("Block", false);
            shieldCenter.GetComponent<SphereCollider>().enabled = false;
            if (shieldParticle != null && secondShieldParticle != null)
            {
                secondShieldParticle.transform.parent = null;
                Destroy(shieldParticle);
                secondShieldParticle.GetComponent<ParticleSystem>().Stop();
                shieldParticle = null;
                secondShieldParticle = null;
            }
            OnBlockFinished();
        }

        //Called by UI button Spell
        public void StartCast()
        {
            if (isAttackGoing || isCastGoing || isBlocking || currentSpell == null || isSpellInCooldown)
            {
                return;
            }
            isCastGoing = true;
            OnCastStarted();

            animator.SetBool("Casting", true);

            switch (currentSpell.GetSpellType())
            {
                case 0:
                {
                    stopCastCoroutine = StopCastTimer(currentSpell.GetMaxTimeUsing());
                    managerUI.ActivateActionSlider(currentSpell.GetMaxTimeUsing(), SliderType.STATE_CASTING, true);
                    StartCoroutine(stopCastCoroutine);

                    playerCamera.SetCameraNoice(
                        currentSpell.GetNoisePreset(), 
                        currentSpell.GetCamAmplitude(), 
                        currentSpell.GetCamFrequency());
                    runtimeSpell = currentSpell.Cast(leftHand);
                    break;
                }
                case 1:
                {
                    managerUI.ActivateActionSlider(currentSpell.GetTime(), SliderType.STATE_CHARGING, false);
                    playerCamera.SetIsChangingCameraDist(true);
                    break;
                }

                default: break;
            }

            spellCoroutine = InstantiateSpellInSeconds(currentSpell, currentSpell.GetTime());
            StartCoroutine(spellCoroutine);
        }

        public void StopCast()
        {   
            if (!isCastGoing)
            {
                return;
            }

            if (stopCastCoroutine != null)
            {
                StopCoroutine(stopCastCoroutine);
                stopCastCoroutine = null;
            }

            managerUI.DeactivateActionSlider();
            StopCoroutine(spellCoroutine);
            isCastGoing = false;

            switch (currentSpell.GetSpellType())
            {
                case 0:
                {
                    StartCoroutine(SpellCooldown(currentSpell.GetCooldown()));
                    runtimeSpell.transform.GetComponent<ParticleSystem>().Stop();
                    playerCamera.ChangeVirtualCameraStat(1.5f, 1, 1);
                    playerCamera.ChangeVirtualCameraStat(1.5f, 1, 2);
                    break;
                }
                case 1:
                {
                    playerCamera.ChangeVirtualCameraStat(playerCamera.GetDefDist(), 1, 3);
                    break;
                }

                default: break;
            }

            runtimeSpell = null;
            animator.SetBool("Casting", false);
            playerCamera.SetIsChangingCameraDist(false);
            playerCamera.SetDefaultNoiseSettings();
            OnCastFinished();
        }

        //Called by UI button Attack
        public void Attack()
        {
            if (isAttackGoing || isCastGoing || isBlocking) 
            {
                return;
            }

            remainingTimeToHideWeapon = Time.time + timeToHideWeapon;
            remainingTimeToStopAttackSeries = Time.time + timeToStopAttackSeries;

            isAttackGoing = true;
            isCalledToHideWeapon = false;

            if (currentWeapon.name == "Unarmed" && backWeapon != null)
            {
                TakeHideBackWeapon(true);
                return;
            }

            if (weaponTrailParticle != null)
            {
                weaponTrailParticle.Play();
            }

            OnAttackStarted();

            attackType = GetAttackID();
            animator.SetInteger("Attack_ID", attackType);
            animator.SetTrigger("Attack");

            StartCoroutine(NoSlowDwonThreshold());
        }

        public void PowerfullAttack()
        {          
            if (isAttackGoing || isCastGoing || isBlocking || isPowerfullAtkInCooldown || chargesLeft == 0 || backWeapon == null)
            {
                return;
            }
            if (currentWeapon.name == "Unarmed" && backWeapon != null)
            {
                if (!backWeapon.HasPowerfullAttack())
                {
                    return;
                }
            }

            mover.RestrictMovement();

            remainingTimeToHideWeapon = Time.time + timeToHideWeapon;
            remainingTimeToStopAttackSeries = Time.time + timeToStopAttackSeries;

            isAttackGoing = true;
            isCalledToHideWeapon = false;

            if (currentWeapon.name == "Unarmed" && backWeapon != null)
            {
                TakeHideBackWeapon(true);
                return;
            }

            OnPowerfullAttackStarted();
            
            animator.SetBool("ChargingPowerfullAttack", true);

            switch (currentWeapon.GetWeaponType())
            {
                case 1:
                {
                    playerCamera.SetIsChangingCameraDist(true);
                    chargeParticle = Instantiate(currentWeapon.GetChargePowParticle(), this.transform);
                    chargeParticle.transform.parent = null;
                    break;
                }
                case 2:
                {
                    chargeParticle = Instantiate(
                        currentWeapon.GetChargePowParticle(), 
                        GameObject.Find("AxeChargeArea").transform);
                    playerCamera.SetIsChangingCameraDist(true);
                    break;
                }
                default: break;
            }

            powAttackCoroutine = PowerfullAttackCharging(currentWeapon.GetPowChargeTime());
            StartCoroutine(powAttackCoroutine);
        }

        public Spell GetCurrentSpell()
        {
            return currentSpell;
        }

        public Weapon GetCurrentWeapon()
        {
            return currentWeapon;
        }

        public bool IsBlocking()
        {
            return isBlocking;
        }

        public void StartAttackFinishCoroutine()
        {   
            managerUI.DeactivateActionSlider();
            playerCamera.SetIsChangingCameraDist(false);
            if (chargeParticle != null)
            {
                if (!chargeParticle.GetComponent<ParticleSystem>().isStopped)
                {
                    chargeParticle.GetComponent<ParticleSystem>().Stop();
                }
            }
            if (powAttackCoroutine != null)
            {   
                StopCoroutine(powAttackCoroutine);   
            }
            if ((!calledFinishAttack && isAttackGoing))
            {   
                StartCoroutine(AttackFinish());
            }
        }

    #endregion

    #region Private Methods

        private IEnumerator PowerfullAttackCooldown(float time)
        {
            managerUI.StartCooldown(CooldownType.POWERFUL_ATTACK, time);
            yield return new WaitForSeconds(time);
            isPowerfullAtkInCooldown = false;
        }

        private IEnumerator ShieldCooldown()
        {
            managerUI.StartCooldown(CooldownType.SHIELD, shieldCooldown);
            yield return new WaitForSeconds(shieldCooldown);
            isShieldInCooldown = false;
        }

        private IEnumerator SpellCooldown(float time)
        {
            isSpellInCooldown = true;
            managerUI.StartCooldown(CooldownType.CAST_SPELL, time);
            yield return new WaitForSeconds(time);
            isSpellInCooldown = false;
        }

        private IEnumerator StopCastTimer(float timeToStop)
        {
            yield return new WaitForSeconds(timeToStop);
            StopCast();
        }

        private IEnumerator ShieldCastTimer()
        {
            yield return new WaitForSeconds(shieldTimeLimit);
            StopBlock();
        }

        private IEnumerator PowerfullAttackCharging(float time)
        {
            managerUI.ActivateActionSlider(time, SliderType.STATE_CHARGING, false);
            yield return new WaitForSeconds(time);
            animator.SetTrigger("PowerAttack");
            yield return new WaitForSeconds(currentWeapon.GetPowTimeOffset());
            GameObject damageArea = Instantiate(currentWeapon.GetDamageAreaPrefab(), this.transform.position, this.transform.rotation);
            switch (currentWeapon.GetWeaponType())
            {
                case 1:
                {
                    this.transform.position = damageArea.transform.GetChild(0).position;
                    GameObject temp = Instantiate(currentWeapon.GetMovePowParticle(), transform);
                    temp.transform.parent = null;
                    if (chargeParticle != null)
                    {
                        chargeParticle.GetComponent<ParticleSystem>().Stop();
                    }
                    playerCamera.SetIsChangingCameraDist(false);
                    playerCamera.ChangeVirtualCameraStat(2.5f, 1, 1);
                    playerCamera.ChangeVirtualCameraStat(2.5f, 1, 2);
                    playerCamera.ChangeVirtualCameraStat(playerCamera.GetDefDist(), 1, 3);
                    break;
                }
                case 2:
                {
                    GameObject temp1 = Instantiate(
                        currentWeapon.GetMovePowParticle(), 
                        currentWeaponPrefab.transform.GetChild(0).transform);
                    GameObject temp2 = Instantiate(currentWeapon.GetSplashPowParticle(), this.transform);
                    temp2.transform.parent = null;
                    temp2 = Instantiate(currentWeapon.GetEffectOvertime(), this.transform);
                    temp2.transform.parent = null;
                    if (chargeParticle != null)
                    {
                        chargeParticle.GetComponent<ParticleSystem>().Stop();
                    }
                    playerCamera.SetIsChangingCameraDist(false);
                    playerCamera.ChangeVirtualCameraStat(4, 1, 1);
                    playerCamera.ChangeVirtualCameraStat(4, 1, 2);
                    playerCamera.ChangeVirtualCameraStat(playerCamera.GetDefDist(), 1, 3);
                    animator.SetFloat("AttackSpeed", 2);
                    yield return new WaitForSeconds(0.1f);
                    temp1.transform.parent = null; 
                    break;
                }
                default: yield break;
            }
            isPowerfullAtkInCooldown = true;
            chargesLeft -= 1;
            StartCoroutine(PowerfullAttackCooldown(currentWeapon.GetPowerfullCooldown()));
            StartAttackFinishCoroutine();
        }

        private void ToggleWeaponMedParticle()
        {
            if (weaponChargeMedParticle != null)
            {
                if (weaponChargeMedParticle.isPlaying)
                {
                    weaponChargeMedParticle.Stop();
                }
                else
                {
                    weaponChargeMedParticle.Play();
                }
            }
        }

        private void MeditationStartProcessing()
        {
            if (!isWeaponInHands)
            {
                TakeHideBackWeapon(true);    
            }
            isMeditating = true;
            ToggleWeaponMedParticle();
        }

        private void MeditationFinishProcessing()
        {
            isMeditating = false;
            if (isWeaponInHands)
            {
                TakeHideBackWeapon(false);
            }
            chargesLeft = currentWeapon.GetChargesAmount();
            ToggleWeaponMedParticle();
        }

        private void ChangeBackWeapon(Weapon weapon)
        {
            ChangeBackWeaponCover(weapon);

            if (!isMeditating)
            {
                if (!isWeaponInHands)
                {
                    backWeapon = weapon;
                    HideShowBackWeapon();
                    newWeaponToChange = null;
                }
                else
                {
                    newWeaponToChange = weapon;
                    TakeHideBackWeapon(false);
                }
            }
            else
            {
                ClearHand();
                if (weapon.name == "Unarmed")
                {
                    backWeapon = null;
                    isWeaponInHands = false;
                }
                else
                {
                    backWeapon = inventory.GetWeapon("Unarmed");
                    isWeaponInHands = true;
                }
                currentWeapon = weapon;
                if (particleChangWepMed.activeSelf)
                    particleChangWepMed.SetActive(false);
                particleChangWepMed.SetActive(true);
                EquipWeapon(currentWeapon);
            }
        }

        private void ClearHand()
        {
            if (rightHand.childCount > 4)
            {
                Transform temp = rightHand.GetChild(4);
                if (temp != null)
                {
                    Destroy(temp.gameObject);
                }
            }
        }

        private void ChangeSpell(Spell spell)
        {
            currentSpell = spell;
            animator.runtimeAnimatorController = CreateMixedController();
        }

        private IEnumerator InstantiateSpellInSeconds(Spell spell, float seconds)
        {

            yield return new WaitForSeconds (seconds);
            if (isCastGoing)
            {
                switch (spell.GetSpellType())
                {
                    case 0:
                    {   
                        break;
                    }
                    case 1:
                    {
                        StartCoroutine(SpellCooldown(currentSpell.GetCooldown()));
                        playerCamera.SetIsChangingCameraDist(false);
                        playerCamera.ChangeVirtualCameraStat(playerCamera.GetDefDist(), 1, 3);
                        runtimeSpell = currentSpell.Cast(transform);
                        runtimeSpell.transform.SetParent(null);
                        OnCauseSpellDamage();
                        yield return new WaitForSeconds(0.05f);
                        StopCast();
                        break;
                    }
                    default: break;
                }
            }
        }

        private IEnumerator AttackFinish()
        {
            calledFinishAttack = true;
            animator.SetBool("ChargingPowerfullAttack", false);
            yield return new WaitForSeconds(0.2f);
            animator.SetFloat("AttackSpeed", currentWeapon.GetAttackSpeed());
            OnAttackFinished();
            if (weaponTrailParticle != null)
            {
                weaponTrailParticle.Stop();
            }
            isAttackGoing = false;
            calledFinishAttack = false;
            yield return new WaitForSeconds(0.5f);
            if (!mover.IsNavMeshAgentEnabled() && !isBlocking)
            {
                mover.AllowMove();
            }
            if (chargeParticle != null && !isAttackGoing)
            {
                Destroy(chargeParticle);
                chargeParticle = null;
            }
            yield return new WaitForSeconds(0.8f);
            playerCamera.SetCustomNoiseSettings(playerCamera.GetDefaultNoiseSettings());
        }

        private IEnumerator NoSlowDwonThreshold()
        {
            yield return new WaitForSeconds(0.1f);
            mover.AllowSlowDown(true);
        }

        private AnimatorOverrideController CreateMixedController()
        {
            AnimatorOverrideController mixedOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
            List<KeyValuePair<AnimationClip, AnimationClip>> overridesMixed, overridesWeapon, overridesSpell;
            int overridesCount = currentWeapon.GetAnimatorController().overridesCount;
            overridesWeapon = new List<KeyValuePair<AnimationClip, AnimationClip>>(overridesCount);
            overridesSpell = new List<KeyValuePair<AnimationClip, AnimationClip>>(overridesCount);
            overridesMixed = new List<KeyValuePair<AnimationClip, AnimationClip>>(overridesCount);
            
            currentWeapon.GetAnimatorController().GetOverrides(overridesWeapon);
            currentWeapon.GetAnimatorController().GetOverrides(overridesMixed);
            if (currentSpell != null)
            {
                currentSpell.GetAnimatorController().GetOverrides(overridesSpell);
            }

            for (int i = 0; i < overridesCount; i++)
            {
                if (overridesWeapon[i].Value != null)
                {
                    overridesMixed[i] = overridesWeapon[i];
                }
                else if (overridesSpell.Any())
                {
                    if (overridesSpell[i].Value != null)
                    {
                        overridesMixed[i] = overridesSpell[i];
                    }
                }
            }

            mixedOverrideController.ApplyOverrides(overridesMixed);

            List<KeyValuePair<AnimationClip, AnimationClip>> test;
            test = new List<KeyValuePair<AnimationClip, AnimationClip>>(overridesCount);
            mixedOverrideController.GetOverrides(test);
            
            return mixedOverrideController;
        }

        private void CheckForAttackInactivityWeaponHide()
        {
            if (remainingTimeToHideWeapon > Time.time) return;
        
            isCalledToHideWeapon = true;
            TakeHideBackWeapon(false);
        }

        private void CheckForAttackInactivityAtkSeriesFinish()
        {
            if (remainingTimeToStopAttackSeries > Time.time) return;
            finishedAttackSequence = true;
        }

        private void TakeHideBackWeapon(bool take)
        {     
            if (isMeditating || backWeapon == null) return;
            if (take)
            {
                animator.SetTrigger("TakeWeapon");
            }
            else
            {
                animator.SetTrigger("HideWeapon");
            }
        }

        private void EquipWeapon(Weapon weapon)
        {
            if (weapon == null) return;
            ResetAttackSeries();
            currentWeaponPrefab = weapon.Equip(rightHand, animator, attackArea, attackSeries);
            if (currentWeaponPrefab != null)
            {
                Transform temp = currentWeaponPrefab.transform.Find("ChargeMedParticle");
                if (temp.childCount != 0)
                    weaponChargeMedParticle = temp.GetChild(0).GetComponent<ParticleSystem>();
            }
            if (weaponChargeMedParticle != null)
            {
                if (isMeditating)
                {
                    weaponChargeMedParticle.Play();
                }
                else
                {
                    weaponChargeMedParticle.Stop();
                }
            }
            chargesLeft = weapon.GetChargesAmount();
            animator.SetFloat("AttackSpeed", weapon.GetAttackSpeed());
            animator.runtimeAnimatorController = CreateMixedController();
            OnWeaponChange(weapon);
            HideShowBackWeapon();
            playerCamera.SetCustomNoiseSettings(weapon.GetNoisePreset());
            if (weapon.HasTrail())
            {
                weaponTrailParticle = currentWeaponPrefab.transform.Find("WeaponTrail").transform.GetChild(0).GetComponent<ParticleSystem>();
                weaponTrailParticle.Stop();
            }
            else
            {
                weaponTrailParticle = null;
            }
            isAttackGoing = false;
            if (newWeaponToChange != null)
            {
                ChangeBackWeapon(newWeaponToChange);
            }
        }

        private void ResetAttackSeries()
        {
            attackSeries.Clear();
            attackType = 1;
            lastAttackID = 0; 
            lastAttackSeriesID = 1;
        }

        private void ChangeBackWeaponCover(Weapon weapon)
        {
            if (backWeaponCoverHolder.childCount > 0)
            {
                Transform temp = backWeaponCoverHolder.transform.GetChild(0);
                if (temp != null)
                {
                    Destroy(temp.gameObject);
                }
            }

            if (weapon.GetBackCoverPrefab() != null)
            {
                Instantiate(weapon.GetBackCoverPrefab(), backWeaponCoverHolder.transform);
            }
        }

        private void HideShowBackWeapon()
        {
            if (backWeaponHolder.childCount > 0)
            {
                Transform temp = backWeaponHolder.transform.GetChild(0);
                if (temp != null)
                {
                    Destroy(temp.gameObject);
                }
            }
            if (backWeapon != null && backWeapon.HavePrefab())
            {
                GameObject backWeaponPref = Instantiate(backWeapon.GetBackPrefab(), backWeaponHolder.transform);
                backWeaponHolder.gameObject.SetActive(true);
            }
            else
            {
                backWeaponHolder.gameObject.SetActive(false);
            }
        }

        private int GetAttackID()
        {
            int attackIdToReturn = 0;

            lastAttackID++;

            if (lastAttackID > attackSeries[lastAttackSeriesID].Count)
            {
                lastAttackID = 1;
                finishedAttackSequence = true;
            }

            if (finishedAttackSequence)
            {
                lastAttackID = 1;
                if (attackSeries.Count == 1)
                {
                    lastAttackSeriesID = 1;
                }
                else if (attackSeries.Count == 2 || attackSeries.Count == 3 || attackSeries.Count == 4)
                {
                    lastAttackSeriesID = UnityEngine.Random.Range(1, attackSeries.Count + 1);
                }
                else
                {
                    int temp = UnityEngine.Random.Range(1, attackSeries.Count + 1);
                    while (lastAttackSeriesID == temp)
                    {
                        temp = UnityEngine.Random.Range(1, attackSeries.Count + 1);
                    }

                    lastAttackSeriesID = temp;
                }

                finishedAttackSequence = false;
            }

            attackIdToReturn = attackSeries[lastAttackSeriesID][lastAttackID];
            
            playerCamera.ChangeVirtCamStatsForAttack(
                        currentWeapon.GetAmplitudeVC(), 
                        currentWeapon.GetFrequencyVC(),
                        currentWeapon.GetDistanceVC());

            return attackIdToReturn;
        }

        private void RemoveWeaponPrefab()
        {
            GameObject equippedWeapon = GameObject.Find(currentWeapon.name);
            if (equippedWeapon != null)
            {
                Destroy(equippedWeapon);
            }
        }

    #endregion

    #region  Animation Events

        public void Shoot()
        {

        }

        public void Hit() 
        {   
            OnHit(isAttackGoing);
            StartAttackFinishCoroutine();
        }

        public void WeaponSwitch()
        {
            if (isWeaponInHands)
            {
                RemoveWeaponPrefab();
                isWeaponInHands = false;
            }
            else
            {
                isWeaponInHands = true;
            }
            
            Weapon temp = backWeapon;
            backWeapon = currentWeapon;
            currentWeapon = temp;
            EquipWeapon(currentWeapon);
        }

    #endregion

    }
}