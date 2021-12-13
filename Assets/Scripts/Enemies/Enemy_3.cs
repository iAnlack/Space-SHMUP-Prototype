using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_3 : Enemy // Enemy_3 ��������� ����� Enemy
{
    // ���������� �������� Enemy_3 ����������� ���� �������� ������������ ������ ����� �� ����� ��� 2 ������
    [Header("Set in Inspector: Enemy_3")]
    public float LifeTime = 5;

    [Header("Set Dynamically: Enemy_3")]
    public Vector3[] Points;
    public float BirthTime;

    // � ����� ����� Start ������ �������� ��� ����� �����, �.�. �� ������������ ������������ Enemy
    private void Start()
    {
        Points = new Vector3[3]; // ���������������� ������ �����

        // ��������� ������� ��� ���������� � Main.SpawnEnemy()
        Points[0] = pos;

        // ���������� xMin � xMax ��� ��, ��� ��� ������ Main.SpawnEnemy()
        float xMin = -bndCheck.CamWidth + bndCheck.Radius;
        float xMax = bndCheck.CamWidth - bndCheck.Radius;
        
        Vector3 v;
        // �������� ������� ������� ����� ���� ������ ������� ������
        v = Vector3.zero;
        v.x = Random.Range(xMin, xMax);
        v.y = -bndCheck.CamHeight * Random.Range(2.75f, 2);
        Points[1] = v;

        // �������� ������� �������� ����� ���� ������� ������� ������
        v = Vector3.zero;
        v.y = pos.y;
        v.x = Random.Range(xMin, xMax);
        Points[2] = v;

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
            // ���� ��������� Enemy_3 �������� ���� ��������� ����
            Destroy(this.gameObject);
            return;
        }

        // ��������������� ������ ����� �� ��� ������
        Vector3 p01, p12;
        u = u - 0.2f * Mathf.Sin(u * Mathf.PI * 2);
        p01 = (1 - u) * Points[0] + u * Points[1];
        p12 = (1 - u) * Points[1] + u * Points[2];
        pos = (1 - u) * p01 + u * p12;
    }
}
