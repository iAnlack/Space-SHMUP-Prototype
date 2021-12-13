using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enemy_4 создаётся за верхней границей, выбирает случайную точку на экране и перемещается к ней. 
/// Добравшись до места, выбирает другую случайную точку и продолжает двигаться, пока игрок не уничтожит его.
/// </summary>

public class Enemy_4 : Enemy
{
    [Header("Set in Inspector: Enemy_4")]
    public Part[] Parts;           // Массив частей, составляющих корабль

    private Vector3 _p0, _p1;      // Две точки интерполяции
    private float _timeStart;      // Время создания этого корабля
    private float _duration = 4;   // Продолжительность перемещения

    private void Start()
    {
        // Начальная позиция уже выбрана в Main.SpawnEnemy(),
        // поэтому запишем её как начальные значения в _p0 и _p1
        _p0 = _p1 = pos;

        InitMovement();

        // Записать в кэш игровой объект и материал каждой части в Parts
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

    // Переопределяет метод OnCollisionEnter из сценария Enemy.cs
    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("что-нибудь");
        GameObject otherGO = collision.gameObject;
        switch (otherGO.tag)
        {
            case "ProjectileHero":
                Projectile projectile = otherGO.GetComponent<Projectile>();

                // Если корабль за границей экрана, не повреждать его
                if (!bndCheck.IsOnScreen)
                {
                    Destroy(otherGO);
                    break;
                }

                // Поразить вражеский корабль
                GameObject gameObjectHit = collision.contacts[0].thisCollider.gameObject;
                Part partHit = FindPart(gameObjectHit);
                if (partHit == null)   // Если partHit не обнаружен...
                {
                    //Debug.Log("partHit == null");
                    gameObjectHit = collision.contacts[0].otherCollider.gameObject;
                    partHit = FindPart(gameObjectHit);
                }

                // Проверить, защищена ли ещё эта часть корабля
                if (partHit.ProtectedBy != null)
                {
                    //Debug.Log("partHit.ProtectedBy != null");
                    foreach (string s in partHit.ProtectedBy)
                    {
                        // Если хотя бы одна из защищающих частей ещё не разрушена...
                        if (!Destroyed(s))
                        {
                            //... не наносить повреждений этой части
                            //Debug.Log("!Destroyed(s)");
                            Destroy(otherGO);   // Уничтожить снаряд ProjectileHero
                            return;             // выйти, не повреждая Enemy_4
                        }
                    }
                }

                // Эта часть не защищена, нанести ей повреждение
                // Получить разрушающую силу из Projectile.Type и Main.WEAP_DICT
                partHit.Health -= Main.GetWeaponDefinition(projectile.Type).DamageOnHit;
                // Показать эффект попадания в часть
                ShowLocalizedDamage(partHit.DamageMaterial);
                if (partHit.Health <= 0)
                {
                    // Вместо разрушения всего корабля деактивировать уничтоженную часть
                    partHit.PartGO.SetActive(false);
                }

                // Проверить, был ли корабль полностью разрушен
                bool allDestroyed = true;       // Предположить, что разрушен
                foreach (Part part in Parts)
                {
                    if (!Destroyed(part))       // Если какая-то часть ещё существует...
                    {
                        allDestroyed = false;   // ... записать false в allDestroyed
                        break;                  // и прервать цикл foreach
                    }
                }

                if (allDestroyed)               // Если корабль разрушен полностью...
                {
                    // ... уведомить объект-одиночку Main, что этот корабль разрушен
                    Main.S.ShipDestroyed(this);
                    // Уничтожить этот объект Enemy
                    Destroy(this.gameObject);
                }

                Destroy(otherGO); // Уничтожить снаряд ProjectileHero
                break;
        }
    }

    private void InitMovement()
    {
        _p0 = _p1; // Переписать _p1 в _p0
        // Выбрать новую точку _p1 на экране
        float widthMinRad = bndCheck.CamWidth - bndCheck.Radius;
        float heightMinRad = bndCheck.CamHeight - bndCheck.Radius;
        _p1.x = Random.Range(-widthMinRad, widthMinRad);
        _p1.y = Random.Range(-heightMinRad, heightMinRad);

        // Сбросить время
        _timeStart = Time.time;
    }

    public override void Move()
    {
        // Этот метод переопределяет Enemy.Move() и реализует линейную интерполяцию
        float u = (Time.time - _timeStart) / _duration;

        if (u >= 1)
        {
            InitMovement();
            u = 0;
        }

        u = 1 - Mathf.Pow(1 - u, 2);     // Применить плавное замедление
        pos = (1 - u) * _p0 + u * _p1;   // Простая линейная интерполяция
    }

    // Эти две функции выполняют поиск части в массиве Parts n по имени или по ссылке на игровой объект
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

    // Эти функции возвращают true, если данная часть уничтожена
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
        if (part == null)    // Если ссылка на часть не была передана
        {
            return (true);   // Вернуть true (т.е.: да, была уничтожена)
        }

        // Вернуть результат сравнения part.Health <= 0
        // Если part.Health <= 0, вернуть true (да, была уничтожена)
        return (part.Health <= 0);
    }

    // Окрашивает в красный цвет только одну часть, а не весь корабль
    private void ShowLocalizedDamage(Material material)
    {
        material.color = Color.red;
        DamageDoneTime = Time.time + ShowDamageDuration;
        ShowingDamage = true;
    }
}

/// <summary>
/// Part -- ещё один сериализуемый класс подобно WeaponDefinition, предназначенный для хранения данных
/// </summary>

[System.Serializable]
public class Part
{
    // Значения этих трёх полей должны определяться в инспекторе
    public string Name;            // Имя этой части
    public float Health;           // Степень стойкости этой части
    public string[] ProtectedBy;   // Другие части, защищающие эту

    // Эти два поля инициализируются автоматически в Start().
    // Кэширование, как здесь ускоряет получение необходимых данных
    [HideInInspector] // Не позволяет следующему полю появиться в инспекторе
    public GameObject PartGO;  // Игровой объект этой части
    [HideInInspector]
    public Material DamageMaterial;      // Материал для отображения повреждений
}
