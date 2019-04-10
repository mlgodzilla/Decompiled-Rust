// Decompiled with JetBrains decompiler
// Type: VLB.TriggerZone
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

namespace VLB
{
  [DisallowMultipleComponent]
  [RequireComponent(typeof (VolumetricLightBeam))]
  [HelpURL("http://saladgamer.com/vlb-doc/comp-triggerzone/")]
  public class TriggerZone : MonoBehaviour
  {
    public bool setIsTrigger;
    public float rangeMultiplier;
    private const int kMeshColliderNumSides = 8;
    private Mesh m_Mesh;

    private void Update()
    {
      VolumetricLightBeam component = (VolumetricLightBeam) ((Component) this).GetComponent<VolumetricLightBeam>();
      if (!Object.op_Implicit((Object) component))
        return;
      MeshCollider orAddComponent = ((Component) this).get_gameObject().GetOrAddComponent<MeshCollider>();
      Debug.Assert(Object.op_Implicit((Object) orAddComponent));
      float lengthZ = component.fadeEnd * this.rangeMultiplier;
      float radiusEnd = Mathf.LerpUnclamped(component.coneRadiusStart, component.coneRadiusEnd, this.rangeMultiplier);
      this.m_Mesh = MeshGenerator.GenerateConeZ_Radius(lengthZ, component.coneRadiusStart, radiusEnd, 8, 0, false);
      ((Object) this.m_Mesh).set_hideFlags(Consts.ProceduralObjectsHideFlags);
      orAddComponent.set_sharedMesh(this.m_Mesh);
      if (this.setIsTrigger)
      {
        orAddComponent.set_convex(true);
        ((Collider) orAddComponent).set_isTrigger(true);
      }
      Object.Destroy((Object) this);
    }

    public TriggerZone()
    {
      base.\u002Ector();
    }
  }
}
