using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    [Header("Set in Inspector")]
    public float RotationsPerSecond = 0.1f;

    [Header("Set Dynamycally")]
    public int LevelShown = 0;

    // ������� ���������� �� ������������ � ����������
    private Material _mat;

    private void Start()
    {
        _mat = GetComponent<Renderer>().material;
    }

    private void Update()
    {
        // ��������� ������� �������� ��������� ���� �� �������-�������� Hero
        int currLevel = Mathf.FloorToInt(Hero.S.ShieldLevel);
        // ���� ��� ���������� �� LevelShown...
        if (LevelShown != currLevel)
        {
            LevelShown = currLevel;
            // ��������������� �������� � ��������, ����� ���������� ���� � ������ ���������
            _mat.mainTextureOffset = new Vector2(0.2f * LevelShown, 0);
        }
        // ������������ ���� � ������ ����� � ���������� ���������
        float rZ = -(RotationsPerSecond * Time.time * 360) % 360f;
        transform.rotation = Quaternion.Euler(0, 0, rZ);
    }
}
