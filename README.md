# PopupSystem

A lightweight, Addressables-backed popup management system for Unity. Provides async loading, stacking support, and a clean `IPopupService` interface that integrates easily with or without a DI container.

## Requirements

- Unity 6000.0+
- [Addressables](https://docs.unity3d.com/Packages/com.unity.addressables@latest) 2.2.0+
- TextMeshPro (for the included demo popup)

## Installation

Add the package via the Unity Package Manager using the git URL:

```
https://github.com/HoromneaTudor/PopupSystem.git
```

Or add it directly to your project's `Packages/manifest.json`:

```json
{
  "dependencies": {
    "com.tro.popupsystem": "https://github.com/HoromneaTudor/PopupSystem.git"
  }
}
```

## Setup

### 1. Create a PopupReferenceTable

Right-click in your Project window and select **tRo > PopupSystem > PopupReferenceTable** to create the ScriptableObject asset.

For each popup prefab:
- Set a unique **Popup Id** string.
- Assign the prefab via its **Popup Prefab** Addressable reference. The prefab must have a component that extends `PopupBase`.

### 2. Add PopupManager to your scene

Add the `PopupManager` prefab (or component) to a persistent scene object. Assign:
- **Popup Parent** — the `Transform` under which instantiated popups will be parented (typically a Canvas).
- **Popup Reference Table** — the asset created above.

`PopupManager` calls `DontDestroyOnLoad` on itself and exposes two static accessors:

```csharp
PopupManager.PopupService  // IPopupService
PopupManager.PopupLoader   // IPopupLoader
```

> **DI users:** Replace `PopupManager` with your own composition root that constructs `PopupService` and `AddresablesPopupLoader` and binds them to `IPopupService` / `IPopupLoader`.

## Usage

### Show a popup

```csharp
var data = new PopupData(
    id: "my-popup",
    confirmCallback: (id) => Debug.Log($"Confirmed: {id}"),
    description: "Are you sure?"
);

bool success = await PopupManager.PopupService.ShowPopup(data);
```

### Close a popup

```csharp
// Close and keep prefab cached
PopupManager.PopupService.ClosePopup("my-popup");

// Close and release the Addressable asset
PopupManager.PopupService.ClosePopup("my-popup", release: true);

// Close all open popups
PopupManager.PopupService.CloseAllPopups(release: true);
```

### Query state

```csharp
bool isOpen = PopupManager.PopupService.IsPopupOpened("my-popup");
IReadOnlyList<string> openIds = PopupManager.PopupService._openedPopupIds;
```

### React to open/close events

```csharp
PopupManager.PopupService.PopupOpened += id => Debug.Log($"Opened: {id}");
PopupManager.PopupService.PopupClosed += id => Debug.Log($"Closed: {id}");
```

## Creating a Custom Popup

Extend `PopupBase` and implement `Show` and `Close`:

```csharp
using tRo.PopupSystem;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConfirmPopup : PopupBase
{
    [SerializeField] private Button _confirmButton;
    [SerializeField] private Button _cancelButton;
    [SerializeField] private TextMeshProUGUI _label;

    public override void Show(PopupData popupData)
    {
        PopupData = popupData;
        _label.text = popupData.Description;
        _confirmButton.onClick.AddListener(() => popupData.ConfirmCallback?.Invoke(popupData.Id));
        _cancelButton.onClick.AddListener(() => PopupManager.PopupService.ClosePopup(popupData.Id));
    }

    public override void Close()
    {
        Destroy(gameObject);
    }
}
```

Make the prefab Addressable, register it in the `PopupReferenceTable`, and call `ShowPopup` with the matching id.

## Architecture

| Type | Role |
|---|---|
| `IPopupService` / `PopupService` | Opens, closes, and tracks active popups |
| `IPopupLoader` / `AddresablesPopupLoader` | Async loads and caches popup prefabs via Addressables |
| `PopupBase` | Abstract MonoBehaviour base for all popup views |
| `PopupData` | Payload passed to a popup on show (id, description, callback) |
| `PopupReferenceTable` | ScriptableObject mapping string ids to `PopupAssetReference`s |
| `PopupManager` | MonoBehaviour bootstrap; exposes static service/loader accessors |
| `ComponentReference<T>` | `AssetReference` subclass that resolves a component directly from a prefab asset |

## License

MIT License

Copyright (c) 2026 Horomnea Tudor

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
