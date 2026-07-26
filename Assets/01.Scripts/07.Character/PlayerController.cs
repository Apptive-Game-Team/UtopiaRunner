using System;
using _01.Scripts._00.Manager;
using _01.Scripts._03.Data;
using _01.Scripts._05.Utility;
using UnityEngine;

namespace _01.Scripts._07.Character
{
    public class PlayerController : MonoBehaviour
    {
        private static readonly int IsJump = Animator.StringToHash("isJump");
        private static readonly int IsDoubleJump = Animator.StringToHash("isDoubleJump");
        private static readonly int IsSliding = Animator.StringToHash("isSliding");

        public Action OnJumpDetected;
        public Action OnSlideDetected;
        public Action OnHpChanged;
        public Action OnDead;

        [Header("Info")] 
        public _03.Data.CharacterInfo characterInfo;
        public int id;
        public float maxHp;
        public float hp;
        public float damage;
        
        [Header("Ground Check")]
        public Transform groundCheck;
        public float checkDistance = 1f;
        public LayerMask groundLayer;

        [Header("Status")]
        public bool isJump;
        public bool isDoubleJump;
        public bool isSliding;
        public bool isGrounded;
        public bool isDead;
        
        [Header("Jump")]
        public float jumpForce = 10f;
        public int maxJumpCount = 2;
        public int jumpCount;
        
        [Header("Slide")]
        [SerializeField] private Vector2 slideSize;
        [SerializeField] private Vector2 slideOffset;
        
        protected bool IsSet;
        
        private Rigidbody2D _rb;
        private Animator _anim;
        private BoxCollider2D _boxCol;
        private Vector2 _normalSize;
        private Vector2 _normalOffset;
        

        protected virtual void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _anim = GetComponent<Animator>();
            _boxCol = GetComponent<BoxCollider2D>();

            jumpCount = maxJumpCount;

            _normalSize = _boxCol.size;
            _normalOffset = _boxCol.offset;
            
            HandleInput();
        }
        
        protected virtual void Update()
        {
            CheckGround();

            _anim.SetBool(IsJump, isJump);
            _anim.SetBool(IsDoubleJump, isDoubleJump);
            _anim.SetBool(IsSliding, isSliding);
        }

        private void FixedUpdate()
        {
            CheckLanding();
        }

        public virtual void Init()
        {
            IsSet = true;

            maxHp = ValueFormula.GetCharacterHp(characterInfo);
            hp = maxHp;
            damage = ValueFormula.GetCharacterAp(characterInfo);
        }

        // 무기 Init 이후 추가 작업
        public virtual void AfterInit()
        {
            
        }

        public virtual void TakeDamage(float d)
        {
            hp -= d;
            
            OnHpChanged?.Invoke();

            if (hp <= 0)
            {
                isDead = true;
                OnDead?.Invoke();
            }
        }
        
        public virtual void Heal(float amount)
        {
            hp = Mathf.Clamp(hp + amount, 0f, maxHp);
            
            OnHpChanged?.Invoke();
        }

        private void Jump()
        {
            if (jumpCount == 0)
            {
                return;
            }
            if (isDead)
            {
                return;
            }

            if (jumpCount == maxJumpCount)
            {
                isJump = true;
            }
            else
            {
                isJump = false;
                isDoubleJump = true;
            }

            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, 0f);
            _rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

            OnJumpDetected?.Invoke();
            jumpCount--;
        }

        private void SlideStart()
        {
            isSliding = true;
            OnSlideDetected?.Invoke();
            _boxCol.size = slideSize;
            _boxCol.offset = slideOffset;
        }

        private void EndSlide()
        {
            isSliding = false;
            _boxCol.size = _normalSize;
            _boxCol.offset = _normalOffset;
        }

        private void CheckGround()
        {
            isGrounded = Physics2D.Raycast(
                groundCheck.position,
                Vector2.down,
                checkDistance,
                groundLayer
            );
        }

        private void CheckLanding()
        {
            if (_rb.linearVelocityY > 0.01f)
            {
                return;
            }

            if (isGrounded)
            {
                isJump = false;
                isDoubleJump = false;
                jumpCount = maxJumpCount;
            }
        }

        private void HandleInput()
        {
            InputManager.AddListener(ActionCode.Jump, InputType.Down, JumpInput);
            InputManager.AddListener(ActionCode.Slide, InputType.Press, SlideInput);
            InputManager.AddListener(ActionCode.Slide, InputType.Up, SlideEndInput);
        }

        private void OnDestroy()
        {
            InputManager.RemoveListener(ActionCode.Jump, InputType.Down, JumpInput);
            InputManager.RemoveListener(ActionCode.Slide, InputType.Press, SlideInput);
            InputManager.RemoveListener(ActionCode.Slide, InputType.Up, SlideEndInput);
        }

        private void JumpInput()
        {
            if (isDead)
            {
                return;
            }
                
            Jump();

            if (isSliding)
            {
                EndSlide();
            }
        }

        private void SlideInput()
        {
            if (isDead)
            {
                return;
            }
                
            if (!isJump && !isDoubleJump && !isSliding)
            {
                SlideStart();
            }
        }

        private void SlideEndInput()
        {
            if (isDead)
            {
                return;
            }
                
            if (isSliding)
            {
                EndSlide();
            }
        }
    }
}
