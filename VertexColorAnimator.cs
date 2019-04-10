// Decompiled with JetBrains decompiler
// Type: VertexColorAnimator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class VertexColorAnimator : MonoBehaviour
{
  public List<MeshHolder> animationMeshes;
  public List<float> animationKeyframes;
  public float timeScale;
  public int mode;
  private float elapsedTime;

  public void initLists()
  {
    this.animationMeshes = new List<MeshHolder>();
    this.animationKeyframes = new List<float>();
  }

  public void addMesh(Mesh mesh, float atPosition)
  {
    MeshHolder meshHolder = new MeshHolder();
    meshHolder.setAnimationData(mesh);
    this.animationMeshes.Add(meshHolder);
    this.animationKeyframes.Add(atPosition);
  }

  private void Start()
  {
    this.elapsedTime = 0.0f;
  }

  public void replaceKeyframe(int frameIndex, Mesh mesh)
  {
    this.animationMeshes[frameIndex].setAnimationData(mesh);
  }

  public void deleteKeyframe(int frameIndex)
  {
    this.animationMeshes.RemoveAt(frameIndex);
    this.animationKeyframes.RemoveAt(frameIndex);
  }

  public void scrobble(float scrobblePos)
  {
    if (this.animationMeshes.Count == 0)
      return;
    Color[] _vertexColors = new Color[((MeshFilter) ((Component) this).GetComponent<MeshFilter>()).get_sharedMesh().get_colors().Length];
    int index1 = 0;
    for (int index2 = 0; index2 < this.animationKeyframes.Count; ++index2)
    {
      if ((double) scrobblePos >= (double) this.animationKeyframes[index2])
        index1 = index2;
    }
    if (index1 >= this.animationKeyframes.Count - 1)
    {
      ((VertexColorStream) ((Component) this).GetComponent<VertexColorStream>()).setColors(this.animationMeshes[index1]._colors);
    }
    else
    {
      float num1 = this.animationKeyframes[index1 + 1] - this.animationKeyframes[index1];
      float animationKeyframe = this.animationKeyframes[index1];
      float num2 = (scrobblePos - animationKeyframe) / num1;
      for (int index2 = 0; index2 < _vertexColors.Length; ++index2)
        _vertexColors[index2] = Color.Lerp(this.animationMeshes[index1]._colors[index2], this.animationMeshes[index1 + 1]._colors[index2], num2);
      ((VertexColorStream) ((Component) this).GetComponent<VertexColorStream>()).setColors(_vertexColors);
    }
  }

  private void Update()
  {
    if (this.mode == 0)
      this.elapsedTime += Time.get_fixedDeltaTime() / this.timeScale;
    else if (this.mode == 1)
    {
      this.elapsedTime += Time.get_fixedDeltaTime() / this.timeScale;
      if ((double) this.elapsedTime > 1.0)
        this.elapsedTime = 0.0f;
    }
    else if (this.mode == 2)
    {
      if (Mathf.FloorToInt(Time.get_fixedTime() / this.timeScale) % 2 == 0)
        this.elapsedTime += Time.get_fixedDeltaTime() / this.timeScale;
      else
        this.elapsedTime -= Time.get_fixedDeltaTime() / this.timeScale;
    }
    Color[] _vertexColors = new Color[((MeshFilter) ((Component) this).GetComponent<MeshFilter>()).get_sharedMesh().get_colors().Length];
    int index1 = 0;
    for (int index2 = 0; index2 < this.animationKeyframes.Count; ++index2)
    {
      if ((double) this.elapsedTime >= (double) this.animationKeyframes[index2])
        index1 = index2;
    }
    if (index1 < this.animationKeyframes.Count - 1)
    {
      float num1 = this.animationKeyframes[index1 + 1] - this.animationKeyframes[index1];
      float num2 = (this.elapsedTime - this.animationKeyframes[index1]) / num1;
      for (int index2 = 0; index2 < _vertexColors.Length; ++index2)
        _vertexColors[index2] = Color.Lerp(this.animationMeshes[index1]._colors[index2], this.animationMeshes[index1 + 1]._colors[index2], num2);
    }
    else
      _vertexColors = this.animationMeshes[index1]._colors;
    ((VertexColorStream) ((Component) this).GetComponent<VertexColorStream>()).setColors(_vertexColors);
  }

  public VertexColorAnimator()
  {
    base.\u002Ector();
  }
}
