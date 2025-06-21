using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

public class Scaner : MonoBehaviour
{
    [SerializeField] private float _scaneRadius;

    public Queue<Resurs> Scane(Queue<Resurs> resurses)
    {
        Collider[] triggerColliders = Physics.OverlapSphere(transform.position, _scaneRadius);

        foreach (Collider collider in triggerColliders)
        {
            if (collider.gameObject.TryGetComponent<Resurs>(out Resurs resurs))
            {
                if (!resurses.Contains(resurs))
                {
                    if (!resurs.IsIncludeFree)
                    {
                        resurs.SetInclude();
                        resurses.Enqueue(resurs); 
                    }
                }
            }
        }
        return resurses;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green; // Цвет Gizmos
        Gizmos.DrawSphere(transform.position, _scaneRadius); 
    }

}
