using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace tRo.PopupSystem
{
    public class PopupService : IPopupService
    {
        private readonly Transform _popupParent;
        private readonly IPopupLoader _popupLoader;

        private Dictionary<string, PopupBase> _openedPopups = new();
        public IReadOnlyList<string> _openedPopupIds => _openedPopups.Keys.ToList();

        public event Action<string> PopupClosed;
        public event Action<string> PopupOpened;

        public PopupService(IPopupLoader popupLoader, Transform parentTransform)
        {
            _popupLoader = popupLoader;
            _popupParent = parentTransform;
        }

        public void CloseAllPopups(bool release = false)
        {
            foreach (var (key, value) in _openedPopups)
            {
                value.Close();
                PopupClosed?.Invoke(key);
            }

            _openedPopups.Clear();

            if (release)
            {
                _popupLoader.UnloadAll();
            }
        }

        public void ClosePopup(string id, bool release = false)
        {
            if (!_openedPopups.TryGetValue(id, out var value))
            {
                return;
            }

            value.Close();
            PopupClosed?.Invoke(id);
            _openedPopups.Remove(id);

            if (release)
            {
                _popupLoader.UnloadPopup(id);
            }
        }

        public bool IsPopupOpened(string id)
        {
            return _openedPopups.ContainsKey(id);
        }

        public async Task<bool> ShowPopup(PopupData popupData)
        {
            var popup = await _popupLoader.LoadPopupById(popupData.Id);

            if (popup == null)
            {
                return false;
            }

            var instance = UnityEngine.Object.Instantiate(popup, _popupParent);
            instance.Show(popupData);
            _openedPopups[popupData.Id] = instance;

            PopupOpened?.Invoke(popupData.Id);

            return true;
        }
    }
}
