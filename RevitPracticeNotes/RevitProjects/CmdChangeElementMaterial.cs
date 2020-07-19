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
           // List<Material> materials =new List<Material>( new FilteredElementCollector(doc).OfClass(typeof(Material)).WhereElementIsNotElementType().ToElements().Where<Element>(m => m.Id.IntegerValue != elem.Category.Material.Id.IntegerValue).Cast<Material>());
            List<Material> materials =new List<Material>( new FilteredElementCollector(doc).OfClass(typeof(Material)).WhereElementIsNotElementType().ToElements().Cast<Material>());
            Material mm = default(Material);
            List<Material> res = new List<Material>();
            foreach (Material m in materials)
            {
                if (m.Name.Contains("混凝土"))
                    {
                    res.Add(m);
                }
                
            }
            //foreach (Material m in materials)
            //{
            //    message += m.Name + "\n";
            //}

            Random r = new Random();
            try
            {

            using(Transaction ts=new Transaction(doc,"change element material"))
            {
                ts.Start();
                    elem.Category.Material = res[r.Next(res.Count)];
                    //elem.Category.Material = doc.GetElement(Material.Create(doc, "混凝土")) as Material;
                    //if(mm!=null)
                    //{

                    //elem.Category.Material = mm;
                    //}
                    //else
                    //{
                    //    TaskDialog.Show("Error", "null");
                    //}

                ts.Commit();
            }

            }
            catch(Exception ex)
            {
                TaskDialog.Show("Error", ex.Message);
            }
           TaskDialog.Show("Message", $"{res.Count}");

            return Result.Succeeded;
        }
    }
}
