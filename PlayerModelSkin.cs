// Decompiled with JetBrains decompiler
// Type: PlayerModelSkin
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class PlayerModelSkin : MonoBehaviour
{
  public void Setup(SkinSetCollection skin, float materialNum, float meshNum)
  {
    SkinSet skinSet = skin.Get(meshNum);
    if (Object.op_Equality((Object) skinSet, (Object) null))
      Debug.LogError((object) "Skin.Get returned a NULL skin");
    skinSet.Process(((Component) this).get_gameObject(), materialNum);
  }

  public PlayerModelSkin()
  {
    base.\u002Ector();
  }
}
