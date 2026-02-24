using UnityEngine;

[CreateAssetMenu(fileName = "Tag Search - ", menuName = "Tags/Tag Search", order = 1)]
public class TagSearch : RuntimeSet<Tag>
{
    public MatchType filterType;
}