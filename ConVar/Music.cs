// Decompiled with JetBrains decompiler
// Type: ConVar.Music
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System.Text;
using UnityEngine;

namespace ConVar
{
  [ConsoleSystem.Factory("music")]
  public class Music : ConsoleSystem
  {
    [ClientVar]
    public static bool enabled = true;
    [ClientVar]
    public static int songGapMin = 240;
    [ClientVar]
    public static int songGapMax = 480;

    [ClientVar]
    public static void info(ConsoleSystem.Arg arg)
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (Object.op_Equality((Object) SingletonComponent<MusicManager>.Instance, (Object) null))
      {
        stringBuilder.Append("No music manager was found");
      }
      else
      {
        stringBuilder.Append("Current music info: ");
        stringBuilder.AppendLine();
        stringBuilder.Append("  theme: " + (object) ((MusicManager) SingletonComponent<MusicManager>.Instance).currentTheme);
        stringBuilder.AppendLine();
        stringBuilder.Append("  intensity: " + (object) ((MusicManager) SingletonComponent<MusicManager>.Instance).intensity);
        stringBuilder.AppendLine();
        stringBuilder.Append("  next music: " + (object) ((MusicManager) SingletonComponent<MusicManager>.Instance).nextMusic);
        stringBuilder.AppendLine();
        stringBuilder.Append("  current time: " + (object) Time.get_time());
        stringBuilder.AppendLine();
      }
      arg.ReplyWith(stringBuilder.ToString());
    }

    public Music()
    {
      base.\u002Ector();
    }
  }
}
