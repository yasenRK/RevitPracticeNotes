using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using System.Runtime.InteropServices.WindowsRuntime;
/// <summary>
/// 这个类实现了改变门的开启方向-->左右方向
/// </summary>
namespace RevitProjects
{
    [Autodesk.Revit.Attributes.Regeneration(RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Transaction(TransactionMode.Manual)]
    class CmdFlipDoorHand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {

            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection selection = uidoc.Selection;

            IList<FamilyInstance> elems = selection.PickElementsByRectangle(new DoorFilter(), "select doors").Cast<FamilyInstance>().ToList();
            using(Transaction ts=new Transaction(doc,"flip doorhand"))
            {
                ts.Start();
                foreach(FamilyInstance fi in elems)
                {
                    fi.flipHand(); 
                }

                ts.Commit();
            }



            return Result.Succeeded;
        }
    }
    public class DoorFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            return elem.Category.Name == "门";
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return true;
        }
    }
}
