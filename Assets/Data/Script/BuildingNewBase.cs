using System.Collections.Generic;
using UnityEngine;

//Скрипт строительства базы
public class BuildingNewBase : MonoBehaviour
{
    [SerializeField] private Camera _camera;    //Компонент камеры
    [SerializeField] private CommandCenter _prefabCommandCenter;  //Перфаб базы
    [SerializeField] private Transform _containerBase;  //Контейнер баз
    [SerializeField] private float _scaneRadius;    //Радиус области сканирования
    [SerializeField] private Color _colorCanBuild;  //Цвет базы если строительство возможно
    [SerializeField] private Color _colorCantBuild; //Цвет базы если строительство не возможно

    private RaycastHit _raycastHit; //Компонент столкновения луча
    private Ray _ray;   //Луч
    private float _rayDistance = 1000f; //Дальность луча
    private CommandCenter _tempCommandCenter = null;    //Контейнер базы
    private bool _isHaveBuildBase = false;  //Состояние строительства новой базы
    private bool _isChangeColor = false;    //Состояние изменения цвета

    private void Update()
    {
        _ray = _camera.ScreenPointToRay(Input.mousePosition);   //Рисуем луч от камеры до курсора мыши
        Physics.Raycast(_ray, out _raycastHit); //Столкновение луча с объектом
        Debug.DrawRay(_ray.origin, _ray.direction * _rayDistance);   //Рисуем луч
        SelectionBase();
        BuildBase();
    }

    //Метод выбора базы
    private void SelectionBase()
    {
        //Проверяем нажатие мыши
        if (Input.GetMouseButtonDown(0) && !_isHaveBuildBase)
        {
            //Проверяем, что мы смотрим на необъодимый объект
            if (_raycastHit.transform.TryGetComponent<CommandCenter>(out CommandCenter center))
            {
                _isHaveBuildBase = true;    //Начинаем строительство
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            if (_tempCommandCenter != null)
            {
                Destroy(_tempCommandCenter.gameObject); //Отменяем строительство
                _isHaveBuildBase = false;   //Заканчиваем строительство
            }
        }
        else if (Input.GetMouseButtonDown(0) && _isHaveBuildBase && !IsCollited())
        {
            _tempCommandCenter = null;
            _isHaveBuildBase = false;
        }
    }

    //Метод строительства базы
    public void BuildBase()
    {
        //Если в режиме строительства и база не создана
        if (_isHaveBuildBase && _tempCommandCenter == null)
        {
            _tempCommandCenter = Instantiate(_prefabCommandCenter); //Создаём базу в руке
            _tempCommandCenter.SetLayer();
        }
        else if (_tempCommandCenter != null)
        {
            _tempCommandCenter.transform.position = new Vector3(_raycastHit.point.x, 1, _raycastHit.point.z);   //Выбираем позицию строительства базы по положению курсора
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
        //Собираем все коллайдеры которые попали в область сканирования
        Collider[] triggerColliders = Physics.OverlapSphere(_raycastHit.point, _scaneRadius);

        //Перебираем полученные коллайдеры
        foreach (Collider collider in triggerColliders)
        {
            //Ищем коллайдеры с компонентом Resus 
            if (collider.gameObject.TryGetComponent<CommandCenter>(out CommandCenter center))
            {
                if (_tempCommandCenter != center)
                {
                    //_isChangeColor = true;  //Изменяем цвет базы в руке
                    return true;
                }
            }
        }

        return false;
    }
}
