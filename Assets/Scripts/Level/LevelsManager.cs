using System;
using UnityEngine;
using System.Collections.Generic;
using Clipper2Lib;
using UnityEngine.Serialization;

namespace Game.Level
{
    public class LevelsManager : MonoBehaviour
    {
        public static LevelsManager Instance;
        public List<LevelManager> LevelManagers = new List<LevelManager>();
        public int CurrentLevelIndex = 0;
        public int CurrentMaskIndex = -1;
        private LevelManager currentLevel => LevelManagers[CurrentLevelIndex];
        private LevelManager nextLevel => LevelManagers[CurrentLevelIndex + 1];

        private LevelManager CurrentLevelMask
        {
            get { return LevelManagers[CurrentMaskIndex]; }
        }

        public Transform Propmt;
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            
        }

        private void Start()
        {
            Init();
            SavePoint();
            Restart();
        }

        private void Init()
        {
            for (int i = 1; i < LevelManagers.Count; i++)
            {
                LevelManagers[i].gameObject.SetActive(false);
            }
        }

        private void Update()
        {
            if (CurrentMaskIndex >= 0)
            {
                //通过遮罩关卡的变换矩阵更新下一关卡的碰撞体
                Paths64 local = CurrentLevelMask.GetPaths();
                Paths64 world = LocalPathsToWorld(local, CurrentLevelMask.transform);
                // nextLevel.UpdateCollider2Ds(CurrentLevelMask.GetPaths());
                nextLevel.UpdateCollider2Ds(world);
                currentLevel.ReserveUpdateCollider2Ds(world);
            }
        }

        // 将 Paths64（表示一组本地坐标点）转换为指定 transform 的世界坐标 Vector2 数组
        Paths64 LocalPathsToWorld(Paths64 local, Transform t)
        {
            Paths64 world = new Paths64(local.Count);
            for (int i = 0; i < local.Count; i++)
            {
                Path64 path = local[i];
                Path64 worldPath = new Path64(path.Count);
                for (int j = 0; j < path.Count; j++)
                {
                    Point64 p = path[j];
                    Vector3 localPoint = new Vector3(p.X / 1000f, p.Y / 1000f, 0);
                    Vector3 worldPoint = t.TransformPoint(localPoint);
                    worldPath.Add(new Point64((long)(worldPoint.x * 1000), (long)(worldPoint.y * 1000)));
                }
                world.Add(worldPath);
            }

            return world;
        }

        public void NextLevel(int index)
        {
            if (index >= LevelManagers.Count-1)
            {
                GlobalSceneManager.Instance.LoadScene(SceneType.Menu);
            }
            else
            {
                if (Propmt.gameObject.activeSelf)
                {
                    Propmt.gameObject.SetActive(false);
                }
                ScreenShotUI.Instance.Shot();
                while (CurrentLevelIndex < index)
                {
                    NextLevel();
                }
            }
            
        }

        public void NextLevel()
        {
            LevelManagers[CurrentLevelIndex].ChangeMaskState(MaskState.Mask);
            LevelManagers[CurrentLevelIndex].gameObject.SetActive(false);
            CurrentLevelIndex++;
            // 激活当前关卡
            LevelManagers[CurrentLevelIndex].gameObject.SetActive(true);
            LevelManagers[CurrentLevelIndex].ChangeMaskState(MaskState.Object);
            LevelManagers[CurrentLevelIndex + 1].gameObject.SetActive(true);
            // 设置遮罩
            LevelManagers[CurrentLevelIndex + 1].ChangeMaskState(MaskState.VisibleInMask);
            if (CurrentMaskIndex > -1)
            {
                InitCurrentMask();
                CurrentLevelMask.gameObject.SetActive(false);
                BagPanelUIController.Instance.UseMask(CurrentMaskIndex);
                BagPanelUIController.Instance.AddLevelItem(CurrentMaskIndex);
                CurrentMaskIndex = -1;
            }

            LevelPanelUIController.Instance.UnlockTags(CurrentLevelIndex);
            BagPanelUIController.Instance.AddLevelItem(CurrentLevelIndex - 1);
            TopUI.Instance.ShowStartButton();
            SavePoint();
            FreezePlayer();
        }
        /// <summary>
        /// 设置当前遮罩关卡索引,如果之前有遮罩则先归还, 如果index小于0则表示取消遮罩
        /// </summary>
        /// <param name="index"></param>
        public void SetMask(int index)
        {
            if(index==CurrentMaskIndex) return;
            
            // 更换遮罩前归还之前的遮罩关卡
            if (CurrentMaskIndex > -1 && CurrentMaskIndex < CurrentLevelIndex)
            {
                InitCurrentMask();
                CurrentLevelMask.gameObject.SetActive(false);
                BagPanelUIController.Instance.AddLevelItem(CurrentMaskIndex);
            }
            
            CurrentMaskIndex = index;
            // 激活新的遮罩关卡
            if (index >-1 && index < CurrentLevelIndex)
            {
                CurrentLevelMask.gameObject.SetActive(true);
            }
        }

