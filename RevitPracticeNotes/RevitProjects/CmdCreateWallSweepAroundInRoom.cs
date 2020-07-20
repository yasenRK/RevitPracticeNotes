using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
namespace RevitProjects
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class CmdCreateWallSweepAroundInRoom : IExternalCommand
    {
        public const double _tolerance = 0.001;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection selection = uidoc.Selection;
            View acview = uidoc.ActiveView;
            try
            {
                Reference reff = selection.PickObject(ObjectType.Element,new MySelectionFilter<Room>(), "select a room");
                Room room = doc.GetElement(reff) as Room;
                ElementType wallSweepType = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Cornices).WhereElementIsElementType().Cast<ElementType>().FirstOrDefault();

                if(wallSweepType!=null)
                {
                    using(Transaction ts=new Transaction(doc,"创建踢脚线"))
                    {
                        ts.Start();
                       ElementType type = wallSweepType.Duplicate("踢脚线2");
                        SpatialElementBoundaryOptions opts = new SpatialElementBoundaryOptions();
                        opts.SpatialElementBoundaryLocation = SpatialElementBoundaryLocation.Finish;
                        IList < IList < Autodesk.Revit.DB.BoundarySegment> > boundList = room.GetBoundarySegments(opts);
                        foreach(IList<BoundarySegment> bs in boundList)
                        {
                            foreach(BoundarySegment bs1 in bs)
                            {
                                Wall wall = doc.GetElement(bs1.ElementId) as Wall;
                                if(wall!=null)
                                {

                                    WallSweepInfo info = new WallSweepInfo(WallSweepType.Sweep, false);
                                    // 墙的正反或者里面
                                    info.WallSide = WallSide.Exterior;
                                    // 距离墙底的高度
                                    info.Distance = 10 / 304.8;
                                    // 偏移距离
                                    info.WallOffset = 0;
                                    WallSweep.Create(wall, type.Id, info);
                                }
                            }
                        }
                        ts.Commit();
                    }
                }



            }
            catch(Exception ex)
            {
                TaskDialog.Show("Error", ex.Message);
            }



            return Result.Succeeded;
        }
    }
}