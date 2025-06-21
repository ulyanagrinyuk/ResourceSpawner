using UnityEngine;
using UnityEngine.UI;

public class DrowCounter : MonoBehaviour
{
    [SerializeField] private Text _text;
    public void DrowCounterResurs(int count)
    {
        _text.text = count.ToString(); 
    }
}
