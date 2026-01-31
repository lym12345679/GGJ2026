using UnityEngine;

public class CollisionMaskFilter : MonoBehaviour
{
    // 引用你的 NewBehaviourScript（包含 IsPointBlocked）
    public MaskColliderTest maskProvider;

    Collider2D selfCollider;

    void Awake()
    {
        selfCollider = GetComponent<Collider2D>();
        if (selfCollider == null)
            Debug.LogWarning("CollisionMaskFilter requires a Collider2D on the same GameObject.");
    }

    // 当物理引擎报告碰撞时，检查所有接触点是否被遮挡；若是则忽略该碰撞对（用于后续帧）
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (maskProvider == null || selfCollider == null) return;

        // 如果没有接触点则不处理
        if (collision.contactCount == 0) return;

        bool allBlocked = true;
        foreach (var cp in collision.contacts)
        {
            // 使用 maskProvider.IsPointBlocked(worldPoint) 判定像素级遮挡
            if (!maskProvider.IsPointBlocked(cp.point))
            {
                allBlocked = false;
                break;
            }
        }

        if (allBlocked)
        {
            Physics2D.IgnoreCollision(selfCollider, collision.collider, true);
        }
    }

    // 碰撞结束时恢复（避免永久忽略）
    void OnCollisionExit2D(Collision2D collision)
    {
        if (selfCollider == null) return;
        //Physics2D.IgnoreCollision(selfCollider, collision.collider, false);
    }
}