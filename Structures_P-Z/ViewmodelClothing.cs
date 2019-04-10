// Decompiled with JetBrains decompiler
// Type: ViewmodelClothing
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using Rust.Workshop;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ViewmodelClothing : MonoBehaviour
{
  public SkeletonSkin[] SkeletonSkins;

  internal void CopyToSkeleton(Skeleton skeleton, GameObject parent, Item item)
  {
    foreach (SkeletonSkin skeletonSkin in this.SkeletonSkins)
    {
      GameObject gameObject1 = new GameObject();
      gameObject1.get_transform().set_parent(parent.get_transform());
      GameObject gameObject2 = gameObject1;
      Skeleton skeleton1 = skeleton;
      skeletonSkin.DuplicateAndRetarget(gameObject2, skeleton1).set_updateWhenOffscreen(true);
      if (item != null && item.skin > 0UL)
      {
        ItemSkinDirectory.Skin skin = ((IEnumerable<ItemSkinDirectory.Skin>) item.info.skins).FirstOrDefault<ItemSkinDirectory.Skin>((Func<ItemSkinDirectory.Skin, bool>) (x => (long) x.id == (long) item.skin));
        if (skin.id == 0 && item.skin > 0UL)
        {
          WorkshopSkin.Apply(gameObject1, item.skin, (Action) null);
          break;
        }
        if ((long) skin.id != (long) item.skin)
          break;
        ItemSkin invItem = skin.invItem as ItemSkin;
        if (Object.op_Equality((Object) invItem, (Object) null))
          break;
        invItem.ApplySkin(gameObject1);
      }
    }
  }

  public ViewmodelClothing()
  {
    base.\u002Ector();
  }
}
