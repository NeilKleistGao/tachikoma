using System;
using System.Runtime.InteropServices;

namespace Tachikoma {
  internal static class WindowManager {
    [DllImport("user32.dll", SetLastError = true)]
    private static extern uint GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", SetLastError = true)]
    private static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool SetLayeredWindowAttributes(IntPtr hWnd, uint crKey, byte bAlpha, uint dwFlags);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool UpdateWindow(IntPtr hWnd);

    private const int GWL_EXSTYLE = -20;

    private const uint WS_EX_LAYERED = 0x00080000;
    private const uint WS_EX_APPWINDOW = 0x00040000;
    private const uint WS_EX_TOOLWINDOW = 0x00000080;

    private const uint LWA_COLORKEY = 0x01;

    private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);

    private const uint SWP_FRAMECHANGED = 0x0020;
    private const uint SWP_NOMOVE = 0x0002;
    private const uint SWP_NOSIZE = 0x0001;
    private const uint SWP_NOACTIVATE = 0x0010;
    private const uint SWP_SHOWWINDOW = 0x0040;
    private const uint SWP_NOZORDER = 0x0004;

    private static void RefreshWindow(IntPtr hWnd) {
      SetWindowPos(hWnd, IntPtr.Zero, 0, 0, 0, 0, SWP_FRAMECHANGED | SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE | SWP_SHOWWINDOW | SWP_NOZORDER);
      UpdateWindow(hWnd);
    }

    public static void SetColorKeyTransparent(IntPtr hWnd, uint colorKey) {
      uint exStyle = GetWindowLong(hWnd, GWL_EXSTYLE);
      SetWindowLongPtr(hWnd, GWL_EXSTYLE, new IntPtr(exStyle | WS_EX_LAYERED));
      SetLayeredWindowAttributes(hWnd, colorKey, 0, LWA_COLORKEY);
      RefreshWindow(hWnd);
    }

    public static void SetTopMost(IntPtr hWnd) {
      SetWindowPos(hWnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE | SWP_SHOWWINDOW);
    }

    public static void SetToolWindow(IntPtr hWnd) {
      uint exStyle = GetWindowLong(hWnd, GWL_EXSTYLE);
      SetWindowLongPtr(hWnd, GWL_EXSTYLE, new IntPtr((exStyle & ~WS_EX_APPWINDOW) | WS_EX_TOOLWINDOW));
      RefreshWindow(hWnd);
    }
  }
}
