// Decompiled with JetBrains decompiler
// Type: LevelInfo
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class LevelInfo : SingletonComponent<LevelInfo>
{
  public string shortName;
  public string displayName;
  [TextArea]
  public string description;
  [Tooltip("A background image to be shown when loading the map")]
  public Texture2D image;
  [Space(10f)]
  [Tooltip("You should incrememnt this version when you make changes to the map that will invalidate old saves")]
  public int version;

  public LevelInfo()
  {
    base.\u002Ector();
  }
}
