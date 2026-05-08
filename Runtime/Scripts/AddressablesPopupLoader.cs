using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace tRo.PopupSystem
{
    public class AddressablesPopupLoader : IPopupLoader
    {
        private readonly PopupReferenceTable _popupReferenceTable;
        private readonly ILogger _logger;

        // Handle required for correct release
        private readonly Dictionary<string, AsyncOperationHandle<GameObject>> _handles = new();

        // Cached prefab references
        private readonly Dictionary<string, PopupBase> _prefabs = new();

        // Prevent duplicate loads
        private readonly Dictionary<string, Task<PopupBase>> _loadingTasks = new();

        public AddressablesPopupLoader(PopupReferenceTable popupReferenceTable, ILogger logger)
        {
            _popupReferenceTable = popupReferenceTable;
            _logger = logger;
        }

        /// <summary>
        /// Loads and returns the popup prefab (NOT an instance).
        /// </summary>
        public async Task<PopupBase> LoadPopupById(string id)
        {
            // Already loaded
            if (_prefabs.TryGetValue(id, out var prefab))
                return prefab;

            // Already loading
            if (_loadingTasks.TryGetValue(id, out var existingTask))
                return await existingTask;

            var loadTask = LoadInternal(id);
            _loadingTasks[id] = loadTask;

            try
            {
                var result = await loadTask;

                if (result != null)
                {
                    _prefabs[id] = result;
                }

                return result;
            }
            finally
            {
                _loadingTasks.Remove(id);
            }
        }

        private async Task<PopupBase> LoadInternal(string id)
        {
            var popupRef = _popupReferenceTable.GetPopupById(id);

            if (popupRef == null)
            {
                _logger.Log(LogType.Warning, $"Popup with id: {id} not found");
                return null;
            }

            var handle = popupRef.LoadAssetAsync<GameObject>();

            GameObject loadedGO;

            try
            {
                loadedGO = await handle.Task;
            }
            catch
            {
                _logger.Log(LogType.Error, $"Exception while loading popup with id: {id}");
                return null;
            }

            if (loadedGO == null)
            {
                _logger.Log(LogType.Error, $"Failed to load popup with id: {id}");
                return null;
            }

            var popup = loadedGO.GetComponent<PopupBase>();

            if (popup == null)
            {
                _logger.Log(LogType.Error, $"Popup with id: {id} has no PopupBase component");
                Addressables.Release(handle);
                return null;
            }

            _handles[id] = handle;
            return popup;
        }

        /// <summary>
        /// Releases a specific popup prefab.
        /// </summary>
        public void UnloadPopup(string id)
        {
            if (_handles.TryGetValue(id, out var handle))
            {
                Addressables.Release(handle);
                _handles.Remove(id);
                _prefabs.Remove(id);
            }
        }

        /// <summary>
        /// Releases all loaded popups.
        /// </summary>
        public void UnloadAll()
        {
            foreach (var handle in _handles.Values)
            {
                Addressables.Release(handle);
            }

            _handles.Clear();
            _prefabs.Clear();
            _loadingTasks.Clear();
        }

        /// <summary>
        /// Checks if prefab is loaded.
        /// </summary>
        public bool IsPopupLoaded(string id)
        {
            return _prefabs.ContainsKey(id);
        }
    }
}