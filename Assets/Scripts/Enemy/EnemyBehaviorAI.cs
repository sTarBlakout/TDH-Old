using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using TDH.Stats;
using TDH.Player;

namespace TDH.EnemyAI
{
    public class EnemyBehaviorAI : MonoBehaviour, IEnemy
    {
        [SerializeField] float attackDistance = 1f;
        [SerializeField] float attackRate = 1f;
        [SerializeField] float damage = 10f;
        [SerializeField] float rotationSpeedWhileAttacking = 100f;
        [SerializeField] float triggerRadius = 5f;
        [SerializeField] float lookAtRadius = 10f;
        [SerializeField] float powerToFall = 5f;
        [SerializeField] float secondsRecoverFromHeavyAttack = 5f;
        [SerializeField] float secondsRecoverFromEasyAttack = 1f;
        [SerializeField] GameObject particleWhenHitPlayer = null;
        [SerializeField] AnimatorOverrideController animatorOverrideController = null;
        [SerializeField] Vector2 attackRangeID;

        private CurrentEnemyState currentState;

        private float timeSinceLastAttack = 0f;
        private float currentAttackDistance = 0f;

        private bool alreadyHitTargetOnceDuringAttack = false;
        private bool isTriedToHitAlready = false;

        private IEnumerator hitCoroutine;

        private NavMeshAgent agent;
        private Health health;
        private Animator animator;
        private GameObject player;

        private Vector3 previousPosition;
        private Vector3 getHitVelocity;
        private Vector3 lastHitPosition;
        private float getHitPower;
        private float currentSpeed;

        private bool isPlayerShieldActivated = false;
        private bool isPlayerDead = false;

        #region  Unity Methods

            private void Awake() 
            {
                agent = this.transform.GetComponent<NavMeshAgent>();    
                animator = this.transform.GetComponent<Animator>();
                health = this.transform.GetComponent<Health>();
                player = GameObject.FindWithTag("Player");

                currentAttackDistance = attackDistance;

                health.OnDie += Die;
                health.OnHit += GetHit;
            }

            private void Start() 
            {
                if (animatorOverrideController != null)
                {
                    animator.runtimeAnimatorController = animatorOverrideController;
                }
                ActivatedPlayerShield(player.GetComponent<PlayerFighter>().IsBlocking());
            }

            private void Update() 
            {
                if  (currentState == CurrentEnemyState.DEAD || currentState == CurrentEnemyState.FALLEN) return;

                CalculateCurrentSpeed();
                animator.SetFloat("forwardSpeed", currentSpeed);

                if (currentState == CurrentEnemyState.HIT)
                {
                    animator.SetFloat("forwardSpeed", 0);
                    return;
                }

                if (CalculateDistanceToTarget() > triggerRadius || isPlayerDead)
                {
                    IdleBehavior();
                    return;
                }

                if (CalculateDistanceToTarget() > currentAttackDistance)
                {   
                    if (currentState == CurrentEnemyState.ATTACKING && !isTriedToHitAlready)
                    {
                        return;
                    }
                    else
                    {
                        GoingToTargetBehavior();
                    }
                }
                else
                {
                    TargetReachedBehavior();
                    AttackBehavior();
                }
            }

        private void LateUpdate() 
        {
            if (currentState == CurrentEnemyState.ATTACKING || currentState == CurrentEnemyState.HIT)
                RotateTowardsPlayer(1000f);  
            if (currentState == CurrentEnemyState.FALLEN || currentState == CurrentEnemyState.DEAD)  
                RotateTowardsHitPosition(1000f);
        }

        #endregion

        #region Private Methods

        private void AttackBehavior()
        {
            if (timeSinceLastAttack + attackRate > Time.time || currentState != CurrentEnemyState.ATTACKING) 
            {
                return;
            }

            alreadyHitTargetOnceDuringAttack = false;
            isTriedToHitAlready = false;
            
            int randomID = Random.Range((int)attackRangeID.x, (int)attackRangeID.y + 1);
            animator.SetInteger("AttackID", randomID);
            animator.SetTrigger("Attack");
            timeSinceLastAttack = Time.time;
        }

