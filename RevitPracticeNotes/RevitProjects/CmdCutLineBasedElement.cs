using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.UI;

namespace RevitProjects
{
    [Autodesk.Revit.Attributes.Transaction(TransactionMode.Manual)]
    class CmdCutLineBasedElement:IExternalCommand
    {

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection selection = uidoc.Selection;
            while (true)
            {
                try
                {
                    using (Transaction ts = new Transaction(doc))
                    {
                        ts.Start("打断");
                        Reference refElement = selection.PickObject(ObjectType.PointOnElement, "请选择需要打断的点");
                        XYZ point = refElement.GlobalPoint;
                        Element elem = doc.GetElement(refElement);
                        LocationCurve lc = elem.Location as LocationCurve;
                        XYZ newPoint = lc.Curve.Project(point).XYZPoint;
                        if (lc == null)
                        {
                            TaskDialog.Show("Error", "您选择的构建不可以分割，请重新选择");
                            continue;
                        }
                        XYZ startPoint = lc.Curve.GetEndPoint(0);
                        XYZ endPoint = lc.Curve.GetEndPoint(1);

                        Line l1 = Line.CreateBound(startPoint, newPoint);
                        Line l2 = Line.CreateBound(newPoint, endPoint);
                        Element el1 = doc.GetElement(ElementTransformUtils.CopyElement(doc, elem.Id, new XYZ(1, 0, 0)).First());
                        Element el2 = doc.GetElement(ElementTransformUtils.CopyElement(doc, elem.Id, new XYZ(1, 0, 0)).First());
                        (el1.Location as LocationCurve).Curve = l1;
                        (el2.Location as LocationCurve).Curve = l2;
                        doc.Delete(elem.Id);
                        ts.Commit();
                    }
                }
                catch
                {
                    break;
                }
            }


            return Result.Succeeded;
        }
    }
}
