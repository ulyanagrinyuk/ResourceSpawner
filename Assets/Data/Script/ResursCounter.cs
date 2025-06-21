using UnityEngine;

public class ResursCounter : MonoBehaviour
{
    [SerializeField] DrowCounter _drowCounter;
    private int _resursCounter = 0;

    public void AddCounter()
    {
        _resursCounter++;
        _drowCounter.DrowCounterResurs(_resursCounter);
    }

    public void RemoveResurs(int count)
    {
        if (_resursCounter < 0) _resursCounter = 0;
        _resursCounter -= count;
        _drowCounter.DrowCounterResurs(_resursCounter);
    }
}
    
   

