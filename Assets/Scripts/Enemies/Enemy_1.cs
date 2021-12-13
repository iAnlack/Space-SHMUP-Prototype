using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_1 : Enemy // Enemy_1 ��������� ����� Enemy
{
    [Header("Set in Inspector: Enemy_1")]
    public float WaveFrequency = 2; // ����� ������ ������� ����� ���������
    public float WaveWidth = 4; // ������ ��������� � ������
    public float WaveRotY = 45;

    private float _x0; // ��������� �������� ���������� x
    private float _birthTime;

    // ����� Start ������ �������� ��� ����� �����, ������ ��� �� ������������ ������������ Enemy
    private void Start()
    {
        
    }

    // �������������� ������� Move() ����������� Enemy
    public override void Move()
    {
        // �.�. pos - ��� ��������, ������ �������� �������� pos.x,
        // ������� ������� pos � ���� ������� Vector3, ���������� ��� ���������
        Vector3 tempPos = pos;
        // �������� theta ���������� � �������� �������
        float age = Time.time - _birthTime;
        float theta = Mathf.PI * 2 * age / WaveFrequency;
        float sin = Mathf.Sin(theta);
        tempPos.x = _x0 + WaveWidth * sin;
        pos = tempPos;

        // ��������� ������� ������������ ��� Y
        Vector3 rot = new Vector3(0, sin * WaveRotY, 0);
        this.transform.rotation = Quaternion.Euler(rot);

        // base.Move() ������������ �������� ����, ����� ��� Y
        base.Move();

        //Debug.Log(bndCheck.IsOnScreen);
    }
}
