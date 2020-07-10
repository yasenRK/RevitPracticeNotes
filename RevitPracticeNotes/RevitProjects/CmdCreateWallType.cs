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
    class CmdCreateWallType : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection selection = uidoc.Selection;

            WallType firstWallType = new FilteredElementCollector(doc).OfClass(typeof(WallType)).WhereElementIsElementType().ToElements().Cast<WallType>().First();
            // set new wall type to null
            WallType newWallType = null;

            using(Transaction ts=new Transaction(doc,"create new wall type"))
            {
                ts.Start();
                // duplicate wwalltype
                newWallType = firstWallType.Duplicate("New Wall") as WallType;


                // grab material of wall
                ElementId oldLayerMaterialId = firstWallType.GetCompoundStructure().GetLayers()[0].MaterialId;


                // dimentions


                // set wall strcutre

                // middle


                // interior

                // exterior



                ts.Commit();
            }


            return Result.Succeeded;
        }
    }
}
