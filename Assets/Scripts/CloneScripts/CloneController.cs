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
    private float correctionThreshold = 0.1f;
    private int correctionInterval = 10;


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
        if (!isInitialized || actions == null || actions.Count == 0)
            return;

        if (_currentActionIndex >= actions.Count)
        {
            Disappear();
            return;
        }

        var action = actions[_currentActionIndex];
        //Debug.Log($"Replay Action #{_currentActionIndex}: pos={action.position}, move={action.moveInput}, sprint={action.sprint}, jump={action.jump}");

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

        if (action.leftClick && !string.IsNullOrEmpty(action.buttonID))
        {
            Debug.Log(action.buttonID);
            PressButton[] allButtons = GameObject.FindObjectsOfType<PressButton>();
            //PressButton[] allButtons = GameObject.FindObjectsByType<PressButton>((FindObjectsSortMode)FindObjectsInactive.Include);
            foreach (var button in allButtons)
            {
                if (button.buttonID == action.buttonID)
                {
                    button.TriggerPressExternally(action.leftClickHoldDuration);
                    break;
                }
            }
        }

        // --- Soft position correction to reduce drift/offset ---
        if (_currentActionIndex % correctionInterval == 0 && _currentActionIndex < actions.Count &&
    _animIDJump == 0)
        {
            Vector3 recordedPos = actions[_currentActionIndex].position;
            Vector3 delta = recordedPos - transform.position;

            if (delta.magnitude > correctionThreshold)
            {
                _controller.enabled = false;
                transform.position = recordedPos;
                _controller.enabled = true;
            }
        }

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
        if (action.moveInput != Vector2.zero)
        {
            _speed = Mathf.Lerp(_speed, targetSpeed, Time.deltaTime * SpeedChangeRate);
        }
        else
        {
            _speed = Mathf.Lerp(_speed, 0f, Time.deltaTime * SpeedChangeRate);
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
            _jumpTimeoutDelta = 0.30f;

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

    private void Disappear()
    {
        Debug.Log("Clone finished its actions and will disappear.");

        foreach (var renderer in GetComponentsInChildren<Renderer>())
        {
            renderer.enabled = false;
        }

        foreach (var collider in GetComponentsInChildren<Collider>())
        {
            collider.enabled = false;
        }

        if (_controller != null)
            _controller.enabled = false;

        if (_animator != null)
            _animator.enabled = false;

        this.enabled = false;
    }

}
