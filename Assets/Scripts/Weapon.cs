using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Это перечисление всех возможных видов оружия.
/// Также включает тип "shield", чтобы дать возможность совершенствовать защиту.
/// Аббревиатурой [HP] ниже отмечены элементы, не реализованные в этой книге.
/// </summary>

public enum WeaponType
{
    none,     // По умолчанию / нет оружия 
    blaster,  // Простой бластер
    spread,   // Веерная пушка, стреляющая несколькими снарядами
    phaser,   // [HP] Волновой фазер
    missile,  // [HP] Самонаводящиеся ракеты
    laser,    // [HP] Наносит повреждения при долговременном воздействии
    shield    // Увеличивает ShieldLevel
}

/// <summary>
/// Класс WeaponDefinition позводяет настраивать свойства конкретного вида оружия в инспекторе.
/// Для этого класс Main будет хранить массив элементов типа WeaponDefinition
/// </summary>
[System.Serializable]
public class WeaponDefinition
{
    public WeaponType Type = WeaponType.none;
    public GameObject ProjectilePrefab;          // Шаблон снарядов
    public Color Color = Color.white;            // Цвет ствола оружия и кубика бонуса
    public Color ProjectileColor = Color.white;
    public string Letter;                        // Буква на кубике, изображающем бонус
    public float DamageOnHit = 0;                // Разрушительная мощность
    public float ContinuousDamage = 0;           // Степень разрушения в секунду (для laser)
    public float DelayBetweenShots = 0;
    public float Velocity = 20;                  // Скорость полёта снарядов
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

        // Вызвать SetType(), чтобы заменить тип оружия по умолчанию Weapon.none
        SetType(_type);
        // Динамически создать точку привязки для всех снарядов
        if (PROJECTILE_ANCHOR == null)
        {
            GameObject gameObject = new GameObject("_projectileAnchor");
            PROJECTILE_ANCHOR = gameObject.transform;
        }
        // Найти FireDelegate в корневом игровом объекте
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
        LastShotTime = 0; // Сразу после установки _type можно выстрелить
    }

    public void Fire()
    {
        // Если this.gameObject неактивен, выйти
        if (!gameObject.activeInHierarchy)
        {
            return;
        }
        // Если между выстрелами прошло недостаточно много времени, выйти
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
                projectile = MakeProjectile(); // Снаряд, летящий прямо
                projectile.Rigidbody.velocity = velocity;
                projectile = MakeProjectile(); // Снаряд, летящий влево
                projectile.transform.rotation = Quaternion.AngleAxis(10, Vector3.back);
                projectile.Rigidbody.velocity = projectile.transform.rotation * velocity;
                projectile = MakeProjectile(); // Снаряд, летящий вправо
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
