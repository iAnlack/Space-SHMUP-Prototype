using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    static public Hero S; // Одиночка

    [Header("Set in Inspector")]
    public GameObject ProjectilePrefab;
    public float ProjectileSpeed = 40;
    public Weapon[] Weapons;
    // Поля, управляющие движением корабля
    public float Speed = 30;
    public float RollMult = -45;
    public float PitchMult = 30;
    public float GameStartDelay = 2f;

    [Header("Set Dynamycally")]
    [SerializeField] private float _shieldLevel = 1;

    // Эта переменная хранит ссылку на последний столкнувшийся игровой объект
    private GameObject _lastTriggerGo = null;

    // Объявление нового делегата типа WeaponFireDelegate
    public delegate void WeaponFireDelegate();
    // Создать поле типа WeaponFireDelegate с именем FireDelegate
    public WeaponFireDelegate FireDelegate;

    private void Start()
    {
        if (S == null)
        {
            S = this; // Сохранить ссылку на одиночку
        }
        else
        {
            Debug.LogError("Hero.Awake() - Attemped to assign second Hero.S!");
        }

        //FireDelegate += TempFire;

        // Очистить массив Weapons и начать игру с 1 бластером
        ClearWeapons();
        Weapons[0].SetType(WeaponType.blaster);
    }

    private void Update()
    {
        // Извлечь информацию из класса Input
        float xAxis = Input.GetAxis("Horizontal");
        float yAxis = Input.GetAxis("Vertical");

        // Изменить transform.position, опираясь на информацию по осям
        Vector3 pos = transform.position;
        pos.x += xAxis * Speed * Time.deltaTime;
        pos.y += yAxis * Speed * Time.deltaTime;
        transform.position = pos;

        // Повернуть корабль, чтобы придать ощущение динамизма
        transform.rotation = Quaternion.Euler(yAxis * PitchMult, xAxis * RollMult, 0);



        // Позволить кораблю выстрелить
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    TempFire();
        //}

        // Произвести выстрел из всех видов оружия вызовом FireDelegate
        // Сначала проверить нажатие клавиши: Axis("Jump")
        // Затем убедиться, что значение FireDelegate не равно null, чтобы избежать ошибки
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

        // Гарантировать невозможность повторного столкновения с тем же объектом
        if (gameObject == _lastTriggerGo)
        {
            return;
        }
        _lastTriggerGo = gameObject;

        if (gameObject.tag == "Enemy") // Если защитное поле столкнулось с вражеским кораблём,
        {                              // то
            ShieldLevel--;             // уменьшить уровень защиты на 1,
            Destroy(gameObject);       // и уничтожить врага
        }
        else if (gameObject.tag == "PowerUp")
        {
            // Если защитное поле столкнулось с бонусом
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
                    Weapon weapon = GetEmptyWeaponSlot(); // Если оружие того же типа
                    if (weapon != null)
                    {
                        // Установить в powerUp.Type
                        weapon.SetType(powerUp.Type);
                    }
                }
                else                                      // Если оружие другого типа
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
            // Если уровень упал ниже нуля
            if (value < 0)
            {
                Destroy(this.gameObject);
                // Сообщить объекту Main.S о необходимости перезапустить игру
                Main.S.DelayedRestart(GameStartDelay);
            }
        }
    }
}
