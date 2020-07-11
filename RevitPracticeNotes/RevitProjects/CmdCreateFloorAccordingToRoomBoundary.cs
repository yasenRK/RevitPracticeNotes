using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Structure;
/// <summary>
/// 这个类实现了根据房间边界生成楼板，这里用的是房间边界的面层作为边界
/// </summary>
namespace RevitProjects
{
    [Autodesk.Revit.Attributes.Transaction(TransactionMode.Manual)]
    class CmdCreateFloorAccordingToRoomBoundary : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection selection = uidoc.Selection;
            CurveArray curveArray = new CurveArray();
            Reference reff = selection.PickObject(ObjectType.Element, "please select a room");
            Room room = doc.GetElement(reff) as Room;
            SpatialElementBoundaryOptions opt = new SpatialElementBoundaryOptions();
            opt.SpatialElementBoundaryLocation = SpatialElementBoundaryLocation.Finish;
            IList<IList<BoundarySegment>> loops = room.GetBoundarySegments(opt);
            foreach(IList<BoundarySegment> bs1 in loops)
            {
                foreach (BoundarySegment bs2 in bs1)
                {
                    Curve curve = bs2.GetCurve();
                    //TaskDialog.Show("1", "2");
                    curveArray.Append(curve);
                }
            }
            using (Transaction ts=new Transaction(doc,"Create Floor"))
            {
                ts.Start();
                doc.Create.NewFloor(curveArray, false);
                ts.Commit();
            }    
            return Result.Succeeded;
        }
    }
}
