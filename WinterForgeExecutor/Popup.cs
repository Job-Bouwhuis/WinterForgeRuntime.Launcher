using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace WinterForgeRuntime.Launcher;

public static class Popup
{
    private const string TITLE = "WinterForge Runtime";

    public static void Show(string message)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            ShowWindows(message);
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            ShowLinux(message);
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            ShowMac(message);
        else
            Console.WriteLine($"[{TITLE}] {message}");
    }

    private static void ShowWindows(string message) => MessageBox(IntPtr.Zero, message, TITLE, 0x00000040);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);

    private static void ShowLinux(string message)
    {
        try
        {
            Process.Start("zenity", $"--info --title=\"{TITLE}\" --text=\"{Escape(message)}\"");
        }
        catch
        {
            Console.WriteLine($"[{TITLE}] {message}");
        }
    }

    private static void ShowMac(string message)
    {
        try
        {
            Process.Start("osascript", $"-e \"display dialog \\\"{Escape(message)}\\\" with title \\\"{TITLE}\\\" buttons \\\"OK\\\"\"");
        }
        catch
        {
            Console.WriteLine($"[{TITLE}] {message}");
        }
    }

    private static string Escape(string value)
    {
        return value.Replace("\"", "\\\"");
    }
}
