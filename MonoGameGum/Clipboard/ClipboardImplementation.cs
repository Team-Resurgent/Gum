using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if !IOS && !XBOX
using TextCopy;
#endif

namespace Gum.Clipboard;

public class ClipboardImplementation
{
#if !IOS && !XBOX
    public static IClipboard? InjectedClipboard { get; set; }
    private static Task<string?>? _injectedClipboardTask;
#endif

    public static string GetText(Action? callback)
    {
#if IOS || XBOX
        return string.Empty;
#else
        if (InjectedClipboard != null)
        {
            if (_injectedClipboardTask == null)
            {
                _injectedClipboardTask = Task.Run(async () => await InjectedClipboard.GetTextAsync());
                _injectedClipboardTask.ContinueWith((t) => callback?.Invoke());
            }
            else
            {
                if (_injectedClipboardTask.IsCompleted)
                {
                    string? result = _injectedClipboardTask.Result;
                    _injectedClipboardTask = null;
                    return result;
                }
            }

            return null;
        }

        return ClipboardService.GetText();
#endif
    }

    public static void PushStringToClipboard(string text)
    {
#if IOS || XBOX
        // todo - do we care to fix this?
#else
        if (InjectedClipboard != null)
        {
            InjectedClipboard.SetTextAsync(text);
            return;
        }

        ClipboardService.SetText(text);
#endif

    }
}
