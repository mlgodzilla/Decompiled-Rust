// Decompiled with JetBrains decompiler
// Type: ColliderGrid
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Rust;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ColliderGrid : SingletonComponent<ColliderGrid>, IServerComponent
{
  public static bool Paused;
  public GameObjectRef BatchPrefab;
  public float CellSize;
  public float MaxMilliseconds;
  private WorldSpaceGrid<ColliderCell> grid;
  private Stopwatch watch;

  protected void OnEnable()
  {
    ((MonoBehaviour) this).StartCoroutine(this.UpdateCoroutine());
  }

  public static void RefreshAll()
  {
    if (!Object.op_Implicit((Object) SingletonComponent<ColliderGrid>.Instance))
      return;
    ((ColliderGrid) SingletonComponent<ColliderGrid>.Instance).Refresh();
  }

  public void Refresh()
  {
    if (this.grid == null)
      this.Init();
    for (int index1 = 0; index1 < this.grid.CellCount; ++index1)
    {
      for (int index2 = 0; index2 < this.grid.CellCount; ++index2)
        this.grid.get_Item(index1, index2).Refresh();
    }
  }

  public ColliderCell this[Vector3 worldPos]
  {
    get
    {
      if (this.grid == null)
        this.Init();
      return this.grid.get_Item(worldPos);
    }
  }

  private void Init()
  {
    this.grid = new WorldSpaceGrid<ColliderCell>((float) (TerrainMeta.Size.x * 2.0), this.CellSize);
    for (int index1 = 0; index1 < this.grid.CellCount; ++index1)
    {
      for (int index2 = 0; index2 < this.grid.CellCount; ++index2)
        this.grid.set_Item(index1, index2, new ColliderCell(this, this.grid.GridToWorldCoords(new Vector2i(index1, index2))));
    }
  }

  public MeshColliderBatch CreateInstance()
  {
    GameObject prefab = GameManager.server.CreatePrefab(this.BatchPrefab.resourcePath, true);
    SceneManager.MoveGameObjectToScene(prefab, Generic.BatchingScene);
    return (MeshColliderBatch) prefab.GetComponent<MeshColliderBatch>();
  }

  public void RecycleInstance(MeshColliderBatch instance)
  {
    GameManager.server.Retire(((Component) instance).get_gameObject());
  }

  public int MeshCount()
  {
    if (this.grid == null)
      return 0;
    int num = 0;
    for (int index1 = 0; index1 < this.grid.CellCount; ++index1)
    {
      for (int index2 = 0; index2 < this.grid.CellCount; ++index2)
        num += this.grid.get_Item(index1, index2).MeshCount();
    }
    return num;
  }

  public int BatchedMeshCount()
  {
    if (this.grid == null)
      return 0;
    int num = 0;
    for (int index1 = 0; index1 < this.grid.CellCount; ++index1)
    {
      for (int index2 = 0; index2 < this.grid.CellCount; ++index2)
        num += this.grid.get_Item(index1, index2).BatchedMeshCount();
    }
    return num;
  }

  public bool NeedsTimeout
  {
    get
    {
      return this.watch.Elapsed.TotalMilliseconds > (double) this.MaxMilliseconds;
    }
  }

  public void ResetTimeout()
  {
    this.watch.Reset();
    this.watch.Start();
  }

  private IEnumerator UpdateCoroutine()
  {
label_1:
    do
    {
      yield return (object) CoroutineEx.waitForEndOfFrame;
    }
    while (Application.isReceiving != null || Application.isLoading != null || (ColliderGrid.Paused || this.grid == null));
    this.ResetTimeout();
    for (int x = 0; x < this.grid.CellCount; ++x)
    {
      for (int z = 0; z < this.grid.CellCount; ++z)
      {
        ColliderCell colliderCell = this.grid.get_Item(x, z);
        if (colliderCell.NeedsRefresh())
        {
          IEnumerator enumerator = colliderCell.RefreshAsync();
          while (enumerator.MoveNext())
            yield return enumerator.Current;
          enumerator = (IEnumerator) null;
        }
        if (this.NeedsTimeout)
        {
          yield return (object) CoroutineEx.waitForEndOfFrame;
          this.ResetTimeout();
        }
      }
    }
    goto label_1;
  }

  public ColliderGrid()
  {
    base.\u002Ector();
  }
}
