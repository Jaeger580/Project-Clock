using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UI_CamButtonHandler : MonoBehaviour
{
    [SerializeField] private UIDocument uidoc;
    [SerializeField] private VisualTreeAsset camListButtonPrefab;
    private List<Button> camButtons = new();
    [SerializeField] private GameEvent goToCamEvent;

    private void Start()
    {
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
            camList.Add(btn);
            //if (!btnIndexes.TryAdd(man.CamIndex, btn))
            //    Debug.LogWarning("ERROR: A cam with that index was already found.");
            if (man.CamIndex == index) btn.AddToClassList("camListButtonSelected");
            else btn.RemoveFromClassList("camListButtonSelected");
        }
    }
}
