using UnityEngine;
using StarterAssets;
using System.Collections.Generic;

public class CloneController : MonoBehaviour
{
    public AudioClip LandingAudioClip;
    public AudioClip[] FootstepAudioClips;
    [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

    [Header("Settings")]
    public float MoveSpeed = 2.0f;
    public float SprintSpeed = 5.335f;
    public float RotationSmoothTime = 0.12f;
    public float SpeedChangeRate = 10.0f;
    public float JumpHeight = 1.2f;
    public float Gravity = -15.0f;
    public float GroundedOffset = -0.14f;
    public float GroundedRadius = 0.28f;
    public LayerMask GroundLayers;

    private float _speed;
    private float _animationBlend;
    private float _targetRotation = 0.0f;
    private float _rotationVelocity;
    private float _verticalVelocity;
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;

    private CharacterController _controller;
    private Animator _animator;
    private GameObject _mainCamera;
    private bool _hasAnimator;
    private bool Grounded;

    private int _animIDSpeed;
    private int _animIDGrounded;
    private int _animIDJump;
    private int _animIDFreeFall;
    private int _animIDMotionSpeed;

    [Header("References")]
    public ActionRecorder actionRecorder;
    private List<ActionRecorder.PlayerAction> actions;
    private int _currentActionIndex = 0;
    private bool isInitialized = false;

    public void Init(ActionRecorder recorder, List<ActionRecorder.PlayerAction> recordedActions)
    {
        actionRecorder = recorder;
        actions = new List<ActionRecorder.PlayerAction>(recordedActions);
        isInitialized = true;
        _currentActionIndex = 0;
        Debug.Log("Clone initialized successfully.");
    }

    void Start()
    {
        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");

        _hasAnimator = _animator != null;
        AssignAnimationIDs();

        _verticalVelocity = 0f; // Start grounded
    }

    void Update()
    {
        if (!isInitialized || actions == null || actions.Count == 0 || _currentActionIndex >= actions.Count) return;

        var action = actions[_currentActionIndex];

        // Ensure we are grounded properly at start
        if (_currentActionIndex == 0)
        {
            GroundedCheck();
            if (!Grounded)
            {
                _controller.Move(Vector3.down * 0.5f);
                GroundedCheck();
            }
        }

        GroundedCheck();

        Move(action);
        JumpAndGravity(action);

        _currentActionIndex++;
    }

    void AssignAnimationIDs()
    {
        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDGrounded = Animator.StringToHash("Grounded");
        _animIDJump = Animator.StringToHash("Jump");
        _animIDFreeFall = Animator.StringToHash("FreeFall");
        _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
    }

    void Move(ActionRecorder.PlayerAction action)
    {
        float targetSpeed = action.sprint ? SprintSpeed : MoveSpeed;
        if (action.moveInput == Vector2.zero) targetSpeed = 0.0f;

        float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

        if (currentHorizontalSpeed < targetSpeed - 0.1f || currentHorizontalSpeed > targetSpeed + 0.1f)
        {
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed, Time.deltaTime * SpeedChangeRate);
            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = targetSpeed;
        }

        _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
        if (_animationBlend < 0.01f) _animationBlend = 0f;

        Vector3 forward = Quaternion.Euler(0f, action.cameraYaw, 0f) * Vector3.forward;
        Vector3 right = Quaternion.Euler(0f, action.cameraYaw, 0f) * Vector3.right;
        Vector3 moveDirection = forward * action.moveInput.y + right * action.moveInput.x;

        if (action.moveInput != Vector2.zero)
        {
            _targetRotation = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);
            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }

        _controller.Move(moveDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

        if (_hasAnimator)
        {
            _animator.SetFloat(_animIDSpeed, _animationBlend);
            _animator.SetFloat(_animIDMotionSpeed, action.moveInput.magnitude);
        }
    }

    void JumpAndGravity(ActionRecorder.PlayerAction action)
    {
        if (Grounded)
        {
            _fallTimeoutDelta = 0.15f;

            if (_verticalVelocity < 0.0f)
                _verticalVelocity = -2f;

            if (action.jump && _jumpTimeoutDelta <= 0.0f)
            {
                _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                if (_hasAnimator)
                    _animator.SetBool(_animIDJump, true);
            }
            else
            {
                if (_hasAnimator)
                    _animator.SetBool(_animIDJump, false);
            }

            if (_jumpTimeoutDelta >= 0.0f)
                _jumpTimeoutDelta -= Time.deltaTime;
        }
        else
        {
            _jumpTimeoutDelta = 0.50f;

            if (_fallTimeoutDelta >= 0.0f)
            {
                _fallTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                if (_hasAnimator)
                    _animator.SetBool(_animIDFreeFall, true);
            }

            if (_hasAnimator)
                _animator.SetBool(_animIDJump, false);
        }

        _verticalVelocity += Gravity * Time.deltaTime;
    }

    void GroundedCheck()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
        Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);

        if (_hasAnimator)
        {
            _animator.SetBool(_animIDGrounded, Grounded);
            _animator.SetBool(_animIDFreeFall, !Grounded);
        }
    }

    private void OnFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            if (FootstepAudioClips.Length > 0)
            {
                var index = Random.Range(0, FootstepAudioClips.Length);
                AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
            }
        }
    }

    private void OnLand(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
        }
    }
}
