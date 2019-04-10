// Decompiled with JetBrains decompiler
// Type: TerrainPath
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class TerrainPath : TerrainExtension
{
  public List<PathList> Roads = new List<PathList>();
  public List<PathList> Rivers = new List<PathList>();
  public List<PathList> Powerlines = new List<PathList>();
  public List<MonumentInfo> Monuments = new List<MonumentInfo>();
  public List<RiverInfo> RiverObjs = new List<RiverInfo>();
  public List<LakeInfo> LakeObjs = new List<LakeInfo>();
  internal List<Vector3> OceanPatrolClose = new List<Vector3>();
  internal List<Vector3> OceanPatrolFar = new List<Vector3>();
  private Dictionary<string, List<PowerlineNode>> wires = new Dictionary<string, List<PowerlineNode>>();

  public void Clear()
  {
    this.Roads.Clear();
    this.Rivers.Clear();
    this.Powerlines.Clear();
  }

  public void AddWire(PowerlineNode node)
  {
    string name = ((Object) ((Component) node).get_transform().get_root()).get_name();
    if (!this.wires.ContainsKey(name))
      this.wires.Add(name, new List<PowerlineNode>());
    this.wires[name].Add(node);
  }

  public void CreateWires()
  {
    List<GameObject> objects = new List<GameObject>();
    int num = 0;
    Material material = (Material) null;
    foreach (KeyValuePair<string, List<PowerlineNode>> wire in this.wires)
    {
      foreach (PowerlineNode powerlineNode in wire.Value)
      {
        MegaWireConnectionHelper component = (MegaWireConnectionHelper) ((Component) powerlineNode).GetComponent<MegaWireConnectionHelper>();
        if (Object.op_Implicit((Object) component))
        {
          if (objects.Count == 0)
          {
            material = powerlineNode.WireMaterial;
            num = ((List<MegaWireConnectionDef>) component.connections).Count;
          }
          else
          {
            GameObject gameObject = objects[objects.Count - 1];
            if (!Object.op_Inequality((Object) powerlineNode.WireMaterial, (Object) material) && ((List<MegaWireConnectionDef>) component.connections).Count == num)
            {
              Vector3 vector3 = Vector3.op_Subtraction(gameObject.get_transform().get_position(), ((Component) powerlineNode).get_transform().get_position());
              if ((double) ((Vector3) ref vector3).get_sqrMagnitude() <= (double) powerlineNode.MaxDistance * (double) powerlineNode.MaxDistance)
                goto label_10;
            }
            this.CreateWire(wire.Key, objects, material);
            objects.Clear();
          }
label_10:
          objects.Add(((Component) powerlineNode).get_gameObject());
        }
      }
      this.CreateWire(wire.Key, objects, material);
      objects.Clear();
    }
  }

  private void CreateWire(string name, List<GameObject> objects, Material material)
  {
    if (objects.Count < 3 || !Object.op_Inequality((Object) material, (Object) null))
      return;
    MegaWire megaWire = MegaWire.Create((MegaWire) null, objects, material, "Powerline Wires", (MegaWire) null, 1f, 0.1f);
    if (!Object.op_Implicit((Object) megaWire))
      return;
    ((Behaviour) megaWire).set_enabled(false);
    megaWire.RunPhysics((float) megaWire.warmPhysicsTime);
    ((Component) megaWire).get_gameObject().SetHierarchyGroup(name, true, false);
  }
}
