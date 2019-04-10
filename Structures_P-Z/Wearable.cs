// Decompiled with JetBrains decompiler
// Type: Wearable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class Wearable : MonoBehaviour, IItemSetup, IPrefabPreProcess
{
  private static LOD[] emptyLOD = new LOD[1];
  [InspectorFlags]
  public Wearable.RemoveSkin removeSkin;
  [InspectorFlags]
  public Wearable.RemoveHair removeHair;
  public Wearable.DeformHair deformHair;
  public bool showCensorshipCube;
  public bool showCensorshipCubeBreasts;
  public bool forceHideCensorshipBreasts;
  [InspectorFlags]
  public Wearable.OccupationSlots occupationUnder;
  [InspectorFlags]
  public Wearable.OccupationSlots occupationOver;
  public string followBone;

  public void OnItemSetup(Item item)
  {
  }

  public virtual void PreProcess(
    IPrefabProcessor preProcess,
    GameObject rootObj,
    string name,
    bool serverside,
    bool clientside,
    bool bundling)
  {
    foreach (LODGroup componentsInChild in (LODGroup[]) ((Component) this).GetComponentsInChildren<LODGroup>(true))
    {
      componentsInChild.SetLODs(Wearable.emptyLOD);
      preProcess.RemoveComponent((Component) componentsInChild);
    }
  }

  public void SetupRendererCache(IPrefabProcessor preProcess)
  {
  }

  public Wearable()
  {
    base.\u002Ector();
  }

  [System.Flags]
  public enum RemoveSkin
  {
    Torso = 1,
    Feet = 2,
    Hands = 4,
    Legs = 8,
    Head = 16, // 0x00000010
  }

  [System.Flags]
  public enum RemoveHair
  {
    Head = 1,
    Eyebrow = 2,
    Facial = 4,
    Armpit = 8,
    Pubic = 16, // 0x00000010
  }

  [System.Flags]
  public enum DeformHair
  {
    None = 0,
    BaseballCap = 1,
    BoonieHat = 2,
    CandleHat = BoonieHat | BaseballCap, // 0x00000003
    MinersHat = 4,
    WoodHelmet = MinersHat | BaseballCap, // 0x00000005
  }

  [System.Flags]
  public enum OccupationSlots
  {
    HeadTop = 1,
    Face = 2,
    HeadBack = 4,
    TorsoFront = 8,
    TorsoBack = 16, // 0x00000010
    LeftShoulder = 32, // 0x00000020
    RightShoulder = 64, // 0x00000040
    LeftArm = 128, // 0x00000080
    RightArm = 256, // 0x00000100
    LeftHand = 512, // 0x00000200
    RightHand = 1024, // 0x00000400
    Groin = 2048, // 0x00000800
    Bum = 4096, // 0x00001000
    LeftKnee = 8192, // 0x00002000
    RightKnee = 16384, // 0x00004000
    LeftLeg = 32768, // 0x00008000
    RightLeg = 65536, // 0x00010000
    LeftFoot = 131072, // 0x00020000
    RightFoot = 262144, // 0x00040000
  }
}
