using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Drone : MonoBehaviour
{
   
    [SerializeField] float _speed; // �������� �������� �����
   
    private Transform[] _pointPatrul; // ���������� ��� �������� ����� ��������������
    private int _currentPoint = 0; // ������� ����� ��������������
    private Transform _commandCenterPoint; // ���������� ��� �������� ����� ���������� ������
    private Transform _target; // ���������� ��� �������� ����, � ������� �������� ����
    private Resurs _carriedResurs; // ���������� ��� �������� �������
    private Scaner _scaner; // ���������� ��� �������� �������
    private Queue<Resurs> _resurses; // ������� ��� �������� ��������, ������� ���� ����� �������
    private CommandCenter _commandCenter; // ���������� ��� �������� ���������� ������, � �������� ����������� ����
    private Resurs _nextResurs; // ��������� ������
 
    private bool _isReady = false; // ����, ����������, ����� �� ���� � ������
    private bool _isHaveTarget = false; // ����, ����������, ����� �� � ����� ���� ��� ��������
    private bool _haveResurs = false; // ����, ����������, ����� �� ���� ������
    private bool _isWait = false; // ����, ����������, ���� �� ���� ���������� ��������
    private List<Resurs> _product;
    private CommandCenter _targetBase;

    private void Update() 
    {
        if (_isReady)
        {
            //  1. ���� ������� �� ����� ����
            if (_haveResurs && _targetBase != null)
            {
                MoveToTarget(_targetBase.transform);

                if (Vector3.Distance(transform.position, _targetBase.transform.position) < 0.5f)
                {
                    foreach (var res in _product)
                        _targetBase.StoreResurs(res);

                    _product = null;
                    _haveResurs = false;

                    TakeCommandCenter(_targetBase);
                    _targetBase = null;
                }
            }
            else if (_isHaveTarget)
            {
                MoveToTarget(_target);

                if (Vector3.Distance(transform.position, _target.position) < 0.5f)
                {
                    _carriedResurs = _target.GetComponent<Resurs>();

                    if (_carriedResurs != null)
                    {
                        _carriedResurs.gameObject.SetActive(false);
                        _haveResurs = true;
                        _isHaveTarget = false;
                    }
                    else
                    {
                        _isHaveTarget = false;
                        _target = null;
                    }

                }
            }

            // ���� ������ �� ���� ����
            else if (_haveResurs)
            {
                MoveToTarget(_commandCenterPoint);

                if (Vector3.Distance(transform.position, _commandCenterPoint.position) < 0.5f && !_isWait)
                {
                    _carriedResurs = null;
                    _haveResurs = false;
                    StartCoroutine(CompletionDelivery());
                }
                
            }

            // ��� ���� �������
            else
            {
                _scaner.Scane(_resurses);

                while (_resurses.Count > 0)
                {
                    Resurs next = _resurses.Dequeue();
                    if (next != null && next.gameObject.activeInHierarchy)
                    {
                        TakeTarget(next.transform);
                        break;
                    }                   
                }
            }
        }
        else
        {
            FreeMove(); // ��������������
        }

    }

    // ����� ��� �������� ����� �������� �������
    private IEnumerator CompletionDelivery() 
    {
        _isWait = true; 
        yield return new WaitForSeconds(5f);

        // ���� �� ������� ����� ������������ � ������ ������ ������
        _nextResurs = _scaner.Scane(_resurses).Count > 0 ? _resurses.Dequeue() : null;

        if (_nextResurs != null)
        {
            TakeTarget(_nextResurs.transform);
        }
        _isWait = false; // ���������� ���� ��������
    }

       private void FreeMove() 
    {
        // ���� ���� �� ����� ����, ��������� � ��������� ����� ��������������
        transform.position = Vector3.MoveTowards(transform.position, _pointPatrul[_currentPoint].position, _speed * Time.deltaTime);
        transform.LookAt(_pointPatrul[_currentPoint]); // �������������� � ����� ��������������
    }

    //����� ������� 
    private void OnTriggerEnter(Collider other)
    {
        // ���� ���� ������ � ������� ������ ����� ��������������, ����������� �� ��������� �����
        if (other.gameObject.TryGetComponent<PointPatrul>(out PointPatrul pointPatrul))
        {
            if (_pointPatrul != null && _pointPatrul.Length > 0)
            {
                _currentPoint = ++_currentPoint % _pointPatrul.Length;
            }
        }
    }

    // ����� ��� �������� ����� � ����
    private void MoveToTarget(Transform target)
    {
        transform.position = Vector3.MoveTowards(transform.position, target.position, _speed * Time.deltaTime); // ��������� � ���� � �������� ���������
        transform.LookAt(target.position); // �������������� � ����
    }
    public void TakePositionComandCenter(Transform comandCenter) // ����� ��� ��������� ������� ���������� ������
    {
        _commandCenterPoint = comandCenter; // ������������� ������� ���������� ������ ��� �����
    }
    public void TakePatrulPoint(Transform[] pointPatrul) // ����� ��� ��������� ����� ��������������
    {
        _pointPatrul = pointPatrul; // ������������� ����� �������������� ��� �����
        _isReady = true; // ������������� ���� ���������� �����
    }
    public void TakeTarget(Transform target)
    {
        if (target == null)
        {
            return;
        }

        _target = target;
        _isHaveTarget = true;
    }

    public void Scanner(Scaner scaner) // ����� ��� ��������� ������� ��� �����
    {
        _scaner = scaner; // ��������� ������ ��� ����������� ��������
    }
    public void ResurserQueue(Queue<Resurs> resursers) // ����� ��� ��������� ������� �������� ��� �����
    {
        _resurses = resursers; // ��������� ������� ��������, ������� ���� ����� �������
    }
    public void TakeCommandCenter(CommandCenter commandCenter) 
    {
        _commandCenter = commandCenter; 
    }
    public void SupplyBase(List<Resurs> product, CommandCenter target, Transform[] patrol)
    {
        TakePatrulPoint(patrol);
        TakeCommandCenter(target);
        TakeTarget(target.transform);

        _product = product;
        _targetBase = target;      
        _isHaveTarget = true;
        _haveResurs = true;
    }
}