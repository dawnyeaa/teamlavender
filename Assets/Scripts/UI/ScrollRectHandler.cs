using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

[RequireComponent(typeof(RectTransform))]
public class ScrollRectHandler : MonoBehaviour {
  public RectTransform content;
  private RectTransform rect;
  private float spacing = 0;

  void Start() {
    spacing = content.GetComponent<VerticalLayoutGroup>().spacing;
    rect = transform as RectTransform;
  }

  public void ScrollToElement(RectTransform element) {
    content.anchoredPosition = new(content.anchoredPosition.x, content.anchoredPosition.y-GetRelativeToFrame(element));
  }

  float GetBottomOfElement(RectTransform element) {
    return (element.anchoredPosition.y - element.sizeDelta.y) - spacing/2f;
  }
  

  float GetTopOfElement(RectTransform element) {
    return element.anchoredPosition.y + spacing/2f;
  }

  float GetRelativeToFrame(RectTransform element) {
    var viewportBottom = -content.anchoredPosition.y - rect.sizeDelta.y;
    var viewportTop = -content.anchoredPosition.y;

    var elementBottom = GetBottomOfElement(element);
    var elementTop = GetTopOfElement(element);

    if (elementBottom < viewportBottom) {
      return elementBottom - viewportBottom;
    }
    else if (elementTop > viewportTop) {
      return elementTop - viewportTop;
    }
    else {
      return 0;
    }
  }
}
