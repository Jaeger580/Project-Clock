using System.Collections.Generic;

public interface ITagged
{
    public List<Tag> Tags();
}

static public class TagOperator
{
    public enum MatchType { ANY, ALL, NONE, EXACT }

    //private List<ITagged> AnyMatch

    /// <summary>
    /// Given a set of matchingTags, tagsToCheck to filter for matches, and the type of match,
    /// check if the tagsToCheck match the matching tags.
    /// </summary>
    /// <param name="matchingTags">The tags to filter for.</param>
    /// <param name="tagsToCheck">The tags being filtered.</param>
    /// <param name="matchType">The type of match, i.e. any/all/none/exact.</param>
    /// <returns>True if the tagsToCheck match the matchingTags as the matchType requests, and false otherwise.</returns>
    static public bool MatchingTags(List<Tag> matchingTags, List<Tag> tagsToCheck, MatchType matchType = MatchType.ANY)
    {
        bool matching = true;

        if (matchType == MatchType.ANY || matchType == MatchType.NONE)
        {
            matching = matchType != MatchType.ANY;  //hard to explain, but long story short, opposite of the line in the foreach below

            foreach (var tag in matchingTags)
            {
                if (!tagsToCheck.Contains(tag)) continue;
                //we found at least one match - matching would be true if I was looking for ANY match, or false if I'm looking for none
                matching = matchType == MatchType.ANY;
                break;
            }
        }
        else
        {
            if (matchType == MatchType.EXACT)
            {
                if (matchingTags.Count != tagsToCheck.Count)
                {//not an exact match if their counts don't match
                    matching = false;
                }
            }

            foreach (var tag in matchingTags)
            {//if all my matchingTags are contained in the tags I'm checking, we're good
                if (tagsToCheck.Contains(tag)) continue;
                matching = false;
                break;
            }
        }

        return matching;
    }

    /// <summary>
    /// Given a list of matchingTags to filter by, a list of taggedObjs, and the matchType,
    /// return all objects that adhere to the request.
    /// </summary>
    /// <param name="matchingTags">The list of tags to filter by.</param>
    /// <param name="taggedObjs">The list of objects to filter.</param>
    /// <param name="matchType">The type of match being performed.</param>
    /// <returns>A list of all objects from the given list that match the given tags as requested, or an empty list if none match.</returns>
    static public List<ITagged> MatchQuery(List<Tag> matchingTags, List<ITagged> taggedObjs, MatchType matchType = MatchType.ANY)
    {
        List<ITagged> queryMatches = new();

        foreach (var obj in taggedObjs)
        {
            if (MatchingTags(matchingTags, obj.Tags(), matchType)) queryMatches.Add(obj);
        }

        return queryMatches;
    }
}