// Decompiled with JetBrains decompiler
// Type: VLB.BeamGeometry
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

namespace VLB
{
  [AddComponentMenu("")]
  [ExecuteInEditMode]
  [HelpURL("http://saladgamer.com/vlb-doc/comp-lightbeam/")]
  public class BeamGeometry : MonoBehaviour
  {
    private VolumetricLightBeam m_Master;
    private Matrix4x4 m_ColorGradientMatrix;
    private MeshType m_CurrentMeshType;

    public MeshRenderer meshRenderer { get; private set; }

    public MeshFilter meshFilter { get; private set; }

    public Material material { get; private set; }

    public Mesh coneMesh { get; private set; }

    public bool visible
    {
      get
      {
        return ((Renderer) this.meshRenderer).get_enabled();
      }
      set
      {
        ((Renderer) this.meshRenderer).set_enabled(value);
      }
    }

    public int sortingLayerID
    {
      get
      {
        return ((Renderer) this.meshRenderer).get_sortingLayerID();
      }
      set
      {
        ((Renderer) this.meshRenderer).set_sortingLayerID(value);
      }
    }

    public int sortingOrder
    {
      get
      {
        return ((Renderer) this.meshRenderer).get_sortingOrder();
      }
      set
      {
        ((Renderer) this.meshRenderer).set_sortingOrder(value);
      }
    }

    private void Start()
    {
    }

    private void OnDestroy()
    {
      if (!Object.op_Implicit((Object) this.material))
        return;
      Object.DestroyImmediate((Object) this.material);
      this.material = (Material) null;
    }

    private static bool IsUsingCustomRenderPipeline()
    {
      if (RenderPipelineManager.get_currentPipeline() == null)
        return Object.op_Inequality((Object) GraphicsSettings.get_renderPipelineAsset(), (Object) null);
      return true;
    }

    private void OnEnable()
    {
      if (!BeamGeometry.IsUsingCustomRenderPipeline())
        return;
      RenderPipeline.add_beginCameraRendering(new Action<Camera>(this.OnBeginCameraRendering));
    }

    private void OnDisable()
    {
      if (!BeamGeometry.IsUsingCustomRenderPipeline())
        return;
      RenderPipeline.remove_beginCameraRendering(new Action<Camera>(this.OnBeginCameraRendering));
    }

    public void Initialize(VolumetricLightBeam master, Shader shader)
    {
      HideFlags objectsHideFlags = Consts.ProceduralObjectsHideFlags;
      this.m_Master = master;
      ((Component) this).get_transform().SetParent(((Component) master).get_transform(), false);
      this.material = new Material(shader);
      ((Object) this.material).set_hideFlags(objectsHideFlags);
      this.meshRenderer = ((Component) this).get_gameObject().GetOrAddComponent<MeshRenderer>();
      ((Object) this.meshRenderer).set_hideFlags(objectsHideFlags);
      ((Renderer) this.meshRenderer).set_material(this.material);
      ((Renderer) this.meshRenderer).set_shadowCastingMode((ShadowCastingMode) 0);
      ((Renderer) this.meshRenderer).set_receiveShadows(false);
      ((Renderer) this.meshRenderer).set_lightProbeUsage((LightProbeUsage) 0);
      if (SortingLayer.IsValid(this.m_Master.sortingLayerID))
        this.sortingLayerID = this.m_Master.sortingLayerID;
      else
        Debug.LogError((object) string.Format("Beam '{0}' has an invalid sortingLayerID ({1}). Please fix it by setting a valid layer.", (object) Utils.GetPath(((Component) this.m_Master).get_transform()), (object) this.m_Master.sortingLayerID));
      this.sortingOrder = this.m_Master.sortingOrder;
      this.meshFilter = ((Component) this).get_gameObject().GetOrAddComponent<MeshFilter>();
      ((Object) this.meshFilter).set_hideFlags(objectsHideFlags);
      ((Object) ((Component) this).get_gameObject()).set_hideFlags(objectsHideFlags);
    }

