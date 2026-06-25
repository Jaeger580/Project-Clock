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

    // HARD CODED FOR NOW
    //[SerializeField] TMP_Text textOne;
    //[SerializeField] TMP_Text textTwo;
    //[SerializeField] TMP_Text textThree;


    // Tracks which tag we currently have selected
    private int currentTagIndex = 0;

    private void Start()
    {
        InitializeTags();

        // initiliaze
        ChangeTagType(0);

        // Display first three tags on the device
        UpdateText();

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
        //print("CHANGING TAG TYPE");
        taggerSpawner.anomalyType = tags.items[tagIndex];
        currentTagIndex = tagIndex;
        UpdateText();
    }

    // update the text on the device
    private void UpdateText()
    {
        int topTag;
        int firstTag;
        int secondTag;
        int thirdTag;
        int bottomTag;

        // check if we need to loop
        if (currentTagIndex - 1 < 0)
        {
            firstTag = tags.items.Count - 1;
            topTag = tags.items.Count - 2;
        }
        else if (currentTagIndex - 2 < 0)
        {
            topTag = tags.items.Count - 1;
            firstTag = currentTagIndex - 1;
        }
        else 
        {
            firstTag = currentTagIndex - 1;
            topTag = currentTagIndex - 2;
        }

        if (currentTagIndex + 1 > tags.items.Count - 1)
        {
            thirdTag = 0;
            bottomTag = tags.items.Count - 1;
        }
        else if(currentTagIndex + 2 > tags.items.Count - 1)
        {
            thirdTag = currentTagIndex + 1;
            bottomTag = 0;
        }
        else 
        {
            thirdTag = currentTagIndex + 1;
            bottomTag = currentTagIndex + 2;
        }

        secondTag = currentTagIndex;
        //// check if we need to loop
        //if (currentTagIndex - 1 < 0)
        //{
        //    firstTag = tags.items.Count - 1;
        //    secondTag = currentTagIndex;
        //    thirdTag = currentTagIndex + 1;
        //}
        //else if (currentTagIndex + 1 > tags.items.Count - 1)
        //{
        //    firstTag = currentTagIndex - 1;
        //    secondTag = currentTagIndex;
        //    thirdTag = 0;
        //}
        //else 
        //{
        //    firstTag = currentTagIndex - 1;
        //    secondTag = currentTagIndex;
        //    thirdTag = currentTagIndex + 1;
        //}

        //textOne.text = tags.items[firstTag].humanReadableTagIdentifier;
        //textTwo.text = tags.items[secondTag].humanReadableTagIdentifier;
        //textThree.text = tags.items[thirdTag].humanReadableTagIdentifier;
        activeTags[topTag].GetComponent<SplineAnimate>().ElapsedTime = 1.0f;
        activeTags[firstTag].GetComponent<SplineAnimate>().ElapsedTime = 0.75f;
        activeTags[secondTag].GetComponent<SplineAnimate>().ElapsedTime = 0.50f;
        activeTags[thirdTag].GetComponent<SplineAnimate>().ElapsedTime = 0.25f;
        activeTags[bottomTag].GetComponent<SplineAnimate>().ElapsedTime = 0.0f;

        //activeTags[activeTags.Count - 1].GetComponent<SplineAnimate>().ElapsedTime = 1.0f;


    }

    // Shift the index up or down. Should take +1 or -1 as inputs.
    public void ShiftTagsUp(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            // Get the new index after shifting.
            // SHOULD LOOP AROUND ON IT OWN IN THEORY.
            int indexChange = (currentTagIndex + 1) % tags.items.Count;

            foreach (GameObject obj in activeTags) 
            {
                SplineAnimate ani = obj.GetComponent<SplineAnimate>();
                obj.GetComponent<SplineAnimate>().ElapsedTime += 0.25f;
            }

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
