using UnityEngine;

namespace ApplicationPanels
{
    public class PanelController : MonoBehaviour
    {
        public GameObject[] panels;
        public Panels currentPanel;

        void Start()
        {
            FillPanelsArray();
        }
    
        private void FillPanelsArray()
        {
            panels = new GameObject[transform.childCount];
            for (int i = 0; i < transform.childCount; i++)
            {
                panels[i] = transform.GetChild(i).gameObject;
            }

            if (panels.Length == transform.childCount)
            {
                ShowPanelOnStart();
                Debug.Log("Panels array filled successfully");
            }
            else
            {
                Debug.LogError("Panels array filled unsuccessfully");
                //Pop Action to show error message. Will be made later!!!
            }
            
        }
        private void ShowPanelOnStart()
        {
            currentPanel = Panels.AnimePanel;
            panels[(int)currentPanel].SetActive(true);
        }
    
        public void ChangePanel(Panels panel)
        {
            panels[(int)currentPanel].SetActive(false);
            currentPanel = panel;
            panels[(int)currentPanel].SetActive(true);
        }
    }
}
