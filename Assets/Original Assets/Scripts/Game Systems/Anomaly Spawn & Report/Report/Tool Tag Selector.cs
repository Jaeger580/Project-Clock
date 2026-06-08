using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class ToolTagSelector : MonoBehaviour
{
    [SerializeField] private AnomalyRayCheck taggerSpawner;
    [SerializeField] private TagSet tags;


    // HARD CODED FOR NOW
    [SerializeField] TMP_Text textOne;
    [SerializeField] TMP_Text textTwo;
    [SerializeField] TMP_Text textThree;


    // Tracks which tag we currently have selected
    private int currentTagIndex = 0;

    private void Start()
    {
        // initiliaze
        ChangeTagType(0);

        // Display first three tags on the device
        UpdateText();

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
        int firstTag;
        int secondTag;
        int thirdTag;

        // check if we need to loop
        if (currentTagIndex - 1 < 0)
        {
            firstTag = tags.items.Count - 1;
            secondTag = currentTagIndex;
            thirdTag = currentTagIndex + 1;
        }
        else if (currentTagIndex + 1 > tags.items.Count - 1)
        {
            firstTag = currentTagIndex - 1;
            secondTag = currentTagIndex;
            thirdTag = 0;
        }
        else 
        {
            firstTag = currentTagIndex - 1;
            secondTag = currentTagIndex;
            thirdTag = currentTagIndex + 1;
        }

        textOne.text = tags.items[firstTag].humanReadableTagIdentifier;
        textTwo.text = tags.items[secondTag].humanReadableTagIdentifier;
        textThree.text = tags.items[thirdTag].humanReadableTagIdentifier;

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
