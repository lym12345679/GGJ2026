using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ScreenShotUI : MonoInstance<ScreenShotUI>
{
    public RawImage RImage;
    public RectTransform RRect; 
    public void Shot()
    {
        int width = Screen.width;
        int height = Screen.height;

        RenderTexture rt = RenderTexture.GetTemporary(width, height, 24, RenderTextureFormat.Default);
        RenderTexture prevActive = RenderTexture.active;

        Camera cam = Camera.main;
        RenderTexture prevCamTarget = cam != null ? cam.targetTexture : null;

        // 把相机渲染到临时 RenderTexture
        if (cam != null)
        {
            cam.targetTexture = rt;
            cam.Render();
            cam.targetTexture = prevCamTarget;
        }

        RenderTexture.active = rt;
        Texture2D screenShot = new Texture2D(width, height, TextureFormat.RGB24, false);
        screenShot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        screenShot.Apply();

        // 恢复并释放
        RenderTexture.active = prevActive;
        RenderTexture.ReleaseTemporary(rt);

        // 将结果显示到 RawImage
        RImage.texture = screenShot;
        RImage.SetNativeSize();
        RImage.gameObject.SetActive(true);
        RRect.DOKill();
        InitRImgTrans();
        RRect.DORotate(new Vector3(0, 0, 30), 0.5f, RotateMode.FastBeyond360);
        RRect.DOScale(new Vector3(0.2f, 0.2f, 0.5f), 0.5f);
        RRect.DOAnchorPos(new Vector3(-Screen.width,-Screen.height,0), 1f);
    }

    public void InitRImgTrans()
    {
        RRect.anchoredPosition = new Vector2(0, 0);
        RRect.rotation = Quaternion.Euler(0, 0, 0);
        RRect.localScale = new Vector3(1, 1, 1);
    }
}
