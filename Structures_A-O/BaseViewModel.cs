// Decompiled with JetBrains decompiler
// Type: BaseViewModel
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class BaseViewModel : MonoBehaviour
{
  [Header("BaseViewModel")]
  public LazyAimProperties lazyaimRegular;
  public LazyAimProperties lazyaimIronsights;
  public Transform pivot;
  public bool wantsHeldItemFlags;
  public GameObject[] hideSightMeshes;
  public Transform MuzzlePoint;
  [Header("Skin")]
  public SubsurfaceProfile subsurfaceProfile;

  public BaseViewModel()
  {
    base.\u002Ector();
  }
}
