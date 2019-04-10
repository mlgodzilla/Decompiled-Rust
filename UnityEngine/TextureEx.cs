// Decompiled with JetBrains decompiler
// Type: UnityEngine.TextureEx
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

namespace UnityEngine
{
  public static class TextureEx
  {
    private static Color32[] buffer = new Color32[8192];

    public static void Clear(this Texture2D tex, Color32 color)
    {
      if (((Texture) tex).get_width() > TextureEx.buffer.Length)
      {
        Debug.LogError((object) ("Trying to clear texture that is too big: " + (object) ((Texture) tex).get_width()));
      }
      else
      {
        for (int index = 0; index < ((Texture) tex).get_width(); ++index)
          TextureEx.buffer[index] = color;
        for (int index = 0; index < ((Texture) tex).get_height(); ++index)
          tex.SetPixels32(0, index, ((Texture) tex).get_width(), 1, TextureEx.buffer);
        tex.Apply();
      }
    }

    public static int GetSizeInBytes(this Texture texture)
    {
      int width = texture.get_width();
      int height = texture.get_height();
      if (texture is Texture2D)
      {
        Texture2D texture2D = texture as Texture2D;
        int bitsPerPixel = TextureEx.GetBitsPerPixel(texture2D.get_format());
        int mipmapCount = texture2D.get_mipmapCount();
        int num1 = 1;
        int num2 = 0;
        for (; num1 <= mipmapCount; ++num1)
        {
          num2 += width * height * bitsPerPixel / 8;
          width /= 2;
          height /= 2;
        }
        return num2;
      }
      if (texture is Texture2DArray)
      {
        Texture2DArray texture2Darray = texture as Texture2DArray;
        int bitsPerPixel = TextureEx.GetBitsPerPixel(texture2Darray.get_format());
        int num1 = 10;
        int num2 = 1;
        int num3 = 0;
        int depth = texture2Darray.get_depth();
        for (; num2 <= num1; ++num2)
        {
          num3 += width * height * bitsPerPixel / 8;
          width /= 2;
          height /= 2;
        }
        return num3 * depth;
      }
      if (!(texture is Cubemap))
        return 0;
      int bitsPerPixel1 = TextureEx.GetBitsPerPixel((texture as Cubemap).get_format());
      return width * height * bitsPerPixel1 / 8 * 6;
    }

    public static int GetBitsPerPixel(TextureFormat format)
    {
      switch (format - 1)
      {
        case 0:
          return 8;
        case 1:
          return 16;
        case 2:
          return 24;
        case 3:
          return 32;
        case 4:
          return 32;
        case 5:
        case 7:
        case 8:
        case 10:
label_19:
          return 0;
        case 6:
          return 16;
        case 9:
          return 4;
        case 11:
label_11:
          return 8;
        case 12:
          return 16;
        case 13:
          return 32;
        default:
          switch (format - 25)
          {
            case 0:
              goto label_11;
            case 1:
            case 2:
            case 3:
            case 4:
              goto label_19;
            case 5:
              return 2;
            case 6:
              return 2;
            case 7:
              return 4;
            case 8:
              return 4;
            case 9:
              return 4;
            default:
              if (format == 47)
                return 8;
              goto label_19;
          }
      }
    }
  }
}
