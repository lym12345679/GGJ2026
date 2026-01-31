using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace Platforms
{
    public class Platform : MonoBehaviour
    {
        [SerializeField] List<Vector3> Points = new List<Vector3>();
        [SerializeField] float speed = 2f;
        [SerializeField] float waitAtPoint; // 到达点后等待时间（秒）
        [SerializeField] bool startAtFirstPoint = true; // 启动时是否移动到第一个点
        [SerializeField] bool readPointsFromChildren; // 若为 true，则从子物体读取点并覆盖 points
        readonly Vector3 originalPosition;
        int _currentIndex;
        int _dir; // 1 forward, -1 backward
        float _waitTimer;

        private void Awake()
        {
            
        }

        void Start()
        {
            if (readPointsFromChildren)
            {
                Points.Clear();
                for (int i = 0; i < transform.childCount; i++)
                {
                    var child = transform.GetChild(i);
                    Points.Add(new Vector3(child.localPosition.x+transform.localPosition.x,
                        child.localPosition.y+ transform.localPosition.y,
                        child.localPosition.z+ transform.localPosition.z));
                }
            }
            else
            {
                for (int i = 0; i < Points.Count; i++)
                {
                    Points[i]=new Vector3(Points[i].x+ transform.localPosition.x,
                        Points[i].y+ transform.localPosition.y,
                        Points[i].z+ transform.localPosition.z);
                }
            }
            if (Points.Count == 0) return;

            _currentIndex = 0;
            _dir = 1;

            if (startAtFirstPoint)
            {
                // 直接设置到第一个点位置（根据坐标类型转换）
                Vector3 world = Points[0];
                transform.localPosition = world;
            }
        }

        void Update()
        {
            if (Points.Count == 0) return;

            if (_waitTimer > 0f)
            {
                _waitTimer -= Time.deltaTime;
                return;
            }

            // 目标点世界坐标
            Vector3 target = Points[_currentIndex];
            Vector3 pos = transform.localPosition;

            // 移动
            Vector3 newPos = Vector3.MoveTowards(pos, target, speed * Time.deltaTime);
            transform.localPosition = newPos;

            // 到达判定
            if ((target - newPos).sqrMagnitude < 0.0001f)
            {
                // 到达当前点，设置等待
                if (waitAtPoint > 0f)
                {
                    _waitTimer = waitAtPoint;
                }

                // 计算下一个索引（来回）
                int next = _currentIndex + _dir;
                if (next >= Points.Count)
                {
                    // 到达末端，反向
                    _dir = -1;
                    _currentIndex = Points.Count - 1; // 保持在末端
                    if (Points.Count > 1) _currentIndex += _dir; // 移向倒数第二个
                }
                else if (next < 0)
                {
                    // 到达起点，反向
                    _dir = 1;
                    _currentIndex = 0;
                    if (Points.Count > 1) _currentIndex += _dir;
                }
                else
                {
                    _currentIndex = next;
                }

                // 特殊情况：如果只有一个点，则保持不动
                if (Points.Count == 1)
                {
                    _currentIndex = 0;
                }
            }
        }

        void OnDrawGizmosSelected()
        {
            if (Points == null || Points.Count == 0) return;

            Gizmos.color = Color.cyan;
            Vector3 prev = transform.localPosition;
            for (int i = 0; i < Points.Count; i++)
            {
                Vector3 w;
                if (Application.isPlaying)
                {
                    w = Points[i];
                }
                else
                {
                    w = Points[i] + transform.localPosition;
                }
                Gizmos.DrawSphere(w, 0.075f);
                Gizmos.DrawLine(prev, w);
                prev = w;
            }

            // 画回到起点的连线（可选，表示循环方向）
            if (Points.Count > 1)
            {
                Vector3 first;
                if (Application.isPlaying)
                {
                    first = Points[0];
                }
                else
                {
                    first = Points[0] + transform.localPosition;
                }
                Gizmos.DrawLine(prev, first);
            }
        }
    }
}
