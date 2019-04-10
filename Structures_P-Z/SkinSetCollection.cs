// Decompiled with JetBrains decompiler
// Type: SkinSetCollection
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

[CreateAssetMenu(menuName = "Rust/Skin Set Collection")]
public class SkinSetCollection : ScriptableObject
{
  public SkinSet[] Skins;

  public int GetIndex(float MeshNumber)
  {
    return Mathf.Clamp(Mathf.FloorToInt(MeshNumber * (float) this.Skins.Length), 0, this.Skins.Length - 1);
  }

  public SkinSet Get(float MeshNumber)
  {
    return this.Skins[this.GetIndex(MeshNumber)];
  }

  public SkinSetCollection()
  {
    base.\u002Ector();
  }
}
