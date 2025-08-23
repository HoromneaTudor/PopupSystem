using System;

namespace tRo.PopupSystem
{
    public class PopupData
    {
        public string Id { get; }
        public Action<string> ConfirmCallback { get; }
        public string Description { get; }

        public PopupData(string id, Action<string> confirmCallback, string description)
        {
            Id = id;
            ConfirmCallback = confirmCallback;
            Description = description;
        }
    }
}
