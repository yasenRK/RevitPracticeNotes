
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB.Architecture;


namespace RevitProjects
{
    [Autodesk.Revit.Attributes.Transaction(TransactionMode.Manual)]
    public class CmdTest : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection selection = uidoc.Selection;
            Reference reff = selection.PickObject(ObjectType.PointOnElement);
            Element elem = doc.GetElement(reff);
            GeometryObject geoObj = elem.GetGeometryObjectFromReference(reff);
            Category targetCategory = null;
            if(geoObj.GraphicsStyleId!=ElementId.InvalidElementId)
            {
                GraphicsStyle gs = doc.GetElement(geoObj.GraphicsStyleId) as GraphicsStyle;
                if(gs!=null)
                {
                    targetCategory = gs.GraphicsStyleCategory;
                }
            }

            using(Transaction ts=new Transaction(doc,"隐藏特定的图层"))
            {
                ts.Start();

                doc.ActiveView.SetVisibility(targetCategory, false);

                ts.Commit();
            }


            return Result.Succeeded;
        }
    }

}
