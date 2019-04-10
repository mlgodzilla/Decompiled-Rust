// Decompiled with JetBrains decompiler
// Type: Occludee
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class Occludee : MonoBehaviour
{
  public float minTimeVisible;
  public bool isStatic;
  public bool autoRegister;
  public bool stickyGizmos;
  public OccludeeState state;
  protected int occludeeId;
  protected Vector3 center;
  protected float radius;
  protected Renderer renderer;
  protected Collider collider;

  protected virtual void Awake()
  {
    this.renderer = (Renderer) ((Component) this).GetComponent<Renderer>();
    this.collider = (Collider) ((Component) this).GetComponent<Collider>();
  }

  public void OnEnable()
  {
    if (!this.autoRegister || !Object.op_Inequality((Object) this.collider, (Object) null))
      return;
    this.Register();
  }

  public void OnDisable()
  {
    if (!this.autoRegister || this.occludeeId < 0)
      return;
    this.Unregister();
  }

  public void Register()
  {
    Bounds bounds1 = this.collider.get_bounds();
    this.center = ((Bounds) ref bounds1).get_center();
    Bounds bounds2 = this.collider.get_bounds();
    // ISSUE: variable of the null type
    __Null x = ((Bounds) ref bounds2).get_extents().x;
    bounds2 = this.collider.get_bounds();
    // ISSUE: variable of the null type
    __Null y = ((Bounds) ref bounds2).get_extents().y;
    double num = (double) Mathf.Max((float) x, (float) y);
    bounds2 = this.collider.get_bounds();
    // ISSUE: variable of the null type
    __Null z = ((Bounds) ref bounds2).get_extents().z;
    this.radius = Mathf.Max((float) num, (float) z);
    this.occludeeId = OcclusionCulling.RegisterOccludee(this.center, this.radius, this.renderer.get_enabled(), this.minTimeVisible, this.isStatic, ((Component) this).get_gameObject().get_layer(), new OcclusionCulling.OnVisibilityChanged(this.OnVisibilityChanged));
    if (this.occludeeId < 0)
      Debug.LogWarning((object) ("[OcclusionCulling] Occludee registration failed for " + ((Object) this).get_name() + ". Too many registered."));
    this.state = OcclusionCulling.GetStateById(this.occludeeId);
  }

  public void Unregister()
  {
    OcclusionCulling.UnregisterOccludee(this.occludeeId);
  }

  protected virtual void OnVisibilityChanged(bool visible)
  {
    if (!Object.op_Inequality((Object) this.renderer, (Object) null))
      return;
    this.renderer.set_enabled(visible);
  }

  public Occludee()
  {
    base.\u002Ector();
  }
}
