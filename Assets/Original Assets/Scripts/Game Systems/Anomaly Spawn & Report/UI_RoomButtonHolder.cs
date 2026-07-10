using UnityEngine;
using UnityEngine.UIElements;

public class UI_RoomButtonHolder : MonoBehaviour
{
    [SerializeField] private AnomalyRoomManager roomManager;
    [SerializeField] private string roomCamElementName;
    [SerializeField] private GameEvent goToCamEvent;
    private EventCallback<ClickEvent> roomBtnClicked;

    private void Start()
    {
        var listener = GameEventListener.AddGeneralListener(gameObject, goToCamEvent);
        listener.IntResponse = new();
        listener.IntResponse.AddListener((index) => UpdateRoomButton(index));
    }

    private void UpdateRoomButton(int index)
    {
        var root = UI_CamButtonHandler.Instance?.UIDoc.rootVisualElement;
        var map = root.Q<VisualElement>("Map");
        var roomBtn = map.Q<Button>(roomCamElementName);

        if (index == roomManager.CamIndex) roomBtn.AddToClassList("camRoomFillSelected");
        else
        {
            roomBtn.RemoveFromClassList("camRoomFillSelected");
            roomBtn.Blur();
        }

        roomBtnClicked = (evt) => CameraManager.instance.SelectCam(roomManager.CamIndex);

        //roomBtn.UnregisterCallback(roomBtnClicked);
        roomBtn.RegisterCallback(roomBtnClicked); 
    }
}
