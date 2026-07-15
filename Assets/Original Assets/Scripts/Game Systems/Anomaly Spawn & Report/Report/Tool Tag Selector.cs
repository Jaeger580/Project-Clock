using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;

public class ToolTagSelector : MonoBehaviour
{
    [SerializeField] private AnomalyRayCheck taggerSpawner;
    [SerializeField] private TagSet tags;

    // Tags prefabs we will spawn and manipulate
    [SerializeField] private GameObject textPrefab;

    // spline we will animate along
    [SerializeField] private SplineContainer spline;

    // Canvas that we will insantiate objects under
    [SerializeField] private GameObject tagsCanvas;
    private List<GameObject> activeTags = new List<GameObject>();


    private bool isChanging = false;

    // Tracks which tag we currently have selected
    private int currentTagIndex = 0;

    // Audio Source for clicks when tag changes
    private AudioSource clickAudio;

    private void Start()
    {
        InitializeTags();

        clickAudio = this.GetComponentInChildren<AudioSource>();

        // initiliaze
        InitialTagType(0);
    }

    // Instantiates ui objects with the correct text based on the TagSet 
    private void InitializeTags() 
    {
        activeTags.Clear();

        foreach (Tag tag in tags.items) 
        {
            var newObject = Instantiate(textPrefab, tagsCanvas.transform);
            TMP_Text textObj = newObject.GetComponentInChildren<TMP_Text>();
            SplineAnimate aniObj = newObject.GetComponent<SplineAnimate>();

            textObj.text = tag.humanReadableTagIdentifier;
            aniObj.Container = spline;
            aniObj.ObjectForwardAxis = SplineComponent.AlignAxis.NegativeZAxis;
            aniObj.ElapsedTime = 0.0f;

            activeTags.Add(newObject);
        }
    }

    // change active tag
    private void ChangeTagType(int tagIndex)
    {
        if (!isChanging) 
        {
            //print("CHANGING TAG TYPE");
            taggerSpawner.anomalyType = tags.items[tagIndex];
            currentTagIndex = tagIndex;

            int topTag, firstTag, secondTag, thirdTag, bottomTag;

            secondTag = currentTagIndex;
            topTag = (currentTagIndex + 2) % tags.items.Count;
            firstTag = (currentTagIndex + 1) % tags.items.Count;
            thirdTag = (currentTagIndex - 1 + tags.items.Count) % tags.items.Count;
            bottomTag = (currentTagIndex - 2 + tags.items.Count) % tags.items.Count;

            SplineAnimate topText = activeTags[topTag].GetComponent<SplineAnimate>();
            SplineAnimate firstText = activeTags[firstTag].GetComponent<SplineAnimate>();
            SplineAnimate secondText = activeTags[secondTag].GetComponent<SplineAnimate>();
            SplineAnimate thirdText = activeTags[thirdTag].GetComponent<SplineAnimate>();
            SplineAnimate bottomText = activeTags[bottomTag].GetComponent<SplineAnimate>();

            clickAudio.Play();

            StartCoroutine(LerpTags(topText, firstText, secondText, thirdText, bottomText, 0.2f));
        }
    }

    // change active tag the first time
    private void InitialTagType(int tagIndex)
    {
        if (!isChanging)
        {
            //print("CHANGING TAG TYPE");
            taggerSpawner.anomalyType = tags.items[tagIndex];
            currentTagIndex = tagIndex;

            int topTag, firstTag, secondTag, thirdTag, bottomTag;

            secondTag = currentTagIndex;
            topTag = (currentTagIndex + 2) % tags.items.Count;
            firstTag = (currentTagIndex + 1) % tags.items.Count;
            thirdTag = (currentTagIndex - 1 + tags.items.Count) % tags.items.Count;
            bottomTag = (currentTagIndex - 2 + tags.items.Count) % tags.items.Count;

            SplineAnimate topText = activeTags[topTag].GetComponent<SplineAnimate>();
            SplineAnimate firstText = activeTags[firstTag].GetComponent<SplineAnimate>();
            SplineAnimate secondText = activeTags[secondTag].GetComponent<SplineAnimate>();
            SplineAnimate thirdText = activeTags[thirdTag].GetComponent<SplineAnimate>();
            SplineAnimate bottomText = activeTags[bottomTag].GetComponent<SplineAnimate>();

            topText.ElapsedTime = 0.999f;
            firstText.ElapsedTime = 0.75f;
            secondText.ElapsedTime = 0.50f;
            thirdText.ElapsedTime = 0.25f;
            bottomText.ElapsedTime = 0.0f;
        }
    }

    IEnumerator LerpTags(SplineAnimate topTag, SplineAnimate firstTag, SplineAnimate secondTag, SplineAnimate thirdTag, SplineAnimate bottomTag, float stepValue) 
    {
        float timeTaken = 0f;
        isChanging = true;

        var topTime = topTag.ElapsedTime;
        var firstTime = firstTag.ElapsedTime;
        var secTime = secondTag.ElapsedTime;
        var thirdTime = thirdTag.ElapsedTime;
        var botTime = bottomTag.ElapsedTime;

        // if it is at the bottom, teleport to top
        if (topTime < 0.25f)
            topTime = 0.999f;

        if (botTime > 0.75f)
            botTime = 0f;


        while (timeTaken < 0.25f) 
        {
            topTag.ElapsedTime = Mathf.Lerp(topTime, 0.999f, timeTaken / stepValue);
            firstTag.ElapsedTime = Mathf.Lerp(firstTime, 0.75f, timeTaken / stepValue);
            secondTag.ElapsedTime = Mathf.Lerp(secTime, 0.50f, timeTaken / stepValue);
            thirdTag.ElapsedTime = Mathf.Lerp(thirdTime, 0.25f, timeTaken / stepValue);
            bottomTag.ElapsedTime = Mathf.Lerp(botTime, 0.0f, timeTaken / stepValue);


            timeTaken += Time.deltaTime;
            yield return null;
        }

        isChanging = false;
    }

    // Shift the index up or down. Should take +1 or -1 as inputs.
    public void ShiftTagsUp(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            // Get the new index after shifting.
            // SHOULD LOOP AROUND ON IT OWN IN THEORY.
            int indexChange = (currentTagIndex + 1) % tags.items.Count;

            ChangeTagType(indexChange);

        }
    }

    public void ShiftTagsDown(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            // Get the new index after shifting.
            // SHOULD LOOP AROUND ON IT OWN IN THEORY.
            int indexChange = (currentTagIndex - 1 + tags.items.Count) % tags.items.Count;

            ChangeTagType(indexChange);
        }
    }
}
