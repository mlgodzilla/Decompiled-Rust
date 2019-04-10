// Decompiled with JetBrains decompiler
// Type: VLB.VolumetricLightBeam
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace VLB
{
  [DisallowMultipleComponent]
  [ExecuteInEditMode]
  [SelectionBase]
  [HelpURL("http://saladgamer.com/vlb-doc/comp-lightbeam/")]
  public class VolumetricLightBeam : MonoBehaviour
  {
    public bool colorFromLight;
    public ColorMode colorMode;
    [FormerlySerializedAs("colorValue")]
    [ColorUsage(true, true)]
    public Color color;
    public Gradient colorGradient;
    [Range(0.0f, 1f)]
    public float alphaInside;
    [FormerlySerializedAs("alpha")]
    [Range(0.0f, 1f)]
    public float alphaOutside;
    public BlendingMode blendingMode;
    [FormerlySerializedAs("angleFromLight")]
    public bool spotAngleFromLight;
    [Range(0.1f, 179.9f)]
    public float spotAngle;
    [FormerlySerializedAs("radiusStart")]
    public float coneRadiusStart;
    public MeshType geomMeshType;
    [FormerlySerializedAs("geomSides")]
    public int geomCustomSides;
    public int geomCustomSegments;
    public bool geomCap;
    public bool fadeEndFromLight;
    public AttenuationEquation attenuationEquation;
    [Range(0.0f, 1f)]
    public float attenuationCustomBlending;
    public float fadeStart;
    public float fadeEnd;
    public float depthBlendDistance;
    public float cameraClippingDistance;
    [Range(0.0f, 1f)]
    public float glareFrontal;
    [Range(0.0f, 1f)]
    public float glareBehind;
    [Obsolete("Use 'glareFrontal' instead")]
    public float boostDistanceInside;
    [Obsolete("This property has been merged with 'fresnelPow'")]
    public float fresnelPowInside;
    [FormerlySerializedAs("fresnelPowOutside")]
    public float fresnelPow;
    public bool noiseEnabled;
    [Range(0.0f, 1f)]
    public float noiseIntensity;
    public bool noiseScaleUseGlobal;
    [Range(0.01f, 2f)]
    public float noiseScaleLocal;
    public bool noiseVelocityUseGlobal;
    public Vector3 noiseVelocityLocal;
    private Plane m_PlaneWS;
    [SerializeField]
    private int pluginVersion;
    [SerializeField]
    [FormerlySerializedAs("trackChangesDuringPlaytime")]
    private bool _TrackChangesDuringPlaytime;
    [SerializeField]
    private int _SortingLayerID;
    [SerializeField]
    private int _SortingOrder;
    private BeamGeometry m_BeamGeom;
    private Coroutine m_CoPlaytimeUpdate;
    private Light _CachedLight;

    public float coneAngle
    {
      get
      {
        return (float) ((double) Mathf.Atan2(this.coneRadiusEnd - this.coneRadiusStart, this.fadeEnd) * 57.2957801818848 * 2.0);
      }
    }

    public float coneRadiusEnd
    {
      get
      {
        return this.fadeEnd * Mathf.Tan((float) ((double) this.spotAngle * (Math.PI / 180.0) * 0.5));
      }
    }

    public float coneVolume
    {
      get
      {
        float coneRadiusStart = this.coneRadiusStart;
        float coneRadiusEnd = this.coneRadiusEnd;
        return (float) (1.04719758033752 * ((double) coneRadiusStart * (double) coneRadiusStart + (double) coneRadiusStart * (double) coneRadiusEnd + (double) coneRadiusEnd * (double) coneRadiusEnd)) * this.fadeEnd;
      }
    }

    public float coneApexOffsetZ
    {
      get
      {
        float num = this.coneRadiusStart / this.coneRadiusEnd;
        if ((double) num != 1.0)
          return (float) ((double) this.fadeEnd * (double) num / (1.0 - (double) num));
        return float.MaxValue;
      }
    }

    public int geomSides
    {
      get
      {
        if (this.geomMeshType != MeshType.Custom)
          return Config.Instance.sharedMeshSides;
        return this.geomCustomSides;
      }
      set
      {
        this.geomCustomSides = value;
        Debug.LogWarning((object) "The setter VLB.VolumetricLightBeam.geomSides is OBSOLETE and has been renamed to geomCustomSides.");
      }
    }

    public int geomSegments
    {
      get
      {
        if (this.geomMeshType != MeshType.Custom)
          return Config.Instance.sharedMeshSegments;
        return this.geomCustomSegments;
      }
      set
      {
        this.geomCustomSegments = value;
        Debug.LogWarning((object) "The setter VLB.VolumetricLightBeam.geomSegments is OBSOLETE and has been renamed to geomCustomSegments.");
      }
    }

    public float attenuationLerpLinearQuad
    {
      get
      {
        if (this.attenuationEquation == AttenuationEquation.Linear)
          return 0.0f;
        if (this.attenuationEquation == AttenuationEquation.Quadratic)
          return 1f;
        return this.attenuationCustomBlending;
      }
    }

    public int sortingLayerID
    {
      get
      {
        return this._SortingLayerID;
      }
      set
      {
        this._SortingLayerID = value;
        if (!Object.op_Implicit((Object) this.m_BeamGeom))
          return;
        this.m_BeamGeom.sortingLayerID = value;
      }
    }

    public string sortingLayerName
    {
      get
      {
        return SortingLayer.IDToName(this.sortingLayerID);
      }
      set
      {
        this.sortingLayerID = SortingLayer.NameToID(value);
      }
    }

    public int sortingOrder
    {
      get
      {
        return this._SortingOrder;
      }
      set
      {
        this._SortingOrder = value;
        if (!Object.op_Implicit((Object) this.m_BeamGeom))
          return;
        this.m_BeamGeom.sortingOrder = value;
      }
    }

    public bool trackChangesDuringPlaytime
    {
      get
      {
        return this._TrackChangesDuringPlaytime;
      }
      set
      {
        this._TrackChangesDuringPlaytime = value;
        this.StartPlaytimeUpdateIfNeeded();
      }
    }

    public bool isCurrentlyTrackingChanges
    {
      get
      {
        return this.m_CoPlaytimeUpdate != null;
      }
    }

    public bool hasGeometry
    {
      get
      {
        return Object.op_Inequality((Object) this.m_BeamGeom, (Object) null);
      }
    }

    public Bounds bounds
    {
      get
      {
        if (!Object.op_Inequality((Object) this.m_BeamGeom, (Object) null))
          return new Bounds(Vector3.get_zero(), Vector3.get_zero());
        return ((Renderer) this.m_BeamGeom.meshRenderer).get_bounds();
      }
    }

    public void SetClippingPlane(Plane planeWS)
    {
      if (Object.op_Implicit((Object) this.m_BeamGeom))
        this.m_BeamGeom.SetClippingPlane(planeWS);
      this.m_PlaneWS = planeWS;
    }

    public void SetClippingPlaneOff()
    {
      if (Object.op_Implicit((Object) this.m_BeamGeom))
        this.m_BeamGeom.SetClippingPlaneOff();
      this.m_PlaneWS = (Plane) null;
    }

    public bool IsColliderHiddenByDynamicOccluder(Collider collider)
    {
      Debug.Assert(Object.op_Implicit((Object) collider), "You should pass a valid Collider to VLB.VolumetricLightBeam.IsColliderHiddenByDynamicOccluder");
      if (!this.m_PlaneWS.IsValid())
        return false;
      return !GeometryUtility.TestPlanesAABB(new Plane[1]
      {
        this.m_PlaneWS
      }, collider.get_bounds());
    }

    public int blendingModeAsInt
    {
      get
      {
        return Mathf.Clamp((int) this.blendingMode, 0, Enum.GetValues(typeof (BlendingMode)).Length);
      }
    }

    public string meshStats
    {
      get
      {
        Mesh mesh = Object.op_Implicit((Object) this.m_BeamGeom) ? this.m_BeamGeom.coneMesh : (Mesh) null;
        if (Object.op_Implicit((Object) mesh))
          return string.Format("Cone angle: {0:0.0} degrees\nMesh: {1} vertices, {2} triangles", (object) this.coneAngle, (object) mesh.get_vertexCount(), (object) (mesh.get_triangles().Length / 3));
        return "no mesh available";
      }
    }

    public int meshVerticesCount
    {
      get
      {
        if (!Object.op_Implicit((Object) this.m_BeamGeom) || !Object.op_Implicit((Object) this.m_BeamGeom.coneMesh))
          return 0;
        return this.m_BeamGeom.coneMesh.get_vertexCount();
      }
    }

    public int meshTrianglesCount
    {
      get
      {
        if (!Object.op_Implicit((Object) this.m_BeamGeom) || !Object.op_Implicit((Object) this.m_BeamGeom.coneMesh))
          return 0;
        return this.m_BeamGeom.coneMesh.get_triangles().Length / 3;
      }
    }

    private Light lightSpotAttached
    {
      get
      {
        if (Object.op_Equality((Object) this._CachedLight, (Object) null))
          this._CachedLight = (Light) ((Component) this).GetComponent<Light>();
        if (Object.op_Implicit((Object) this._CachedLight) && this._CachedLight.get_type() == null)
          return this._CachedLight;
        return (Light) null;
      }
    }

    public float GetInsideBeamFactor(Vector3 posWS)
    {
      return this.GetInsideBeamFactorFromObjectSpacePos(((Component) this).get_transform().InverseTransformPoint(posWS));
    }

    public float GetInsideBeamFactorFromObjectSpacePos(Vector3 posOS)
    {
      if (posOS.z < 0.0)
        return -1f;
      Vector2 vector2 = posOS.xy();
      vector2 = new Vector2(((Vector2) ref vector2).get_magnitude(), (float) posOS.z + this.coneApexOffsetZ);
      return Mathf.Clamp((float) (((double) Mathf.Abs(Mathf.Sin((float) ((double) this.coneAngle * (Math.PI / 180.0) / 2.0))) - (double) Mathf.Abs((float) ((Vector2) ref vector2).get_normalized().x)) / 0.100000001490116), -1f, 1f);
    }

    [Obsolete("Use 'GenerateGeometry()' instead")]
    public void Generate()
    {
      this.GenerateGeometry();
    }

    public virtual void GenerateGeometry()
    {
      this.HandleBackwardCompatibility(this.pluginVersion, 1510);
      this.pluginVersion = 1510;
      this.ValidateProperties();
      if (Object.op_Equality((Object) this.m_BeamGeom, (Object) null))
      {
        Shader beamShader = Config.Instance.beamShader;
        if (!Object.op_Implicit((Object) beamShader))
        {
          Debug.LogError((object) "Invalid BeamShader set in VLB Config");
          return;
        }
        this.m_BeamGeom = Utils.NewWithComponent<BeamGeometry>("Beam Geometry");
        this.m_BeamGeom.Initialize(this, beamShader);
      }
      this.m_BeamGeom.RegenerateMesh();
      this.m_BeamGeom.visible = ((Behaviour) this).get_enabled();
    }

    public virtual void UpdateAfterManualPropertyChange()
    {
      this.ValidateProperties();
      if (!Object.op_Implicit((Object) this.m_BeamGeom))
        return;
      this.m_BeamGeom.UpdateMaterialAndBounds();
    }

    private void Start()
    {
      this.GenerateGeometry();
    }

    private void OnEnable()
    {
      if (Object.op_Implicit((Object) this.m_BeamGeom))
        this.m_BeamGeom.visible = true;
      this.StartPlaytimeUpdateIfNeeded();
    }

    private void OnDisable()
    {
      if (Object.op_Implicit((Object) this.m_BeamGeom))
        this.m_BeamGeom.visible = false;
      this.m_CoPlaytimeUpdate = (Coroutine) null;
    }

    private void StartPlaytimeUpdateIfNeeded()
    {
    }

    private IEnumerator CoPlaytimeUpdate()
    {
      VolumetricLightBeam volumetricLightBeam = this;
      while (volumetricLightBeam.trackChangesDuringPlaytime && ((Behaviour) volumetricLightBeam).get_enabled())
      {
        volumetricLightBeam.UpdateAfterManualPropertyChange();
        yield return (object) null;
      }
      volumetricLightBeam.m_CoPlaytimeUpdate = (Coroutine) null;
    }

    private void OnDestroy()
    {
      this.DestroyBeam();
    }

    private void DestroyBeam()
    {
      if (Object.op_Implicit((Object) this.m_BeamGeom))
        Object.DestroyImmediate((Object) ((Component) this.m_BeamGeom).get_gameObject());
      this.m_BeamGeom = (BeamGeometry) null;
    }

    private void AssignPropertiesFromSpotLight(Light lightSpot)
    {
      if (!Object.op_Implicit((Object) lightSpot) || lightSpot.get_type() != null)
        return;
      if (this.fadeEndFromLight)
        this.fadeEnd = lightSpot.get_range();
      if (this.spotAngleFromLight)
        this.spotAngle = lightSpot.get_spotAngle();
      if (!this.colorFromLight)
        return;
      this.colorMode = ColorMode.Flat;
      this.color = lightSpot.get_color();
    }

    private void ClampProperties()
    {
      this.alphaInside = Mathf.Clamp01(this.alphaInside);
      this.alphaOutside = Mathf.Clamp01(this.alphaOutside);
      this.attenuationCustomBlending = Mathf.Clamp01(this.attenuationCustomBlending);
      this.fadeEnd = Mathf.Max(0.01f, this.fadeEnd);
      this.fadeStart = Mathf.Clamp(this.fadeStart, 0.0f, this.fadeEnd - 0.01f);
      this.spotAngle = Mathf.Clamp(this.spotAngle, 0.1f, 179.9f);
      this.coneRadiusStart = Mathf.Max(this.coneRadiusStart, 0.0f);
      this.depthBlendDistance = Mathf.Max(this.depthBlendDistance, 0.0f);
      this.cameraClippingDistance = Mathf.Max(this.cameraClippingDistance, 0.0f);
      this.geomCustomSides = Mathf.Clamp(this.geomCustomSides, 3, 256);
      this.geomCustomSegments = Mathf.Clamp(this.geomCustomSegments, 0, 64);
      this.fresnelPow = Mathf.Max(0.0f, this.fresnelPow);
      this.glareBehind = Mathf.Clamp01(this.glareBehind);
      this.glareFrontal = Mathf.Clamp01(this.glareFrontal);
      this.noiseIntensity = Mathf.Clamp(this.noiseIntensity, 0.0f, 1f);
    }

    private void ValidateProperties()
    {
      this.AssignPropertiesFromSpotLight(this.lightSpotAttached);
      this.ClampProperties();
    }

    private void HandleBackwardCompatibility(int serializedVersion, int newVersion)
    {
      if (serializedVersion == -1 || serializedVersion == newVersion)
        return;
      if (serializedVersion < 1301)
        this.attenuationEquation = AttenuationEquation.Linear;
      if (serializedVersion < 1501)
      {
        this.geomMeshType = MeshType.Custom;
        this.geomCustomSegments = 5;
      }
      Utils.MarkCurrentSceneDirty();
    }

    public VolumetricLightBeam()
    {
      base.\u002Ector();
    }
  }
}
