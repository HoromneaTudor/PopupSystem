using System;
using System.Collections.Generic;
using UnityEngine;

namespace tRo.PopupSystem
{
    [CreateAssetMenu(fileName = "PopupReferenceTable", menuName = "tRo/PopupSystem/PopupReferenceTable")]
    public class PopupReferenceTable : ScriptableObject
    {
        [SerializeField] private List<PopupEntry> _popupList;

        public PopupAssetReference GetPopupById(string id)
        {
            return _popupList.Find(x => x.PopupId == id)?.PopupPrefab; 
        }
    }

    [Serializable]
    public class PopupAssetReference : ComponentReference<PopupBase>
    {
        public PopupAssetReference(string guid) : base(guid)
        {
        }
    }

    [System.Serializable]
    public class PopupEntry
    {
        [field:SerializeField] public string PopupId{ get; private set; }
        [field:SerializeField] public PopupAssetReference PopupPrefab{ get; private set; }
    }
}