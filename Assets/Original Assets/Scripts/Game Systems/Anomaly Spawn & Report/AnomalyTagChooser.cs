using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public partial class InputHandler : MonoBehaviour
{
    [Header("Mouse Lock Inputs")]
    [SerializeField] private GameEvent input_FreeMouse;
    [SerializeField] private GameEvent input_LockMouse;

    public void FreeMouse(InputAction.CallbackContext context)
    {
        if (context.started)
            input_FreeMouse?.Trigger();
        else if (context.canceled)
            input_LockMouse?.Trigger();
    }
}

public class AnomalyTagChooser : MonoBehaviour
{
    [SerializeField] private AnomalyRayCheck taggerSpawner;
    [SerializeField] private TMP_Dropdown dropdown;
    [SerializeField] private TagSet tags;

    [SerializeField] private GameEvent input_FreeMouse;
    [SerializeField] private GameEvent input_LockMouse;
    static public bool FreeMouse => Cursor.lockState == CursorLockMode.None;

    private void Start()
    {
        dropdown.ClearOptions();
        List<TMP_Dropdown.OptionData> options = new();
        foreach (var tag in tags.items)
        {
            TMP_Dropdown.OptionData option = new(tag.humanReadableTagIdentifier);
            options.Add(option);
        }
        dropdown.AddOptions(options);

        var freeListener = GameEventListener.AddGeneralListener(gameObject, input_FreeMouse);
        freeListener.Response = new();
        freeListener.Response.AddListener(() => { Cursor.lockState = CursorLockMode.None; Cursor.visible = true; });

        var lockListener = GameEventListener.AddGeneralListener(gameObject, input_LockMouse);
        lockListener.Response = new();
        lockListener.Response.AddListener(() => { Cursor.lockState = CursorLockMode.Locked; Cursor.visible = false; });

        ChangeTagType(0);
    }

    public void ChangeTagType(int tagIndex)
    {
        //print("CHANGING TAG TYPE");
        taggerSpawner.anomalyType = tags.items[tagIndex];
    }
}