// Decompiled with JetBrains decompiler
// Type: VLB.DynamicOcclusion
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

namespace VLB
{
  [RequireComponent(typeof (VolumetricLightBeam))]
  [HelpURL("http://saladgamer.com/vlb-doc/comp-dynocclusion/")]
  [DisallowMultipleComponent]
  [ExecuteInEditMode]
  public class DynamicOcclusion : MonoBehaviour
  {
    public LayerMask layerMask;
    public float minOccluderArea;
    public int waitFrameCount;
    public float minSurfaceRatio;
    public float maxSurfaceDot;
    public PlaneAlignment planeAlignment;
    public float planeOffset;
    private VolumetricLightBeam m_Master;
    private int m_FrameCountToWait;
    private float m_RangeMultiplier;
    private uint m_PrevNonSubHitDirectionId;

    private void OnValidate()
    {
      this.minOccluderArea = Mathf.Max(this.minOccluderArea, 0.0f);
      this.waitFrameCount = Mathf.Clamp(this.waitFrameCount, 1, 60);
    }

    private void OnEnable()
    {
      this.m_Master = (VolumetricLightBeam) ((Component) this).GetComponent<VolumetricLightBeam>();
      Debug.Assert(Object.op_Implicit((Object) this.m_Master));
    }

    private void OnDisable()
    {
      this.SetHitNull();
    }

    private void Start()
    {
      if (!Application.get_isPlaying())
        return;
      TriggerZone component = (TriggerZone) ((Component) this).GetComponent<TriggerZone>();
      if (!Object.op_Implicit((Object) component))
        return;
      this.m_RangeMultiplier = Mathf.Max(1f, component.rangeMultiplier);
    }

    private void LateUpdate()
    {
      if (this.m_FrameCountToWait <= 0)
      {
        this.ProcessRaycasts();
        this.m_FrameCountToWait = this.waitFrameCount;
      }
      --this.m_FrameCountToWait;
    }

    private Vector3 GetRandomVectorAround(Vector3 direction, float angleDiff)
    {
      float num = angleDiff * 0.5f;
      return Quaternion.op_Multiply(Quaternion.Euler(Random.Range(-num, num), Random.Range(-num, num), Random.Range(-num, num)), direction);
    }

    private RaycastHit GetBestHit(Vector3 rayPos, Vector3 rayDir)
    {
      RaycastHit[] raycastHitArray = Physics.RaycastAll(rayPos, rayDir, this.m_Master.fadeEnd * this.m_RangeMultiplier, ((LayerMask) ref this.layerMask).get_value());
      int index1 = -1;
      float num = float.MaxValue;
      for (int index2 = 0; index2 < raycastHitArray.Length; ++index2)
      {
        if (!((RaycastHit) ref raycastHitArray[index2]).get_collider().get_isTrigger() && (double) ((RaycastHit) ref raycastHitArray[index2]).get_collider().get_bounds().GetMaxArea2D() >= (double) this.minOccluderArea && (double) ((RaycastHit) ref raycastHitArray[index2]).get_distance() < (double) num)
        {
          num = ((RaycastHit) ref raycastHitArray[index2]).get_distance();
          index1 = index2;
        }
      }
      if (index1 != -1)
        return raycastHitArray[index1];
      return (RaycastHit) null;
    }

    private Vector3 GetDirection(uint dirInt)
    {
      dirInt %= (uint) Enum.GetValues(typeof (DynamicOcclusion.Direction)).Length;
      switch (dirInt)
      {
        case 0:
          return ((Component) this).get_transform().get_up();
        case 1:
          return ((Component) this).get_transform().get_right();
        case 2:
          return Vector3.op_UnaryNegation(((Component) this).get_transform().get_up());
        case 3:
          return Vector3.op_UnaryNegation(((Component) this).get_transform().get_right());
        default:
          return Vector3.get_zero();
      }
    }

    private bool IsHitValid(RaycastHit hit)
    {
      if (Object.op_Implicit((Object) ((RaycastHit) ref hit).get_collider()))
        return (double) Vector3.Dot(((RaycastHit) ref hit).get_normal(), Vector3.op_UnaryNegation(((Component) this).get_transform().get_forward())) >= (double) this.maxSurfaceDot;
      return false;
    }

    private void ProcessRaycasts()
    {
      RaycastHit hit = this.GetBestHit(((Component) this).get_transform().get_position(), ((Component) this).get_transform().get_forward());
      if (this.IsHitValid(hit))
      {
        if ((double) this.minSurfaceRatio > 0.5)
        {
          for (uint index = 0; index < (uint) Enum.GetValues(typeof (DynamicOcclusion.Direction)).Length; ++index)
          {
            Vector3 direction = this.GetDirection(index + this.m_PrevNonSubHitDirectionId);
            Vector3 rayPos = Vector3.op_Addition(((Component) this).get_transform().get_position(), Vector3.op_Multiply(Vector3.op_Multiply(direction, this.m_Master.coneRadiusStart), (float) ((double) this.minSurfaceRatio * 2.0 - 1.0)));
            Vector3 vector3 = Vector3.op_Addition(Vector3.op_Addition(((Component) this).get_transform().get_position(), Vector3.op_Multiply(((Component) this).get_transform().get_forward(), this.m_Master.fadeEnd)), Vector3.op_Multiply(Vector3.op_Multiply(direction, this.m_Master.coneRadiusEnd), (float) ((double) this.minSurfaceRatio * 2.0 - 1.0)));
            RaycastHit bestHit = this.GetBestHit(rayPos, Vector3.op_Subtraction(vector3, rayPos));
            if (this.IsHitValid(bestHit))
            {
              if ((double) ((RaycastHit) ref bestHit).get_distance() > (double) ((RaycastHit) ref hit).get_distance())
                hit = bestHit;
            }
            else
            {
              this.m_PrevNonSubHitDirectionId = index;
              this.SetHitNull();
              return;
            }
          }
        }
        this.SetHit(hit);
      }
      else
        this.SetHitNull();
    }

    private void SetHit(RaycastHit hit)
    {
      switch (this.planeAlignment)
      {
        case PlaneAlignment.Beam:
          this.SetClippingPlane(new Plane(Vector3.op_UnaryNegation(((Component) this).get_transform().get_forward()), ((RaycastHit) ref hit).get_point()));
          break;
        default:
          this.SetClippingPlane(new Plane(((RaycastHit) ref hit).get_normal(), ((RaycastHit) ref hit).get_point()));
          break;
      }
    }

    private void SetHitNull()
    {
      this.SetClippingPlaneOff();
    }

    private void SetClippingPlane(Plane planeWS)
    {
      planeWS = planeWS.TranslateCustom(Vector3.op_Multiply(((Plane) ref planeWS).get_normal(), this.planeOffset));
      this.m_Master.SetClippingPlane(planeWS);
    }

    private void SetClippingPlaneOff()
    {
      this.m_Master.SetClippingPlaneOff();
    }

    public DynamicOcclusion()
    {
      base.\u002Ector();
    }

    private enum Direction
    {
      Up,
      Right,
      Down,
      Left,
    }
  }
}
