// Decompiled with JetBrains decompiler
// Type: MaterialPropertyDesc
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public struct MaterialPropertyDesc
{
  public int nameID;
  public System.Type type;

  public MaterialPropertyDesc(string name, System.Type type)
  {
    this.nameID = Shader.PropertyToID(name);
    this.type = type;
  }
}
