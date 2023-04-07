using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class ColourPannel : MonoBehaviour
{
    public int ID;

    [SerializeField] private Color deathColor;

    [SerializeField] Image backgroundColourImage;
    [SerializeField] Text usernameText;
    [SerializeField] Text stateText;

    [SerializeField] Gradient colourGradient = new Gradient();
    [SerializeField] Image bar;

    [SerializeField] List<ColourVector> colours = new List<ColourVector>();
    [SerializeField] List<DeathToText> deathToTexts = new List<DeathToText>();

    private void Start()
    {
        stateText.text = "Alive";
    }

    public void SetName(string name)
    {
        usernameText.text = name;
    }

    public void SetColor(ColourType type)
    {
        foreach (var item in colours)
        {
            if (item.colourType != type) continue;
            backgroundColourImage.color = item.colourMaterial;
        }
    }

    public void SetDeathEvent(DeathType type)
    {
        backgroundColourImage.color = deathColor;
        foreach (var item in deathToTexts)
        {
            if (item.type != type) continue;
            stateText.text = item.Text;
        }
    }

    public void SetBar(float current, float max)
    {
        bar.fillAmount = current / max;
        bar.color = colourGradient.Evaluate(bar.fillAmount);
    }
}

[System.Serializable]
public struct DeathToText
{
    public DeathType type;
    public string Text;
}