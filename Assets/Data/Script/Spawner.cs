using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private Transform _startPointSpawanPosition;
    [SerializeField] private Transform _endPointSpawanPosition;
    [SerializeField] private Transform _container;
    [SerializeField] private Resurs _prefabResurs;
    [SerializeField] private float _delaySpawn;

    private WaitForSeconds _wait;

    private void Start()
    {
        _wait = new WaitForSeconds(_delaySpawn);
        StartCoroutine(SpawnResurs());
    }

    private IEnumerator SpawnResurs()
    {
        while (enabled)
        {
            Instantiate(_prefabResurs, 
                new Vector3( 
                    Random.Range(_startPointSpawanPosition.position.x, _endPointSpawanPosition.position.x), 
                    1, 
                    Random.Range(_startPointSpawanPosition.position.z, _endPointSpawanPosition.position.z)), 
                    Quaternion.identity, 
                    _container);
            yield return _wait;
        }
    }
}
