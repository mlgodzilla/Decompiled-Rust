// Decompiled with JetBrains decompiler
// Type: GameAnalyticsSDK.Setup.Studio
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System.Collections.Generic;

namespace GameAnalyticsSDK.Setup
{
  public class Studio
  {
    public string Name { get; private set; }

    public string ID { get; private set; }

    public List<Game> Games { get; private set; }

    public Studio(string name, string id, List<Game> games)
    {
      this.Name = name;
      this.ID = id;
      this.Games = games;
    }

    public static string[] GetStudioNames(List<Studio> studios, bool addFirstEmpty = true)
    {
      if (studios == null)
        return new string[1]{ "-" };
      if (addFirstEmpty)
      {
        string[] strArray = new string[studios.Count + 1];
        strArray[0] = "-";
        string str = "";
        for (int index = 0; index < studios.Count; ++index)
        {
          strArray[index + 1] = studios[index].Name + str;
          str += " ";
        }
        return strArray;
      }
      string[] strArray1 = new string[studios.Count];
      string str1 = "";
      for (int index = 0; index < studios.Count; ++index)
      {
        strArray1[index] = studios[index].Name + str1;
        str1 += " ";
      }
      return strArray1;
    }

    public static string[] GetGameNames(int index, List<Studio> studios)
    {
      if (studios == null || studios[index].Games == null)
        return new string[1]{ "-" };
      string[] strArray = new string[studios[index].Games.Count + 1];
      strArray[0] = "-";
      string str = "";
      for (int index1 = 0; index1 < studios[index].Games.Count; ++index1)
      {
        strArray[index1 + 1] = studios[index].Games[index1].Name + str;
        str += " ";
      }
      return strArray;
    }
  }
}
