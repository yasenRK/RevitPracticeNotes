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
/// 这个类实现了梁对齐屋顶或者楼板
/// 参考地址：https://github.com/binbinstrong/tangsengjiewa/blob/master/%E5%94%90%E5%83%A7%E8%A7%A3%E7%93%A6/%E5%BB%BA%E7%AD%91/Cmd_BeamAlignToRoofAndFloor.cs
/// </summary>
namespace RevitProjects
{
    [Autodesk.Revit.Attributes.Transaction(TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(RegenerationOption.Manual)]
    class CmdBeamAlignToRoofAndFloor:IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elementSet)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection selection = uidoc.Selection;
            View acview = uidoc.ActiveView;

            bool IsAlignTopFace = false;
            bool IsAlignBottomFace = false;
            List<ElementId> selectedIds =new List<ElementId>(selection.GetElementIds());
            FilteredElementCollector selectionColl = new FilteredElementCollector(doc, selectedIds);
            ElementCategoryFilter beamFilter = new ElementCategoryFilter(BuiltInCategory.OST_StructuralFraming);



                

            try
            {

            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException ex)
            {
                TaskDialog.Show("Error", $"{ex.Message}");
                return Result.Cancelled;
            }

            return Result.Succeeded;
        }
    }
}
