using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
namespace RevitProjects
{
    [Autodesk.Revit.Attributes.Transaction(TransactionMode.Manual)]
    class CmdHideLinkedCADDrawingLayer : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection selection = uidoc.Selection;

            Reference reff = selection.PickObject(ObjectType.PointOnElement);
            Element elem = doc.GetElement(reff);
            GeometryObject obj = elem.GetGeometryObjectFromReference(reff);
            Category targetCategory = null;
            if(obj.GraphicsStyleId!=ElementId.InvalidElementId)
            {
                GraphicsStyle gs = doc.GetElement(obj.GraphicsStyleId) as GraphicsStyle;
                if(gs!=null)
                {
                    targetCategory = gs.Category;
                }
            }
            using(Transaction ts=new Transaction(doc,"hide selected cad layer"))
            {
                ts.Start();
                doc.ActiveView.SetVisibility(targetCategory, false);
                ts.Commit();
            }

            return Result.Succeeded;
        }
    }
}
