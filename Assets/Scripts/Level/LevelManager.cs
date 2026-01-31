using System.Collections.Generic;
using System.Linq;
using Clipper2Lib;
using UnityEngine;

namespace Game.Level
{
    public class LevelManager:MonoBehaviour
    {
        private List<LevelItemController> levelItems = new List<LevelItemController>();
        private MaskState maskState;
        
        public Paths64 SelfPaths;
        
        private SpriteMask selfSpriteMask;
        private bool isDynamicLevel;
        private int freshCounter = 0;
        private void Awake()
        {
            levelItems = GetComponentsInChildren<LevelItemController>().ToList();
            foreach (LevelItemController levelItem in levelItems)
            {
                if (levelItem.itemState == ItemState.Dynamic)
                {
                    isDynamicLevel = true;
                    break;
                }
            }
            SelfPaths = GetPaths();
        }
        
        #region 一键调用子物体的相关方法
        /// <summary>
        /// 初始化所有LevelItem的碰撞体
        /// </summary>
        public void InitCollider2D()
        {
            foreach (LevelItemController levelItem in levelItems)
            {
                levelItem.InitCollider2Ds();
            }
        }
        /// <summary>
        /// 根据传入的clip更新所有LevelItem的碰撞体
        /// </summary>
        /// <param name="clip"></param>
        public void UpdateCollider2Ds(Paths64 clip)
        {
            foreach (LevelItemController levelItem in levelItems)
            {
                levelItem.UpdateCollider2D(clip);
            }
        }
        
        public void ReserveUpdateCollider2Ds(Paths64 clip)
        {
            foreach (LevelItemController levelItem in levelItems)
            {
                levelItem.ReserveUpdateCollider2D(clip);
            }
        }
        /// <summary>
        /// 清除所有LevelItem的碰撞体
        /// </summary>
        private void ClearColliders()
        {
            foreach (LevelItemController levelItem in levelItems)
            {
                levelItem.ClearCollider2Ds();
            }
        }
        private void ChangeMaskStates(MaskState newState)
        {
            foreach (LevelItemController levelItem in levelItems)
            {
                levelItem.ChangeState(newState);
            }
        }
        #endregion

        public void ChangeMaskState(MaskState newState)
        {
            this.maskState = newState;
            ChangeMaskStates(newState);
        }
        

        public Paths64 GetPaths()
        {
            if (!isDynamicLevel&& freshCounter>0)
            {
                return SelfPaths;
            }
            else
            {
                freshCounter++;
                Paths64 paths64 = new Paths64();
                foreach (LevelItemController item in levelItems)
                {
                    var pasths = item.SelfPaths64;
                    foreach (var p in pasths)
                    {
                        paths64.Add(p);
                    }
                }
                return paths64;
            }
            
            
        }
    }
}