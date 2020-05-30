using UnityEngine;
using UnityEngine.AI;
using TDH.UI;

namespace TDH.Player
{
    public class PlayerMover : MonoBehaviour
    {
        [SerializeField] FixedJoystick joystick; 
        [SerializeField] Transform stepRight, stepLeft;
        [SerializeField] GameObject stepGlow;
        [SerializeField] float joystickDeadZone = 0f;
        [SerializeField] float maxMoveSpeed = 1f;
        [SerializeField] float acceleration = 1f;
        [SerializeField] float rotationSpeed = 1f;
        [SerializeField] float slowdownFactor = 0f;

        private float currentMaxMoveSpeed = 0f;
        private float moveSpeed = 0f;

        private bool isAllowedToMove = true;
        private bool isAttacking = false;
        private bool isCasting = false;
        private bool isAllowedToSlowDown = true;

        private NavMeshAgent navMeshAgent;
        private Animator animator;
        private PlayerFighter fighter;
        private UIManager managerUI;

        private bool isForceAction = false;
        private bool isForceMove = false, isForceRotate = false;
        private string forceMoveEndTriggerName = null;
        private Quaternion forceRotationTarget;
        private Vector3 forceMoveTarget;

    #region Unity Methods

        void Awake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
            fighter = GetComponent<PlayerFighter>();
            managerUI = GameObject.Find("UI").GetComponent<UIManager>();

            fighter.OnAttackStarted += StartAttack;
            fighter.OnAttackFinished += StopAttack;

            fighter.OnPowerfullAttackStarted += StartPowerAttack;

            fighter.OnCastStarted += CastStarted;
            fighter.OnCastFinished += CastFinished;

            fighter.OnBlockStarted += RestrictMovement;
            fighter.OnBlockFinished += AllowMove;
        }

        private void Start() 
        {
            currentMaxMoveSpeed = maxMoveSpeed;   
        }

        private void FixedUpdate() 
        {
            if (isCasting)
            {
                RotateControl(60f);
            }
            MovementControl();
            UpdateAnimator();

            if (isForceAction)
            {
                ForceMoveAction();
            }
        }

    #endregion

    #region Public Methods

        public void AllowSlowDown(bool allow)
        {
            isAllowedToSlowDown = allow;
        }

        public void AllowMove()
        {
            navMeshAgent.enabled = true;
            navMeshAgent.velocity = Vector3.zero;
            isAllowedToMove = true;
        }

        public void RestrictMovement()
        {
            isAllowedToMove = false;
            navMeshAgent.velocity = Vector3.zero;
            navMeshAgent.enabled = false;
        }

        public void ActivateNavMeshAgent(bool activate)
        {
           navMeshAgent.enabled = activate;
        }

        public void SetVelocity(float vel)
        {  
            navMeshAgent.velocity = transform.forward * vel;
        }
        
        public bool CanReachDestination(Vector3 destination)
        {
            NavMeshPath path = new NavMeshPath();
            navMeshAgent.CalculatePath(destination, path);
            if (path.status == NavMeshPathStatus.PathInvalid)
                return false;
            else
                return true;
        }

        public bool IsNavMeshAgentEnabled()
        {
            return navMeshAgent.enabled;
        }

        public void ForceMove(Vector3 position, Quaternion rotation, string moveEndAnimTrigger)
        {
            RestrictMovement();
            forceMoveTarget = position;
            forceRotationTarget = rotation;
            forceMoveEndTriggerName = moveEndAnimTrigger;
            isForceMove = true;
            isForceRotate = true;
            isForceAction = true;
        }

    #endregion

    #region Private Methods

        private void ForceMoveAction()
        {
            if (isForceMove || isForceRotate)
            {
                if (transform.position != forceMoveTarget && isForceMove)
                {
                    transform.position = Vector3.MoveTowards(transform.position, forceMoveTarget, 0.05f);
                }
                else
                {
                    isForceMove = false;
                }
                if (Quaternion.Angle(transform.rotation, forceRotationTarget) > 15f && isForceRotate)
                {
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, forceRotationTarget, 5f);
                }
                else
                {
                    isForceRotate = false;
                }
            }
            else
            {
                isForceAction = false;
                animator.SetTrigger(forceMoveEndTriggerName);
            }
        }

        private void StartPowerAttack()
        {
            isAttacking = true;
        }

        private void CastStarted()
        {
            isCasting = true;
        }

        private void CastFinished()
        {
            isCasting = false;
        }

        private void StartAttack()
        {
            isAllowedToSlowDown = false;
            isAttacking = true;
            navMeshAgent.velocity += transform.forward * fighter.GetAtkVelBoost();
        }

        private void StopAttack()
        { 
            isAttacking = false;
        }

        private bool IsJoystickInDeadZone()
        {
            if (joystick.Horizontal < joystickDeadZone && joystick.Horizontal > -joystickDeadZone && 
                joystick.Vertical < joystickDeadZone && joystick.Vertical > -joystickDeadZone) 
                return true;
            else
                return false;
        }

        private void UpdateAnimator()
        {
            Vector3 velocity = navMeshAgent.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            float speed = localVelocity.z;
            animator.SetFloat("forwardSpeed", speed);
        }

        private void SlowDown()
        {   
            if (!isAllowedToSlowDown) return;
            navMeshAgent.velocity *= slowdownFactor * 0.1f;
            moveSpeed *= slowdownFactor * 0.1f;
        }

        private void RotateControl(float rotSpeed)
        {
            float _xInput = joystick.Horizontal * 0.01f; 
            float _yInput = joystick.Vertical * 0.01f;

            Vector3 _rotation = new Vector3(_xInput, 0, _yInput);

            if (_rotation == Vector3.zero)
            {
                return;
            }

            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, 
                Quaternion.LookRotation(_rotation), 
                rotSpeed * Time.deltaTime);     
        }

        private void MovementControl()
        {
            if (IsJoystickInDeadZone() || isAttacking || !isAllowedToMove || isCasting) 
            {      
                if (navMeshAgent.velocity == Vector3.zero) return;
                
                SlowDown();
                
                return;
            } 

            if (moveSpeed < currentMaxMoveSpeed)
                moveSpeed += acceleration * Time.deltaTime;

            float _xInput = joystick.Horizontal * moveSpeed; 
            float _yInput = joystick.Vertical * moveSpeed;

            Vector3 _movement = new Vector3(_xInput, 0, _yInput);

            navMeshAgent.velocity = _movement;

            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, 
                Quaternion.LookRotation(_movement), 
                rotationSpeed * Time.deltaTime);
        }

    #endregion

    #region  Animation Events

        private void FootR()
        {
            Instantiate(stepGlow, stepRight.position, stepGlow.transform.rotation);
        }

        private void FootL()
        {
            Instantiate(stepGlow, stepLeft.position, stepGlow.transform.rotation);
        }

    #endregion

    }
}