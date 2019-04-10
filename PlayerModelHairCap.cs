// Decompiled with JetBrains decompiler
// Type: PlayerModelHairCap
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

[RequireComponent(typeof (PlayerModelSkin))]
public class PlayerModelHairCap : MonoBehaviour
{
  [InspectorFlags]
  public HairCapMask hairCapMask;

  public void SetupHairCap(
    SkinSetCollection skin,
    float hairNum,
    float meshNum,
    MaterialPropertyBlock block)
  {
    int index = skin.GetIndex(meshNum);
    SkinSet skin1 = skin.Skins[index];
    if (!Object.op_Inequality((Object) skin1, (Object) null))
      return;
    for (int typeIndex = 0; typeIndex < 5; ++typeIndex)
    {
      if ((this.hairCapMask & (HairCapMask) (1 << typeIndex)) != (HairCapMask) 0)
      {
        float typeNum;
        float dyeNum;
        PlayerModelHair.GetRandomVariation(hairNum, typeIndex, index, out typeNum, out dyeNum);
        HairType hairType = (HairType) typeIndex;
        HairSetCollection.HairSetEntry hairSetEntry = skin1.HairCollection.Get(hairType, typeNum);
        if (Object.op_Inequality((Object) hairSetEntry.HairSet, (Object) null))
        {
          HairDyeCollection hairDyeCollection = hairSetEntry.HairDyeCollection;
          if (Object.op_Inequality((Object) hairDyeCollection, (Object) null))
            hairDyeCollection.Get(dyeNum)?.ApplyCap(hairDyeCollection, hairType, block);
        }
      }
    }
  }

  public PlayerModelHairCap()
  {
    base.\u002Ector();
  }
}
