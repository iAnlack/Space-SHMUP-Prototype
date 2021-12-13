using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// � ������ �� 4� ��������� ����� ������� ������� /// � ����� ������� Tab.
/// <summary>
/// ������������� ����� �������� ������� �� ������� ������.
/// �����: �������� ������ � ��������������� ������� Main Camera � [0, 0, 0].
/// </summary>

public class BoundsCheck : MonoBehaviour
{
    [Header("Set in Inspector")]
    public float Radius = 1f;
    public bool KeepOnScreen = true;

    [Header("Set Dynamically")]
    public float CamWidth;
    public float CamHeight;
    public bool IsOnScreen = true;

    [HideInInspector]
    public bool OffRight, OffLeft, OffUp, OffDown;

    private void Awake()
    {
        CamHeight = Camera.main.orthographicSize;
        CamWidth = CamHeight * Camera.main.aspect;
    }

    private void LateUpdate()
    {
        Vector3 pos = transform.position;
        IsOnScreen = true;
        OffRight = OffLeft = OffUp = OffDown = false;

        if (pos.x > CamWidth - Radius)
        {
            pos.x = CamWidth - Radius;
            OffRight = true;
        }
        if (pos.x < -CamWidth + Radius)
        {
            pos.x = -CamWidth + Radius;
            OffLeft = true;
        }
        if (pos.y > CamHeight - Radius)
        {
            pos.y = CamHeight - Radius;
            OffUp = true;
        }
        if (pos.y < -CamHeight + Radius)
        {
            pos.y = -CamHeight + Radius;
            OffDown = true;
        }

        IsOnScreen = !(OffRight || OffLeft || OffUp || OffDown);
        if (KeepOnScreen && !IsOnScreen)
        {
            transform.position = pos;
            IsOnScreen = true;
            OffRight = OffLeft = OffUp = OffDown = false;
        }
        
    }

    // ������ ������� � ������ Scene � ������� OnDrawGizmos()
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            return;
        }
        Vector3 boundSize = new Vector3(CamWidth * 2, CamHeight * 2, 0.1f);
        Gizmos.DrawWireCube(Vector3.zero, boundSize);
    }
}
