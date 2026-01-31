using System;
using System.Collections;
using System.Collections.Generic;
using Clipper2Lib;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Level
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(SpriteMask))]
    [RequireComponent(typeof(PolygonCollider2D))]
    public class LevelItemController : MonoBehaviour
    {
        public Paths64 colliderPaths;
        public PolygonCollider2D PCollider2D,RecordCollider2D;
        public SpriteRenderer SRenderer;
        public SpriteMask SMask;
        private Paths64 selfPaths;
        public ItemState itemState;
        private Vector3 originLocalPos;
        public Paths64 SelfPaths64
        {
            get
            {
                if (selfPaths == null || selfPaths.Count == 0)
                {
                    selfPaths = Collider2DToPaths64(PCollider2D);
                    originLocalPos = Vector3.zero; // 记录首次的 localPosition 作为基准
                }

                if (itemState == ItemState.Static)
                {
                    return selfPaths;
                }
                else
                {
                    Paths64 fixedPaths = new Paths64();

                    // 计算相对于首次记录的 localPosition 的增量，避免直接修改 selfPaths
                    Vector3 deltaLocal = transform.localPosition - originLocalPos;
                    long offsetX = (long)(deltaLocal.x * 1000f);
                    long offsetY = (long)(deltaLocal.y * 1000f);

                    foreach (var path in selfPaths)
                    {
                        Path64 newPath = new Path64();
                        for (int i = 0; i < path.Count; i++)
                        {
                            Point64 p = path[i];
                            newPath.Add(new Point64(p.X + offsetX, p.Y + offsetY));
                        }

                        fixedPaths.Add(newPath);
                    }

                    return fixedPaths;
                }
            }
        }

        private void Awake()
        {
            if (ReferenceEquals(SMask.sprite, null))
            {
                SMask.sprite = SRenderer.sprite;
            }
        }

        private Paths64 Collider2DToPaths64(PolygonCollider2D co)
        {
            Paths64 ps = new Paths64();
            for (int i = 0; i < co.pathCount; i++)
            {
                Vector2[] path = co.GetPath(i);
                Path64 clipperPath = new Path64();
                foreach (var point in path)
                {
                    // 将 Vector2 转换为 Point64，并应用缩放因子
                    clipperPath.Add(new Point64((long)(point.x * 1000), (long)(point.y * 1000)));
                }
                ps.Add(clipperPath);
            }
            return ps;
        }

        public void ChangeState(MaskState newState)
        {
            switch (newState)
            {
                case MaskState.Mask: ToMaskState(); break;
                case MaskState.Object: ToObjectState(); break;
                case MaskState.VisibleInMask : ToVisibleInMask(); break;
                default: break;
            }
        }

        private void ToMaskState()
        {
            SRenderer.enabled = false;
            SMask.enabled = true;
            ClearCollider2Ds();
        }

        private void ToObjectState()
        {
            SRenderer.maskInteraction= SpriteMaskInteraction.VisibleOutsideMask;
            SRenderer.enabled = true;
            SMask.enabled = false;
            InitCollider2Ds();
        }

        private void ToVisibleInMask()
        {
            SRenderer.maskInteraction= SpriteMaskInteraction.VisibleInsideMask;
            SRenderer.enabled = true;
            SMask.enabled = false;
            ClearCollider2Ds();
        }
        
        
        public void UpdateCollider2D(Paths64 clip)
        {
            ClearCollider2Ds();
            Paths64 result = Clipper.Intersect(SelfPaths64, clip, FillRule.EvenOdd);
            DrawColliders(result);
        }

        public void ReserveUpdateCollider2D(Paths64 clip)
        {
            ClearCollider2Ds();
            Paths64 result = Clipper.Difference(SelfPaths64, clip, FillRule.EvenOdd);
            DrawColliders(result);
        }
        public void InitCollider2Ds()
        {
            DrawColliders(SelfPaths64);
        }

        /// <summary>
        /// 清除Collider2D的所有路径
        /// </summary>
        public void ClearCollider2Ds()
        {
            PCollider2D.pathCount = 0;
        }

        /// <summary>
        /// 根据Paths64绘制Collider2D
        /// </summary>
        /// <param name="paths"></param>
        private void DrawColliders(Paths64 paths)
        {
            PCollider2D.pathCount = paths.Count;
            for (int i = 0; i < paths.Count; i++)
            {
                Path64 path = paths[i];
                Vector2[] unityPath = new Vector2[path.Count];
                for (int j = 0; j < path.Count; j++)
                {
                    Point64 point = path[j];
                    
                    if(itemState== ItemState.Dynamic)
                    {
                        unityPath[j] = new Vector2(point.X / 1000f - transform.localPosition.x,
                            point.Y / 1000f - transform.localPosition.y);
                    }
                    else
                    {
                        unityPath[j] = new Vector2(point.X / 1000f, point.Y / 1000f);
                    }
                }

                PCollider2D.SetPath(i, unityPath);
            }
        }
    }
}