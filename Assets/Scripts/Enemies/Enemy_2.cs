using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_2 : Enemy // Enemy_2 расширяет класс Enemy
{
    [Header("Set in Inspector: Enemy_2")]
    // Определяют, насколько ярко будет выражен синусоидальный характер движения
    public float SinEccentricity = 0.6f;
    public float LifeTime = 10;

    [Header("Set Dynamically: Enemy_2")]
    // Enemy_2 использует линейную интерполяцию между двумя точками, изменяя результат по синусоиде
    public Vector3 P0;
    public Vector3 P1;
    public float BirthTime;

    private void Start()
    {
        // Выбрать соучайную точку на левой границе экрана
        P0 = Vector3.zero;
        P0.x = -bndCheck.CamWidth - bndCheck.Radius;
        P0.y = Random.Range(-bndCheck.CamHeight, bndCheck.CamHeight);

        // Выбрать случайную точку на правой границе экрана
        P1 = Vector3.zero;
        P1.x = bndCheck.CamWidth + bndCheck.Radius;
        P1.y = Random.Range(-bndCheck.CamHeight, bndCheck.CamHeight);

        // Случайно поменять начальную и конечную точки экрана
        if (Random.value > 0.5f)
        {
            // Изменение знака .x каждой точки переносит её на другой край экрана
            P0.x *= -1;
            P1.x *= -1;
        }

        // Записать в BirthTime текущее время
        BirthTime = Time.time;
    }

    public override void Move()
    {
        // Кривые Безье вычисляются на основе значения u между 0 и 1
        float u = (Time.time - BirthTime) / LifeTime;

        // Если u > 1, значит, корабль существует дольше, чем LifeTime
        if (u > 1)
        {
            // Этот экземпляр Enemy_2 завершил свой жизненный цикл
            Destroy(this.gameObject);
            return;
        }

        // Скорректровать u добавлением значения кривой, изменяющейся по синусоиде
        u = u + SinEccentricity * (Mathf.Sin(u * Mathf.PI * 2));

        // Интерполировать местоположение между двумя точками
        pos = (1 - u) * P0 + u * P1;
    }
}
