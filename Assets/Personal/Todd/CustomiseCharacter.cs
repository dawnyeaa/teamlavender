using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CustomiseCharacter : MonoBehaviour {
    private GameObject body;
    private List<CustomiseSlot> slots;

    void Start() {
        slots = transform.GetComponents<CustomiseSlot>().ToList();
        body = transform.Find("body").gameObject;
        for (int i = 0; i < transform.childCount; ++i) {
            GameObject child = transform.GetChild(i).gameObject;
            if (child != body) {
                Destroy(child);
            }
        }

        foreach (CustomiseSlot slot in slots) {
            List<GameObject> options = slot.GetOptionMeshes();
            for (int i = 0; i < options.Count; ++i) {
                slot.SetOptionInstance(Instantiate(options[i], transform), i);
            }
        }
    }

    void Update() {
        
    }
}
