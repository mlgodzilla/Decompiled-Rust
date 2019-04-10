// Decompiled with JetBrains decompiler
// Type: ConVar.FileConVar
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

namespace ConVar
{
  [ConsoleSystem.Factory("file")]
  public class FileConVar : ConsoleSystem
  {
    [ClientVar]
    public static bool debug
    {
      get
      {
        return FileSystem.LogDebug;
      }
      set
      {
        FileSystem.LogDebug = value;
      }
    }

    [ClientVar]
    public static bool time
    {
      get
      {
        return FileSystem.LogTime;
      }
      set
      {
        FileSystem.LogTime = value;
      }
    }

    public FileConVar()
    {
      base.\u002Ector();
    }
  }
}
