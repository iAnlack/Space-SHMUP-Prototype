using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [Header("Set in Inspector")]
    // ���������, �� ������� ���������� Vector2. x ������ ����������� ��������, � y - ������������ ��������
    // ��� ������ Random.Range(), ������� ����� ���������� �����
    public Vector2 RotMinMax = new Vector2(15, 90);
    public Vector2 DriftMinMax = new Vector2(0.25f, 2);
    public float LifeTime = 6f;    // ����� ������������� PowerUp � ��������
    public float FadeTime = 4f;    // ����� ���������???

    [Header("Set Dynamically")]
    public WeaponType Type;        // ��� ������
    public GameObject Cube;        // ������ �� ��������� ���
    public TextMesh Letter;        // ������ �� TextMesh
    public Vector3 RotPerSecond;   // �������� ��������
    public float BirthTime;

    private Rigidbody _rigidbody;
    private BoundsCheck _bndCheck;
    private Renderer _cubeRenderer;

    private void Awake()
    {
        // �������� ������ �� ���
        Cube = transform.Find("Cube").gameObject;
        // �������� ������ �� TextMesh � ������ ����������
        Letter = GetComponent<TextMesh>();
        _rigidbody = GetComponent<Rigidbody>();
        _bndCheck = GetComponent<BoundsCheck>();
        _cubeRenderer = Cube.GetComponent<Renderer>();

        // ������� ��������� ��������
        Vector3 velocity = Random.onUnitSphere; // �������� ��������� �������� XYZ
        // Random.onUnitSphere ���������� ������, ����������� �� ��������� �����, ����������� ��
        // ����������� ����� �������� 1� � � ������� � ������ ���������
        velocity.z = 0; // ���������� velocity �� ��������� XY
        velocity.Normalize(); // ������������ ������������� ����� Vector3 ������ 1�
        velocity *= Random.Range(DriftMinMax.x, DriftMinMax.y);
        _rigidbody.velocity = velocity;

        // ���������� ���� �������� ����� �������� ������� ������ R: [0, 0, 0]
        transform.rotation = Quaternion.identity;
        // Quaternion.identity ���������� ���������� ��������

        // ������� ��������� �������� �������� ��� ���������� ���� � �������������� RotMinMax.x � RotMinMax.y
        RotPerSecond = new Vector3
            (Random.Range(RotMinMax.x, RotMinMax.y),
            Random.Range(RotMinMax.x, RotMinMax.y),
            Random.Range(RotMinMax.x, RotMinMax.y));

        BirthTime = Time.time;
    }

    private void Update()
    {
        Cube.transform.rotation = Quaternion.Euler(RotPerSecond * Time.time);

        // ������ ����������� ���� PowerUp � �������� �������
        // �� ���������� �� ��������� ����� ���������� 10 ������, � ����� ������������ � ������� 4 ������.
        float u = (Time.time - (BirthTime + LifeTime)) / FadeTime;
        // � ������� LifeTime ������ �������� u ����� <= 0.
        // ����� ��� ������ ������������� � ����� FadeTime ������ ������ ������ 1

        // ���� u >= 1, ���������� �����
        if (u >= 1)
        {
            Destroy(this.gameObject);
            return;
        }

        // ������������ u ��� ����������� �����-�������� ���� � �����
        if (u > 0)
        {
            Color c = _cubeRenderer.material.color;
            c.a = 1f - u;
            _cubeRenderer.material.color = c;
            // ����� ���� ������ ������������, �� ���������
            c = Letter.color;
            c.a = 1f - (u * 0.5f);
            Letter.color = c;
        }

        if (!_bndCheck.IsOnScreen)
        {
            // ���� ����� ��������� ����� �� ������� ������, ���������� ���
            Destroy(gameObject);
        }
    }

    public void SetType(WeaponType weaponType)
    {
        // �������� WeaponDefinition �� Main
        WeaponDefinition def = Main.GetWeaponDefinition(weaponType);
        // ���������� ���� ��������� ����
        _cubeRenderer.material.color = def.Color;
        //Letter.color = def.Color; // ����� ���� ����� �������� � ��� �� ����
        Letter.text = def.Letter; // ���������� ������������ �����
        Type = weaponType; // � ���������� ���������� ����������� ���
    }

    public void AbsorbedBy(GameObject target)
    {
        // ��� ������� ���������� ������� Hero, ����� ����� ��������� �����
        // ����� ���� ����������� ������ ���������� ������, �������� ��� ������� � ������� ���������� ������,
        // �� ���� ������ ��������� this.gameObject
        Destroy(this.gameObject);
    }
}
