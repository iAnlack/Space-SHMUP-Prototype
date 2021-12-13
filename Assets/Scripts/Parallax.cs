using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    [Header("Set in Inspector")]
    public GameObject PointOfInterest;   // ������� ������
    public GameObject[] Panels;          // �������������� ������ ��������� �����
    public float ScrollSpeed = -30f;
    // MotionMult ���������� ������� ������� ������� �� ����������� ������� ������
    public float MotionMult = 0.25f;

    private float _panelHigh;            // ������ ������ ������
    private float _depth;                // ������� ������� (�.�. pos.z)

    private void Start()
    {
        _panelHigh = Panels[0].transform.localScale.y;
        _depth = Panels[0].transform.position.z;

        // ���������� ������ � ��������� �������
        Panels[0].transform.position = new Vector3(0, 0, _depth);
        Panels[1].transform.position = new Vector3(0, _panelHigh, _depth);
    }

    private void Update()
    {
        float tY, tX = 0;
        tY = Time.time * ScrollSpeed % _panelHigh + (_panelHigh * 0.5f);

        if (PointOfInterest != null)
        {
            tX = -PointOfInterest.transform.position.x * MotionMult;
        }

        // �������� ������ Panels[0]
        Panels[0].transform.position = new Vector3(tX, tY, _depth);
        // �������� ������ Panels[1], ����� ������� ������ ������������� �������� ����
        if (tY >= 0)
        {
            Panels[1].transform.position = new Vector3(tX, tY - _panelHigh, _depth);
        }
        else
        {
            Panels[1].transform.position = new Vector3(tX, tY + _panelHigh, _depth);
        }
    }
}
