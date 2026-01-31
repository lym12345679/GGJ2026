using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 把此组件放在含 Collider2D 的物体上。可在 inspector 手动指定一个或多个 SpriteMask（若留空，会在 Awake 时自动收集场景中的所有 SpriteMask）。
/// 在碰撞发生时，按接触点采样 mask 的 sprite 像素 alpha；若碰撞的所有接触点都被 mask 遮住，则忽略该碰撞对（从下一帧开始生效）。
/// </summary>
// [RequireComponent(typeof(Collider2D))]
public class SpriteMaskCollisionFilter : MonoBehaviour
{
    [Tooltip("若为空，会自动收集场景中所有 SpriteMask")]
    public SpriteMask[] masks;

    [Tooltip("判断为被遮挡的 alpha 阈值")]
    [Range(0f, 1f)]
    public float alphaThreshold = 0.1f;

    Collider2D selfCollider;
    // 已经被忽略的 collider 对集合（只保存对方 collider）
    HashSet<Collider2D> ignored = new HashSet<Collider2D>();

    void Awake()
    {
        selfCollider = GetComponent<Collider2D>();
        if (selfCollider == null) Debug.LogWarning("SpriteMaskCollisionFilter 需要附加在有 Collider2D 的 GameObject 上。");

        if (masks == null || masks.Length == 0)
        {
            // 自动收集场景中的 SpriteMask（如果用户未手动指定）
            masks = FindObjectsOfType<SpriteMask>();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        TryIgnoreByMasks(collision);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        // 如果在碰撞过程中 mask 状态改变，也可能需要忽略
        TryIgnoreByMasks(collision);
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (selfCollider == null) return;
        if (collision.collider == null) return;

        if (ignored.Contains(collision.collider))
        {
            Physics2D.IgnoreCollision(selfCollider, collision.collider, false);
            ignored.Remove(collision.collider);
        }
    }
    // 检查碰撞的所有接触点，若均被 mask 遮挡则忽略该碰撞对
    void TryIgnoreByMasks(Collision2D collision)
    {
        if (selfCollider == null || collision.collider == null) return;
        if (collision.contactCount == 0) return;

        bool allBlocked = true;
        foreach (var cp in collision.contacts)
        {
            Vector2 worldPoint = cp.point;
            bool pointBlocked = IsPointCoveredByAnyMask(worldPoint);
            if (!pointBlocked)
            {
                allBlocked = false;
                break;
            }
        }

        if (allBlocked)
        {
            // 从下一帧开始忽略该碰撞对（无法阻止本次已发生的物理响应）
            if (!ignored.Contains(collision.collider))
            {
                Physics2D.IgnoreCollision(selfCollider, collision.collider, true);
                ignored.Add(collision.collider);
            }
        }
    }
    // 判断 worldPoint 是否被任一 mask 的不透明像素覆盖
    bool IsPointCoveredByAnyMask(Vector2 worldPoint)
    {
        if (masks == null || masks.Length == 0) return false;

        foreach (var mask in masks)
        {
            if (mask == null) continue;
            var sprite = mask.sprite;
            if (sprite == null) continue;

            // 使用 TextureReadHelper 运行时裁剪纹理并采样像素（该类应存在于项目里）
            var tex = TextureReadHelper.Instance.GetReadableSpriteTexture(sprite);
            if (tex == null) continue;

            // 将世界点转换到 mask 的本地空间
            Vector2 local = mask.transform.InverseTransformPoint(worldPoint);

            float ppu = sprite.pixelsPerUnit;
            Vector2 pivot = sprite.pivot; // 以像素为单位，相对 sprite.rect 的左下角
            float px = local.x * ppu + pivot.x;
            float py = local.y * ppu + pivot.y;

            int ix = Mathf.FloorToInt(px);
            int iy = Mathf.FloorToInt(py);

            if (ix < 0 || ix >= tex.width || iy < 0 || iy >= tex.height)
            {
                // 点不在这个 mask 的精灵矩形内
                continue;
            }

            // 用整数像素读取避免插值带来的不确定性
            Color c = tex.GetPixel(ix, iy);
            if (c.a > alphaThreshold)
            {
                return true;
            }
        }

        return false;
    }
}