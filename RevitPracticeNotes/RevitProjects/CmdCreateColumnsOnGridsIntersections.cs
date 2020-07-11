using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.UI; 
namespace RevitProjects
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class CmdCreateColumnsOnGridsIntersections:IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elementSet)
        {
            UIApplication uiapp = new UIApplication(commandData.Application.Application);
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection selection = uidoc.Selection;

            List<Grid> xGrid = new List<Grid>();
            List<Grid> yGrid = new List<Grid>();
            List<XYZ> points = new List<XYZ>();
            List<Reference> reffs = (selection.PickObjects(ObjectType.Element, new GridFilter(), "请选择所有的轴线")).ToList();

            foreach (Reference item in reffs)
            {
                Grid grid = doc.GetElement(item) as Grid;
                if (grid != null)
                {
                    Line line = grid.Curve as Line;
                    if (line != null)
                    {
                        XYZ dir = line.Direction;
                        if (dir.Y.Equals(-1) || dir.Y.Equals(1))
                        {
                            yGrid.Add(grid);
                        }

                        else if (dir.X.Equals(-1) || dir.X.Equals(1))
                        {
                            xGrid.Add(grid);
                        }
                    }
                }
                else
                {
                    continue;
                }
            }
            foreach (Grid grx in xGrid)
            {
                foreach (Grid gry in yGrid)
                {
                    XYZ res = CmdCreateColumnsOnGridsIntersections.GetIntersectPoint(grx, gry);
                    if (res == null) continue;
                    points.Add(res);
                }
            }
            // 过滤出一个柱子的族类型
            ElementId columnTypeId = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Columns).OfClass(typeof(FamilySymbol)).ToElementIds().First();
            FamilySymbol fmSymbol = doc.GetElement(columnTypeId) as FamilySymbol;
            // 过滤出一个默认的标高
            Level level = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Levels).OfClass(typeof(Level)).ToList<Element>().FirstOrDefault() as Level;

            using (Transaction ts = new Transaction(doc, "创建柱子"))
            {
                ts.Start();
                if (!fmSymbol.IsActive)
                {
                    fmSymbol.Activate();
                }
                foreach (XYZ ptn in points)
                {

                    doc.Create.NewFamilyInstance(ptn, fmSymbol, level, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
                }

                ts.Commit();
            }


            return Result.Succeeded;
        }
        public static XYZ GetIntersectPoint(Grid grid1, Grid grid2)
        {
            Line line1 = grid1.Curve as Line;
            Line line2 = grid2.Curve as Line;
            IntersectionResultArray res;
            line1.Intersect(line2, out res);
            XYZ point = res.get_Item(0).XYZPoint;


            return point;
        }

    }
    // 用户只能选择轴线
    public class GridFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            return elem is Grid ? true : false;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
}
