using System.Collections.Generic;
using UnityEngine;

public class AnomalyTagger : MonoBehaviour
{
    [SerializeField] private float tagCheckRadius = 10f;
    public float TagCheckRadius => tagCheckRadius;
    [SerializeField] private List<Tag> tagsToMatch = new();
    public List<Tag> TagsToMatch => tagsToMatch;

    private void Start()
    {
        if (AnomalyResolver.Instance == null)
            AnomalyResolver.Instance.SubscribeToResolver(this);
    }

    private void OnDestroy()
    {
        AnomalyResolver.Instance.UnsubscribeFromResolver(this);
    }
}
