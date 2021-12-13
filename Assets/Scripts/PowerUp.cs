using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [Header("Set in Inspector")]
    // Необычное, но удобное применение Vector2. x хранит минимальное значение, а y - максимальное значение
    // для метода Random.Range(), который будет вызываться позже
    public Vector2 RotMinMax = new Vector2(15, 90);
    public Vector2 DriftMinMax = new Vector2(0.25f, 2);
    public float LifeTime = 6f;    // Время существования PowerUp в секундах
    public float FadeTime = 4f;    // Время затухания???

    [Header("Set Dynamically")]
    public WeaponType Type;        // Тип бонуса
    public GameObject Cube;        // Ссылка на вложенный куб
    public TextMesh Letter;        // Ссылка на TextMesh
    public Vector3 RotPerSecond;   // Скорость вращения
    public float BirthTime;

    private Rigidbody _rigidbody;
    private BoundsCheck _bndCheck;
    private Renderer _cubeRenderer;

    private void Awake()
    {
        // Получить ссылку на куб
        Cube = transform.Find("Cube").gameObject;
        // Получить ссылки на TextMesh и другие компоненты
        Letter = GetComponent<TextMesh>();
        _rigidbody = GetComponent<Rigidbody>();
        _bndCheck = GetComponent<BoundsCheck>();
        _cubeRenderer = Cube.GetComponent<Renderer>();

        // Выбрать случайную скорость
        Vector3 velocity = Random.onUnitSphere; // Получить случайную скорость XYZ
        // Random.onUnitSphere возвращает вектор, указывающий на случайную точку, находящуюся на
        // поверхности сферы радиусом 1м и с центром в начале координат
        velocity.z = 0; // Отобразить velocity на плоскость XY
        velocity.Normalize(); // Нормализация устанавливает длину Vector3 равной 1м
        velocity *= Random.Range(DriftMinMax.x, DriftMinMax.y);
        _rigidbody.velocity = velocity;

        // Установить угол поворота этого игрового объекта равным R: [0, 0, 0]
        transform.rotation = Quaternion.identity;
        // Quaternion.identity равноценно отсутствию поворота

        // Выбрать случайную скорость вращения для вложенного куба с использованием RotMinMax.x и RotMinMax.y
        RotPerSecond = new Vector3
            (Random.Range(RotMinMax.x, RotMinMax.y),
            Random.Range(RotMinMax.x, RotMinMax.y),
            Random.Range(RotMinMax.x, RotMinMax.y));

        BirthTime = Time.time;
    }

    private void Update()
    {
        Cube.transform.rotation = Quaternion.Euler(RotPerSecond * Time.time);

        // Эффект растворения куба PowerUp с течением времени
        // Со значениями по умолчанию бонус существует 10 секунд, а затем растворяется в течение 4 секунд.
        float u = (Time.time - (BirthTime + LifeTime)) / FadeTime;
        // В течение LifeTime секунд значение u будет <= 0.
        // Затем оно станет положительным и через FadeTime секунд станет больше 1

        // Если u >= 1, уничтожить бонус
        if (u >= 1)
        {
            Destroy(this.gameObject);
            return;
        }

        // Использовать u для определения альфа-значения куба и буквы
        if (u > 0)
        {
            Color c = _cubeRenderer.material.color;
            c.a = 1f - u;
            _cubeRenderer.material.color = c;
            // Буква тоже должна растворяться, но медленнее
            c = Letter.color;
            c.a = 1f - (u * 0.5f);
            Letter.color = c;
        }

        if (!_bndCheck.IsOnScreen)
        {
            // Если бонус полностью вышел за границу экрана, уничтожать его
            Destroy(gameObject);
        }
    }

    public void SetType(WeaponType weaponType)
    {
        // Получить WeaponDefinition из Main
        WeaponDefinition def = Main.GetWeaponDefinition(weaponType);
        // Установить цвет дочернего куба
        _cubeRenderer.material.color = def.Color;
        //Letter.color = def.Color; // Букву тоже можно окрасить в тот же цвет
        Letter.text = def.Letter; // Установить отображаемую букву
        Type = weaponType; // В заключение установить фактический тип
    }

    public void AbsorbedBy(GameObject target)
    {
        // Эта функция вызывается классом Hero, когда игрок подбирает бонус
        // Можно было реализовать эффект поглощения бонуса, уменьшая его размеры в течение нескольких кадров,
        // но пока просто уничтожим this.gameObject
        Destroy(this.gameObject);
    }
}
