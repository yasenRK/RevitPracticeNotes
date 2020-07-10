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
    class CmdChangeElementColor : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection selection = uidoc.Selection;

            Element elem =doc.GetElement(selection.PickObject(ObjectType.Element, "please select the element that you want to change color"));
            try
            {
                ChangeColor(doc, elem.Id);
            }
            catch(Autodesk.Revit.Exceptions.OperationCanceledException ex)
            {
                TaskDialog.Show("Error", $"{ex.Message}");
                return Result.Cancelled;
            }



            return Result.Succeeded;
        }
        void ChangeColor(Document doc,ElementId id)
        {
            byte _R = (byte)new Random().Next(0, 255);
            byte _G = (byte)new Random().Next(0, 255);
            byte _B = (byte)new Random().Next(0, 255);
            Color color = new Color(_R,_G,_B);
            //Color color = new Color(255,0,0);
            FillPatternElement fp = new FilteredElementCollector(doc).OfClass(typeof(FillPatternElement)).First(m => (m as FillPatternElement).GetFillPattern().IsSolidFill) as FillPatternElement;
            OverrideGraphicSettings ogs = new OverrideGraphicSettings();
            ogs.SetProjectionFillPatternId(fp.Id);
            ogs.SetProjectionFillColor(color);
            
            using(Transaction ts=new Transaction(doc,"change element color"))
            {
                ts.Start();
                doc.ActiveView.SetElementOverrides(id, ogs);
                ts.Commit();
            }

        }

    }
}
