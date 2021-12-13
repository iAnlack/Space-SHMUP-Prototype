using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Set in Inspector: Enemy")]
    public float Speed = 10f;                // �������� � �/�
    public float FireRate = 0.3f;            // ������ ����� ���������� (�� ������������)
    public float Health = 10;
    public int Score = 100;                  // ���� �� ����������� ����� �������
    public float ShowDamageDuration = 0.1f;  // ������������ ������� ��������� � ��������
    public float PowerUpDropChance = 1f;     // ����������� �������� �����

    [Header("Set Dynamically: Enemy")]
    public Color[] OriginalColors;
    public Material[] Materials;             // ��� ��������� �������� ������� � ��� ��������
    public float DamageDoneTime;             // ����� ����������� ����������� �������
    public bool ShowingDamage = false;
    public bool NotifiedOfDestruction;       // ����� ������������ �����

    protected BoundsCheck bndCheck;

    private void Awake()
    {
        bndCheck = GetComponent<BoundsCheck>();
        // �������� ��������� � ���� ����� �������� ������� � ��� ��������
        Materials = Utils.GetAllMaterials(gameObject);
        OriginalColors = new Color[Materials.Length];
        for (int i = 0; i < Materials.Length; i++)
        {
            OriginalColors[i] = Materials[i].color;
        }
    }

    private void Update()
    {
        Move();

        if (ShowingDamage && Time.time > DamageDoneTime)
        {
            UnShowDamage();
        }

        if (bndCheck != null && bndCheck.OffDown)
        {
            // ������� �� ������ ��������, ������� ��� ����� ����������
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject otherGO = collision.gameObject;
        switch (otherGO.tag)
        {
            case "ProjectileHero":
                Projectile projectile = otherGO.GetComponent<Projectile>();
                
                // ���� ��������� ������� �� ��������� ������, �� �������� ��� �����������
                if (!bndCheck.IsOnScreen)
                {
                    Destroy(otherGO);
                    break;
                }

                // �������� ��������� �������
                ShowDamage();
                // �������� ����������� ���� �� WEAP_DICT � ������ Main.
                Health -= Main.GetWeaponDefinition(projectile.Type).DamageOnHit;
                if (Health <= 0)
                {
                    // �������� �������-�������� Main �� �����������
                    if (!NotifiedOfDestruction)
                    {
                        Main.S.ShipDestroyed(this);
                    }
                    NotifiedOfDestruction = true;

                    // ���������� ���� ��������� �������
                    Destroy(this.gameObject);
                }

                Destroy(otherGO);
                break;
            
            default:
                Debug.Log("Enemy hit by non-ProjectileHero: " + otherGO.name);
                break;
        }
    }

    public virtual void Move()
    {
        Vector3 tempPos = pos;
        tempPos.y -= Speed * Time.deltaTime;
        pos = tempPos;
    }

    private void ShowDamage()
    {
        foreach (Material material in Materials)
        {
            material.color = Color.red;
        }

        ShowingDamage = true;
        DamageDoneTime = Time.time + ShowDamageDuration;
    }

    private void UnShowDamage()
    {
        for (int i = 0; i < Materials.Length; i++)
        {
            Materials[i].color = OriginalColors[i];
        }

        ShowingDamage = false;
    }

    // ��� ��������: �����, ����������� ��� ����
    public Vector3 pos
    {
        get
        {
            return (this.transform.position);
        }
        set
        {
            this.transform.position = value;
        }
    }
}
