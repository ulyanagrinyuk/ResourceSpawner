using System.Collections.Generic;
using UnityEngine;

//������ ������������� ����
public class BuildingNewBase : MonoBehaviour
{
    [SerializeField] private Camera _camera;    //��������� ������
    [SerializeField] private CommandCenter _prefabCommandCenter;  //������ ����
    [SerializeField] private Transform _containerBase;  //��������� ���
    [SerializeField] private float _scaneRadius;    //������ ������� ������������
    [SerializeField] private Color _colorCanBuild;  //���� ���� ���� ������������� ��������
    [SerializeField] private Color _colorCantBuild; //���� ���� ���� ������������� �� ��������

    private RaycastHit _raycastHit; //��������� ������������ ����
    private Ray _ray;   //���
    private float _rayDistance = 1000f; //��������� ����
    private CommandCenter _tempCommandCenter = null;    //��������� ����
    private bool _isHaveBuildBase = false;  //��������� ������������� ����� ����
    private bool _isChangeColor = false;    //��������� ��������� �����

    private void Update()
    {
        _ray = _camera.ScreenPointToRay(Input.mousePosition);   //������ ��� �� ������ �� ������� ����
        Physics.Raycast(_ray, out _raycastHit); //������������ ���� � ��������
        Debug.DrawRay(_ray.origin, _ray.direction * _rayDistance);   //������ ���
        SelectionBase();
        BuildBase();
    }

    //����� ������ ����
    private void SelectionBase()
    {
        //��������� ������� ����
        if (Input.GetMouseButtonDown(0) && !_isHaveBuildBase)
        {
            //���������, ��� �� ������� �� ����������� ������
            if (_raycastHit.transform.TryGetComponent<CommandCenter>(out CommandCenter center))
            {
                _isHaveBuildBase = true;    //�������� �������������
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            if (_tempCommandCenter != null)
            {
                Destroy(_tempCommandCenter.gameObject); //�������� �������������
                _isHaveBuildBase = false;   //����������� �������������
            }
        }
        else if (Input.GetMouseButtonDown(0) && _isHaveBuildBase && !IsCollited())
        {
            _tempCommandCenter = null;
            _isHaveBuildBase = false;
        }
    }

    //����� ������������� ����
    public void BuildBase()
    {
        //���� � ������ ������������� � ���� �� �������
        if (_isHaveBuildBase && _tempCommandCenter == null)
        {
            _tempCommandCenter = Instantiate(_prefabCommandCenter); //������ ���� � ����
            _tempCommandCenter.SetLayer();
        }
        else if (_tempCommandCenter != null)
        {
            _tempCommandCenter.transform.position = new Vector3(_raycastHit.point.x, 1, _raycastHit.point.z);   //�������� ������� ������������� ���� �� ��������� �������
        }

        if (IsCollited() && _tempCommandCenter != null)
        {
            _tempCommandCenter.ChangeColor(_colorCantBuild);
        }
        else if (_tempCommandCenter != null)
        {
            _tempCommandCenter.ChangeColor(_colorCanBuild);
        }
    }

    private bool IsCollited()
    {
        //�������� ��� ���������� ������� ������ � ������� ������������
        Collider[] triggerColliders = Physics.OverlapSphere(_raycastHit.point, _scaneRadius);

        //���������� ���������� ����������
        foreach (Collider collider in triggerColliders)
        {
            //���� ���������� � ����������� Resus 
            if (collider.gameObject.TryGetComponent<CommandCenter>(out CommandCenter center))
            {
                if (_tempCommandCenter != center)
                {
                    //_isChangeColor = true;  //�������� ���� ���� � ����
                    return true;
                }
            }
        }

        return false;
    }
}
