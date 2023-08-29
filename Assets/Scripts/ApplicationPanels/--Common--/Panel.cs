using UnityEngine;

namespace ApplicationPanels.__Common__
{
    public abstract class Panel : MonoBehaviour,IPanel
    {
        public virtual void OnPanelEnter()
        {
        }

        public virtual void OnPanelExit()
        {
        }
    }
}
