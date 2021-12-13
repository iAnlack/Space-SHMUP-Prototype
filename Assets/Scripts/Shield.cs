using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    [Header("Set in Inspector")]
    public float RotationsPerSecond = 0.1f;

    [Header("Set Dynamycally")]
    public int LevelShown = 0;

    // Скрытые переменные не появляющиеся в инспекторе
    private Material _mat;

    private void Start()
    {
        _mat = GetComponent<Renderer>().material;
    }

    private void Update()
    {
        // Прочитать текущую мощность защитного поля из объекта-одиночки Hero
        int currLevel = Mathf.FloorToInt(Hero.S.ShieldLevel);
        // Если она отличается от LevelShown...
        if (LevelShown != currLevel)
        {
            LevelShown = currLevel;
            // Скорректировать смещение в текстуре, чтобы отобразить поле с другой мощностью
            _mat.mainTextureOffset = new Vector2(0.2f * LevelShown, 0);
        }
        // Поворачивать поле в каждом кадре с постоянной скоростью
        float rZ = -(RotationsPerSecond * Time.time * 360) % 360f;
        transform.rotation = Quaternion.Euler(0, 0, rZ);
    }
}
