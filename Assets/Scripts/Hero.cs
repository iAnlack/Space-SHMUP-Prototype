using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    static public Hero S; // ��������

    [Header("Set in Inspector")]
    public GameObject ProjectilePrefab;
    public float ProjectileSpeed = 40;
    public Weapon[] Weapons;
    // ����, ����������� ��������� �������
    public float Speed = 30;
    public float RollMult = -45;
    public float PitchMult = 30;
    public float GameStartDelay = 2f;

    [Header("Set Dynamycally")]
    [SerializeField] private float _shieldLevel = 1;

    // ��� ���������� ������ ������ �� ��������� ������������� ������� ������
    private GameObject _lastTriggerGo = null;

    // ���������� ������ �������� ���� WeaponFireDelegate
    public delegate void WeaponFireDelegate();
    // ������� ���� ���� WeaponFireDelegate � ������ FireDelegate
    public WeaponFireDelegate FireDelegate;

    private void Start()
    {
        if (S == null)
        {
            S = this; // ��������� ������ �� ��������
        }
        else
        {
            Debug.LogError("Hero.Awake() - Attemped to assign second Hero.S!");
        }

        //FireDelegate += TempFire;

        // �������� ������ Weapons � ������ ���� � 1 ���������
        ClearWeapons();
        Weapons[0].SetType(WeaponType.blaster);
    }

    private void Update()
    {
        // ������� ���������� �� ������ Input
        float xAxis = Input.GetAxis("Horizontal");
        float yAxis = Input.GetAxis("Vertical");

        // �������� transform.position, �������� �� ���������� �� ����
        Vector3 pos = transform.position;
        pos.x += xAxis * Speed * Time.deltaTime;
        pos.y += yAxis * Speed * Time.deltaTime;
        transform.position = pos;

        // ��������� �������, ����� ������� �������� ���������
        transform.rotation = Quaternion.Euler(yAxis * PitchMult, xAxis * RollMult, 0);



        // ��������� ������� ����������
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    TempFire();
        //}

        // ���������� ������� �� ���� ����� ������ ������� FireDelegate
        // ������� ��������� ������� �������: Axis("Jump")
        // ����� ���������, ��� �������� FireDelegate �� ����� null, ����� �������� ������
        if (Input.GetAxis("Jump") == 1 && FireDelegate != null)
        {
            FireDelegate();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Transform rootT = other.gameObject.transform.root;
        GameObject gameObject = rootT.gameObject;
        //Debug.Log("Triggered: " + gameObject.name);

        // ������������� ������������� ���������� ������������ � ��� �� ��������
        if (gameObject == _lastTriggerGo)
        {
            return;
        }
        _lastTriggerGo = gameObject;

        if (gameObject.tag == "Enemy") // ���� �������� ���� ����������� � ��������� �������,
        {                              // ��
            ShieldLevel--;             // ��������� ������� ������ �� 1,
            Destroy(gameObject);       // � ���������� �����
        }
        else if (gameObject.tag == "PowerUp")
        {
            // ���� �������� ���� ����������� � �������
            AbsorbPowerUp(gameObject);
        }
        else
        {
            Debug.Log("Triggered by non-Enemy: " + gameObject.name);
        }
    }

    private void TempFire()
    {
        GameObject projectileGO = Instantiate<GameObject>(ProjectilePrefab);
        projectileGO.transform.position = transform.position;
        Rigidbody rigidbody = projectileGO.GetComponent<Rigidbody>();
        //rigidbody.velocity = Vector3.up * ProjectileSpeed;

        Projectile projectile = projectileGO.GetComponent<Projectile>();
        projectile.Type = WeaponType.blaster;
        float tSpeed = Main.GetWeaponDefinition(projectile.Type).Velocity;
        rigidbody.velocity = Vector3.up * tSpeed;
    }

    public void AbsorbPowerUp(GameObject gameObject)
    {
        PowerUp powerUp = gameObject.GetComponent<PowerUp>();
        switch (powerUp.Type)
        {
            //case WeaponType.none:
            //    break;
            //case WeaponType.blaster:
            //    break;
            //case WeaponType.spread:
            //    break;
            case WeaponType.shield:
                ShieldLevel++;
                break;
            default:
                if (powerUp.Type == Weapons[0].Type)
                {
                    Weapon weapon = GetEmptyWeaponSlot(); // ���� ������ ���� �� ����
                    if (weapon != null)
                    {
                        // ���������� � powerUp.Type
                        weapon.SetType(powerUp.Type);
                    }
                }
                else                                      // ���� ������ ������� ����
                {
                    ClearWeapons();
                    Weapons[0].SetType(powerUp.Type);
                }
                break;
        }
        powerUp.AbsorbedBy(this.gameObject);
    }

    private Weapon GetEmptyWeaponSlot()
    {
        for (int i = 0; i < Weapons.Length; i++)
        {
            if (Weapons[i].Type == WeaponType.none)
            {
                return (Weapons[i]);
            }
        }
        return (null);
    }

    private void ClearWeapons()
    {
        foreach (Weapon weapon in Weapons)
        {
            weapon.SetType(WeaponType.none);
        }
    }

    public float ShieldLevel
    {
        get
        {
            return (_shieldLevel);
        }
        set
        {
            _shieldLevel = Mathf.Min(value, 4);
            // ���� ������� ���� ���� ����
            if (value < 0)
            {
                Destroy(this.gameObject);
                // �������� ������� Main.S � ������������� ������������� ����
                Main.S.DelayedRestart(GameStartDelay);
            }
        }
    }
}
