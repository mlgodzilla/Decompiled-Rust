// Decompiled with JetBrains decompiler
// Type: SpawnDistribution
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class SpawnDistribution
{
  private Dictionary<uint, int> dict = new Dictionary<uint, int>();
  private ByteQuadtree quadtree = new ByteQuadtree();
  internal SpawnHandler Handler;
  public float Density;
  internal int Count;
  private WorldSpaceGrid<int> grid;
  private Vector3 origin;
  private Vector3 area;

  public SpawnDistribution(SpawnHandler handler, byte[] baseValues, Vector3 origin, Vector3 area)
  {
    this.Handler = handler;
    this.quadtree.UpdateValues(baseValues);
    this.origin = origin;
    float num = 0.0f;
    for (int index = 0; index < baseValues.Length; ++index)
      num += (float) baseValues[index];
    this.Density = num / (float) ((int) byte.MaxValue * baseValues.Length);
    this.Count = 0;
    this.area = new Vector3((float) area.x / (float) this.quadtree.Size, (float) area.y, (float) area.z / (float) this.quadtree.Size);
    this.grid = new WorldSpaceGrid<int>((float) area.x, 20f);
  }

  public bool Sample(
    out Vector3 spawnPos,
    out Quaternion spawnRot,
    bool alignToNormal = false,
    float dithering = 0.0f)
  {
    return this.Sample(out spawnPos, out spawnRot, this.SampleNode(), alignToNormal, dithering);
  }

  public bool Sample(
    out Vector3 spawnPos,
    out Quaternion spawnRot,
    ByteQuadtree.Element node,
    bool alignToNormal = false,
    float dithering = 0.0f)
  {
    if (Object.op_Equality((Object) this.Handler, (Object) null) || Object.op_Equality((Object) TerrainMeta.HeightMap, (Object) null))
    {
      spawnPos = Vector3.get_zero();
      spawnRot = Quaternion.get_identity();
      return false;
    }
    LayerMask placementMask = this.Handler.PlacementMask;
    LayerMask placementCheckMask = this.Handler.PlacementCheckMask;
    float placementCheckHeight = this.Handler.PlacementCheckHeight;
    LayerMask radiusCheckMask = this.Handler.RadiusCheckMask;
    float radiusCheckDistance = this.Handler.RadiusCheckDistance;
    for (int index = 0; index < 15; ++index)
    {
      spawnPos = this.origin;
      ref __Null local1 = ref spawnPos.x;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(float&) ref local1 = ^(float&) ref local1 + (float) (node.Coords.x * this.area.x);
      ref __Null local2 = ref spawnPos.z;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(float&) ref local2 = ^(float&) ref local2 + (float) (node.Coords.y * this.area.z);
      ref __Null local3 = ref spawnPos.x;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(float&) ref local3 = ^(float&) ref local3 + Random.get_value() * (float) this.area.x;
      ref __Null local4 = ref spawnPos.z;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(float&) ref local4 = ^(float&) ref local4 + Random.get_value() * (float) this.area.z;
      ref __Null local5 = ref spawnPos.x;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(float&) ref local5 = ^(float&) ref local5 + Random.Range(-dithering, dithering);
      ref __Null local6 = ref spawnPos.z;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(float&) ref local6 = ^(float&) ref local6 + Random.Range(-dithering, dithering);
      Vector3 vector3;
      ((Vector3) ref vector3).\u002Ector((float) spawnPos.x, TerrainMeta.HeightMap.GetHeight(spawnPos), (float) spawnPos.z);
      if (vector3.y > spawnPos.y)
      {
        RaycastHit raycastHit;
        if (LayerMask.op_Implicit(placementCheckMask) != 0 && Physics.Raycast(Vector3.op_Addition(vector3, Vector3.op_Multiply(Vector3.get_up(), placementCheckHeight)), Vector3.get_down(), ref raycastHit, placementCheckHeight, LayerMask.op_Implicit(placementCheckMask)))
        {
          if ((1 << ((Component) ((RaycastHit) ref raycastHit).get_transform()).get_gameObject().get_layer() & LayerMask.op_Implicit(placementMask)) != 0)
            vector3.y = ((RaycastHit) ref raycastHit).get_point().y;
          else
            continue;
        }
        if (LayerMask.op_Implicit(radiusCheckMask) == 0 || !Physics.CheckSphere(vector3, radiusCheckDistance, LayerMask.op_Implicit(radiusCheckMask)))
        {
          spawnPos.y = vector3.y;
          spawnRot = Quaternion.Euler(new Vector3(0.0f, Random.Range(0.0f, 360f), 0.0f));
          if (alignToNormal)
          {
            Vector3 normal = TerrainMeta.HeightMap.GetNormal(spawnPos);
            spawnRot = QuaternionEx.LookRotationForcedUp(Quaternion.op_Multiply(spawnRot, Vector3.get_forward()), normal);
          }
          return true;
        }
      }
    }
    spawnPos = Vector3.get_zero();
    spawnRot = Quaternion.get_identity();
    return false;
  }

  public ByteQuadtree.Element SampleNode()
  {
    ByteQuadtree.Element element = this.quadtree.Root;
    while (!element.IsLeaf)
      element = element.RandChild;
    return element;
  }

  public void AddInstance(Spawnable spawnable)
  {
    this.UpdateCount(spawnable, 1);
  }

  public void RemoveInstance(Spawnable spawnable)
  {
    this.UpdateCount(spawnable, -1);
  }

  private void UpdateCount(Spawnable spawnable, int delta)
  {
    this.Count += delta;
    WorldSpaceGrid<int> grid = this.grid;
    Vector3 spawnPosition = spawnable.SpawnPosition;
    grid.set_Item(spawnPosition, grid.get_Item(spawnPosition) + delta);
    BaseEntity component = (BaseEntity) ((Component) spawnable).GetComponent<BaseEntity>();
    if (!Object.op_Implicit((Object) component))
      return;
    int num1;
    if (this.dict.TryGetValue(component.prefabID, out num1))
    {
      this.dict[component.prefabID] = num1 + delta;
    }
    else
    {
      int num2 = delta;
      this.dict.Add(component.prefabID, num2);
    }
  }

  public int GetCount(uint prefabID)
  {
    int num;
    this.dict.TryGetValue(prefabID, out num);
    return num;
  }

  public int GetCount(Vector3 position)
  {
    return this.grid.get_Item(position);
  }

  public float GetGridCellArea()
  {
    return (float) this.grid.CellArea;
  }
}
