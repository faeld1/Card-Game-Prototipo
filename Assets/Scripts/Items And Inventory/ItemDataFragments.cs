using UnityEngine;

public enum FragmentType
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary,
    Divine
}


[CreateAssetMenu(fileName = "New Item Data", menuName = "Data/Fragments")]
public class ItemDataFragments : ItemData
{
    public FragmentType fragmentType;
}
