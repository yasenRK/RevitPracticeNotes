
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI.Selection;

/// <summary>
/// 这个类实现了找到墙体最下面的面，并且输出这个面的面积
/// </summary>
namespace RevitProjects
{
    [Autodesk.Revit.Attributes.Transaction(TransactionMode.Manual)]

    class CmdTest : IExternalCommand
    {
        public const double _tolerance = 0.001;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection selection = uidoc.Selection;

            Wall wall1 = doc.GetElement(selection.PickObject(ObjectType.Element, "please select first wall")) as Wall;
            Wall wall2 = doc.GetElement(selection.PickObject(ObjectType.Element, "please select second wall")) as Wall;

            using(Transaction ts=new Transaction(doc,"Unjoint"))
            {
                ts.Start();
                JoinGeometryUtils.JoinGeometry(doc, wall1, wall2);
                ts.Commit();
            }

            return Result.Succeeded;
        }
    }
}
