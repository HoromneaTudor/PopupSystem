using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace tRo.PopupSystem
{
    public class AcknolagePopup : PopupBase
    {
        [SerializeField] private Button _button;
        [SerializeField] private TextMeshProUGUI _text; 

        public override void Close()
        {
            Destroy(this.gameObject);
        }

        public override void Show(PopupData popupData)
        {
            PopupData = popupData;
            _button.onClick.AddListener(() => popupData.ConfirmCallback?.Invoke(popupData.Id));
            _text.text = popupData.Description;
        }
    }
}
