using System.Collections;
using UnityEngine;

public class Resurs : MonoBehaviour
{
    public bool IsIncludeFree { get; private set; } = false;

    public void SetInclude()
    {
        IsIncludeFree = true;
        StartCoroutine(Reboot()); // ������ ������
    }

    public void StandartSetting()
    {
        IsIncludeFree = false;
    }

    private IEnumerator Reboot()
    {
        yield return new WaitForSeconds(5f); // ����� �� ������
        IsIncludeFree = false;
    }


}

