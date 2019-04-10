// Decompiled with JetBrains decompiler
// Type: ConVar.Voice
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

namespace ConVar
{
  [ConsoleSystem.Factory("voice")]
  public class Voice : ConsoleSystem
  {
    [ClientVar(Saved = true)]
    public static bool loopback = false;
    [ClientVar]
    public static float ui_scale = 1f;
    [ClientVar]
    public static float ui_cut = 0.0f;
    [ClientVar]
    public static int ui_samples = 20;
    [ClientVar]
    public static float ui_lerp = 0.2f;

    public Voice()
    {
      base.\u002Ector();
    }
  }
}
