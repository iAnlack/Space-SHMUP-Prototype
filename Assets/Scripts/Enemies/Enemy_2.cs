using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_2 : Enemy // Enemy_2 ��������� ����� Enemy
{
    [Header("Set in Inspector: Enemy_2")]
    // ����������, ��������� ���� ����� ������� �������������� �������� ��������
    public float SinEccentricity = 0.6f;
    public float LifeTime = 10;

    [Header("Set Dynamically: Enemy_2")]
    // Enemy_2 ���������� �������� ������������ ����� ����� �������, ������� ��������� �� ���������
    public Vector3 P0;
    public Vector3 P1;
    public float BirthTime;

    private void Start()
    {
        // ������� ��������� ����� �� ����� ������� ������
        P0 = Vector3.zero;
        P0.x = -bndCheck.CamWidth - bndCheck.Radius;
        P0.y = Random.Range(-bndCheck.CamHeight, bndCheck.CamHeight);

        // ������� ��������� ����� �� ������ ������� ������
        P1 = Vector3.zero;
        P1.x = bndCheck.CamWidth + bndCheck.Radius;
        P1.y = Random.Range(-bndCheck.CamHeight, bndCheck.CamHeight);

        // �������� �������� ��������� � �������� ����� ������
        if (Random.value > 0.5f)
        {
            // ��������� ����� .x ������ ����� ��������� � �� ������ ���� ������
            P0.x *= -1;
            P1.x *= -1;
        }

        // �������� � BirthTime ������� �����
        BirthTime = Time.time;
    }

    public override void Move()
    {
        // ������ ����� ����������� �� ������ �������� u ����� 0 � 1
        float u = (Time.time - BirthTime) / LifeTime;

        // ���� u > 1, ������, ������� ���������� ������, ��� LifeTime
        if (u > 1)
        {
            // ���� ��������� Enemy_2 �������� ���� ��������� ����
            Destroy(this.gameObject);
            return;
        }

        // �������������� u ����������� �������� ������, ������������ �� ���������
        u = u + SinEccentricity * (Mathf.Sin(u * Mathf.PI * 2));

        // ��������������� �������������� ����� ����� �������
        pos = (1 - u) * P0 + u * P1;
    }
}
