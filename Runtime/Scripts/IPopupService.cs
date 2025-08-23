using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace tRo.PopupSystem
{
    public interface IPopupService
    {
        IReadOnlyList<string> _openedPopupIds { get; }

        event Action<string> PopupClosed;
        event Action<string> PopupOpened;

        Task<bool> ShowPopup(PopupData popupData);
        void ClosePopup(string id, bool release=false);
        void CloseAllPopups(bool release=false);
        bool IsPopupOpened(string id);
    }
}
