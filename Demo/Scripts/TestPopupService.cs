using tRo.PopupSystem;
using UnityEngine;

public class TestPopupService : MonoBehaviour
{
    [SerializeField] private string _popupId = "";
    [SerializeField] private bool _releaseAfterClose = false;
    [SerializeField] private bool _showOnStart;

    void Start()
    {
        if (_showOnStart)
        {
            ShowPopup();
        }
    }

    [ContextMenu("ShowPopup")]
    private void ShowPopup()
    {
        PopupManager.PopupService.ShowPopup(new PopupData(_popupId, OnConfirmClicked, "test"));
    }

    private void OnConfirmClicked(string id)
    {
        UnityEngine.Debug.Log("Confirm button pressed");
        PopupManager.PopupService.ClosePopup(id, _releaseAfterClose);
    }
}
