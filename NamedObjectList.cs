// Decompiled with JetBrains decompiler
// Type: NamedObjectList
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Rust/Named Object List")]
public class NamedObjectList : ScriptableObject
{
  public NamedObjectList.NamedObject[] objects;

  public NamedObjectList()
  {
    base.\u002Ector();
  }

  [Serializable]
  public struct NamedObject
  {
    public string name;
    public Object obj;
  }
}
