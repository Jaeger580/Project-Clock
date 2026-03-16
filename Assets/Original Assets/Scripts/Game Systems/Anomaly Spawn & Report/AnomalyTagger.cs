using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnomalyTagger : MonoBehaviour
{
    [SerializeField] private float tagCheckRadius = 10f;
    public float TagCheckRadius => tagCheckRadius;
    [SerializeField] private List<Tag> tagsToMatch = new();
    public List<Tag> TagsToMatch => tagsToMatch;

    private IEnumerator Start()
    {
        if (AnomalyResolver.Instance == null)
        {
            Destroy(gameObject);
            yield break;
        }
        AnomalyResolver.Instance?.SubscribeToResolver(this);

        //yield return new WaitForSeconds(0.5f);
        yield return null;
        AnomalyResolver.Instance?.TaggerResolutionLogic();
    }

    private void OnDestroy()
    {
        AnomalyResolver.Instance?.UnsubscribeFromResolver(this);
    }

    public void SetTags(List<Tag> tagsToSet)
    {
        tagsToMatch = tagsToSet;
    }
    public void SetTags(Tag tagToSet)
    {
        tagsToMatch.Clear();
        tagsToMatch.Add(tagToSet);
        //print(tagToSet.humanReadableTagIdentifier);
    }
}
