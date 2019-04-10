// Decompiled with JetBrains decompiler
// Type: WaterBody
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

[ExecuteInEditMode]
public class WaterBody : MonoBehaviour
{
  public WaterBodyType Type;
  public Renderer Renderer;
  public Collider[] Triggers;

  public Transform Transform { private set; get; }

  private void Awake()
  {
    this.Transform = ((Component) this).get_transform();
  }

  private void OnEnable()
  {
    WaterSystem.RegisterBody(this);
  }

  private void OnDisable()
  {
    WaterSystem.UnregisterBody(this);
  }

  public WaterBody()
  {
    base.\u002Ector();
  }
}
