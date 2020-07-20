
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

            ImportInstance instance = elem as ImportInstance;
            GeometryObject geo = instance.GetGeometryObjectFromReference(reff) as GeometryObject;
            Category targetCategory = null;
            if(geo.GraphicsStyleId!=ElementId.InvalidElementId)
            {
                GraphicsStyle gs = doc.GetElement(geo.GraphicsStyleId) as GraphicsStyle;
                if(gs!=null)
                {
                    targetCategory = gs.GraphicsStyleCategory;
                }
            }
            using(Transaction ts=new Transaction(doc,"hide layer"))
            {
                ts.Start();
                doc.ActiveView.SetVisibility(targetCategory, false);
                ts.Commit();
            }

            GeometryElement geoElement = elem.get_Geometry(new Options());
            if(geoElement==null || geo.GraphicsStyleId==null)
            {
                message += "几何元素或者Id不存在";
                return Result.Failed;
            }

          //  List<CADModel> curveArrayList=


            return Result.Succeeded;
        }
    }

}
