using UnityEngine;

namespace tRo.PopupSystem
{
    //Only so every script has acces to the popupService and loader, if you have a IOC implementation like zenject or strange, 
    //you are recomanded to replace this
    public class PopupManager : MonoBehaviour
    {
        [SerializeField] private Transform _popupParent;
        [SerializeField] private PopupReferenceTable _popupReferenceTable;

        public static IPopupService PopupService { get; private set; }
        public static IPopupLoader PopupLoader { get; private set; }

        private void Awake()
        {
            Initialize(_popupReferenceTable);
            DontDestroyOnLoad(this.gameObject);
        }

        private void Initialize(PopupReferenceTable popupReferenceTable)
        {
            PopupLoader = new AddresablesPopupLoader(popupReferenceTable, UnityEngine.Debug.unityLogger);
            PopupService = new PopupService(PopupLoader, _popupParent);
        }
    }
}
