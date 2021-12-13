using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enemy_4 �������� �� ������� ��������, �������� ��������� ����� �� ������ � ������������ � ���. 
/// ���������� �� �����, �������� ������ ��������� ����� � ���������� ���������, ���� ����� �� ��������� ���.
/// </summary>

public class Enemy_4 : Enemy
{
    [Header("Set in Inspector: Enemy_4")]
    public Part[] Parts;           // ������ ������, ������������ �������

    private Vector3 _p0, _p1;      // ��� ����� ������������
    private float _timeStart;      // ����� �������� ����� �������
    private float _duration = 4;   // ����������������� �����������

    private void Start()
    {
        // ��������� ������� ��� ������� � Main.SpawnEnemy(),
        // ������� ������� � ��� ��������� �������� � _p0 � _p1
        _p0 = _p1 = pos;

        InitMovement();

        // �������� � ��� ������� ������ � �������� ������ ����� � Parts
        Transform t;
        foreach (Part part in Parts)
        {
            t = transform.Find(part.Name);
            if (t != null)
            {
                part.PartGO = t.gameObject;
                part.DamageMaterial = part.PartGO.GetComponent<Renderer>().material;
            }
        }
    }

    // �������������� ����� OnCollisionEnter �� �������� Enemy.cs
    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("���-������");
        GameObject otherGO = collision.gameObject;
        switch (otherGO.tag)
        {
            case "ProjectileHero":
                Projectile projectile = otherGO.GetComponent<Projectile>();

                // ���� ������� �� �������� ������, �� ���������� ���
                if (!bndCheck.IsOnScreen)
                {
                    Destroy(otherGO);
                    break;
                }

                // �������� ��������� �������
                GameObject gameObjectHit = collision.contacts[0].thisCollider.gameObject;
                Part partHit = FindPart(gameObjectHit);
                if (partHit == null)   // ���� partHit �� ���������...
                {
                    //Debug.Log("partHit == null");
                    gameObjectHit = collision.contacts[0].otherCollider.gameObject;
                    partHit = FindPart(gameObjectHit);
                }

                // ���������, �������� �� ��� ��� ����� �������
                if (partHit.ProtectedBy != null)
                {
                    //Debug.Log("partHit.ProtectedBy != null");
                    foreach (string s in partHit.ProtectedBy)
                    {
                        // ���� ���� �� ���� �� ���������� ������ ��� �� ���������...
                        if (!Destroyed(s))
                        {
                            //... �� �������� ����������� ���� �����
                            //Debug.Log("!Destroyed(s)");
                            Destroy(otherGO);   // ���������� ������ ProjectileHero
                            return;             // �����, �� ��������� Enemy_4
                        }
                    }
                }

                // ��� ����� �� ��������, ������� �� �����������
                // �������� ����������� ���� �� Projectile.Type � Main.WEAP_DICT
                partHit.Health -= Main.GetWeaponDefinition(projectile.Type).DamageOnHit;
                // �������� ������ ��������� � �����
                ShowLocalizedDamage(partHit.DamageMaterial);
                if (partHit.Health <= 0)
                {
                    // ������ ���������� ����� ������� �������������� ������������ �����
                    partHit.PartGO.SetActive(false);
                }

                // ���������, ��� �� ������� ��������� ��������
                bool allDestroyed = true;       // ������������, ��� ��������
                foreach (Part part in Parts)
                {
                    if (!Destroyed(part))       // ���� �����-�� ����� ��� ����������...
                    {
                        allDestroyed = false;   // ... �������� false � allDestroyed
                        break;                  // � �������� ���� foreach
                    }
                }

                if (allDestroyed)               // ���� ������� �������� ���������...
                {
                    // ... ��������� ������-�������� Main, ��� ���� ������� ��������
                    Main.S.ShipDestroyed(this);
                    // ���������� ���� ������ Enemy
                    Destroy(this.gameObject);
                }

                Destroy(otherGO); // ���������� ������ ProjectileHero
                break;
        }
    }

    private void InitMovement()
    {
        _p0 = _p1; // ���������� _p1 � _p0
        // ������� ����� ����� _p1 �� ������
        float widthMinRad = bndCheck.CamWidth - bndCheck.Radius;
        float heightMinRad = bndCheck.CamHeight - bndCheck.Radius;
        _p1.x = Random.Range(-widthMinRad, widthMinRad);
        _p1.y = Random.Range(-heightMinRad, heightMinRad);

        // �������� �����
        _timeStart = Time.time;
    }

    public override void Move()
    {
        // ���� ����� �������������� Enemy.Move() � ��������� �������� ������������
        float u = (Time.time - _timeStart) / _duration;

        if (u >= 1)
        {
            InitMovement();
            u = 0;
        }

        u = 1 - Mathf.Pow(1 - u, 2);     // ��������� ������� ����������
        pos = (1 - u) * _p0 + u * _p1;   // ������� �������� ������������
    }

    // ��� ��� ������� ��������� ����� ����� � ������� Parts n �� ����� ��� �� ������ �� ������� ������
    private Part FindPart(string name)
    {
        foreach (Part part in Parts)
        {
            if (part.Name == name)
            {
                return (part);
            }
        }

        return (null);
    }

    private Part FindPart(GameObject gameObject)
    {
        foreach (Part part in Parts)
        {
            if (part.PartGO == gameObject)
            {
                return (part);
            }
        }

        return (null);
    }

    // ��� ������� ���������� true, ���� ������ ����� ����������
    private bool Destroyed(string name)
    {
        return (Destroyed(FindPart(name)));
    }

    private bool Destroyed(GameObject gameObject)
    {
         return (Destroyed(FindPart(gameObject)));
    }

    private bool Destroyed(Part part)
    {
        if (part == null)    // ���� ������ �� ����� �� ���� ��������
        {
            return (true);   // ������� true (�.�.: ��, ���� ����������)
        }

        // ������� ��������� ��������� part.Health <= 0
        // ���� part.Health <= 0, ������� true (��, ���� ����������)
        return (part.Health <= 0);
    }

    // ���������� � ������� ���� ������ ���� �����, � �� ���� �������
    private void ShowLocalizedDamage(Material material)
    {
        material.color = Color.red;
        DamageDoneTime = Time.time + ShowDamageDuration;
        ShowingDamage = true;
    }
}

/// <summary>
/// Part -- ��� ���� ������������� ����� ������� WeaponDefinition, ��������������� ��� �������� ������
/// </summary>

[System.Serializable]
public class Part
{
    // �������� ���� ��� ����� ������ ������������ � ����������
    public string Name;            // ��� ���� �����
    public float Health;           // ������� ��������� ���� �����
    public string[] ProtectedBy;   // ������ �����, ���������� ���

    // ��� ��� ���� ���������������� ������������� � Start().
    // �����������, ��� ����� �������� ��������� ����������� ������
    [HideInInspector] // �� ��������� ���������� ���� ��������� � ����������
    public GameObject PartGO;  // ������� ������ ���� �����
    [HideInInspector]
    public Material DamageMaterial;      // �������� ��� ����������� �����������
}
