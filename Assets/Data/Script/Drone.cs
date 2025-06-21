using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Drone : MonoBehaviour
{
   
    [SerializeField] float _speed; // Скорость движения дрона
   
    private Transform[] _pointPatrul; // Переменная для хранения точек патрулирования
    private int _currentPoint = 0; // Текущая точка патрулирования
    private Transform _commandCenterPoint; // Переменная для хранения точки командного центра
    private Transform _target; // Переменная для хранения цели, к которой движется дрон
    private Resurs _carriedResurs; // Переменная для хранения ресурса
    private Scaner _scaner; // Переменная для хранения сканера
    private Queue<Resurs> _resurses; // Очередь для хранения ресурсов, которые дрон может забрать
    private CommandCenter _commandCenter; // Переменная для хранения командного центра, к которому принадлежит дрон
    private Resurs _nextResurs; // Выбранный ресурс
 
    private bool _isReady = false; // Флаг, показывает, готов ли дрон к работе
    private bool _isHaveTarget = false; // Флаг, показывает, будет ли у дрона цель для движения
    private bool _haveResurs = false; // Флаг, показывает, несет ли дрон ресурс
    private bool _isWait = false; // Флаг, показывает, ждет ли дрон выполнения действий
    private List<Resurs> _product;
    private CommandCenter _targetBase;

    private void Update() 
    {
        if (_isReady)
        {
            //  1. Несём ресурсы на новую базу
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

            // Несём ресурс на свою базу
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

            // Или ищет ресурсы
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
            FreeMove(); // патрулирование
        }

    }

    // Метод для ожидания после доставки ресурса
    private IEnumerator CompletionDelivery() 
    {
        _isWait = true; 
        yield return new WaitForSeconds(5f);

        // Есть ли ресурсы после сканирования и достаём другой ресурс
        _nextResurs = _scaner.Scane(_resurses).Count > 0 ? _resurses.Dequeue() : null;

        if (_nextResurs != null)
        {
            TakeTarget(_nextResurs.transform);
        }
        _isWait = false; // Сбрасываем флаг ожидания
    }

       private void FreeMove() 
    {
        // Если дрон не имеет цели, двигается к следующей точке патрулирования
        transform.position = Vector3.MoveTowards(transform.position, _pointPatrul[_currentPoint].position, _speed * Time.deltaTime);
        transform.LookAt(_pointPatrul[_currentPoint]); // Поворачивается к точке патрулирования
    }

    //Метод тригера 
    private void OnTriggerEnter(Collider other)
    {
        // Если дрон входит в триггер другой точки патрулирования, переключает на следующую точку
        if (other.gameObject.TryGetComponent<PointPatrul>(out PointPatrul pointPatrul))
        {
            if (_pointPatrul != null && _pointPatrul.Length > 0)
            {
                _currentPoint = ++_currentPoint % _pointPatrul.Length;
            }
        }
    }

    // Метод для движения дрона к цели
    private void MoveToTarget(Transform target)
    {
        transform.position = Vector3.MoveTowards(transform.position, target.position, _speed * Time.deltaTime); // Двигается к цели с заданной скоростью
        transform.LookAt(target.position); // Поворачивается к цели
    }
    public void TakePositionComandCenter(Transform comandCenter) // Метод для установки позиции командного центра
    {
        _commandCenterPoint = comandCenter; // Устанавливает позицию командного центра для дрона
    }
    public void TakePatrulPoint(Transform[] pointPatrul) // Метод для установки точек патрулирования
    {
        _pointPatrul = pointPatrul; // Устанавливает точки патрулирования для дрона
        _isReady = true; // Устанавливает флаг готовности дрона
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

    public void Scanner(Scaner scaner) // Метод для установки сканера для дрона
    {
        _scaner = scaner; // Загружает сканер для обнаружения ресурсов
    }
    public void ResurserQueue(Queue<Resurs> resursers) // Метод для установки очереди ресурсов для дрона
    {
        _resurses = resursers; // Загружает очередь ресурсов, которые дрон может забрать
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