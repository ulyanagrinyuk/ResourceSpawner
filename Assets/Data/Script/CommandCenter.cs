using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshRenderer))]
public class CommandCenter : MonoBehaviour
{
    [SerializeField] private Scaner _scaner;
    [SerializeField] private Transform[] _pointPatrul;
    [SerializeField] private Drone _dronePrefab;
    [SerializeField] private Transform _spawnPositionDron;
    [SerializeField] private Transform _dronContainer;
    [SerializeField] private ResursCounter _resursCounter;
    [SerializeField] private List<Resurs> _storage = new List<Resurs>(); 
    [SerializeField] private bool _isTemplate = false;


    private MeshRenderer _meshRenderer;
    private Queue<Resurs> _resurses = new Queue<Resurs>();
    private Queue<Drone> _drons = new Queue<Drone>();
    private Drone _tempDrone;
    private bool _isHaveDrone = false;
    private Resurs _tempResurs;
    private bool _isBuilding = false;
    private LayerMask _layerMask = 6;
    private bool _isSupplyBase = false;
    private int _sendResurs = 5;
    private int _totalDrones = 0;
    private CommandCenter _nextBase;

    public bool IsActive => _isBuilding;
    private void Start()
    {
        if (!_isTemplate)
        {
            Build();
        }
    }

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }
    private void Update()
    {
        if (_isBuilding)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log(_scaner.Scane(_resurses).Count);
            }
            if (Input.GetKeyDown(KeyCode.E) && _isHaveDrone == false)
            {
                CreateDrons();
                _isHaveDrone = true;

                _tempDrone.Scanner(_scaner); // Передаем сканер дрону для обнаружения ресурсов
                _tempDrone.ResurserQueue(_resurses); // Передаем очередь ресурсов дрону
            }

            if (_drons.Count > 0)
            {
                if (_resurses.Count > 0)
                {
                    SentDrone();
                }
            }
        }
        if (_isSupplyBase && _storage.Count >= _sendResurs)
        {
            _isSupplyBase = false;

            // Собираем 5 ресурсов
            List<Resurs> supply = _storage.GetRange(_storage.Count - _sendResurs, _sendResurs);
            _storage.RemoveRange(_storage.Count - _sendResurs, _sendResurs);
            _resursCounter.RemoveResurs(_sendResurs);

            // Создаем одного дрона
            Drone supplierDrone = Instantiate(_dronePrefab, _spawnPositionDron.position, Quaternion.identity, _dronContainer);
            supplierDrone.TakeCommandCenter(_nextBase);
            supplierDrone.SupplyBase(supply, _nextBase, _pointPatrul);
        }

    }
    private void SentDrone()
    {
        _tempDrone = _drons.Dequeue();
        _tempDrone.TakeTarget(_resurses.Dequeue().transform);

    }
    private void CreateDrons()
    {
        _tempDrone = Instantiate(_dronePrefab, _spawnPositionDron.position, Quaternion.identity, _dronContainer);
        _tempDrone.TakePositionComandCenter(this.transform);
        _tempDrone.TakePatrulPoint(_pointPatrul);
        _tempDrone.TakeCommandCenter(this);
        _drons.Enqueue(_tempDrone);
        _totalDrones++;
    }
    public void StoreResurs(Resurs resurs) // Метод для хранения ресурса в командном центре
    {
        _storage.Add(resurs); // Добавляем ресурс в список хранения
        _resursCounter.AddCounter();

        // Если количество ресурсов кратно 3, создаем нового дрона
        if (!_isSupplyBase && _storage.Count % 3 == 0)
        {
            // Создаем нового дрона с использованием prefaba
            Drone newDrone = Instantiate(_dronePrefab, _spawnPositionDron.position, Quaternion.identity, _dronContainer);
            newDrone.TakePositionComandCenter(this.transform);
            newDrone.TakePatrulPoint(_pointPatrul);
            newDrone.TakeCommandCenter(this);
            newDrone.Scanner(_scaner);
            newDrone.ResurserQueue(_resurses);
            _drons.Enqueue(newDrone);
            _totalDrones++;


            // Удаляем 3 ресурса
            _storage.RemoveRange(_storage.Count - 3, 3);
            _resursCounter.RemoveResurs(3);
        }
    }
    public void ChangeColor(Color color)
    {
        Material material = _meshRenderer.material;
        material.color = color;
        _meshRenderer.material = material;
    }

    public void TakeResurs(Drone drone)
    {
        _resursCounter.AddCounter();
        _drons.Enqueue(drone);

    }

    public void SetLayer()
    {
        transform.gameObject.layer = 0;
    }
    public void Build()
    {
        _isBuilding = true;
    }
    public void SendSupply(CommandCenter targetBase)
    {
        _nextBase = targetBase;
        _isSupplyBase = true;
    }
    public void SpendResurs(int amount)
    {
        if (_storage.Count >= amount)
            _storage.RemoveRange(_storage.Count - amount, amount);
    }    
    public bool OneDrone()
    {
        return _totalDrones <= 1;
    }
}
