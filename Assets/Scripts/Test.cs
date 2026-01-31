using UnityEngine;
using Clipper2Lib;

namespace DefaultNamespace
{
    public class Test : MonoBehaviour
    {
        // 缩放因子（将整数坐标映射到 Unity 单位）
        public float scale = 0.5f;

        // Clipper 结果缓存，以免每帧重复计算
        private Paths64 _subj = new Paths64();
        private Paths64 _clip = new Paths64();
        private Paths64 _solutionIntersect = new Paths64();
        private Paths64 _solutionUnion = new Paths64();
        private Paths64 _solutionDifference = new Paths64();

        void Awake()
        {
            // 创建两个简单矩形多边形（顺时针或逆时针均可）
            Path64 rectA = new Path64() {
                new Point64(0, 0),
                new Point64(4, 0),
                new Point64(4, 4),
                new Point64(0, 4)
            };

            Path64 rectB = new Path64() {
                new Point64(2, 2),
                new Point64(6, 2),
                new Point64(6, 6),
                new Point64(2, 6)
            };

            _subj = new Paths64() { rectA };
            _clip = new Paths64() { rectB };

            // 计算布尔运算（使用 EvenOdd 填充规则，常用选择）
            _solutionIntersect = Clipper.Intersect(_subj, _clip, FillRule.EvenOdd);
            _solutionUnion = Clipper.Union(_subj, _clip, FillRule.EvenOdd);
            _solutionDifference = Clipper.Difference(_subj, _clip, FillRule.EvenOdd);

            // 打印信息到控制台
            Debug.Log($"Clipper 示例: subject paths={_subj.Count}, clip paths={_clip.Count}");
            Debug.Log($"Intersect paths={_solutionIntersect.Count}, area={Clipper.Area(_solutionIntersect)}");
            Debug.Log($"Union paths={_solutionUnion.Count}, area={Clipper.Area(_solutionUnion)}");
            Debug.Log($"Difference paths={_solutionDifference.Count}, area={Clipper.Area(_solutionDifference)}");
        }

        void OnDrawGizmos()
        {
            // 如果在编辑器中 Awake 可能未被调用，确保有默认数据用于绘制
            if (_subj == null || _subj.Count == 0)
            {
                // 用同样的初始化逻辑，但不重复日志
                _subj = new Paths64() { new Path64() { new Point64(0,0), new Point64(4,0), new Point64(4,4), new Point64(0,4) } };
                _clip = new Paths64() { new Path64() { new Point64(2,2), new Point64(6,2), new Point64(6,6), new Point64(2,6) } };
                _solutionIntersect = Clipper.Intersect(_subj, _clip, FillRule.EvenOdd);
                _solutionUnion = Clipper.Union(_subj, _clip, FillRule.EvenOdd);
                _solutionDifference = Clipper.Difference(_subj, _clip, FillRule.EvenOdd);
            }

            // 分别用不同颜色绘制：原始 subject（蓝）、clip（红）、交集（绿）、并集（yellow）、差集（magenta）
            DrawPaths(_subj, Color.blue, new Vector2(-8f, 0f));
            DrawPaths(_clip, Color.red, new Vector2(-8f, 6f));

            DrawPaths(_solutionIntersect, Color.green, new Vector2(0f, 0f));
            DrawPaths(_solutionUnion, Color.yellow, new Vector2(0f, 6f));
            DrawPaths(_solutionDifference, Color.magenta, new Vector2(8f, 0f));
        }

        private void DrawPaths(Paths64 paths, Color col, Vector2 offset)
        {
            if (paths == null) return;
            Gizmos.color = col;
            foreach (Path64 path in paths)
            {
                int cnt = path.Count;
                if (cnt < 2) continue;
                for (int i = 0; i < cnt; i++)
                {
                    Point64 a = path[i];
                    Point64 b = path[(i + 1) % cnt];
                    Vector3 va = new Vector3(a.X * scale + offset.x, a.Y * scale + offset.y, 0f);
                    Vector3 vb = new Vector3(b.X * scale + offset.x, b.Y * scale + offset.y, 0f);
                    Gizmos.DrawLine(va, vb);
                }
            }
        }
    }
}