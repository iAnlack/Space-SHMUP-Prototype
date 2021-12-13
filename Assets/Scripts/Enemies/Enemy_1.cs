using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_1 : Enemy // Enemy_1 расширяет класс Enemy
{
    [Header("Set in Inspector: Enemy_1")]
    public float WaveFrequency = 2; // Число секунд полного цикла синусоиды
    public float WaveWidth = 4; // Ширина синусоиды в метрах
    public float WaveRotY = 45;

    private float _x0; // Начальное значение координаты x
    private float _birthTime;

    // Метод Start хорошо подходит для наших целей, потому что не используется суперклассом Enemy
    private void Start()
    {
        
    }

    // Переопределить функцию Move() суперкласса Enemy
    public override void Move()
    {
        // Т.к. pos - это свойство, нельзя напрямую изменить pos.x,
        // поэтому получим pos в виде вектора Vector3, доступного для изменения
        Vector3 tempPos = pos;
        // Значение theta изменяется с течением времени
        float age = Time.time - _birthTime;
        float theta = Mathf.PI * 2 * age / WaveFrequency;
        float sin = Mathf.Sin(theta);
        tempPos.x = _x0 + WaveWidth * sin;
        pos = tempPos;

        // Повернуть немного относительно оси Y
        Vector3 rot = new Vector3(0, sin * WaveRotY, 0);
        this.transform.rotation = Quaternion.Euler(rot);

        // base.Move() обрабатывает движение вниз, вдоль оси Y
        base.Move();

        //Debug.Log(bndCheck.IsOnScreen);
    }
}