        private void InitCurrentMask()
        {
            if(CurrentMaskIndex<0||CurrentMaskIndex>=CurrentLevelIndex) return;
            CurrentLevelMask.transform.localPosition = Vector3.zero;
            CurrentLevelMask.transform.localRotation = Quaternion.identity;
        }
        public void RotateLevelMask(float angle)
        {
            if (CurrentMaskIndex >= 0)
            {
                CurrentLevelMask.transform.Rotate(0, 0, angle);
            }
        }

        public void MoveLevelMask(float x, float y)
        {
            if (CurrentMaskIndex >= 0)
            {
                CurrentLevelMask.transform.Translate(new Vector3(x, y, 0), Space.World);
            }
        }

        // 用于在 Game 窗口运行时绘制线的简单材质
        Material _lineMaterial;

        void CreateLineMaterial()
        {
            if (_lineMaterial != null) return;
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            _lineMaterial = new Material(shader);
            _lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            // 允许半透明
            _lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            _lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            _lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            _lineMaterial.SetInt("_ZWrite", 0);
        }

        // 在 Game 窗口绘制遮罩路径（使用 GL）
        void OnRenderObject()
        {
            if (CurrentMaskIndex < 0) return;
            if (!gameObject.activeInHierarchy) return;

            CreateLineMaterial();
            if (_lineMaterial == null) return;

            Paths64 local = CurrentLevelMask.GetPaths();
            Paths64 world = LocalPathsToWorld(local, CurrentLevelMask.transform);

            _lineMaterial.SetPass(0);
            GL.PushMatrix();
            // 使用模型视图矩阵为单位，传入世界位置坐标直接绘制
            GL.MultMatrix(Matrix4x4.identity);
            GL.Begin(GL.LINES);
            GL.Color(Color.red);

            for (int i = 0; i < world.Count; i++)
            {
                Path64 path = world[i];
                for (int j = 0; j < path.Count; j++)
                {
                    Point64 p1 = path[j];
                    Point64 p2 = path[(j + 1) % path.Count];
                    Vector3 v1 = new Vector3(p1.X / 1000f, p1.Y / 1000f, 0);
                    Vector3 v2 = new Vector3(p2.X / 1000f, p2.Y / 1000f, 0);
                    GL.Vertex(v1);
                    GL.Vertex(v2);
                }
            }

            GL.End();
            GL.PopMatrix();
        }

        private void OnDrawGizmos()
        {
            if(CurrentMaskIndex<0) return;
            Paths64 local = CurrentLevelMask.GetPaths();
            Paths64 world = LocalPathsToWorld(local, CurrentLevelMask.transform);
            Gizmos.color = Color.red;
            for (int i = 0; i < world.Count; i++)
            {
                Path64 path = world[i];
                for (int j = 0; j < path.Count; j++)
                {
                    Point64 p1 = path[j];
                    Point64 p2 = path[(j + 1) % path.Count];
                    Vector3 v1 = new Vector3(p1.X / 1000f, p1.Y / 1000f, 0);
                    Vector3 v2 = new Vector3(p2.X / 1000f, p2.Y / 1000f, 0);
                    Gizmos.DrawLine(v1, v2);
                }
            }
        }

        #region MyRegion

        public Transform PlayerTransform;
        private Vector3 savePoint;
        public void FreezePlayer()
        {
            if (PlayerTransform != null)
            {
                var rb = PlayerTransform.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.velocity = Vector2.zero;
                    rb.angularVelocity = 0f;
                    rb.isKinematic = true;
                    rb.constraints = RigidbodyConstraints2D.FreezeAll;
                }
            }
        }
        public void UnfreezePlayer()
        {
            if (PlayerTransform != null)
            {
                var rb = PlayerTransform.GetComponent<Rigidbody2D>();
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                if (rb != null)
                {
                    rb.isKinematic = false;
                }
            }
        }
        public void SavePoint()
        {
            if (PlayerTransform != null)
            {
                savePoint = PlayerTransform.position;
            }
        }

        public void Restart()
        {
            TopUI.Instance.ShowStartButton();
            PlayerTransform.position = savePoint;
            FreezePlayer();
            SetMask(-1);
        }
        
        #endregion
    }
}