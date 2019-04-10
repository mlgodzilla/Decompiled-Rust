// Decompiled with JetBrains decompiler
// Type: ArticulatedOccludee
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Rust;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ArticulatedOccludee : BaseMonoBehaviour
{
  public List<Collider> colliders = new List<Collider>();
  private OccludeeSphere localOccludee = new OccludeeSphere(-1);
  private List<Renderer> renderers = new List<Renderer>();
  private bool isVisible = true;
  private const float UpdateBoundsFadeStart = 20f;
  private const float UpdateBoundsFadeLength = 1000f;
  private const float UpdateBoundsMaxFrequency = 15f;
  private const float UpdateBoundsMinFrequency = 0.5f;
  private LODGroup lodGroup;
  private Action TriggerUpdateVisibilityBoundsDelegate;

  public bool IsVisible
  {
    get
    {
      return this.isVisible;
    }
  }

  protected virtual void OnDisable()
  {
    if (Application.isQuitting != null)
      return;
    this.UnregisterFromCulling();
    this.ClearVisibility();
  }

  public void ClearVisibility()
  {
    if (Object.op_Inequality((Object) this.lodGroup, (Object) null))
    {
      this.lodGroup.set_localReferencePoint(Vector3.get_zero());
      this.lodGroup.RecalculateBounds();
      this.lodGroup = (LODGroup) null;
    }
    if (this.renderers != null)
      this.renderers.Clear();
    this.localOccludee = new OccludeeSphere(-1);
  }

  public void ProcessVisibility(LODGroup lod)
  {
    this.lodGroup = lod;
    if (Object.op_Inequality((Object) lod, (Object) null))
    {
      this.renderers = new List<Renderer>(16);
      foreach (LOD loD in lod.GetLODs())
      {
        foreach (Renderer renderer in (Renderer[]) loD.renderers)
        {
          if (Object.op_Inequality((Object) renderer, (Object) null))
            this.renderers.Add(renderer);
        }
      }
    }
    this.UpdateCullingBounds();
  }

  private void RegisterForCulling(OcclusionCulling.Sphere sphere, bool visible)
  {
    if (this.localOccludee.IsRegistered)
      this.UnregisterFromCulling();
    int id = OcclusionCulling.RegisterOccludee(sphere.position, sphere.radius, visible, 0.25f, false, ((Component) this).get_gameObject().get_layer(), new OcclusionCulling.OnVisibilityChanged(this.OnVisibilityChanged));
    if (id >= 0)
    {
      this.localOccludee = new OccludeeSphere(id, this.localOccludee.sphere);
    }
    else
    {
      this.localOccludee.Invalidate();
      Debug.LogWarning((object) ("[OcclusionCulling] Occludee registration failed for " + ((Object) this).get_name() + ". Too many registered."));
    }
  }

  private void UnregisterFromCulling()
  {
    if (!this.localOccludee.IsRegistered)
      return;
    OcclusionCulling.UnregisterOccludee(this.localOccludee.id);
    this.localOccludee.Invalidate();
  }

  public void UpdateCullingBounds()
  {
    Vector3 vector3_1 = Vector3.get_zero();
    Vector3 vector3_2 = Vector3.get_zero();
    bool flag = false;
    int num1 = this.renderers != null ? this.renderers.Count : 0;
    int num2 = this.colliders != null ? this.colliders.Count : 0;
    if (num1 > 0 && (num2 == 0 || num1 < num2))
    {
      for (int index = 0; index < this.renderers.Count; ++index)
      {
        if (this.renderers[index].get_isVisible())
        {
          Bounds bounds = this.renderers[index].get_bounds();
          Vector3 min = ((Bounds) ref bounds).get_min();
          Vector3 max = ((Bounds) ref bounds).get_max();
          if (!flag)
          {
            vector3_1 = min;
            vector3_2 = max;
            flag = true;
          }
          else
          {
            vector3_1.x = vector3_1.x < min.x ? vector3_1.x : min.x;
            vector3_1.y = vector3_1.y < min.y ? vector3_1.y : min.y;
            vector3_1.z = vector3_1.z < min.z ? vector3_1.z : min.z;
            vector3_2.x = vector3_2.x > max.x ? vector3_2.x : max.x;
            vector3_2.y = vector3_2.y > max.y ? vector3_2.y : max.y;
            vector3_2.z = vector3_2.z > max.z ? vector3_2.z : max.z;
          }
        }
      }
    }
    if (!flag && num2 > 0)
    {
      flag = true;
      Bounds bounds1 = this.colliders[0].get_bounds();
      vector3_1 = ((Bounds) ref bounds1).get_min();
      bounds1 = this.colliders[0].get_bounds();
      vector3_2 = ((Bounds) ref bounds1).get_max();
      for (int index = 1; index < this.colliders.Count; ++index)
      {
        Bounds bounds2 = this.colliders[index].get_bounds();
        Vector3 min = ((Bounds) ref bounds2).get_min();
        Vector3 max = ((Bounds) ref bounds2).get_max();
        vector3_1.x = vector3_1.x < min.x ? vector3_1.x : min.x;
        vector3_1.y = vector3_1.y < min.y ? vector3_1.y : min.y;
        vector3_1.z = vector3_1.z < min.z ? vector3_1.z : min.z;
        vector3_2.x = vector3_2.x > max.x ? vector3_2.x : max.x;
        vector3_2.y = vector3_2.y > max.y ? vector3_2.y : max.y;
        vector3_2.z = vector3_2.z > max.z ? vector3_2.z : max.z;
      }
    }
    if (!flag)
      return;
    Vector3 vector3_3 = Vector3.op_Subtraction(vector3_2, vector3_1);
    OcclusionCulling.Sphere sphere = new OcclusionCulling.Sphere(Vector3.op_Addition(vector3_1, Vector3.op_Multiply(vector3_3, 0.5f)), Mathf.Max(Mathf.Max((float) vector3_3.x, (float) vector3_3.y), (float) vector3_3.z) * 0.5f);
    if (this.localOccludee.IsRegistered)
    {
      OcclusionCulling.UpdateDynamicOccludee(this.localOccludee.id, sphere.position, sphere.radius);
      this.localOccludee.sphere = sphere;
    }
    else
    {
      bool visible = true;
      if (Object.op_Inequality((Object) this.lodGroup, (Object) null))
        visible = this.lodGroup.get_enabled();
      this.RegisterForCulling(sphere, visible);
    }
  }

  protected virtual bool CheckVisibility()
  {
    if (this.localOccludee.state != null)
      return this.localOccludee.state.isVisible;
    return true;
  }

  private void ApplyVisibility(bool vis)
  {
    if (!Object.op_Inequality((Object) this.lodGroup, (Object) null))
      return;
    float num = vis ? 0.0f : 100000f;
    if ((double) num == this.lodGroup.get_localReferencePoint().x)
      return;
    this.lodGroup.set_localReferencePoint(new Vector3(num, num, num));
  }

  protected virtual void OnVisibilityChanged(bool visible)
  {
    if (!Object.op_Inequality((Object) MainCamera.mainCamera, (Object) null) || !this.localOccludee.IsRegistered)
      return;
    this.VisUpdateUsingCulling(Vector3.Distance(((Component) MainCamera.mainCamera).get_transform().get_position(), ((Component) this).get_transform().get_position()), visible);
    this.ApplyVisibility(this.isVisible);
  }

  private void UpdateVisibility(float delay)
  {
  }

  private void VisUpdateUsingCulling(float dist, bool visibility)
  {
  }

  public virtual void TriggerUpdateVisibilityBounds()
  {
    if (!((Behaviour) this).get_enabled())
      return;
    Vector3 vector3 = Vector3.op_Subtraction(((Component) this).get_transform().get_position(), ((Component) MainCamera.mainCamera).get_transform().get_position());
    float sqrMagnitude = ((Vector3) ref vector3).get_sqrMagnitude();
    float num1 = 400f;
    float delay;
    if ((double) sqrMagnitude < (double) num1)
    {
      delay = 1f / Random.Range(5f, 25f);
    }
    else
    {
      double num2 = (double) Mathf.Lerp(0.06666667f, 2f, Mathf.Clamp01((float) (((double) Mathf.Sqrt(sqrMagnitude) - 20.0) * (1.0 / 1000.0))));
      delay = Random.Range((float) num2, (float) (num2 + 0.0666666701436043));
    }
    this.UpdateVisibility(delay);
    this.ApplyVisibility(this.isVisible);
    if (this.TriggerUpdateVisibilityBoundsDelegate == null)
      this.TriggerUpdateVisibilityBoundsDelegate = new Action(this.TriggerUpdateVisibilityBounds);
    this.Invoke(this.TriggerUpdateVisibilityBoundsDelegate, delay);
  }
}
