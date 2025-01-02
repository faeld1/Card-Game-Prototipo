using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stats
{
    [SerializeField] private int baseValue;

    public List<int> modifiers;

    public int GetValue()
    {
        int finalValue = baseValue;

        foreach (int modifier in modifiers)
        {
            finalValue += modifier;
        }

        return finalValue;
    }

    public int GetDefaultValue()
    {
        int value = baseValue;

        return value;
    }
    public void AddModifiers(int _modifier)
    {
        modifiers.Add(_modifier);
    }

    public void RemoveModifiers(int _modifier)
    { 
        modifiers.Remove(_modifier);
    }
}
