using System;
using System.Collections;
using System.Collections.Generic;
using Game.Level;
using UnityEngine;
using UnityEngine.UI;

public class HighlitOperator : MonoBehaviour
{
    public Image MoveHighLight, RotateHighLight;

    bool _isDragging;
    Vector3 _lastMouseWorldPos;
    // 旋转相关
    [SerializeField] float rotateSensitivity = 180f; // 每个世界单位对应的角度，可在 Inspector 调整
    bool _isRotating;
    Vector3 _lastMouseWorldPosForRotate;

    private void Update()
    {
        if (Input.GetKey(KeyCode.R)&&Input.GetMouseButton(0))
        {
            if (!_isRotating)
            { 
                OnRotating();
            }
        }
        else if(_isRotating)
        {
            OnRotating();
        }
        
        if (Input.GetKey(KeyCode.T)&&Input.GetMouseButton(0))
        {
            if (!_isDragging)
            {
                OnMoving();
            }
        }
        else if(_isDragging)
        {
            OnMoving();
        }
        
        // 旋转模式下，根据鼠标水平移动量旋转遮罩
        if (_isRotating)
        {
            if (LevelsManager.Instance == null) return;
            Vector3 cur = MouseWorldPosition();
            float deltaX = cur.x - _lastMouseWorldPosForRotate.x;
            if (Mathf.Abs(deltaX) > Mathf.Epsilon)
            {
                float angle = deltaX * rotateSensitivity;
                LevelsManager.Instance.RotateLevelMask(angle);
                _lastMouseWorldPosForRotate = cur;
            }
        }

        // 拖拽模式下，每帧根据鼠标世界坐标增量移动遮罩
        if (_isDragging)
        {
            if (LevelsManager.Instance == null) return;

            Vector3 cur = MouseWorldPosition();
            Vector3 delta = cur - _lastMouseWorldPos;
            if (delta.sqrMagnitude > Mathf.Epsilon)
            {
                LevelsManager.Instance.MoveLevelMask(delta.x, delta.y);
                _lastMouseWorldPos = cur;
            }
        }
    }

    private void OnMoving()
    {
        _isDragging=!_isDragging;
        if (_isDragging)
        {
            _lastMouseWorldPos = MouseWorldPosition();
            if (MoveHighLight != null) MoveHighLight.enabled = true;
        }
        else
        {
            if (MoveHighLight != null) MoveHighLight.enabled = false;
        }
    }
    private void OnRotating()
    {
        // 切换旋转模式
        _isRotating = !_isRotating;
        if (_isRotating)
        {
            _lastMouseWorldPosForRotate = MouseWorldPosition();
            if (RotateHighLight != null) RotateHighLight.enabled = true;
        }
        else
        {
            if (RotateHighLight != null) RotateHighLight.enabled = false;
        }
    }

    // 将屏幕坐标转为世界坐标（Z 设为 0）
    Vector3 MouseWorldPosition()
    {
        Camera cam = Camera.main;
        if (cam == null) return Vector3.zero;
        Vector3 p = cam.ScreenToWorldPoint(Input.mousePosition);
        p.z = 0f;
        return p;
    }

    // public void HightMoveImg(bool b)
    // {
    //     if(b) MoveHighLight.enabled = true;
    //     else MoveHighLight.enabled = false;
    // }
    //
    // public void HightRotateImg(bool b)
    // {
    //     if (b) RotateHighLight.enabled = true;
    //     else RotateHighLight.enabled = false;
    // }
}
