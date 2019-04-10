// Decompiled with JetBrains decompiler
// Type: NewsSource
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch.Extend;
using Facepunch.Math;
using JSON;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class NewsSource : MonoBehaviour
{
  public NewsSource.Story[] story;
  public UnityEngine.UI.Text title;
  public UnityEngine.UI.Text date;
  public UnityEngine.UI.Text text;
  public UnityEngine.UI.Text authorName;
  public RawImage image;
  public VerticalLayoutGroup layoutGroup;
  public Button button;

  private void OnEnable()
  {
    this.StartCoroutine(this.UpdateNews());
  }

  public void SetStory(int i)
  {
    if (this.story == null || this.story.Length <= i)
      return;
    this.StopAllCoroutines();
    this.title.set_text(this.story[i].name);
    this.date.set_text(NumberExtensions.FormatSecondsLong((long) (Epoch.get_Current() - this.story[i].date)));
    string str = Regex.Replace(Regex.Replace(this.story[i].text, "\\[img\\].*\\[\\/img\\]", string.Empty, RegexOptions.IgnoreCase).Replace("\\n", "\n").Replace("\\r", "").Replace("\\\"", "\"").Replace("[list]", "<color=#F7EBE1aa>").Replace("[/list]", "</color>").Replace("[*]", "\t\t» "), "\\[(.*?)\\]", string.Empty, RegexOptions.IgnoreCase).Trim();
    Match match1 = Regex.Match(this.story[i].text, "url=(http|https):\\/\\/([\\w\\-_]+(?:(?:\\.[\\w\\-_]+)+))([\\w\\-\\.,@?^=%&amp;:/~\\+#]*[\\w\\-\\@?^=%&amp;/~\\+#])");
    Match match2 = Regex.Match(this.story[i].text, "(http|https):\\/\\/([\\w\\-_]+(?:(?:\\.[\\w\\-_]+)+))([\\w\\-\\.,@?^=%&amp;:/~\\+#]*[\\w\\-\\@?^=%&amp;/~\\+#])(.png|.jpg)");
    if (match1 != null)
    {
      // ISSUE: object of a compiler-generated type is created
      // ISSUE: variable of a compiler-generated type
      NewsSource.\u003C\u003Ec__DisplayClass10_0 cDisplayClass100 = new NewsSource.\u003C\u003Ec__DisplayClass10_0();
      // ISSUE: reference to a compiler-generated field
      cDisplayClass100.url = match1.Value.Replace("url=", "");
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      if (cDisplayClass100.url == null || cDisplayClass100.url.Trim().Length <= 0)
      {
        // ISSUE: reference to a compiler-generated field
        cDisplayClass100.url = this.story[i].url;
      }
      ((Component) this.button).get_gameObject().SetActive(true);
      ((UnityEventBase) this.button.get_onClick()).RemoveAllListeners();
      // ISSUE: method pointer
      ((UnityEvent) this.button.get_onClick()).AddListener(new UnityAction((object) cDisplayClass100, __methodptr(\u003CSetStory\u003Eb__0)));
    }
    else
      ((Component) this.button).get_gameObject().SetActive(false);
    this.text.set_text(str);
    this.authorName.set_text(string.Format("posted by {0}", (object) this.story[i].author));
    if (!Object.op_Inequality((Object) this.image, (Object) null))
      return;
    if (Object.op_Implicit((Object) this.story[i].texture))
    {
      this.SetHeadlineTexture(this.story[i].texture);
    }
    else
    {
      if (match2 == null)
        return;
      this.StartCoroutine(this.LoadHeaderImage(match2.Value, i));
    }
  }

  private void SetHeadlineTexture(Texture tex)
  {
    float num1 = (float) tex.get_height() / (float) tex.get_width();
    this.image.set_texture(tex);
    RectTransform rectTransform = ((Graphic) this.image).get_rectTransform();
    Rect rect1 = ((Graphic) this.image).get_rectTransform().get_rect();
    Vector2 vector2 = new Vector2(0.0f, (float) ((double) ((Rect) ref rect1).get_width() * (double) num1));
    rectTransform.set_sizeDelta(vector2);
    ((Behaviour) this.image).set_enabled(true);
    RectOffset padding = ((LayoutGroup) this.layoutGroup).get_padding();
    RectOffset rectOffset = padding;
    Rect rect2 = ((Graphic) this.image).get_rectTransform().get_rect();
    int num2 = (int) ((double) ((Rect) ref rect2).get_width() * (double) num1) / 2;
    rectOffset.set_top(num2);
    ((LayoutGroup) this.layoutGroup).set_padding(padding);
  }

  private IEnumerator LoadHeaderImage(string url, int i)
  {
    ((Behaviour) this.image).set_enabled(false);
    WWW www = new WWW(url);
    yield return (object) www;
    if (!string.IsNullOrEmpty(www.get_error()))
    {
      Debug.LogWarning((object) ("Couldn't load header image: " + www.get_error()));
      www.Dispose();
    }
    else
    {
      Texture2D textureNonReadable = www.get_textureNonReadable();
      ((Object) textureNonReadable).set_name(url);
      this.story[i].texture = (Texture) textureNonReadable;
      this.SetHeadlineTexture(this.story[i].texture);
      www.Dispose();
    }
  }

  private IEnumerator UpdateNews()
  {
    WWW www = new WWW("http://api.steampowered.com/ISteamNews/GetNewsForApp/v0002/?appid=252490&count=8&format=json&feeds=steam_community_announcements");
    yield return (object) www;
    Object @object = Object.Parse(www.get_text());
    www.Dispose();
    if (@object != null)
    {
      Array array = @object.GetObject("appnews").GetArray("newsitems");
      List<NewsSource.Story> storyList = new List<NewsSource.Story>();
      using (IEnumerator<Value> enumerator = array.GetEnumerator())
      {
        while (((IEnumerator) enumerator).MoveNext())
        {
          Value current = enumerator.Current;
          string str = current.get_Obj().GetString("contents", "Missing URL");
          storyList.Add(new NewsSource.Story()
          {
            name = current.get_Obj().GetString("title", "Missing Title"),
            url = current.get_Obj().GetString("url", "Missing URL"),
            date = current.get_Obj().GetInt("date", 0),
            text = str,
            author = current.get_Obj().GetString("author", "Missing Author")
          });
        }
      }
      this.story = storyList.ToArray();
      this.SetStory(0);
    }
  }

  public NewsSource()
  {
    base.\u002Ector();
  }

  public struct Story
  {
    public string name;
    public string url;
    public int date;
    public string text;
    public string author;
    public Texture texture;
  }
}
