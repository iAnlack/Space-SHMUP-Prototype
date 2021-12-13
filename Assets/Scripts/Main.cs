using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    static public Main S; // ������-�������� Main
    static Dictionary<WeaponType, WeaponDefinition> WEAP_DICT;

    [Header("Set in Inspector")]
    public GameObject[] PrefabEnemies; // ������ �������� Enemy
    public float EnemySpawnPerSecond = 0.5f; // ��������� �������� � ������� 
    public float EnemyDefaultPadding = 1.5f; // ������ ��� ����������������
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
        // �������� � _bndCheck ������ �� ��������� BoundsCheck ����� �������� �������
        _bndCheck = GetComponent<BoundsCheck>();
        // �������� SpawnEnemy() ���� ��� (� 2 ������� ��� ��������� �� ���������)
        Invoke("SpawnEnemy", 1f / EnemySpawnPerSecond);

        // ������� � ������� ���� WeaponType
        WEAP_DICT = new Dictionary<WeaponType, WeaponDefinition>();
        foreach (WeaponDefinition def in WeaponDefinitions)
        {
            WEAP_DICT[def.Type] = def;
        }
    }

    public void SpawnEnemy()
    {
        // ������� ��������� ������ Enemy ��� ��������
        int ndx = Random.Range(0, PrefabEnemies.Length);
        GameObject gameObject = Instantiate<GameObject>(PrefabEnemies [ndx]);

        // ���������� ��������� ������� ��� ������� � ��������� ������� x
        float enemyPadding = EnemyDefaultPadding;
        if (gameObject.GetComponent<BoundsCheck>() != null)
        {
            enemyPadding = Mathf.Abs(gameObject.GetComponent<BoundsCheck>().Radius);
        }

        // ���������� ��������� ���������� ���������� ���������� �������
        Vector3 pos = Vector3.zero;
        float xMin = -_bndCheck.CamWidth + enemyPadding;
        float xMax = _bndCheck.CamWidth - enemyPadding;
        pos.x = Random.Range(xMin, xMax);
        pos.y = _bndCheck.CamHeight + enemyPadding;
        gameObject.transform.position = pos;

        // ����� ������� SpawnEnemy()
        Invoke("SpawnEnemy", 1f / EnemySpawnPerSecond);
    }

    public void ShipDestroyed(Enemy enemy)
    {
        // ������������� ����� � �������� ������������
        if (Random.value <= enemy.PowerUpDropChance)
        {
            // ������� ��� ������
            // ������� ���� �� ��������� � PowerUpFrequency
            int ndx = Random.Range(0, PowerUpFrequency.Length);
            WeaponType powerUpType = PowerUpFrequency[ndx];

            // ������� ��������� PowerUp
            GameObject gameObject = Instantiate(PrefabPowerUp) as GameObject;
            PowerUp powerUp = gameObject.GetComponent<PowerUp>();
            // ���������� ��������������� ��� WeaponType
            powerUp.SetType(powerUpType);

            // ��������� � �����, ��� ��������� ����������� �������
            powerUp.transform.position = enemy.transform.position;
        }
    }

    public void DelayedRestart(float delay)
    {
        // ������� ����� Restart() ����� delay ������
        Invoke("Restart", delay);
    }

    public void Restart()
    {
        // ������������� Main Scene, ����� ������������� ����
        SceneManager.LoadScene("Main Scene");
    }

    /// <summary>
    /// ����������� �������, ������������ WeaponDefinition �� 
    /// ������������ ����������� ���� WEAP_DICT ������ Main.
    /// </summary>
    /// <returns>
    /// ��������� WeaponDefinition ���, ���� ��� ������ ����������� ��� ���������� WeaponType,
    /// ���������� ����� ��������� WeaponDefinition � ����� none.
    /// </returns>
    /// <param name="wt">��� ������ WeaponType, ��� �������� ��������� �������� WeaponDefinition
    /// </param>
    static public WeaponDefinition GetWeaponDefinition(WeaponType wt)
    {
        // ��������� ������� ���������� ����� � �������
        // ������� ������� �������� �� �������������� ����� ������� ������,
        // ������� ��������� ���������� ������ ������ ����.
        if (WEAP_DICT.ContainsKey(wt))
        {
            return (WEAP_DICT[wt]);
        }
        // ��������� ���������� ���������� ����� ��������� WeaponDefinition � ����� ������ WeaponType.none,
        // ��� �������� ��������� ������� ����� ��������� ����������� WeaponDefinition
        return (new WeaponDefinition());
    }

}
