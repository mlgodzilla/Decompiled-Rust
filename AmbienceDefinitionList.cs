// Decompiled with JetBrains decompiler
// Type: AmbienceDefinitionList
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Rust/Ambience Definition List")]
public class AmbienceDefinitionList : ScriptableObject
{
  public List<AmbienceDefinition> defs;

  public AmbienceDefinitionList()
  {
    base.\u002Ector();
  }
}
