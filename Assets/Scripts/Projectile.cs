using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private BoundsCheck _bndCheck;
    private Renderer _renderer;

    [Header("Set Dynamically")]
    public Rigidbody Rigidbody;
    [SerializeField] private WeaponType _type;

    private void Awake()
    {
        _bndCheck = GetComponent<BoundsCheck>();
        _renderer = GetComponent<Renderer>();
        Rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (_bndCheck.OffUp)
        {
            Destroy(gameObject);
        }
    }

    // Это общедоступное свойство маскирует поле _type и обрабатывает операции присваивания ему нового значения
    public WeaponType Type
    {
        get
        {
            return (_type);
        }
        set
        {
            SetType(value);
        }
    }
    /// <summary>
    /// Изменяет скрытое поле _type и устанавливает цвет этого снаряда, как определено в WeaponDefinition.
    /// </summary>
    /// <param name="eType">
    /// Тип WeaponType используемого оружия.
    /// </param>
    public void SetType(WeaponType eType)
    {
        // Установить _type
        _type = eType;
        WeaponDefinition def = Main.GetWeaponDefinition(_type);
        _renderer.material.color = def.ProjectileColor;
    }
}
