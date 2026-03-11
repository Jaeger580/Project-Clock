using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnomalyResolver : MonoBehaviour
{
    [SerializeField] private float detectorSpawnCooldown = 2f, resolutionDelay = 2f;
    public float DetectorSpawnCooldown => detectorSpawnCooldown;
    //[SerializeField] private bool debug = false;
    private List<AnomalyTagger> taggers = new();
    private List<AnomalyRoomManager> roomManagers = new();

    [SerializeField, ReadOnly] private LayerMask anomalyLayer;

    [Header("Processing Feedback")]
    [SerializeField] private Canvas processingCanvas;
    [SerializeField] private Image processingBar;
    [Header("None Found Feedback")]
    [SerializeField] private Canvas noAnomaliesFoundCanvas;
    [SerializeField] private TMP_Text noAnomaliesFoundText;
    [SerializeField] private AnimationCurve fadeInOutCurve;
    [SerializeField] private float displayTime;
    [Header("Found Feedback")]
    [SerializeField] private Canvas anomaliesFoundCanvas;

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

    //public void RoomResolutionLogic(List<Tag> tags, int roomID)
    //{
    //    bool anyFound = false;

    //    foreach (var room in roomManagers)
    //    {
    //        if (roomID != room.RoomID) continue;

    //        var validAnomalies = TagOperator.MatchQuery(tags, room.AnomaliesInRoom);
    //        foreach (var anomaly in validAnomalies)
    //        {
    //            if (!anomaly.AnomalyEnabled) continue;
    //            anomaly.DisableAnomaly();
    //            anyFound = true;
    //        }
    //    }

    //    ResolutionFeedback(anyFound);
    //}

    public void TaggerResolutionLogic()
    {
        List<AnomalyHandler> anomaliesToDisable = new();
        foreach (var tagger in taggers)
        {
            Collider[] anomalyCandidates = new Collider[20];
            _ = Physics.OverlapSphereNonAlloc(tagger.transform.position, tagger.TagCheckRadius, anomalyCandidates, anomalyLayer);
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
                if (anomaliesToDisable.Contains(anomaly)) continue;
                anomaliesToDisable.Add(anomaly);
                //anomaly.DisableAnomaly();   //slightly unclean but it's whatever
            }

            Destroy(tagger.gameObject);
        }
        StartCoroutine(ResolutionFeedback(anomaliesToDisable));
    }

    private IEnumerator ProcessingFeedback()
    {
        float journey = 0f;
        float destination = detectorSpawnCooldown;
        processingCanvas.enabled = true;
        while (journey <= destination)
        {
            journey += Time.deltaTime;

            processingBar.fillAmount = Mathf.Lerp(0f, 1.05f, journey/destination);
            
            yield return null;
        }

        processingCanvas.enabled = false;
    }

    private IEnumerator ResolutionFeedback(List<AnomalyHandler> anomalies)
    {
        yield return StartCoroutine(ProcessingFeedback());

        if (anomalies.Count <= 0)
        {
            noAnomaliesFoundCanvas.enabled = true;
            float journey = 0f;
            float destination = displayTime;
            while (journey <= destination)
            {
                journey += Time.deltaTime;

                float curvedPercent = fadeInOutCurve.Evaluate(journey / destination);
                var newColor = noAnomaliesFoundText.color;
                newColor.a = curvedPercent;
                noAnomaliesFoundText.color = newColor;

                yield return null;
            }
            noAnomaliesFoundCanvas.enabled = false;

            yield break;
        }
        //here's where we'd turn off the lights.... IF WE HAD SOME
        anomaliesFoundCanvas.enabled = true;
        yield return null;  //one-frame delay before turning everything off
        foreach (var anom in anomalies)
        {
            anom.DisableAnomaly();
        }
        yield return new WaitForSeconds(resolutionDelay);
        anomaliesFoundCanvas.enabled = false;
    }
}