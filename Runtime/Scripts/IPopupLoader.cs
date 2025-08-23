using System.Threading.Tasks;

namespace tRo.PopupSystem
{
    public interface IPopupLoader
    {
        public bool IsPopupLoaded(string id);
        public Task<PopupBase> LoadPopupById(string id);
        public void UnloadPopup(string id);
        public void UnloadAll();
    }
}
