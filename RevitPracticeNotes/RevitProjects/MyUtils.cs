using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB.Structure;

/// <summary>
/// 这个静态类封装了一些常用的方法
/// </summary>
namespace RevitProjects
{

    public static class MyUtils
    {
        public const double Unit = 0.3048;
        public const double minNumber = -3.14;



        /// <summary>
        /// 获取用户框选的元素的Id
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="reffs"></param>
        /// <returns></returns>
        public static List<ElementId> GetElementIds(Document doc, IList<Reference> reffs)
        {
            List<ElementId> elemIds = new List<ElementId>();
            foreach (Reference item in reffs)
            {
                ElementId elemId = doc.GetElement(item).Id;
                elemIds.Add(elemId);
            }
            return elemIds;
        }
        /// <summary>
        /// 获取用户框选的元素
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="reffs"></param>
        /// <returns></returns>
        public static List<Element> GetElements(Document doc, IList<Reference> reffs)
        {
            List<Element> elems = new List<Element>();
            foreach (Reference item in reffs)
            {
                Element elem = doc.GetElement(item);
                elems.Add(elem);
            }
            return elems;
        }
    }
}
