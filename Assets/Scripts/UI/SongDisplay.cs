using UnityEngine;

public class SongDisplay : MonoBehaviour {
  public TMPro.TextMeshProUGUI textMesh1;
  public TMPro.TextMeshProUGUI textMesh2;
  public int spacing = 21;
  public int paddingBetween = 3;
  public float scrollSpeed = 10;
  private float length;
  public void SetDisplay(string name, string artist) {
    var text = $"<mspace={spacing}>{name} - {artist}";
    textMesh1.text = text;
    textMesh2.text = text;
    length = (text[(text.IndexOf('>')+1)..].Length + paddingBetween) * spacing;
    ((RectTransform)textMesh1.transform).anchoredPosition = new Vector2(0, 3.5f);
    ((RectTransform)textMesh2.transform).anchoredPosition = new Vector2(length, 3.5f);
  }

  void Update() {
    ((RectTransform)textMesh1.transform).anchoredPosition = new Vector2((((RectTransform)textMesh1.transform).anchoredPosition.x - scrollSpeed) % length, 3.5f);
    ((RectTransform)textMesh2.transform).anchoredPosition = new Vector2((((RectTransform)textMesh2.transform).anchoredPosition.x + length - scrollSpeed ) % length, 3.5f);
  }
}