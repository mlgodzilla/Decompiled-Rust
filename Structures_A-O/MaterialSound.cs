// Decompiled with JetBrains decompiler
// Type: MaterialSound
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Rust/MaterialSound")]
public class MaterialSound : ScriptableObject
{
  public SoundDefinition DefaultSound;
  public MaterialSound.Entry[] Entries;

  public MaterialSound()
  {
    base.\u002Ector();
  }

  [Serializable]
  public class Entry
  {
    public PhysicMaterial Material;
    public SoundDefinition Sound;
  }
}
