// Decompiled with JetBrains decompiler
// Type: NPCVendingOrderManifest
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

[CreateAssetMenu(menuName = "Rust/NPCVendingOrderManifest")]
public class NPCVendingOrderManifest : ScriptableObject
{
  public NPCVendingOrder[] orderList;

  public int GetIndex(NPCVendingOrder sample)
  {
    for (int index = 0; index < this.orderList.Length; ++index)
    {
      NPCVendingOrder order = this.orderList[index];
      if (Object.op_Equality((Object) sample, (Object) order))
        return index;
    }
    return -1;
  }

  public NPCVendingOrder GetFromIndex(int index)
  {
    return this.orderList[index];
  }

  public NPCVendingOrderManifest()
  {
    base.\u002Ector();
  }
}
