using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    static public Main S; // Объект-одиночка Main
    static Dictionary<WeaponType, WeaponDefinition> WEAP_DICT;

    [Header("Set in Inspector")]
    public GameObject[] PrefabEnemies; // Массив шаблонов Enemy
    public float EnemySpawnPerSecond = 0.5f; // Вражеских кораблей в секунду 
    public float EnemyDefaultPadding = 1.5f; // Отступ для позиционирования
    public WeaponDefinition[] WeaponDefinitions;
    public GameObject PrefabPowerUp;
    public WeaponType[] PowerUpFrequency = new WeaponType[]
    {
        WeaponType.blaster, WeaponType.blaster, WeaponType.spread, WeaponType.shield
    };

    private BoundsCheck _bndCheck;

    private void Awake()
    {
        S = this;
        // Записать в _bndCheck ссылку на компонент BoundsCheck этого игрового объекта
        _bndCheck = GetComponent<BoundsCheck>();
        // Вызывать SpawnEnemy() один раз (в 2 секунды при значениях по умолчанию)
        Invoke("SpawnEnemy", 1f / EnemySpawnPerSecond);

        // Словарь с ключами типа WeaponType
        WEAP_DICT = new Dictionary<WeaponType, WeaponDefinition>();
        foreach (WeaponDefinition def in WeaponDefinitions)
        {
            WEAP_DICT[def.Type] = def;
        }
    }

    public void SpawnEnemy()
    {
        // Выбрать случайный шаблон Enemy для создания
        int ndx = Random.Range(0, PrefabEnemies.Length);
        GameObject gameObject = Instantiate<GameObject>(PrefabEnemies [ndx]);

        // Разместить вражеский корабль над экраном в случайной позиции x
        float enemyPadding = EnemyDefaultPadding;
        if (gameObject.GetComponent<BoundsCheck>() != null)
        {
            enemyPadding = Mathf.Abs(gameObject.GetComponent<BoundsCheck>().Radius);
        }

        // Установить начальные координаты созданного вражеского корабля
        Vector3 pos = Vector3.zero;
        float xMin = -_bndCheck.CamWidth + enemyPadding;
        float xMax = _bndCheck.CamWidth - enemyPadding;
        pos.x = Random.Range(xMin, xMax);
        pos.y = _bndCheck.CamHeight + enemyPadding;
        gameObject.transform.position = pos;

        // Снова вызвать SpawnEnemy()
        Invoke("SpawnEnemy", 1f / EnemySpawnPerSecond);
    }

    public void ShipDestroyed(Enemy enemy)
    {
        // Сгенерировать бонус с заданной вероятностью
        if (Random.value <= enemy.PowerUpDropChance)
        {
            // Выбрать тип бонуса
            // Выбрать один из элементов в PowerUpFrequency
            int ndx = Random.Range(0, PowerUpFrequency.Length);
            WeaponType powerUpType = PowerUpFrequency[ndx];

            // Создать экземпляр PowerUp
            GameObject gameObject = Instantiate(PrefabPowerUp) as GameObject;
            PowerUp powerUp = gameObject.GetComponent<PowerUp>();
            // Установить соответствующий тип WeaponType
            powerUp.SetType(powerUpType);

            // Поместить в место, где находился разрушенный корабль
            powerUp.transform.position = enemy.transform.position;
        }
    }

    public void DelayedRestart(float delay)
    {
        // Вызвать метод Restart() через delay секунд
        Invoke("Restart", delay);
    }

    public void Restart()
    {
        // Перезагрузить Main Scene, чтобы перезапустить игру
        SceneManager.LoadScene("Main Scene");
    }

    /// <summary>
    /// Статическая функция, возвращающая WeaponDefinition из 
    /// статического защищённого поля WEAP_DICT класса Main.
    /// </summary>
    /// <returns>
    /// Экземпляр WeaponDefinition или, если нет такого определения для указанного WeaponType,
    /// возвращает новый экземпляр WeaponDefinition с типом none.
    /// </returns>
    /// <param name="wt">Тип оружия WeaponType, для которого требуется получить WeaponDefinition
    /// </param>
    static public WeaponDefinition GetWeaponDefinition(WeaponType wt)
    {
        // Проверить наличие указанного ключа в словаре
        // Попытка извлечь значение по отсутствующему ключу вызовет ошибку,
        // поэтому следующая инструкция играет важную роль.
        if (WEAP_DICT.ContainsKey(wt))
        {
            return (WEAP_DICT[wt]);
        }
        // Следующая инструкция возвращает новый экземпляр WeaponDefinition с типом оружия WeaponType.none,
        // что означает неудачную попытку найти требуемое определение WeaponDefinition
        return (new WeaponDefinition());
    }

}
