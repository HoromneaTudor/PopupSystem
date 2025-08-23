using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace tRo.PopupSystem
{
    public class AddresablesPopupLoader : IPopupLoader
    {
        private readonly PopupReferenceTable _popupReferenceTable;
        private readonly ILogger _logger;

        private Dictionary<string, PopupBase> _loadedPrefabs = new();
        private Dictionary<string, Task<PopupBase>> _loadingPrefabTasks = new();

        public AddresablesPopupLoader(PopupReferenceTable popupReferenceTable, ILogger logger)
        {
            _popupReferenceTable = popupReferenceTable;
            _logger = logger;
        }

        public async Task<PopupBase> LoadPopupById(string id)
        {
            if (_loadedPrefabs.TryGetValue(id, out var loadedPrefab))
            {
                return loadedPrefab;
            }

            if (_loadingPrefabTasks.TryGetValue(id, out var loadingPrefabTask))
            {
                return await loadingPrefabTask;
            }

            var popupRef = _popupReferenceTable.GetPopupById(id);

            if (popupRef == null)
            {
                _logger.Log(LogType.Warning, $"Popup with id: {id} not found");
                return null;
            }

            //workaround since LoadAssetAsync can only load types that can exist as standalone assets in Unity.
            //like GameObjects, SO, material, etc. Anything that is stored inside files basically
            var loadGameObjectTask = popupRef.LoadAssetAsync<GameObject>().Task;
            var loadPopupBaseTask = WaitForPopupBaseResponse(loadGameObjectTask);
            _loadingPrefabTasks[id] = loadPopupBaseTask;

            var popupPrefab = await loadPopupBaseTask;

            if (popupPrefab != null)
            {
                _loadedPrefabs[id] = popupPrefab;
            }

            _loadingPrefabTasks.Remove(id);

            return popupPrefab;
        }

        private async Task<PopupBase> WaitForPopupBaseResponse(Task<GameObject> addresableTask)
        {
            var response = await addresableTask;
            var popupBaseReturn = response.GetComponent<PopupBase>();
            return popupBaseReturn;
        }

        public void UnloadPopup(string id)
        {
            if (_loadedPrefabs.TryGetValue(id, out var loadedPrefab))
            {
                Addressables.Release(loadedPrefab);
                _loadedPrefabs.Remove(id);
            }
        }

        public void UnloadAll()
        {
            foreach (var item in _loadedPrefabs)
            {
                Addressables.Release(item.Value);
            }

            _loadedPrefabs.Clear();
        }

        public bool IsPopupLoaded(string id)
        {
            return _loadedPrefabs.ContainsKey(id);
        }
    }
}
