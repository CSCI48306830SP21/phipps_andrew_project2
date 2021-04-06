using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class WorldSpaceButton : MonoBehaviour
{
    [SerializeField]
    private bool mainMenuButton = false;

    private Button button;

    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Button>();

        // If set to main menu button, load the main menu when clicked.
        if (mainMenuButton) {
            GameManager gm = FindObjectOfType<GameManager>();

            button.onClick.AddListener(delegate { gm.LoadMainMenuScene(); });
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Clicks the button
    /// </summary>
    public void Click() {
        button.onClick.Invoke();
    }

    /// <summary>
    /// Toggles the button's highlight.
    /// </summary>
    public void Highlight() {
        EventSystem.current.SetSelectedGameObject(gameObject);
    }
}
