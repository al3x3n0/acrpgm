#region Using Statements
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
#endregion

namespace Delight
{
    /// <summary>
    /// Static class for accessing the data models. References to models added through partial classes generated by the framework.
    /// </summary>
    public static partial class Models
    {
        public static DependencyObject RuntimeModelObject = new DependencyObject(); // used in runtime bindings
        public static Globals Globals = new Globals();
    }
}
