using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_3 : Enemy // Enemy_3 расширяет класс Enemy
{
    // Траектория движения Enemy_3 вычисляется путём линейной интерполяции кривой Безье по более чем 2 точкам
    [Header("Set in Inspector: Enemy_3")]
    public float LifeTime = 5;

    [Header("Set Dynamically: Enemy_3")]
    public Vector3[] Points;
    public float BirthTime;

    // И снова метод Start хорошо подходит для наших целей, т.к. не используется суперклассом Enemy
    private void Start()
    {
        Points = new Vector3[3]; // Инициализировать массив точек

        // Начальная позиция уже определена в Main.SpawnEnemy()
        Points[0] = pos;

        // Установить xMin и xMax так же, как это делает Main.SpawnEnemy()
        float xMin = -bndCheck.CamWidth + bndCheck.Radius;
        float xMax = bndCheck.CamWidth - bndCheck.Radius;
        
        Vector3 v;
        // Случайно выбрать среднюю точку ниже нижней границы экрана
        v = Vector3.zero;
        v.x = Random.Range(xMin, xMax);
        v.y = -bndCheck.CamHeight * Random.Range(2.75f, 2);
        Points[1] = v;

        // Случайно выбрать конечную точку выше верхней границы экрана
        v = Vector3.zero;
        v.y = pos.y;
        v.x = Random.Range(xMin, xMax);
        Points[2] = v;

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
            // Этот экземпляр Enemy_3 завершил свой жизненный цикл
            Destroy(this.gameObject);
            return;
        }

        // Интерполировать кривую Безье по трём точкам
        Vector3 p01, p12;
        u = u - 0.2f * Mathf.Sin(u * Mathf.PI * 2);
        p01 = (1 - u) * Points[0] + u * Points[1];
        p12 = (1 - u) * Points[1] + u * Points[2];
        pos = (1 - u) * p01 + u * p12;
    }
}
