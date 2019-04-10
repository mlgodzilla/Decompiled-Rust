// Decompiled with JetBrains decompiler
// Type: PrefabAttribute
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public abstract class PrefabAttribute : MonoBehaviour, IPrefabPreProcess
{
  public static PrefabAttribute.Library server = new PrefabAttribute.Library(false, true);
  [NonSerialized]
  public Vector3 worldPosition;
  [NonSerialized]
  public Quaternion worldRotation;
  [NonSerialized]
  public Vector3 worldForward;
  [NonSerialized]
  public Vector3 localPosition;
  [NonSerialized]
  public Vector3 localScale;
  [NonSerialized]
  public Quaternion localRotation;
  [NonSerialized]
  public string fullName;
  [NonSerialized]
  public string hierachyName;
  [NonSerialized]
  public uint prefabID;
  [NonSerialized]
  public int instanceID;
  [NonSerialized]
  public PrefabAttribute.Library prefabAttribute;
  [NonSerialized]
  public GameManager gameManager;
  [NonSerialized]
  public bool isServer;

  public bool isClient
  {
    get
    {
      return !this.isServer;
    }
  }

  public virtual void PreProcess(
    IPrefabProcessor preProcess,
    GameObject rootObj,
    string name,
    bool serverside,
    bool clientside,
    bool bundling)
  {
    if (bundling)
      return;
    this.fullName = name;
    this.hierachyName = ((Component) this).get_transform().GetRecursiveName("");
    this.prefabID = StringPool.Get(name);
    this.instanceID = ((Object) this).GetInstanceID();
    this.worldPosition = ((Component) this).get_transform().get_position();
    this.worldRotation = ((Component) this).get_transform().get_rotation();
    this.worldForward = ((Component) this).get_transform().get_forward();
    this.localPosition = ((Component) this).get_transform().get_localPosition();
    this.localScale = ((Component) this).get_transform().get_localScale();
    this.localRotation = ((Component) this).get_transform().get_localRotation();
    if (serverside)
    {
      this.prefabAttribute = PrefabAttribute.server;
      this.gameManager = GameManager.server;
      this.isServer = true;
    }
    this.AttributeSetup(rootObj, name, serverside, clientside, bundling);
    if (serverside)
      PrefabAttribute.server.Add(this.prefabID, this);
    preProcess.RemoveComponent((Component) this);
    preProcess.NominateForDeletion(((Component) this).get_gameObject());
  }

  protected virtual void AttributeSetup(
    GameObject rootObj,
    string name,
    bool serverside,
    bool clientside,
    bool bundling)
  {
  }

  protected abstract System.Type GetIndexedType();

  public static bool operator ==(PrefabAttribute x, PrefabAttribute y)
  {
    return PrefabAttribute.ComparePrefabAttribute(x, y);
  }

  public static bool operator !=(PrefabAttribute x, PrefabAttribute y)
  {
    return !PrefabAttribute.ComparePrefabAttribute(x, y);
  }

  public virtual bool Equals(object o)
  {
    return PrefabAttribute.ComparePrefabAttribute(this, (PrefabAttribute) o);
  }

  public virtual int GetHashCode()
  {
    if (this.hierachyName == null)
      return ((Object) this).GetHashCode();
    return this.hierachyName.GetHashCode();
  }

  public static implicit operator bool(PrefabAttribute exists)
  {
    return exists != null;
  }

  internal static bool ComparePrefabAttribute(PrefabAttribute x, PrefabAttribute y)
  {
    bool flag1 = (object) x == null;
    bool flag2 = (object) y == null;
    if (flag1 & flag2)
      return true;
    if (flag1 | flag2)
      return false;
    return x.instanceID == y.instanceID;
  }

  public virtual string ToString()
  {
    if ((object) this == null)
      return "null";
    return this.hierachyName;
  }

  protected PrefabAttribute()
  {
    base.\u002Ector();
  }

  public class AttributeCollection
  {
    private Dictionary<System.Type, List<PrefabAttribute>> attributes = new Dictionary<System.Type, List<PrefabAttribute>>();
    private Dictionary<System.Type, object> cache = new Dictionary<System.Type, object>();

    internal List<PrefabAttribute> Find(System.Type t)
    {
      List<PrefabAttribute> prefabAttributeList1;
      if (this.attributes.TryGetValue(t, out prefabAttributeList1))
        return prefabAttributeList1;
      List<PrefabAttribute> prefabAttributeList2 = new List<PrefabAttribute>();
      this.attributes.Add(t, prefabAttributeList2);
      return prefabAttributeList2;
    }

    public T[] Find<T>()
    {
      if (this.cache == null)
        this.cache = new Dictionary<System.Type, object>();
      object obj;
      if (this.cache.TryGetValue(typeof (T), out obj))
        return (T[]) obj;
      object array = (object) this.Find(typeof (T)).Cast<T>().ToArray<T>();
      this.cache.Add(typeof (T), array);
      return (T[]) array;
    }

    public void Add(PrefabAttribute attribute)
    {
      List<PrefabAttribute> prefabAttributeList = this.Find(attribute.GetIndexedType());
      Assert.IsTrue(!prefabAttributeList.Contains(attribute), "AttributeCollection.Add: Adding twice to list");
      prefabAttributeList.Add(attribute);
      this.cache = (Dictionary<System.Type, object>) null;
    }
  }

  public class Library
  {
    public Dictionary<uint, PrefabAttribute.AttributeCollection> prefabs = new Dictionary<uint, PrefabAttribute.AttributeCollection>();
    public bool clientside;
    public bool serverside;

    public Library(bool clientside, bool serverside)
    {
      this.clientside = clientside;
      this.serverside = serverside;
    }

    public PrefabAttribute.AttributeCollection Find(uint prefabID, bool warmup = true)
    {
      PrefabAttribute.AttributeCollection attributeCollection1;
      if (this.prefabs.TryGetValue(prefabID, out attributeCollection1))
        return attributeCollection1;
      PrefabAttribute.AttributeCollection attributeCollection2 = new PrefabAttribute.AttributeCollection();
      this.prefabs.Add(prefabID, attributeCollection2);
      if (warmup && (!this.clientside || this.serverside))
      {
        if (!this.clientside && this.serverside)
          GameManager.server.FindPrefab(prefabID);
        else if (this.clientside)
        {
          int num = this.serverside ? 1 : 0;
        }
      }
      return attributeCollection2;
    }

    public T Find<T>(uint prefabID) where T : PrefabAttribute
    {
      T[] objArray = this.Find(prefabID, true).Find<T>();
      if (objArray.Length == 0)
        return default (T);
      return objArray[0];
    }

    public T[] FindAll<T>(uint prefabID) where T : PrefabAttribute
    {
      return this.Find(prefabID, true).Find<T>();
    }

    public void Add(uint prefabID, PrefabAttribute attribute)
    {
      this.Find(prefabID, false).Add(attribute);
    }
  }
}