        private void IdleBehavior()
        {
            if (currentState != CurrentEnemyState.IDLE)
            {
                SetCurrentState(5);
            }
            if (CalculateDistanceToTarget() < lookAtRadius)
            {
                RotateTowardsPlayer(40f);
            }
        }

        private void RotateTowardsHitPosition(float speed)
        {   
            if (lastHitPosition == null) return;
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, 
                Quaternion.LookRotation(lastHitPosition - this.transform.position), 
                speed * Time.deltaTime);    
        }

        private void RotateTowardsPlayer(float speed)
        {   
            if (Vector3.Angle(transform.forward, player.transform.position - this.transform.position) > 10f)
            {
                if (Direction(player.transform.position) == 1)
                {   
                    animator.SetBool("TurnLeft", false);
                    animator.SetBool("TurnRight", true);
                }
                else if (Direction(player.transform.position) == -1)
                {
                    animator.SetBool("TurnRight", false);
                    animator.SetBool("TurnLeft", true);
                }
            }
            else
            {
                animator.SetBool("TurnRight", false);
                animator.SetBool("TurnLeft", false);
            }

            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, 
                Quaternion.LookRotation(player.transform.position - this.transform.position), 
                speed * Time.deltaTime);    
        }

        private int Direction(Vector3 target)
        {
            Vector3 heading = target - transform.position;
            Vector3 perp = Vector3.Cross(transform.forward, heading);
		    float dir = Vector3.Dot(perp, transform.up);
		
            if (dir > 0f) 
            {
                return 1;
            } 
            else if (dir < 0f)
            {
                return -1;
            } 
            else 
            {
                return 0;
            }
        }

        private void TargetReachedBehavior()
        {
            SetCurrentState(2);
            agent.isStopped = true;
        }

        private void GoingToTargetBehavior()
        {
            SetCurrentState(1);
            agent.isStopped = false;
            agent.SetDestination(player.transform.position);
        }

        private void Die()
        {   
            SetCurrentState(3);
            if (getHitPower > powerToFall)
            {
                GetHit();
            }
            else
            {
                animator.SetLayerWeight(1, 0f);
                animator.SetInteger("DieID", DieAnimRandomizer());
                animator.SetTrigger("Die");
                agent.velocity += getHitVelocity;

                StartCoroutine(DieThreshold());
            }
        }

        private IEnumerator DieThreshold()
        {
            yield return new WaitForSeconds(1);
            agent.isStopped = true;
            this.transform.GetComponent<CapsuleCollider>().enabled = false;
            agent.enabled = false;
            this.transform.GetComponent<EnemyDeath>().enabled = true;
            this.enabled = false;
        }

        private int GetHitAnimRandomizer()
        {
            return UnityEngine.Random.Range(1, 5);
        }

        private int DieAnimRandomizer()
        {
            return UnityEngine.Random.Range(1, 3);
        }

        private void GetHit()
        {
            lastHitPosition = player.transform.position;
            if (currentState != CurrentEnemyState.DEAD)
                SetCurrentState(4);
            animator.SetLayerWeight(1, 0);
            agent.velocity += getHitVelocity;
            if (hitCoroutine != null)
            {
                StopCoroutine(hitCoroutine);
                hitCoroutine = null;
            }
            if (currentState == CurrentEnemyState.DEAD)
            {
                hitCoroutine = HitEndThreshold(1);
                animator.SetInteger("GetHitID", 6);
            }
            else
            {
                if (getHitPower > powerToFall)
                {
                    hitCoroutine = HitEndThreshold(secondsRecoverFromHeavyAttack);
                    SetCurrentState(6);
                    animator.SetInteger("GetHitID", 6);
                }
                else
                {       
                    hitCoroutine = HitEndThreshold(secondsRecoverFromEasyAttack);
                    animator.SetInteger("GetHitID", GetHitAnimRandomizer());
                }
            }
            animator.SetTrigger("GetHit");

            StartCoroutine(hitCoroutine);
        }

        private IEnumerator HitEndThreshold(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            if (currentState != CurrentEnemyState.DEAD)
            {
                animator.SetLayerWeight(1, 1);
                SetCurrentState(5);
                hitCoroutine = null;
                agent.ResetPath();
                agent.isStopped = false;
            }
            else
            {
                agent.isStopped = true;
                this.transform.GetComponent<CapsuleCollider>().enabled = false;
                agent.enabled = false;
                this.transform.GetComponent<EnemyDeath>().enabled = true;
                this.enabled = false;
            }
        }

        private float CalculateDistanceToTarget()
        {
            return Vector3.Distance(this.transform.position, player.transform.position);
        }

        private void CalculateCurrentSpeed()
        {   
            Vector3 curMove = transform.position - previousPosition;
            currentSpeed = curMove.magnitude / Time.deltaTime;
            previousPosition = transform.position;
        }

        private void SetCurrentState(int state)
        {
            switch(state)
            {
                case 1: //Moving
                {
                    animator.SetBool("TurnLeft", false);
                    animator.SetBool("TurnRight", false);
                    currentState = CurrentEnemyState.MOVING;
                    break;
                }
                case 2: //Attacking
                {   
                    currentState = CurrentEnemyState.ATTACKING;
                    break;
                }
                case 3: //Dead
                {   
                    currentState = CurrentEnemyState.DEAD;
                    if (hitCoroutine != null)
                        StopCoroutine(hitCoroutine);
                    animator.SetBool("isDead", true);
                    break;
                }
                case 4: //Hit
                {
                    currentState = CurrentEnemyState.HIT;
                    break;
                }
                case 5: //Idle
                {
                    currentState = CurrentEnemyState.IDLE;
                    break;
                }
                case 6: //Fallen
                {   
                    agent.isStopped = true;
                    currentState = CurrentEnemyState.FALLEN;
                    break;
                }
                default:
                {
                    Debug.Log("Provided wrong state ID on " + this.gameObject.name);
                    break;
                }
            }
        }

        #endregion

        #region Public Methods

            public void SetLastHitPosition(Vector3 position)
            {
                lastHitPosition = position;
            }

            public CurrentEnemyState GetCurrentEnemyState()
            {
                return currentState;
            }

            public void SetHitVelocity(Vector3 vel, float power)
            {
                getHitVelocity = vel * power;
                getHitPower = power;
            }

            public void ActivatedPlayerShield(bool activated)
            {
                isPlayerShieldActivated = activated;
                if (activated)
                {
                    currentAttackDistance = attackDistance + 1f;
                }
                else
                {
                    currentAttackDistance = attackDistance;
                }
            }

            public void PlayerDie()
            {
                isPlayerDead = true;
                agent.isStopped = true;
            }

        #endregion

        #region  Animation Events

            private void Hit()
            {
                if (!isPlayerShieldActivated)
                {   
                    isTriedToHitAlready = true;
                    if (CalculateDistanceToTarget() <= attackDistance + 0.5f && !alreadyHitTargetOnceDuringAttack)
                    {
                        alreadyHitTargetOnceDuringAttack = true;
                        player.GetComponent<PlayerLightController>().DealDamage(damage);
                        if (particleWhenHitPlayer != null)
                        {
                            Vector3 spawnPosition = new Vector3(
                                0f, 
                                Random.Range(0.7f, 1f), 
                                CalculateDistanceToTarget() - 0.35f
                            );
                            Instantiate(particleWhenHitPlayer, transform.TransformPoint(spawnPosition), Quaternion.identity);
                        }
                    }
                    else
                    {
                        alreadyHitTargetOnceDuringAttack = false;
                    }
                }
                else
                {
                    PlayerShield shield = player.transform.Find("ShieldCenter").GetComponent<PlayerShield>();
                    Vector3 spawnPosition = new Vector3(
                        0f, 
                        Random.Range(0.5f, 0.6f), 
                        attackDistance - 0.6f
                    );
                    Instantiate(shield.GetHitParticle(), transform.TransformPoint(spawnPosition), Quaternion.identity);
                    SetHitVelocity(-this.transform.forward, shield.GetHitPower());
                    this.transform.GetComponent<Health>().DecreaseHealth(shield.GetDamage());
                }
            }

            private void FootR()
            {

            }

            private void FootL()
            {
                
            }

        #endregion
    }
}