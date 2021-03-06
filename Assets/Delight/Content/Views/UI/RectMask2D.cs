#region Using Statements
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
#endregion

namespace Delight
{
    /// <summary>
    /// A view that masks any content using the background sprite and color as mask. Based on RectMask2D component which has better performance than the regular mask.
    /// </summary>
    public partial class RectMask2D
    {
        /// <summary>
        /// Called just before the view and its children are loaded.
        /// </summary>
        protected override void BeforeLoad()
        {
            base.BeforeLoad();

            RectMask2DComponent = GameObject.AddComponent<UnityEngine.UI.RectMask2D>();
        }
    }
}
