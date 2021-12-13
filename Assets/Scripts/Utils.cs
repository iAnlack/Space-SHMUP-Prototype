using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour
{
    
    //===================== ������� ��� ������ � ����������� =====================\\

    // ���������� ������ ���� ���������� � ������ ������� ������� � ��� �������� ��������

    static public Material[] GetAllMaterials(GameObject gameObject)
    {
        Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();

        List<Material> materials = new List<Material>();
        foreach (Renderer renderer in renderers)
        {
            materials.Add(renderer.material);
        }

        return(materials.ToArray());
    }


}
