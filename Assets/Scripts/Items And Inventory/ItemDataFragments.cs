using UnityEngine;

public enum FragmentType
{
    Gloves,
    Boots,
    Pants,
    Armor,
    Helmet,
    Weapon,
    Wings
}


[CreateAssetMenu(fileName = "New Item Data", menuName = "Data/Fragments")]
public class ItemDataFragments : ItemData
{
    public FragmentType fragmentType;
}
