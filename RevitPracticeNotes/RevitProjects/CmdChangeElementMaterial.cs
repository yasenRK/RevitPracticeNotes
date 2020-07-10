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
    class CmdChangeElementMaterial : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection selection = uidoc.Selection;

            Element elem =doc.GetElement(selection.PickObject(ObjectType.Element, "Please pick the element that you want to change material"));





            //List<Material> materials = new FilteredElementCollector(doc)
            List<Material> materials =new List<Material>( new FilteredElementCollector(doc).OfClass(typeof(Material)).WhereElementIsNotElementType().ToElements().Where<Element>(m => m.Id.IntegerValue != elem.Category.Material.Id.IntegerValue).Cast<Material>());
            Random r = new Random();
            using(Transaction ts=new Transaction(doc,"change element material"))
            {
                ts.Start();
                elem.Category.Material = materials[r.Next(materials.Count)];
                ts.Commit();
            }



            return Result.Succeeded;
        }
    }
}
