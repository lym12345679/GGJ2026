using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MaskColliderTest : MonoBehaviour
{
    // 在 Inspector 中放置会遮挡后层的前景 SpriteRenderer（按需要填入）
    public List<SpriteRenderer> frontSprites;
    [Range(0f, 1f)]
    public float alphaThreshold = 0.1f;
    public RawImage image;
    // 判断世界坐标点是否被任一前景精灵不透明像素遮挡
    public bool IsPointBlocked(Vector2 worldPoint)
    {
        foreach (var fsr in frontSprites)
        {
            if (fsr == null || fsr.sprite == null) continue;
            if (!fsr.bounds.Contains(worldPoint)) continue; // 快速包围盒剔除
            if (IsOpaqueAtPoint(fsr, worldPoint, alphaThreshold)) return true;
        }
        return false;
    }

    // 返回过滤后的 Collider2D 数组：排除了被前景像素遮挡的后层碰撞体
    public Collider2D[] OverlapPointFiltered(Vector2 worldPoint, int layerMask = Physics2D.DefaultRaycastLayers)
    {
        var cols = Physics2D.OverlapPointAll(worldPoint, layerMask);
        var outList = new List<Collider2D>();
        foreach (var col in cols)
        {
            // 若需要基于排序层确定“前后”，尝试获取后层的 SpriteRenderer
            var backSr = col.GetComponent<SpriteRenderer>();
            int backOrder = backSr ? backSr.sortingOrder : int.MinValue;

            bool blocked = false;
            foreach (var fsr in frontSprites)
            {
                if (fsr == null || fsr.sprite == null) continue;
                if (fsr.sortingOrder <= backOrder) continue; // 只考虑在视觉上位于后者之上的前景
                if (!fsr.bounds.Contains(worldPoint)) continue;
                if (IsOpaqueAtPoint(fsr, worldPoint, alphaThreshold)) { blocked = true; break; }
            }

            if (!blocked) outList.Add(col);
        }
        return outList.ToArray();
    }

    // 逐像素判断：worldPoint 在 frontSprite 精灵处是否为不透明像素
    static bool IsOpaqueAtPoint(SpriteRenderer fsr, Vector2 worldPoint, float alphaThreshold)
        => TextureReadHelper.Instance.IsSpritePixelOpaque(fsr, worldPoint, alphaThreshold);
    // {
    //     var sprite = fsr.sprite;
    //     var tex = sprite.texture;
    //     if (tex == null)
    //     {
    //         return false;
    //     }
    //
    //     if (!tex.isReadable)
    //     {
    //         Debug.LogWarning($"Texture on sprite {sprite.name} must be Read/Write enabled to sample pixels.");
    //         return false;
    //     }
    //
    //     // 把世界点转到精灵的本地空间，再转为像素坐标
    //     Vector2 local = fsr.transform.InverseTransformPoint(worldPoint);
    //     float ppu = sprite.pixelsPerUnit;
    //     Vector2 pivot = sprite.pivot; // 像素为单位
    //     float px = local.x * ppu + pivot.x;
    //     float py = local.y * ppu + pivot.y;
    //
    //     var rect = sprite.textureRect; // 在纹理中的矩形（像素）
    //     float u = (rect.x + px) / tex.width;
    //     float v = (rect.y + py) / tex.height;
    //
    //     if (u < 0f || u > 1f || v < 0f || v > 1f) return false;
    //
    //     // 采样 alpha（使用 GetPixelBilinear 以支持缩放）
    //     Color c = tex.GetPixelBilinear(u, v);
    //     return c.a > alphaThreshold;
    // }
}
