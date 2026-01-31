using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Draw
{
    public class DrawingBoard : MonoBehaviour
    {
        private RenderTexture casheTex;//缓存上一帧的图
        private RenderTexture currentTex;//当前帧操作的图
        private Material EffectMat;//用来处理图像的材质
        private RawImage rawImage;
        private int width=1200,height=800;
        private void Awake()
        {
            Initialized();
        }
        /// <summary>
        /// 初始化
        /// </summary>
        private void Initialized()
        {
            //设置画布大小
            RectTransform rectTransform = GetComponent<RectTransform>();
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,width);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,height);
            
            SetRectTransformPivot(rectTransform,Vector2.zero);
            rawImage = GetComponent<RawImage>();
            EffectMat = new Material(Shader.Find("Brush/BrushEffect"));
            casheTex = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32);
            Graphics.Blit(null,casheTex, EffectMat,1);//初始化透明图,使用第二个通道
            currentTex = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32);
            
            Graphics.Blit(casheTex, currentTex);
            rawImage.texture = currentTex;
        }
        /// <summary>
        /// 设置 RectTransform 的 pivot，同时保持其在父物体中的视觉位置不变。
        /// </summary>
        /// <param name="rectTransform"></param>
        /// <param name="pivot"></param>
        public void SetRectTransformPivot(RectTransform rectTransform, Vector2 pivot)
        {
            Vector2 dir= pivot - rectTransform.pivot;
            rectTransform.pivot = pivot;
            Vector3 offset = new Vector3();
            offset.x = dir.x * rectTransform.rect.size.x;
            offset.y = dir.y * rectTransform.rect.size.y;
            rectTransform.localPosition += offset;
        }
    
        public void RenderBrushToBoard(DrawingBrush brush, Vector2 uv)
        {
            Vector2 dir = uv - brush.lastUV;
            float brushSize= brush.brushSize/2;
            int length=Mathf.CeilToInt(dir.magnitude/brushSize);
            if (Vector3.SqrMagnitude(dir) > brushSize * brushSize)
            {
                for (int i = 0; i < length; i++)
                {
                    
                }
            }
        }
    }

}
