// Decompiled with JetBrains decompiler
// Type: OccludeeState
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System.Runtime.InteropServices;
using UnityEngine;

public class OccludeeState : OcclusionCulling.SmartListValue
{
  public int slot;
  public bool isStatic;
  public int layer;
  public OcclusionCulling.OnVisibilityChanged onVisibilityChanged;
  public OcclusionCulling.Cell cell;
  public OcclusionCulling.SimpleList<OccludeeState.State> states;

  public bool isVisible
  {
    get
    {
      return this.states[this.slot].isVisible > (byte) 0;
    }
  }

  public OccludeeState Initialize(
    OcclusionCulling.SimpleList<OccludeeState.State> states,
    OcclusionCulling.BufferSet set,
    int slot,
    Vector4 sphereBounds,
    bool isVisible,
    float minTimeVisible,
    bool isStatic,
    int layer,
    OcclusionCulling.OnVisibilityChanged onVisibilityChanged)
  {
    states[slot] = new OccludeeState.State()
    {
      sphereBounds = sphereBounds,
      minTimeVisible = minTimeVisible,
      waitTime = isVisible ? Time.get_time() + minTimeVisible : 0.0f,
      waitFrame = (uint) (Time.get_frameCount() + 1),
      isVisible = isVisible ? (byte) 1 : (byte) 0,
      active = (byte) 1,
      callback = onVisibilityChanged != null ? (byte) 1 : (byte) 0
    };
    this.slot = slot;
    this.isStatic = isStatic;
    this.layer = layer;
    this.onVisibilityChanged = onVisibilityChanged;
    this.cell = (OcclusionCulling.Cell) null;
    this.states = states;
    return this;
  }

  public void Invalidate()
  {
    this.states[this.slot] = OccludeeState.State.Unused;
    this.slot = -1;
    this.onVisibilityChanged = (OcclusionCulling.OnVisibilityChanged) null;
    this.cell = (OcclusionCulling.Cell) null;
  }

  public void MakeVisible()
  {
    this.states.array[this.slot].waitTime = Time.get_time() + this.states[this.slot].minTimeVisible;
    this.states.array[this.slot].isVisible = (byte) 1;
    if (this.onVisibilityChanged == null)
      return;
    this.onVisibilityChanged(true);
  }

  [StructLayout(LayoutKind.Explicit, Size = 32, Pack = 1)]
  public struct State
  {
    public static OccludeeState.State Unused = new OccludeeState.State()
    {
      active = 0
    };
    [FieldOffset(0)]
    public Vector4 sphereBounds;
    [FieldOffset(16)]
    public float minTimeVisible;
    [FieldOffset(20)]
    public float waitTime;
    [FieldOffset(24)]
    public uint waitFrame;
    [FieldOffset(28)]
    public byte isVisible;
    [FieldOffset(29)]
    public byte active;
    [FieldOffset(30)]
    public byte callback;
    [FieldOffset(31)]
    public byte pad1;
  }
}
