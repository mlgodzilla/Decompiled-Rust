// Decompiled with JetBrains decompiler
// Type: VLB.VolumetricDustParticles
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

namespace VLB
{
  [ExecuteInEditMode]
  [DisallowMultipleComponent]
  [RequireComponent(typeof (VolumetricLightBeam))]
  [HelpURL("http://saladgamer.com/vlb-doc/comp-dustparticles/")]
  public class VolumetricDustParticles : MonoBehaviour
  {
    public static bool isFeatureSupported = true;
    private static bool ms_NoMainCameraLogged = false;
    private static Camera ms_MainCamera = (Camera) null;
    [Range(0.0f, 1f)]
    public float alpha;
    [Range(0.0001f, 0.1f)]
    public float size;
    public VolumetricDustParticles.Direction direction;
    public float speed;
    public float density;
    [Range(0.0f, 1f)]
    public float spawnMaxDistance;
    public bool cullingEnabled;
    public float cullingMaxDistance;
    private ParticleSystem m_Particles;
    private ParticleSystemRenderer m_Renderer;
    private VolumetricLightBeam m_Master;

    public bool isCulled { get; private set; }

    public bool particlesAreInstantiated
    {
      get
      {
        return Object.op_Implicit((Object) this.m_Particles);
      }
    }

    public int particlesCurrentCount
    {
      get
      {
        if (!Object.op_Implicit((Object) this.m_Particles))
          return 0;
        return this.m_Particles.get_particleCount();
      }
    }

    public int particlesMaxCount
    {
      get
      {
        if (!Object.op_Implicit((Object) this.m_Particles))
          return 0;
        ParticleSystem.MainModule main = this.m_Particles.get_main();
        return ((ParticleSystem.MainModule) ref main).get_maxParticles();
      }
    }

    public Camera mainCamera
    {
      get
      {
        if (!Object.op_Implicit((Object) VolumetricDustParticles.ms_MainCamera))
        {
          VolumetricDustParticles.ms_MainCamera = Camera.get_main();
          if (!Object.op_Implicit((Object) VolumetricDustParticles.ms_MainCamera) && !VolumetricDustParticles.ms_NoMainCameraLogged)
          {
            Debug.LogErrorFormat((Object) ((Component) this).get_gameObject(), "In order to use 'VolumetricDustParticles' culling, you must have a MainCamera defined in your scene.", (object[]) Array.Empty<object>());
            VolumetricDustParticles.ms_NoMainCameraLogged = true;
          }
        }
        return VolumetricDustParticles.ms_MainCamera;
      }
    }

    private void Start()
    {
      this.isCulled = false;
      this.m_Master = (VolumetricLightBeam) ((Component) this).GetComponent<VolumetricLightBeam>();
      Debug.Assert(Object.op_Implicit((Object) this.m_Master));
      this.InstantiateParticleSystem();
      this.SetActiveAndPlay();
    }

    private void InstantiateParticleSystem()
    {
      ParticleSystem[] componentsInChildren = (ParticleSystem[]) ((Component) this).GetComponentsInChildren<ParticleSystem>(true);
      for (int index = componentsInChildren.Length - 1; index >= 0; --index)
        Object.DestroyImmediate((Object) ((Component) componentsInChildren[index]).get_gameObject());
      this.m_Particles = Config.Instance.NewVolumetricDustParticles();
      if (!Object.op_Implicit((Object) this.m_Particles))
        return;
      ((Component) this.m_Particles).get_transform().SetParent(((Component) this).get_transform(), false);
      this.m_Renderer = (ParticleSystemRenderer) ((Component) this.m_Particles).GetComponent<ParticleSystemRenderer>();
    }

    private void OnEnable()
    {
      this.SetActiveAndPlay();
    }

    private void SetActiveAndPlay()
    {
      if (!Object.op_Implicit((Object) this.m_Particles))
        return;
      ((Component) this.m_Particles).get_gameObject().SetActive(true);
      this.SetParticleProperties();
      this.m_Particles.Play(true);
    }

    private void OnDisable()
    {
      if (!Object.op_Implicit((Object) this.m_Particles))
        return;
      ((Component) this.m_Particles).get_gameObject().SetActive(false);
    }

    private void OnDestroy()
    {
      if (Object.op_Implicit((Object) this.m_Particles))
        Object.DestroyImmediate((Object) ((Component) this.m_Particles).get_gameObject());
      this.m_Particles = (ParticleSystem) null;
    }

    private void Update()
    {
      if (Application.get_isPlaying())
        this.UpdateCulling();
      this.SetParticleProperties();
    }

