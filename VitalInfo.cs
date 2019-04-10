// Decompiled with JetBrains decompiler
// Type: VitalInfo
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

public class VitalInfo : MonoBehaviour, IClientComponent
{
  public VitalInfo.Vital VitalType;
  public Animator animator;
  public Text text;

  public VitalInfo()
  {
    base.\u002Ector();
  }

  public enum Vital
  {
    BuildingBlocked,
    CanBuild,
    Crafting,
    CraftLevel1,
    CraftLevel2,
    CraftLevel3,
    DecayProtected,
    Decaying,
    SafeZone,
  }
}
