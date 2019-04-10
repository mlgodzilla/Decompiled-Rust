// Decompiled with JetBrains decompiler
// Type: Windows.ConsoleInput
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

namespace Windows
{
  public class ConsoleInput
  {
    public string inputString = "";
    public string[] statusText = new string[3]{ "", "", "" };
    internal float nextUpdate;

    public event Action<string> OnInputText;

    public bool valid
    {
      get
      {
        return Console.BufferWidth > 0;
      }
    }

    public int lineWidth
    {
      get
      {
        return Console.BufferWidth;
      }
    }

    public void ClearLine(int numLines)
    {
      Console.CursorLeft = 0;
      Console.Write(new string(' ', this.lineWidth * numLines));
      Console.CursorTop -= numLines;
      Console.CursorLeft = 0;
    }

    public void RedrawInputLine()
    {
      try
      {
        Console.ForegroundColor = ConsoleColor.White;
        ++Console.CursorTop;
        for (int index = 0; index < this.statusText.Length; ++index)
        {
          Console.CursorLeft = 0;
          Console.Write(this.statusText[index].PadRight(this.lineWidth));
        }
        Console.CursorTop -= this.statusText.Length + 1;
        Console.CursorLeft = 0;
        Console.BackgroundColor = ConsoleColor.Black;
        Console.ForegroundColor = ConsoleColor.Green;
        this.ClearLine(1);
        if (this.inputString.Length == 0)
          return;
        if (this.inputString.Length < this.lineWidth - 2)
          Console.Write(this.inputString);
        else
          Console.Write(this.inputString.Substring(this.inputString.Length - (this.lineWidth - 2)));
      }
      catch (Exception ex)
      {
      }
    }

    internal void OnBackspace()
    {
      if (this.inputString.Length < 1)
        return;
      this.inputString = this.inputString.Substring(0, this.inputString.Length - 1);
      this.RedrawInputLine();
    }

    internal void OnEscape()
    {
      this.inputString = "";
      this.RedrawInputLine();
    }

    internal void OnEnter()
    {
      this.ClearLine(this.statusText.Length);
      Console.ForegroundColor = ConsoleColor.Green;
      Console.WriteLine("> " + this.inputString);
      string inputString = this.inputString;
      this.inputString = "";
      if (this.OnInputText != null)
        this.OnInputText(inputString);
      this.RedrawInputLine();
    }

    public void Update()
    {
      if (!this.valid)
        return;
      if ((double) this.nextUpdate < (double) Time.get_realtimeSinceStartup())
      {
        this.RedrawInputLine();
        this.nextUpdate = Time.get_realtimeSinceStartup() + 0.5f;
      }
      try
      {
        if (!Console.KeyAvailable)
          return;
      }
      catch (Exception ex)
      {
        return;
      }
      ConsoleKeyInfo consoleKeyInfo = Console.ReadKey();
      if (consoleKeyInfo.Key == ConsoleKey.Enter)
        this.OnEnter();
      else if (consoleKeyInfo.Key == ConsoleKey.Backspace)
        this.OnBackspace();
      else if (consoleKeyInfo.Key == ConsoleKey.Escape)
      {
        this.OnEscape();
      }
      else
      {
        if (consoleKeyInfo.KeyChar == char.MinValue)
          return;
        this.inputString += consoleKeyInfo.KeyChar.ToString();
        this.RedrawInputLine();
      }
    }
  }
}