    public void RegenerateMesh()
    {
      Debug.Assert(Object.op_Implicit((Object) this.m_Master));
      ((Component) this).get_gameObject().set_layer(Config.Instance.geometryLayerID);
      ((Component) this).get_gameObject().set_tag(Config.Instance.geometryTag);
      if (Object.op_Implicit((Object) this.coneMesh) && this.m_CurrentMeshType == MeshType.Custom)
        Object.DestroyImmediate((Object) this.coneMesh);
      this.m_CurrentMeshType = this.m_Master.geomMeshType;
      switch (this.m_Master.geomMeshType)
      {
        case MeshType.Shared:
          this.coneMesh = GlobalMesh.mesh;
          this.meshFilter.set_sharedMesh(this.coneMesh);
          break;
        case MeshType.Custom:
          this.coneMesh = MeshGenerator.GenerateConeZ_Radius(1f, 1f, 1f, this.m_Master.geomCustomSides, this.m_Master.geomCustomSegments, this.m_Master.geomCap);
          ((Object) this.coneMesh).set_hideFlags(Consts.ProceduralObjectsHideFlags);
          this.meshFilter.set_mesh(this.coneMesh);
          break;
        default:
          Debug.LogError((object) "Unsupported MeshType");
          break;
      }
      this.UpdateMaterialAndBounds();
    }

    private void ComputeLocalMatrix()
    {
      float num = Mathf.Max(this.m_Master.coneRadiusStart, this.m_Master.coneRadiusEnd);
      ((Component) this).get_transform().set_localScale(new Vector3(num, num, this.m_Master.fadeEnd));
    }

    public void UpdateMaterialAndBounds()
    {
      Debug.Assert(Object.op_Implicit((Object) this.m_Master));
      this.material.set_renderQueue(Config.Instance.geometryRenderQueue);
      float num = (float) ((double) this.m_Master.coneAngle * (Math.PI / 180.0) / 2.0);
      this.material.SetVector("_ConeSlopeCosSin", Vector4.op_Implicit(new Vector2(Mathf.Cos(num), Mathf.Sin(num))));
      Vector2 vector2;
      ((Vector2) ref vector2).\u002Ector(Mathf.Max(this.m_Master.coneRadiusStart, 0.0001f), Mathf.Max(this.m_Master.coneRadiusEnd, 0.0001f));
      this.material.SetVector("_ConeRadius", Vector4.op_Implicit(vector2));
      this.material.SetFloat("_ConeApexOffsetZ", Mathf.Sign(this.m_Master.coneApexOffsetZ) * Mathf.Max(Mathf.Abs(this.m_Master.coneApexOffsetZ), 0.0001f));
      if (this.m_Master.colorMode == ColorMode.Gradient)
      {
        Utils.FloatPackingPrecision packingPrecision = Utils.GetFloatPackingPrecision();
        this.material.EnableKeyword(packingPrecision == Utils.FloatPackingPrecision.High ? "VLB_COLOR_GRADIENT_MATRIX_HIGH" : "VLB_COLOR_GRADIENT_MATRIX_LOW");
        this.m_ColorGradientMatrix = this.m_Master.colorGradient.SampleInMatrix((int) packingPrecision);
      }
      else
      {
        this.material.DisableKeyword("VLB_COLOR_GRADIENT_MATRIX_HIGH");
        this.material.DisableKeyword("VLB_COLOR_GRADIENT_MATRIX_LOW");
        this.material.SetColor("_ColorFlat", this.m_Master.color);
      }
      if (Consts.BlendingMode_AlphaAsBlack[this.m_Master.blendingModeAsInt])
        this.material.EnableKeyword("ALPHA_AS_BLACK");
      else
        this.material.DisableKeyword("ALPHA_AS_BLACK");
      this.material.SetInt("_BlendSrcFactor", (int) Consts.BlendingMode_SrcFactor[this.m_Master.blendingModeAsInt]);
      this.material.SetInt("_BlendDstFactor", (int) Consts.BlendingMode_DstFactor[this.m_Master.blendingModeAsInt]);
      this.material.SetFloat("_AlphaInside", this.m_Master.alphaInside);
      this.material.SetFloat("_AlphaOutside", this.m_Master.alphaOutside);
      this.material.SetFloat("_AttenuationLerpLinearQuad", this.m_Master.attenuationLerpLinearQuad);
      this.material.SetFloat("_DistanceFadeStart", this.m_Master.fadeStart);
      this.material.SetFloat("_DistanceFadeEnd", this.m_Master.fadeEnd);
      this.material.SetFloat("_DistanceCamClipping", this.m_Master.cameraClippingDistance);
      this.material.SetFloat("_FresnelPow", Mathf.Max(1f / 1000f, this.m_Master.fresnelPow));
      this.material.SetFloat("_GlareBehind", this.m_Master.glareBehind);
      this.material.SetFloat("_GlareFrontal", this.m_Master.glareFrontal);
      this.material.SetFloat("_DrawCap", this.m_Master.geomCap ? 1f : 0.0f);
      if ((double) this.m_Master.depthBlendDistance > 0.0)
      {
        this.material.EnableKeyword("VLB_DEPTH_BLEND");
        this.material.SetFloat("_DepthBlendDistance", this.m_Master.depthBlendDistance);
      }
      else
        this.material.DisableKeyword("VLB_DEPTH_BLEND");
      if (this.m_Master.noiseEnabled && (double) this.m_Master.noiseIntensity > 0.0 && Noise3D.isSupported)
      {
        Noise3D.LoadIfNeeded();
        this.material.EnableKeyword("VLB_NOISE_3D");
        this.material.SetVector("_NoiseLocal", new Vector4((float) this.m_Master.noiseVelocityLocal.x, (float) this.m_Master.noiseVelocityLocal.y, (float) this.m_Master.noiseVelocityLocal.z, this.m_Master.noiseScaleLocal));
        this.material.SetVector("_NoiseParam", Vector4.op_Implicit(new Vector3(this.m_Master.noiseIntensity, this.m_Master.noiseVelocityUseGlobal ? 1f : 0.0f, this.m_Master.noiseScaleUseGlobal ? 1f : 0.0f)));
      }
      else
        this.material.DisableKeyword("VLB_NOISE_3D");
      this.ComputeLocalMatrix();
    }

