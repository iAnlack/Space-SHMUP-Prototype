using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��� ������������ ���� ��������� ����� ������.
/// ����� �������� ��� "shield", ����� ���� ����������� ���������������� ������.
/// ������������� [HP] ���� �������� ��������, �� ������������� � ���� �����.
/// </summary>

public enum WeaponType
{
    none,     // �� ��������� / ��� ������ 
    blaster,  // ������� �������
    spread,   // ������� �����, ���������� ����������� ���������
    phaser,   // [HP] �������� �����
    missile,  // [HP] ��������������� ������
    laser,    // [HP] ������� ����������� ��� �������������� �����������
    shield    // ����������� ShieldLevel
}

/// <summary>
/// ����� WeaponDefinition ��������� ����������� �������� ����������� ���� ������ � ����������.
/// ��� ����� ����� Main ����� ������� ������ ��������� ���� WeaponDefinition
/// </summary>
[System.Serializable]
public class WeaponDefinition
{
    public WeaponType Type = WeaponType.none;
    public GameObject ProjectilePrefab;          // ������ ��������
    public Color Color = Color.white;            // ���� ������ ������ � ������ ������
    public Color ProjectileColor = Color.white;
    public string Letter;                        // ����� �� ������, ������������ �����
    public float DamageOnHit = 0;                // �������������� ��������
    public float ContinuousDamage = 0;           // ������� ���������� � ������� (��� laser)
    public float DelayBetweenShots = 0;
    public float Velocity = 20;                  // �������� ����� ��������
}

public class Weapon : MonoBehaviour
{
    static public Transform PROJECTILE_ANCHOR;

    [Header("Set Dynamically")]
    [SerializeField] private WeaponType _type = WeaponType.none;
    private Renderer _collarRenderer;

    public WeaponDefinition Def;
    public GameObject Collar;
    public float LastShotTime;

    private void Start()
    {
        Collar = transform.Find("Collar").gameObject;
        _collarRenderer = Collar.GetComponent<Renderer>();

        // ������� SetType(), ����� �������� ��� ������ �� ��������� Weapon.none
        SetType(_type);
        // ����������� ������� ����� �������� ��� ���� ��������
        if (PROJECTILE_ANCHOR == null)
        {
            GameObject gameObject = new GameObject("_projectileAnchor");
            PROJECTILE_ANCHOR = gameObject.transform;
        }
        // ����� FireDelegate � �������� ������� �������
        GameObject rootGO = transform.root.gameObject;
        if (rootGO.GetComponent<Hero>() != null)
        {
            rootGO.GetComponent<Hero>().FireDelegate += Fire;
        }
    }

    public void SetType(WeaponType weaponType)
    {
        _type = weaponType;
        if (Type == WeaponType.none)
        {
            this.gameObject.SetActive(false);
            return;
        }
        else
        {
            this.gameObject.SetActive(true);
        }
        Def = Main.GetWeaponDefinition(_type);
        _collarRenderer.material.color = Def.Color;
        LastShotTime = 0; // ����� ����� ��������� _type ����� ����������
    }

    public void Fire()
    {
        // ���� this.gameObject ���������, �����
        if (!gameObject.activeInHierarchy)
        {
            return;
        }
        // ���� ����� ���������� ������ ������������ ����� �������, �����
        if (Time.time - LastShotTime < Def.DelayBetweenShots)
        {
            return;
        }

        Projectile projectile;
        Vector3 velocity = Vector3.up * Def.Velocity;
        if (transform.up.y < 0)
        {
            velocity.y = -velocity.y;
        }

        switch (Type)
        {
            case WeaponType.blaster:
                projectile = MakeProjectile();
                projectile.Rigidbody.velocity = velocity;
                break;

            case WeaponType.spread:
                projectile = MakeProjectile(); // ������, ������� �����
                projectile.Rigidbody.velocity = velocity;
                projectile = MakeProjectile(); // ������, ������� �����
                projectile.transform.rotation = Quaternion.AngleAxis(10, Vector3.back);
                projectile.Rigidbody.velocity = projectile.transform.rotation * velocity;
                projectile = MakeProjectile(); // ������, ������� ������
                projectile.transform.rotation = Quaternion.AngleAxis(-10, Vector3.back);
                projectile.Rigidbody.velocity = projectile.transform.rotation * velocity;
                break;
        }
    }

    public Projectile MakeProjectile()
    {
        GameObject gameObject = Instantiate<GameObject>(Def.ProjectilePrefab);
        if (transform.parent.gameObject.tag == "Hero")
        {
            gameObject.tag = "ProjectileHero";
            gameObject.layer = LayerMask.NameToLayer("ProjectileHero");
        }
        else
        {
            gameObject.tag = "ProjectileEnemy";
            gameObject.layer = LayerMask.NameToLayer("ProjectileEnemy");
        }

        gameObject.transform.position = Collar.transform.position;
        gameObject.transform.SetParent(PROJECTILE_ANCHOR, true);
        Projectile projectile = gameObject.GetComponent<Projectile>();
        projectile.Type = Type;
        LastShotTime = Time.time;
        return (projectile);
    }

    public WeaponType Type
    {
        get
        {
            return(_type);
        }
        set
        {
            SetType(value);
        }
    }
}
