using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EndMenuScript : MonoBehaviour
{
    [SerializeField] private Button m_quitButton;
    void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(m_quitButton.gameObject);
    }
}
