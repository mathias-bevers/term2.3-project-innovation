using UnityEngine.UI;
using UnityEngine;

public class TempPlayer : MonoBehaviour
{
    public int ID;
    public string Name;

    [SerializeField] Text nameText;
    [SerializeField] Text idText;


    public void SetID(int id)
    {
        ID = id;
        idText.text = id.ToString();
    }

    public void SetName(string name)
    {
        Name = name;
        nameText.text = name;
    }
}
