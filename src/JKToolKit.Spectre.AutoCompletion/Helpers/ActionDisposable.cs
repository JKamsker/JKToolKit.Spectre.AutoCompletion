using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace JKToolKit.Spectre.AutoCompletion.Helpers;

internal class ActionDisposable : IDisposable
{
    private readonly Action _action;

    public ActionDisposable(Action action)
    {
        _action = action;
    }

    public void Dispose()
    {
        _action();
    }
}