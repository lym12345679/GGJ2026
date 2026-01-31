using Game.Level;
using UnityEngine;
using UnityEngine.Events;

namespace Player
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class PlayerController : MonoBehaviour
    {
        // 可在 Inspector 设置的参数
        [SerializeField] float moveSpeed = 6f;
        [SerializeField] float jumpForce = 12f;
        [SerializeField] Transform groundCheck; // 在角色脚下放一个空物体用来检测是否在地面
        [SerializeField] float groundCheckRadius = 0.1f;
        [SerializeField] LayerMask groundLayer;
        private Animator animator;
        // 私有字段，遵循命名规范
        Rigidbody2D _rb;
        float _horizontal;
        bool _facingRight = true;

        // 重用数组以避免每帧产生 GC 分配
        Collider2D[] _overlapResults = new Collider2D[4];
        private static readonly int VelocityXHash = Animator.StringToHash("VelocityX");
        public UnityEvent OnJump,OnLand;
        void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            LevelsManager.Instance.PlayerTransform = transform;
        }

        void Update()
        {
            // 读取输入（可用键盘 A/D 或 左右 箭头）
            _horizontal = Input.GetAxisRaw("Horizontal");

            // 跳跃输入
            if (Input.GetKeyDown(KeyCode.W) && IsGrounded())
            {
                Jump();
            }

            // 翻转角色朝向
            if (_horizontal > 0.1f && !_facingRight)
                Flip();
            else if (_horizontal < -0.1f && _facingRight)
                Flip();
        }

        void FixedUpdate()
        {
            // 物理移动：直接设置速度 X 分量，保留 Y 分量
            Vector2 vel = _rb.velocity;
            vel.x = _horizontal * moveSpeed;
            animator.SetFloat(VelocityXHash, Mathf.Abs(vel.x));
            _rb.velocity = vel;
        }

        bool IsGrounded()
        {
            if (groundCheck == null)
            {
                // 如果没有设置 groundCheck，使用角色中心往下检测一个短距离
                RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.6f, groundLayer);
                return hit.collider != null;
            }
            else
            {
                int count = Physics2D.OverlapCircleNonAlloc(groundCheck.position, groundCheckRadius, _overlapResults, groundLayer);
                return count > 0;
            }
        }

        void Jump()
        {
            Vector2 vel = _rb.velocity;
            vel.y = 0f; // 重置垂直速度，得到一致的跳跃高度
            _rb.velocity = vel;
            _rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            OnJump?.Invoke();
        }

        void Flip()
        {
            _facingRight = !_facingRight;
            Vector3 s = transform.localScale;
            s.x *= -1f;
            transform.localScale = s;
        }

        // 可选：在编辑器中显示 groundCheck 范围
        void OnDrawGizmosSelected()
        {
            if (groundCheck != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
            }
        }
    }
}
