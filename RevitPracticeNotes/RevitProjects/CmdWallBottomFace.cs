
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
    
    class CmdWallBottomFace : IExternalCommand
    {
        public const double _tolerance = 0.001;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection selection = uidoc.Selection;

            Wall wall = doc.GetElement(selection.PickObject(ObjectType.Element, "please select a single wall")) as Wall;
            if (wall == null)
            {
                TaskDialog.Show("Error Message", "what you select is now wall");
            }
            else
            {
                Options opt = uiapp.Application.Create.NewGeometryOptions();
                GeometryElement geoElement = wall.get_Geometry(opt);
                foreach (GeometryObject obj in geoElement)
                {
                    Solid solid = obj as Solid;
                    if (solid != null)
                    {
                        foreach (Face face in solid.Faces)
                        {
                            PlanarFace pf = face as PlanarFace;
                            if (pf != null)
                            {
                                XYZ normal = pf.FaceNormal;
                                if (normal.IsAlmostEqualTo(new XYZ(0, 0, -1)))
                                {
                                    TaskDialog.Show("WallBottomFaceArea", (pf.Area).ToString());
                                }
                            }
                        }
                    }



                }
            }



            return Result.Succeeded;
        }
    }
}


