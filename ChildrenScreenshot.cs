// Decompiled with JetBrains decompiler
// Type: ChildrenScreenshot
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class ChildrenScreenshot : MonoBehaviour
{
  public Vector3 offsetAngle;
  public int width;
  public int height;
  public float fieldOfView;
  [Tooltip("0 = full recursive name, 1 = object name")]
  public string folder;

  [ContextMenu("Create Screenshots")]
  public void CreateScreenshots()
  {
    RenderTexture renderTexture = new RenderTexture(this.width, this.height, 0);
    GameObject gameObject = new GameObject();
    Camera cam = (Camera) gameObject.AddComponent<Camera>();
    cam.set_targetTexture(renderTexture);
    cam.set_orthographic(false);
    cam.set_fieldOfView(this.fieldOfView);
    cam.set_nearClipPlane(0.1f);
    cam.set_farClipPlane(2000f);
    cam.set_cullingMask(LayerMask.GetMask(new string[1]
    {
      "TransparentFX"
    }));
    cam.set_clearFlags((CameraClearFlags) 2);
    cam.set_backgroundColor(new Color(0.0f, 0.0f, 0.0f, 0.0f));
    cam.set_renderingPath((RenderingPath) 3);
    Texture2D texture2D = new Texture2D(((Texture) renderTexture).get_width(), ((Texture) renderTexture).get_height(), (TextureFormat) 5, false);
    using (IEnumerator<Transform> enumerator = ((IEnumerable) ((Component) this).get_transform()).Cast<Transform>().GetEnumerator())
    {
      while (((IEnumerator) enumerator).MoveNext())
      {
        Transform current = enumerator.Current;
        this.PositionCamera(cam, ((Component) current).get_gameObject());
        int layer = ((Component) current).get_gameObject().get_layer();
        ((Component) current).get_gameObject().SetLayerRecursive(1);
        cam.Render();
        ((Component) current).get_gameObject().SetLayerRecursive(layer);
        string str = current.GetRecursiveName("").Replace('/', '.');
        RenderTexture.set_active(renderTexture);
        texture2D.ReadPixels(new Rect(0.0f, 0.0f, (float) ((Texture) renderTexture).get_width(), (float) ((Texture) renderTexture).get_height()), 0, 0, false);
        RenderTexture.set_active((RenderTexture) null);
        byte[] png = ImageConversion.EncodeToPNG(texture2D);
        string path = string.Format(this.folder, (object) str, (object) ((Object) current).get_name());
        string directoryName = Path.GetDirectoryName(path);
        if (!Directory.Exists(directoryName))
          Directory.CreateDirectory(directoryName);
        File.WriteAllBytes(path, png);
      }
    }
    Object.DestroyImmediate((Object) texture2D, true);
    Object.DestroyImmediate((Object) renderTexture, true);
    Object.DestroyImmediate((Object) gameObject, true);
  }

  public void PositionCamera(Camera cam, GameObject obj)
  {
    Bounds bounds;
    ((Bounds) ref bounds).\u002Ector(obj.get_transform().get_position(), Vector3.op_Multiply(Vector3.get_zero(), 0.1f));
    bool flag = true;
    foreach (Renderer componentsInChild in (Renderer[]) obj.GetComponentsInChildren<Renderer>())
    {
      if (flag)
      {
        bounds = componentsInChild.get_bounds();
        flag = false;
      }
      else
        ((Bounds) ref bounds).Encapsulate(componentsInChild.get_bounds());
    }
    Vector3 size = ((Bounds) ref bounds).get_size();
    float num = ((Vector3) ref size).get_magnitude() * 0.5f / Mathf.Tan((float) ((double) cam.get_fieldOfView() * 0.5 * (Math.PI / 180.0)));
    ((Component) cam).get_transform().set_position(Vector3.op_Addition(((Bounds) ref bounds).get_center(), Vector3.op_Multiply(obj.get_transform().TransformVector(((Vector3) ref this.offsetAngle).get_normalized()), num)));
    ((Component) cam).get_transform().LookAt(((Bounds) ref bounds).get_center());
  }

  public ChildrenScreenshot()
  {
    base.\u002Ector();
  }
}
