﻿﻿using System;
using UnityEngine;

namespace UnityEditor.U2D.Path.GUIFramework
{
    public abstract class HoveredControlAction : GUIAction
    {
        private Control m_HoveredControl;

        public Control hoveredControl
        {
            get { return m_HoveredControl; }
        }

        public HoveredControlAction(Control control)
        {
            m_HoveredControl = control;
        }

        protected override bool CanTrigger(IGUIState guiState)
        {
            return guiState.nearestControl == hoveredControl.ID;
        }

        protected override void OnTrigger(IGUIState guiState)
        {
            m_HoveredControl.SetActionID(ID);
        }
    }
}
