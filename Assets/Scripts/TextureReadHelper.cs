using System.Collections.Generic;
using UnityEngine;


    /// <summary>
    /// 运行时为 Sprite 生成可读的裁剪纹理副本并缓存，提供像素级 alpha 判定函数。
    /// 用法：TextureReadHelper.IsSpritePixelOpaque(spriteRenderer, worldPoint, alphaThreshold)
    /// </summary>
    public class TextureReadHelper
    {
        public static TextureReadHelper Instance { get; } = new TextureReadHelper();
        // 缓存每个 Sprite 的裁剪后可读纹理（key 使用 sprite.GetInstanceID）
        static Dictionary<int, Texture2D> s_cache = new Dictionary<int, Texture2D>();

        // 返回可读的裁剪纹理（只包含 sprite.rect 区域），若已存在则复用
        public Texture2D GetReadableSpriteTexture(Sprite sprite)
        {
            if (sprite == null) return null;
            int key = sprite.GetInstanceID();
            if (s_cache.TryGetValue(key, out var cached) && cached != null) return cached;

            Texture src = sprite.texture;
            if (src == null) return null;

            // 使用 GPU 拷贝：把整张源纹理 blit 到临时 RenderTexture，再 ReadPixels 裁剪区域
            int srcW = src.width;
            int srcH = src.height;
            var prev = RenderTexture.active;
            var rt = RenderTexture.GetTemporary(srcW, srcH, 0, RenderTextureFormat.ARGB32);
            Graphics.Blit(src, rt);
            RenderTexture.active = rt;

            // sprite.textureRect 是在原始纹理中的像素区域
            Rect texRect = sprite.textureRect;
            int w = Mathf.Max(1, (int)texRect.width);
            int h = Mathf.Max(1, (int)texRect.height);

            var newTex = new Texture2D(w, h, TextureFormat.RGBA32, false);
            // ReadPixels 的坐标基于 RenderTexture（与纹理像素坐标一致）
            newTex.ReadPixels(new Rect(texRect.x, texRect.y, texRect.width, texRect.height), 0, 0);
            newTex.Apply();

            RenderTexture.active = prev;
            RenderTexture.ReleaseTemporary(rt);

            s_cache[key] = newTex;
            return newTex;
        }
        /// <summary>
        /// 逐像素判定：worldPoint 在 fsr 精灵处是否为不透明像素（alpha > threshold）
        /// </summary>
        /// <param name="fsr"></param>
        /// <param name="worldPoint"></param>
        /// <param name="alphaThreshold">  </param>
        /// <returns></returns>
        public bool IsSpritePixelOpaque(SpriteRenderer fsr, Vector2 worldPoint, float alphaThreshold = 0.1f)
        {
            if (fsr == null || fsr.sprite == null) return false;
            var sprite = fsr.sprite;
            var tex = GetReadableSpriteTexture(sprite);
            if (tex == null)
            {
                Debug.LogWarning($"Texture for sprite {sprite.name} is not available for sampling.");
                return false;
            }

            // 将世界点转到精灵本地，再转为像素坐标（相对于 sprite.rect）
            Vector2 local = fsr.transform.InverseTransformPoint(worldPoint);
            float ppu = sprite.pixelsPerUnit;
            Vector2 pivot = sprite.pivot; // 以像素为单位，相对 sprite.rect 的左下角
            float px = local.x * ppu + pivot.x;
            float py = local.y * ppu + pivot.y;

            float u = px / sprite.rect.width;
            float v = py / sprite.rect.height;
            // 注意：GetPixelBilinear 的 v 轴是从下到上，而像素坐标系 y 轴是从上到下
            // 如果纹理未翻转则不需要调整v坐标
            if (u < 0f || u > 1f || v < 0f || v > 1f) return false;

            // 使用副本纹理采样（支持缩放）
            Color c = tex.GetPixelBilinear(u, v);
            Color fixerColor=fsr.color;
            return c.a*fixerColor.a > alphaThreshold;
        }

        // 可选：清理缓存（例如场景切换时释放内存）
        public void ClearCache()
        {
            foreach (var kv in s_cache)
            {
                if (kv.Value != null) Object.Destroy(kv.Value);
            }

            s_cache.Clear();
        }
    }
