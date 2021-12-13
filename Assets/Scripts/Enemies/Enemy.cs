using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Set in Inspector: Enemy")]
    public float Speed = 10f;                // Скорость в м/с
    public float FireRate = 0.3f;            // Секунд между выстрелами (не используется)
    public float Health = 10;
    public int Score = 100;                  // Очки за уничтожение этого корабля
    public float ShowDamageDuration = 0.1f;  // Длительность эффекта попадания в секундах
    public float PowerUpDropChance = 1f;     // Вероятность сбросить бонус

    [Header("Set Dynamically: Enemy")]
    public Color[] OriginalColors;
    public Material[] Materials;             // Все материалы игрового объекта и его потомков
    public float DamageDoneTime;             // Время прекращения отображения эффекта
    public bool ShowingDamage = false;
    public bool NotifiedOfDestruction;       // Будет использовано позже

    protected BoundsCheck bndCheck;

    private void Awake()
    {
        bndCheck = GetComponent<BoundsCheck>();
        // Получить материалы и цвет этого игрового объекта и его потомков
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
            // Корабль за нижней границей, поэтому его нужно уничтожить
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
                
                // Если вражеский корабль за границами экрана, не наносить ему повреждений
                if (!bndCheck.IsOnScreen)
                {
                    Destroy(otherGO);
                    break;
                }

                // Поразить вражеский корабль
                ShowDamage();
                // Получить разрушающую силу из WEAP_DICT в классе Main.
                Health -= Main.GetWeaponDefinition(projectile.Type).DamageOnHit;
                if (Health <= 0)
                {
                    // Сообщить объекту-одиночке Main об уничтожении
                    if (!NotifiedOfDestruction)
                    {
                        Main.S.ShipDestroyed(this);
                    }
                    NotifiedOfDestruction = true;

                    // Уничтожить этот вражеский корабль
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

    // Это свойство: метод, действующий как поле
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
