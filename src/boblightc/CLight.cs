﻿using System;
using System.Collections.Generic;

namespace boblightc
{
    internal class CLight
    {
        private float[] m_hscan;
        private float[] m_vscan;
        private List<CColor> m_colors;

        public int NrColors { get; internal set; }
        public string Name { get; internal set; }

        public CLight()
        {
            m_colors = new List<CColor>();
        }

        internal void AddColor(CColor color)
        {
            this.m_colors.Add(color);
        }

        internal void SetHscan(float[] hscan)
        {
            this.m_hscan = hscan;
        }

        internal void SetVscan(float[] vscan)
        {
            this.m_vscan = vscan;
        }
    }
}