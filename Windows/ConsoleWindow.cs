// Decompiled with JetBrains decompiler
// Type: Windows.ConsoleWindow
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Microsoft.Win32.SafeHandles;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using UnityEngine;

namespace Windows
{
  [SuppressUnmanagedCodeSecurity]
  public class ConsoleWindow
  {
    private TextWriter oldOutput;
    private const int STD_INPUT_HANDLE = -10;
    private const int STD_OUTPUT_HANDLE = -11;

    public void Initialize()
    {
      ConsoleWindow.FreeConsole();
      if (!ConsoleWindow.AttachConsole(uint.MaxValue))
        ConsoleWindow.AllocConsole();
      this.oldOutput = Console.Out;
      try
      {
        Console.OutputEncoding = Encoding.UTF8;
        Console.SetOut((TextWriter) new StreamWriter((Stream) new FileStream(new SafeFileHandle(ConsoleWindow.GetStdHandle(-11), true), FileAccess.Write), Encoding.UTF8)
        {
          AutoFlush = true
        });
      }
      catch (Exception ex)
      {
        Debug.Log((object) ("Couldn't redirect output: " + ex.Message));
      }
    }

    public void Shutdown()
    {
      Console.SetOut(this.oldOutput);
      ConsoleWindow.FreeConsole();
    }

    public void SetTitle(string strName)
    {
      ConsoleWindow.SetConsoleTitleA(strName);
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool AttachConsole(uint dwProcessId);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool AllocConsole();

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool FreeConsole();

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    private static extern IntPtr GetStdHandle(int nStdHandle);

    [DllImport("kernel32.dll")]
    private static extern bool SetConsoleTitleA(string lpConsoleTitle);
  }
}
