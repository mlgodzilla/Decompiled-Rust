// Decompiled with JetBrains decompiler
// Type: Poolable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class Poolable : MonoBehaviour, IClientComponent, IPrefabPostProcess
{
  [HideInInspector]
  public uint prefabID;
  [HideInInspector]
  public Behaviour[] behaviours;
  [HideInInspector]
  public Rigidbody[] rigidbodies;
  [HideInInspector]
  public Collider[] colliders;
  [HideInInspector]
  public LODGroup[] lodgroups;
  [HideInInspector]
  public Renderer[] renderers;
  [HideInInspector]
  public ParticleSystem[] particles;
  [HideInInspector]
  public bool[] behaviourStates;
  [HideInInspector]
  public bool[] rigidbodyStates;
  [HideInInspector]
  public bool[] colliderStates;
  [HideInInspector]
  public bool[] lodgroupStates;
  [HideInInspector]
  public bool[] rendererStates;

  public int ClientCount
  {
    get
    {
      if (Object.op_Inequality((Object) ((Component) this).GetComponent<LootPanel>(), (Object) null))
        return 1;
      return (PrefabAttribute) ((Component) this).GetComponent<DecorComponent>() != (PrefabAttribute) null || Object.op_Inequality((Object) ((Component) this).GetComponent<BuildingBlock>(), (Object) null) || (Object.op_Inequality((Object) ((Component) this).GetComponent<Door>(), (Object) null) || Object.op_Inequality((Object) ((Component) this).GetComponent<Projectile>(), (Object) null)) ? 100 : 10;
    }
  }

  public int ServerCount
  {
    get
    {
      return 0;
    }
  }

  public void PostProcess(
    IPrefabProcessor preProcess,
    GameObject rootObj,
    string name,
    bool serverside,
    bool clientside,
    bool bundling)
  {
    if (bundling)
      return;
    this.Initialize(StringPool.Get(name));
  }

  public void Initialize(uint id)
  {
    this.prefabID = id;
    this.behaviours = ((IEnumerable) ((Component) this).get_gameObject().GetComponentsInChildren(typeof (Behaviour), true)).OfType<Behaviour>().ToArray<Behaviour>();
    this.rigidbodies = (Rigidbody[]) ((Component) this).get_gameObject().GetComponentsInChildren<Rigidbody>(true);
    this.colliders = (Collider[]) ((Component) this).get_gameObject().GetComponentsInChildren<Collider>(true);
    this.lodgroups = (LODGroup[]) ((Component) this).get_gameObject().GetComponentsInChildren<LODGroup>(true);
    this.renderers = (Renderer[]) ((Component) this).get_gameObject().GetComponentsInChildren<Renderer>(true);
    this.particles = (ParticleSystem[]) ((Component) this).get_gameObject().GetComponentsInChildren<ParticleSystem>(true);
    this.behaviourStates = new bool[this.behaviours.Length];
    this.rigidbodyStates = new bool[this.rigidbodies.Length];
    this.colliderStates = new bool[this.colliders.Length];
    this.lodgroupStates = new bool[this.lodgroups.Length];
    this.rendererStates = new bool[this.renderers.Length];
  }

  public void EnterPool()
  {
    if (Object.op_Inequality((Object) ((Component) this).get_transform().get_parent(), (Object) null))
      ((Component) this).get_transform().SetParent((Transform) null, false);
    if (Pool.mode <= 1)
    {
      if (!((Component) this).get_gameObject().get_activeSelf())
        return;
      ((Component) this).get_gameObject().SetActive(false);
    }
    else
    {
      this.SetBehaviourEnabled(false);
      this.SetComponentEnabled(false);
      if (((Component) this).get_gameObject().get_activeSelf())
        return;
      ((Component) this).get_gameObject().SetActive(true);
    }
  }

  public void LeavePool()
  {
    if (Pool.mode <= 1)
      return;
    this.SetComponentEnabled(true);
  }

  public void SetBehaviourEnabled(bool state)
  {
    try
    {
      if (!state)
      {
        for (int index = 0; index < this.behaviours.Length; ++index)
        {
          Behaviour behaviour = this.behaviours[index];
          this.behaviourStates[index] = behaviour.get_enabled();
          behaviour.set_enabled(false);
        }
        for (int index = 0; index < this.particles.Length; ++index)
        {
          ParticleSystem particle = this.particles[index];
          particle.Stop();
          particle.Clear();
        }
      }
      else
      {
        for (int index = 0; index < this.particles.Length; ++index)
        {
          ParticleSystem particle = this.particles[index];
          if (particle.get_playOnAwake())
            particle.Play();
        }
        for (int index = 0; index < this.behaviours.Length; ++index)
          this.behaviours[index].set_enabled(this.behaviourStates[index]);
      }
    }
    catch (Exception ex)
    {
      Debug.LogError((object) ("Pooling error: " + ((Object) this).get_name() + " (" + ex.Message + ")"));
    }
  }

  public void SetComponentEnabled(bool state)
  {
    try
    {
      if (!state)
      {
        for (int index = 0; index < this.renderers.Length; ++index)
        {
          Renderer renderer = this.renderers[index];
          this.rendererStates[index] = renderer.get_enabled();
          renderer.set_enabled(false);
        }
        for (int index = 0; index < this.lodgroups.Length; ++index)
        {
          LODGroup lodgroup = this.lodgroups[index];
          this.lodgroupStates[index] = lodgroup.get_enabled();
          lodgroup.set_enabled(false);
        }
        for (int index = 0; index < this.colliders.Length; ++index)
        {
          Collider collider = this.colliders[index];
          this.colliderStates[index] = collider.get_enabled();
          collider.set_enabled(false);
        }
        for (int index = 0; index < this.rigidbodies.Length; ++index)
        {
          Rigidbody rigidbody = this.rigidbodies[index];
          this.rigidbodyStates[index] = rigidbody.get_isKinematic();
          rigidbody.set_isKinematic(true);
          rigidbody.set_detectCollisions(false);
        }
      }
      else
      {
        for (int index = 0; index < this.renderers.Length; ++index)
          this.renderers[index].set_enabled(this.rendererStates[index]);
        for (int index = 0; index < this.lodgroups.Length; ++index)
          this.lodgroups[index].set_enabled(this.lodgroupStates[index]);
        for (int index = 0; index < this.colliders.Length; ++index)
          this.colliders[index].set_enabled(this.colliderStates[index]);
        for (int index = 0; index < this.rigidbodies.Length; ++index)
        {
          Rigidbody rigidbody = this.rigidbodies[index];
          rigidbody.set_isKinematic(this.rigidbodyStates[index]);
          rigidbody.set_detectCollisions(true);
        }
      }
    }
    catch (Exception ex)
    {
      Debug.LogError((object) ("Pooling error: " + ((Object) this).get_name() + " (" + ex.Message + ")"));
    }
  }

  public Poolable()
  {
    base.\u002Ector();
  }
}
