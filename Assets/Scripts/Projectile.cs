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

    // ��� ������������� �������� ��������� ���� _type � ������������ �������� ������������ ��� ������ ��������
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
    /// �������� ������� ���� _type � ������������� ���� ����� �������, ��� ���������� � WeaponDefinition.
    /// </summary>
    /// <param name="eType">
    /// ��� WeaponType ������������� ������.
    /// </param>
    public void SetType(WeaponType eType)
    {
        // ���������� _type
        _type = eType;
        WeaponDefinition def = Main.GetWeaponDefinition(_type);
        _renderer.material.color = def.ProjectileColor;
    }
}