    public void SetClippingPlane(Plane planeWS)
    {
      Vector3 normal = ((Plane) ref planeWS).get_normal();
      this.material.EnableKeyword("VLB_CLIPPING_PLANE");
      this.material.SetVector("_ClippingPlaneWS", new Vector4((float) normal.x, (float) normal.y, (float) normal.z, ((Plane) ref planeWS).get_distance()));
    }

    public void SetClippingPlaneOff()
    {
      this.material.DisableKeyword("VLB_CLIPPING_PLANE");
    }

    private void OnBeginCameraRendering(Camera cam)
    {
      this.UpdateCameraRelatedProperties(cam);
    }

    private void OnWillRenderObject()
    {
      if (BeamGeometry.IsUsingCustomRenderPipeline())
        return;
      Camera current = Camera.get_current();
      if (!Object.op_Inequality((Object) current, (Object) null))
        return;
      this.UpdateCameraRelatedProperties(current);
    }

    private void UpdateCameraRelatedProperties(Camera cam)
    {
      if (!Object.op_Implicit((Object) cam) || !Object.op_Implicit((Object) this.m_Master))
        return;
      if (Object.op_Implicit((Object) this.material))
      {
        Vector3 posOS = ((Component) this.m_Master).get_transform().InverseTransformPoint(((Component) cam).get_transform().get_position());
        this.material.SetVector("_CameraPosObjectSpace", Vector4.op_Implicit(posOS));
        Vector3 vector3 = ((Component) this).get_transform().InverseTransformDirection(((Component) cam).get_transform().get_forward());
        Vector3 normalized = ((Vector3) ref vector3).get_normalized();
        float num = cam.get_orthographic() ? -1f : this.m_Master.GetInsideBeamFactorFromObjectSpacePos(posOS);
        this.material.SetVector("_CameraParams", new Vector4((float) normalized.x, (float) normalized.y, (float) normalized.z, num));
        if (this.m_Master.colorMode == ColorMode.Gradient)
          this.material.SetMatrix("_ColorGradientMatrix", this.m_ColorGradientMatrix);
      }
      if ((double) this.m_Master.depthBlendDistance <= 0.0)
        return;
      Camera camera = cam;
      camera.set_depthTextureMode((DepthTextureMode) (camera.get_depthTextureMode() | 1));
    }

    public BeamGeometry()
    {
      base.\u002Ector();
    }
  }
}
