// Decompiled with JetBrains decompiler
// Type: ConVar.Weather
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

namespace ConVar
{
  [ConsoleSystem.Factory("weather")]
  public class Weather : ConsoleSystem
  {
    [ServerVar]
    public static void clouds(ConsoleSystem.Arg args)
    {
      if (Object.op_Equality((Object) SingletonComponent<Climate>.Instance, (Object) null))
        return;
      float clouds = ((Climate) SingletonComponent<Climate>.Instance).Overrides.Clouds;
      float num = args.GetFloat(0, -1f);
      string str1 = (double) clouds < 0.0 ? "automatic" : Mathf.RoundToInt(100f * clouds).ToString() + "%";
      string str2 = (double) num < 0.0 ? "automatic" : Mathf.RoundToInt(100f * num).ToString() + "%";
      args.ReplyWith("Clouds: " + str2 + " (was " + str1 + ")");
      ((Climate) SingletonComponent<Climate>.Instance).Overrides.Clouds = num;
    }

    [ServerVar]
    public static void fog(ConsoleSystem.Arg args)
    {
      if (Object.op_Equality((Object) SingletonComponent<Climate>.Instance, (Object) null))
        return;
      float fog = ((Climate) SingletonComponent<Climate>.Instance).Overrides.Fog;
      float num = args.GetFloat(0, -1f);
      string str1 = (double) fog < 0.0 ? "automatic" : Mathf.RoundToInt(100f * fog).ToString() + "%";
      string str2 = (double) num < 0.0 ? "automatic" : Mathf.RoundToInt(100f * num).ToString() + "%";
      args.ReplyWith("Fog: " + str2 + " (was " + str1 + ")");
      ((Climate) SingletonComponent<Climate>.Instance).Overrides.Fog = num;
    }

    [ServerVar]
    public static void wind(ConsoleSystem.Arg args)
    {
      if (Object.op_Equality((Object) SingletonComponent<Climate>.Instance, (Object) null))
        return;
      float wind = ((Climate) SingletonComponent<Climate>.Instance).Overrides.Wind;
      float num = args.GetFloat(0, -1f);
      string str1 = (double) wind < 0.0 ? "automatic" : Mathf.RoundToInt(100f * wind).ToString() + "%";
      string str2 = (double) num < 0.0 ? "automatic" : Mathf.RoundToInt(100f * num).ToString() + "%";
      args.ReplyWith("Wind: " + str2 + " (was " + str1 + ")");
      ((Climate) SingletonComponent<Climate>.Instance).Overrides.Wind = num;
    }

    [ServerVar]
    public static void rain(ConsoleSystem.Arg args)
    {
      if (Object.op_Equality((Object) SingletonComponent<Climate>.Instance, (Object) null))
        return;
      float rain = ((Climate) SingletonComponent<Climate>.Instance).Overrides.Rain;
      float num = args.GetFloat(0, -1f);
      string str1 = (double) rain < 0.0 ? "automatic" : Mathf.RoundToInt(100f * rain).ToString() + "%";
      string str2 = (double) num < 0.0 ? "automatic" : Mathf.RoundToInt(100f * num).ToString() + "%";
      args.ReplyWith("Rain: " + str2 + " (was " + str1 + ")");
      ((Climate) SingletonComponent<Climate>.Instance).Overrides.Rain = num;
    }

    public Weather()
    {
      base.\u002Ector();
    }
  }
}
