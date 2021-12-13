using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    [Header("Set in Inspector")]
    public GameObject PointOfInterest;   // Корабль игрока
    public GameObject[] Panels;          // Прокручиваемые панели переднего плана
    public float ScrollSpeed = -30f;
    // MotionMult определяет степень реакции панелей на перемещение корабля игрока
    public float MotionMult = 0.25f;

    private float _panelHigh;            // Высота каждой панели
    private float _depth;                // Глубина панелей (т.е. pos.z)

    private void Start()
    {
        _panelHigh = Panels[0].transform.localScale.y;
        _depth = Panels[0].transform.position.z;

        // Установить панели в начальные позиции
        Panels[0].transform.position = new Vector3(0, 0, _depth);
        Panels[1].transform.position = new Vector3(0, _panelHigh, _depth);
    }

    private void Update()
    {
        float tY, tX = 0;
        tY = Time.time * ScrollSpeed % _panelHigh + (_panelHigh * 0.5f);

        if (PointOfInterest != null)
        {
            tX = -PointOfInterest.transform.position.x * MotionMult;
        }

        // Сместить панель Panels[0]
        Panels[0].transform.position = new Vector3(tX, tY, _depth);
        // Сместить панель Panels[1], чтобы создать эффект непрерывности звёздного поля
        if (tY >= 0)
        {
            Panels[1].transform.position = new Vector3(tX, tY - _panelHigh, _depth);
        }
        else
        {
            Panels[1].transform.position = new Vector3(tX, tY + _panelHigh, _depth);
        }
    }
}
