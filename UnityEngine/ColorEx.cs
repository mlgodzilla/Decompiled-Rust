// Decompiled with JetBrains decompiler
// Type: UnityEngine.ColorEx
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

namespace UnityEngine
{
  public static class ColorEx
  {
    public static string ToHex(this Color32 color)
    {
      return color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
    }

    public static Color Parse(string str)
    {
      string[] strArray = str.Split(' ');
      if (strArray.Length == 3)
        return new Color(float.Parse(strArray[0]), float.Parse(strArray[1]), float.Parse(strArray[2]));
      if (strArray.Length == 4)
        return new Color(float.Parse(strArray[0]), float.Parse(strArray[1]), float.Parse(strArray[2]), float.Parse(strArray[3]));
      return Color.get_white();
    }
  }
}
