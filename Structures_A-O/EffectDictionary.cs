// Decompiled with JetBrains decompiler
// Type: EffectDictionary
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Rust;
using System.Collections.Generic;
using UnityEngine;

public class EffectDictionary
{
  private static Dictionary<string, string[]> effectDictionary;

  public static string GetParticle(string impactType, string materialName)
  {
    return EffectDictionary.LookupEffect("impacts", impactType, materialName);
  }

  public static string GetParticle(DamageType damageType, string materialName)
  {
    switch (damageType)
    {
      case DamageType.Bullet:
        return EffectDictionary.GetParticle("bullet", materialName);
      case DamageType.Slash:
        return EffectDictionary.GetParticle("slash", materialName);
      case DamageType.Blunt:
        return EffectDictionary.GetParticle("blunt", materialName);
      case DamageType.Stab:
        return EffectDictionary.GetParticle("stab", materialName);
      case DamageType.Arrow:
        return EffectDictionary.GetParticle("bullet", materialName);
      default:
        return EffectDictionary.GetParticle("blunt", materialName);
    }
  }

  public static string GetDecal(string impactType, string materialName)
  {
    return EffectDictionary.LookupEffect("decals", impactType, materialName);
  }

  public static string GetDecal(DamageType damageType, string materialName)
  {
    switch (damageType)
    {
      case DamageType.Bullet:
        return EffectDictionary.GetDecal("bullet", materialName);
      case DamageType.Slash:
        return EffectDictionary.GetDecal("slash", materialName);
      case DamageType.Blunt:
        return EffectDictionary.GetDecal("blunt", materialName);
      case DamageType.Stab:
        return EffectDictionary.GetDecal("stab", materialName);
      case DamageType.Arrow:
        return EffectDictionary.GetDecal("bullet", materialName);
      default:
        return EffectDictionary.GetDecal("blunt", materialName);
    }
  }

  public static string GetDisplacement(string impactType, string materialName)
  {
    return EffectDictionary.LookupEffect("displacement", impactType, materialName);
  }

  private static string LookupEffect(string category, string effect, string material)
  {
    if (EffectDictionary.effectDictionary == null)
      EffectDictionary.effectDictionary = GameManifest.LoadEffectDictionary();
    string format = "assets/bundled/prefabs/fx/{0}/{1}/{2}";
    string[] strArray;
    if (!EffectDictionary.effectDictionary.TryGetValue(StringFormatCache.Get(format, category, effect, material), out strArray) && !EffectDictionary.effectDictionary.TryGetValue(StringFormatCache.Get(format, category, effect, "generic"), out strArray))
      return string.Empty;
    return strArray[Random.Range(0, strArray.Length)];
  }
}
