// Decompiled with JetBrains decompiler
// Type: NetworkVisibilityGrid
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Network;
using Network.Visibility;
using Rust;
using System.Collections.Generic;
using UnityEngine;

public class NetworkVisibilityGrid : MonoBehaviour, Provider
{
  public int startID;
  public int gridSize;
  public int cellCount;
  public int visibilityRadius;
  public float switchTolerance;

  private void Awake()
  {
    Debug.Assert(Net.sv != null, "Network.Net.sv is NULL when creating Visibility Grid");
    Debug.Assert(((Server) Net.sv).visibility == null, "Network.Net.sv.visibility is being set multiple times");
    ((Server) Net.sv).visibility = (__Null) new Manager((Provider) this);
  }

  private void OnDisable()
  {
    if (Application.isQuitting != null || Net.sv == null || ((Server) Net.sv).visibility == null)
      return;
    ((Manager) ((Server) Net.sv).visibility).Dispose();
    ((Server) Net.sv).visibility = null;
  }

  private void OnDrawGizmosSelected()
  {
    Gizmos.set_color(Color.get_blue());
    float num1 = this.CellSize();
    float num2 = (float) this.gridSize / 2f;
    Vector3 position = ((Component) this).get_transform().get_position();
    for (int index = 0; index <= this.cellCount; ++index)
    {
      float num3 = (float) (-(double) num2 + (double) index * (double) num1 - (double) num1 / 2.0);
      Gizmos.DrawLine(new Vector3(num2, (float) position.y, num3), new Vector3(-num2, (float) position.y, num3));
      Gizmos.DrawLine(new Vector3(num3, (float) position.y, num2), new Vector3(num3, (float) position.y, -num2));
    }
  }

  private int PositionToGrid(float f)
  {
    f += (float) this.gridSize / 2f;
    return Mathf.RoundToInt(f / this.CellSize());
  }

  private float GridToPosition(int i)
  {
    return (float) ((double) i * (double) this.CellSize() - (double) this.gridSize / 2.0);
  }

  public uint CoordToID(int x, int y)
  {
    return (uint) (x * this.cellCount + y + this.startID);
  }

  public uint GetID(Vector3 vPos)
  {
    int grid1 = this.PositionToGrid((float) vPos.x);
    int grid2 = this.PositionToGrid((float) vPos.z);
    if (grid1 < 0 || grid1 >= this.cellCount || (grid2 < 0 || grid2 >= this.cellCount))
      return 0;
    uint id = this.CoordToID(grid1, grid2);
    if ((long) id < (long) this.startID)
      Debug.LogError((object) ("NetworkVisibilityGrid.GetID - group is below range " + (object) grid1 + " " + (object) grid2 + " " + (object) id + " " + (object) this.cellCount));
    if ((long) id > (long) (this.startID + this.cellCount * this.cellCount))
      Debug.LogError((object) ("NetworkVisibilityGrid.GetID - group is higher than range " + (object) grid1 + " " + (object) grid2 + " " + (object) id + " " + (object) this.cellCount));
    return id;
  }

  public Vector3 GetPosition(uint uid)
  {
    uid -= (uint) this.startID;
    return new Vector3(this.GridToPosition((int) ((long) uid / (long) this.cellCount)), 0.0f, this.GridToPosition((int) ((long) uid % (long) this.cellCount)));
  }

  public Bounds GetBounds(uint uid)
  {
    float num = this.CellSize();
    return new Bounds(this.GetPosition(uid), new Vector3(num, 1048576f, num));
  }

  public float CellSize()
  {
    return (float) this.gridSize / (float) this.cellCount;
  }

  public void OnGroupAdded(Group group)
  {
    group.bounds = (__Null) this.GetBounds((uint) group.ID);
  }

  public bool IsInside(Group group, Vector3 vPos)
  {
    if (((false ? 1 : (group.ID == 0 ? 1 : 0)) != 0 ? 1 : (((Bounds) ref group.bounds).Contains(vPos) ? 1 : 0)) == 0)
      return (double) ((Bounds) ref group.bounds).SqrDistance(vPos) < (double) this.switchTolerance;
    return true;
  }

  public Group GetGroup(Vector3 vPos)
  {
    uint id = this.GetID(vPos);
    if (id == 0U)
      return (Group) null;
    Group group = ((Manager) ((Server) Net.sv).visibility).Get(id);
    if (!this.IsInside(group, vPos))
    {
      float num = ((Bounds) ref group.bounds).SqrDistance(vPos);
      Debug.Log((object) ("Group is inside is all fucked " + (object) id + "/" + (object) num + "/" + (object) vPos));
    }
    return group;
  }

  public void GetVisibleFrom(Group group, List<Group> groups)
  {
    groups.Add(((Manager) ((Server) Net.sv).visibility).Get(0U));
    uint id = (uint) group.ID;
    if ((long) id < (long) this.startID)
      return;
    uint num = id - (uint) this.startID;
    int x = (int) ((long) num / (long) this.cellCount);
    int y = (int) ((long) num % (long) this.cellCount);
    groups.Add(((Manager) ((Server) Net.sv).visibility).Get(this.CoordToID(x, y)));
    for (int index1 = 1; index1 <= this.visibilityRadius; ++index1)
    {
      groups.Add(((Manager) ((Server) Net.sv).visibility).Get(this.CoordToID(x - index1, y)));
      groups.Add(((Manager) ((Server) Net.sv).visibility).Get(this.CoordToID(x + index1, y)));
      groups.Add(((Manager) ((Server) Net.sv).visibility).Get(this.CoordToID(x, y - index1)));
      groups.Add(((Manager) ((Server) Net.sv).visibility).Get(this.CoordToID(x, y + index1)));
      for (int index2 = 1; index2 < index1; ++index2)
      {
        groups.Add(((Manager) ((Server) Net.sv).visibility).Get(this.CoordToID(x - index1, y - index2)));
        groups.Add(((Manager) ((Server) Net.sv).visibility).Get(this.CoordToID(x - index1, y + index2)));
        groups.Add(((Manager) ((Server) Net.sv).visibility).Get(this.CoordToID(x + index1, y - index2)));
        groups.Add(((Manager) ((Server) Net.sv).visibility).Get(this.CoordToID(x + index1, y + index2)));
        groups.Add(((Manager) ((Server) Net.sv).visibility).Get(this.CoordToID(x - index2, y - index1)));
        groups.Add(((Manager) ((Server) Net.sv).visibility).Get(this.CoordToID(x + index2, y - index1)));
        groups.Add(((Manager) ((Server) Net.sv).visibility).Get(this.CoordToID(x - index2, y + index1)));
        groups.Add(((Manager) ((Server) Net.sv).visibility).Get(this.CoordToID(x + index2, y + index1)));
      }
      groups.Add(((Manager) ((Server) Net.sv).visibility).Get(this.CoordToID(x - index1, y - index1)));
      groups.Add(((Manager) ((Server) Net.sv).visibility).Get(this.CoordToID(x - index1, y + index1)));
      groups.Add(((Manager) ((Server) Net.sv).visibility).Get(this.CoordToID(x + index1, y - index1)));
      groups.Add(((Manager) ((Server) Net.sv).visibility).Get(this.CoordToID(x + index1, y + index1)));
    }
  }

  public NetworkVisibilityGrid()
  {
    base.\u002Ector();
  }
}
