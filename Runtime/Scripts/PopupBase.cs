using UnityEngine;

namespace tRo.PopupSystem
{
    public abstract class PopupBase : MonoBehaviour
    {
        public PopupData PopupData { get; protected set; }
        public abstract void Show(PopupData popupData);
        public abstract void Close();
    }
}
