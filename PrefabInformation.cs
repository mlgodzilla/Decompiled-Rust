// Decompiled with JetBrains decompiler
// Type: PrefabInformation
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class PrefabInformation : PrefabAttribute
{
  public ItemDefinition associatedItemDefinition;
  public Translate.Phrase title;
  public Translate.Phrase description;
  public Sprite sprite;

  protected override System.Type GetIndexedType()
  {
    return typeof (PrefabInformation);
  }
}
