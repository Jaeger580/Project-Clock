using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UI_CamButtonHandler : MonoBehaviour
{
    public static UI_CamButtonHandler Instance;

    [SerializeField] private UIDocument uidoc;
    public UIDocument UIDoc => uidoc;
    [SerializeField] private VisualTreeAsset camListButtonPrefab;
    [SerializeField] private GameEvent goToCamEvent;

    private void Start()
    {
        if (Instance != null)
        {
            Debug.Log("Instance of UI found; killing myself.", this);
            Destroy(gameObject);
        }
        Instance = this;

        var listener = GameEventListener.AddGeneralListener(gameObject, goToCamEvent);
        listener.IntResponse = new();
        listener.IntResponse.AddListener((index) => PopulateList(index));
    }

    private void PopulateList(int index)
    {
        var root = uidoc.rootVisualElement;
        var camList = root.Q<VisualElement>("CamsList");
        camList.Clear();
        //Dictionary<int, Button> btnIndexes = new();
        foreach (var man in AnomalyCentralController.Instance.Managers)
        {
            var template = camListButtonPrefab.CloneTree();
            var btn = template.Q<Button>();
            btn.clicked += () => CameraManager.instance.SelectCam(man.CamIndex);
            btn.text = man.HumanReadableName();
            camList.Add(btn);
            //if (!btnIndexes.TryAdd(man.CamIndex, btn))
            //    Debug.LogWarning("ERROR: A cam with that index was already found.");
            if (man.CamIndex == index) btn.AddToClassList("camListButtonSelected");
            else
            {
                btn.RemoveFromClassList("camListButtonSelected");
                btn.Blur();
            }
        }
    }
}