    private void SetParticleProperties()
    {
      if (!Object.op_Implicit((Object) this.m_Particles) || !((Component) this.m_Particles).get_gameObject().get_activeSelf())
        return;
      float num1 = Mathf.Clamp01((float) (1.0 - (double) this.m_Master.fresnelPow / 10.0));
      float num2 = this.m_Master.fadeEnd * this.spawnMaxDistance;
      float num3 = num2 * this.density;
      int num4 = (int) ((double) num3 * 4.0);
      ParticleSystem.MainModule main = this.m_Particles.get_main();
      ParticleSystem.MinMaxCurve startLifetime = ((ParticleSystem.MainModule) ref main).get_startLifetime();
      ((ParticleSystem.MinMaxCurve) ref startLifetime).set_mode((ParticleSystemCurveMode) 3);
      ((ParticleSystem.MinMaxCurve) ref startLifetime).set_constantMin(4f);
      ((ParticleSystem.MinMaxCurve) ref startLifetime).set_constantMax(6f);
      ((ParticleSystem.MainModule) ref main).set_startLifetime(startLifetime);
      ParticleSystem.MinMaxCurve startSize = ((ParticleSystem.MainModule) ref main).get_startSize();
      ((ParticleSystem.MinMaxCurve) ref startSize).set_mode((ParticleSystemCurveMode) 3);
      ((ParticleSystem.MinMaxCurve) ref startSize).set_constantMin(this.size * 0.9f);
      ((ParticleSystem.MinMaxCurve) ref startSize).set_constantMax(this.size * 1.1f);
      ((ParticleSystem.MainModule) ref main).set_startSize(startSize);
      ParticleSystem.MinMaxGradient startColor = ((ParticleSystem.MainModule) ref main).get_startColor();
      if (this.m_Master.colorMode == ColorMode.Flat)
      {
        ((ParticleSystem.MinMaxGradient) ref startColor).set_mode((ParticleSystemGradientMode) 0);
        Color color = this.m_Master.color;
        ref __Null local = ref color.a;
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        ^(float&) ref local = ^(float&) ref local * this.alpha;
        ((ParticleSystem.MinMaxGradient) ref startColor).set_color(color);
      }
      else
      {
        ((ParticleSystem.MinMaxGradient) ref startColor).set_mode((ParticleSystemGradientMode) 1);
        Gradient colorGradient = this.m_Master.colorGradient;
        GradientColorKey[] colorKeys = colorGradient.get_colorKeys();
        GradientAlphaKey[] alphaKeys = colorGradient.get_alphaKeys();
        for (int index = 0; index < alphaKeys.Length; ++index)
        {
          ref __Null local = ref alphaKeys[index].alpha;
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          ^(float&) ref local = ^(float&) ref local * this.alpha;
        }
        Gradient gradient = new Gradient();
        gradient.SetKeys(colorKeys, alphaKeys);
        ((ParticleSystem.MinMaxGradient) ref startColor).set_gradient(gradient);
      }
      ((ParticleSystem.MainModule) ref main).set_startColor(startColor);
      ParticleSystem.MinMaxCurve startSpeed = ((ParticleSystem.MainModule) ref main).get_startSpeed();
      ((ParticleSystem.MinMaxCurve) ref startSpeed).set_constant(this.speed);
      ((ParticleSystem.MainModule) ref main).set_startSpeed(startSpeed);
      ((ParticleSystem.MainModule) ref main).set_maxParticles(num4);
      ParticleSystem.ShapeModule shape = this.m_Particles.get_shape();
      ((ParticleSystem.ShapeModule) ref shape).set_shapeType((ParticleSystemShapeType) 8);
      ((ParticleSystem.ShapeModule) ref shape).set_radius(this.m_Master.coneRadiusStart * Mathf.Lerp(0.3f, 1f, num1));
      ((ParticleSystem.ShapeModule) ref shape).set_angle(this.m_Master.coneAngle * 0.5f * Mathf.Lerp(0.7f, 1f, num1));
      ((ParticleSystem.ShapeModule) ref shape).set_length(num2);
      ((ParticleSystem.ShapeModule) ref shape).set_arc(360f);
      ((ParticleSystem.ShapeModule) ref shape).set_randomDirectionAmount(this.direction == VolumetricDustParticles.Direction.Random ? 1f : 0.0f);
      ParticleSystem.EmissionModule emission = this.m_Particles.get_emission();
      ParticleSystem.MinMaxCurve rateOverTime = ((ParticleSystem.EmissionModule) ref emission).get_rateOverTime();
      ((ParticleSystem.MinMaxCurve) ref rateOverTime).set_constant(num3);
      ((ParticleSystem.EmissionModule) ref emission).set_rateOverTime(rateOverTime);
      if (!Object.op_Implicit((Object) this.m_Renderer))
        return;
      ((Renderer) this.m_Renderer).set_sortingLayerID(this.m_Master.sortingLayerID);
      ((Renderer) this.m_Renderer).set_sortingOrder(this.m_Master.sortingOrder);
    }

    private void UpdateCulling()
    {
      if (!Object.op_Implicit((Object) this.m_Particles))
        return;
      bool flag = true;
      if (this.cullingEnabled && this.m_Master.hasGeometry)
      {
        if (Object.op_Implicit((Object) this.mainCamera))
        {
          float num = this.cullingMaxDistance * this.cullingMaxDistance;
          Bounds bounds = this.m_Master.bounds;
          flag = (double) ((Bounds) ref bounds).SqrDistance(((Component) this.mainCamera).get_transform().get_position()) <= (double) num;
        }
        else
          this.cullingEnabled = false;
      }
      if (((Component) this.m_Particles).get_gameObject().get_activeSelf() != flag)
      {
        ((Component) this.m_Particles).get_gameObject().SetActive(flag);
        this.isCulled = !flag;
      }
      if (!flag || this.m_Particles.get_isPlaying())
        return;
      this.m_Particles.Play();
    }

    public VolumetricDustParticles()
    {
      base.\u002Ector();
    }

    public enum Direction
    {
      Beam,
      Random,
    }
  }
}
