// Decompiled with JetBrains decompiler
// Type: GameContentList
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class GameContentList : MonoBehaviour
{
  public GameContentList.ResourceType resourceType;
  public List<Object> foundObjects;

  public GameContentList()
  {
    base.\u002Ector();
  }

  public enum ResourceType
  {
    Audio,
    Textures,
    Models,
  }
}
