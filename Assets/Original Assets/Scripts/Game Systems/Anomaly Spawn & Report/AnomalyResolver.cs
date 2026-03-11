using System.Collections.Generic;
using UnityEngine;

public class AnomalyResolver : MonoBehaviour
{
    [SerializeField] private bool debug = false;
    private List<AnomalyTagger> taggers = new();
    private List<AnomalyRoomManager> roomManagers = new();

    [SerializeField, ReadOnly] private LayerMask anomalyLayer;

    private static AnomalyResolver instance;
    static public AnomalyResolver Instance
    {
        get
        {
            if (instance == null)
            {
                //Debug.LogError("ERR: No anomaly resolver found, needs to exist in scene BEFORE taggers spawn.");
                return null;
            }

            return instance;
        }

        private set => instance = value;
    }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            //print("AnomalyResolver already found!");
            return;
        }

        instance = this;

        anomalyLayer = LayerMask.GetMask("Anomaly");
    }

    public void SubscribeToResolver(AnomalyTagger tagger)
    {//only necessary if we end up wanting to tag multiple and then basically do a burst all at once; probably bad design?
        if (!taggers.Contains(tagger))
        {
            //print("Tagger subscribed.");
            taggers.Add(tagger);
        }
    }

    public void SubscribeToResolver(AnomalyRoomManager manager)
    {
        if (roomManagers.Contains(manager))
        {
            //print("Room subscribed.");
            roomManagers.Add(manager);
        }
    }

    public void UnsubscribeFromResolver(AnomalyTagger tagger)
    {
        if (taggers.Contains(tagger))
        {
            //print("Tagger subscribed.");
            taggers.Remove(tagger);
        }
    }

    public void UnsubscribeFromResolver(AnomalyRoomManager manager)
    {
        if (roomManagers.Contains(manager))
        {
            //print("Room unsubscribed.");
            roomManagers.Remove(manager);
        }
    }

    public void RoomResolutionLogic(List<Tag> tags, int roomID)
    {
        bool anyFound = false;

        foreach (var room in roomManagers)
        {
            if (roomID != room.RoomID) continue;

            var validAnomalies = TagOperator.MatchQuery(tags, room.AnomaliesInRoom);
            foreach (var anomaly in validAnomalies)
            {
                if (!anomaly.AnomalyEnabled) continue;
                anomaly.DisableAnomaly();
                anyFound = true;
            }
        }

        ResolutionFeedback(anyFound);
    }

    public void TaggerResolutionLogic()
    {
        bool anyFound = false;

        foreach (var tagger in taggers)
        {
            Collider[] anomalyCandidates = new Collider[20];
            var total = Physics.OverlapSphereNonAlloc(tagger.transform.position, tagger.TagCheckRadius, anomalyCandidates, anomalyLayer);
            List<AnomalyHandler> foundAnomalies = new();

            foreach (var candidate in anomalyCandidates)
            {
                if (candidate == null) continue;
                if (candidate.TryGetComponent(out AnomalyHandler handler))
                {
                    if (foundAnomalies.Contains(handler)) continue;
                    foundAnomalies.Add(handler);
                    print("Found anomaly.");
                }
                else if (candidate.TryGetComponent(out AnomalyHandlerPassthrough passthrough))
                {
                    if (foundAnomalies.Contains(passthrough.ParentAnomaly)) continue;
                    foundAnomalies.Add(passthrough.ParentAnomaly);
                    print("Found parent anomaly.");
                }
            }

            var validAnomalies = TagOperator.MatchQuery(tagger.TagsToMatch, foundAnomalies);
            foreach (var anomaly in validAnomalies)
            {
                anomaly.DisableAnomaly();   //slightly unclean but it's whatever
                anyFound = true;
            }

            Destroy(tagger.gameObject);
        }

        ResolutionFeedback(anyFound);
    }

    private void ResolutionFeedback(bool anyAnomalyFound)
    {
        if (anyAnomalyFound)
        {
            //feedback for "some were found"; in Observation Duty, it cuts to a separate screen briefly while the anomaly "gets fixed"
        }
        else
        {
            //feedback for "none were found"; probably just text or something?
        }
    }
}