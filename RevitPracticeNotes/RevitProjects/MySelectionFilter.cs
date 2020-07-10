using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
/// <summary>
/// 这个泛型类实现了用户进行框选时的过滤限制
/// </summary>
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
