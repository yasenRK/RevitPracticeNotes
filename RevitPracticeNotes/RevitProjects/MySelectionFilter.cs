using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;

namespace RevitProjects
{
    class MySelectionFilter<T> : ISelectionFilter where T:Element
    {
        public bool AllowElement(Element elem)
        {
            return elem is T;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return true;
        }
    }
}
