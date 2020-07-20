using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 
 
namespace RevitProjects
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class NewTest : IExternalCommand
    {
        Application app;
        Document doc;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            app = commandData.Application.Application;
            doc = uidoc.Document;
            //Reference refer = uidoc.Selection.PickObject(ObjectType.Element, "Select a CAD Link");
            //Element elem = doc.GetElement(refer);
            //GeometryElement geoElem = elem.get_Geometry(new Options());




            Reference r = uidoc.Selection.PickObject(ObjectType.PointOnElement);
            string ss = r.ConvertToStableRepresentation(doc);




            Element elem = doc.GetElement(r);
            GeometryElement geoElem = elem.get_Geometry(new Options());
            GeometryObject geoObj = elem.GetGeometryObjectFromReference(r);


            //获取选中的cad图层
            Category targetCategory = null;
            ElementId graphicsStyleId = null;


            if (geoObj.GraphicsStyleId != ElementId.InvalidElementId)
            {
                graphicsStyleId = geoObj.GraphicsStyleId;
                GraphicsStyle gs = doc.GetElement(geoObj.GraphicsStyleId) as GraphicsStyle;
                if (gs != null)
                {
                    targetCategory = gs.GraphicsStyleCategory;
                    var name = gs.GraphicsStyleCategory.Name;
                }
            }
            //隐藏选中的cad图层
            Transaction trans = new Transaction(doc, "隐藏图层");
            trans.Start();
            if (targetCategory != null)
                doc.ActiveView.SetVisibility(targetCategory, false);


            trans.Commit();




            TransactionGroup transGroup = new TransactionGroup(doc, "绘制模型线");
            transGroup.Start();
            CurveArray curveArray = new CurveArray();


            //判断元素类型
            foreach (var gObj in geoElem)
            {
                GeometryInstance geomInstance = gObj as GeometryInstance;
                //坐标转换。如果选择的是“自动-中心到中心”，或者移动了importInstance，需要进行坐标转换
                Transform transform = geomInstance.Transform;


                if (null != geomInstance)
                {
                    foreach (var insObj in geomInstance.SymbolGeometry)
                    {
                        if (insObj.GraphicsStyleId.IntegerValue != graphicsStyleId.IntegerValue)
                            continue;


                        if (insObj.GetType().ToString() == "Autodesk.Revit.DB.NurbSpline")
                        {
                            //未实现
                        }
                        if (insObj.GetType().ToString() == "Autodesk.Revit.DB.Line")
                        {
                            Line line = insObj as Line;
                            XYZ normal = XYZ.BasisZ;
                            XYZ point = line.GetEndPoint(0);
                            point = transform.OfPoint(point);


                            curveArray.Append(TransformLine(transform, line));


                            CreateModelCurveArray(curveArray, normal, point);
                        }
                        if (insObj.GetType().ToString() == "Autodesk.Revit.DB.Arc")
                        {
                            //未实现
                        }
                        if (insObj.GetType().ToString() == "Autodesk.Revit.DB.PolyLine")
                        {
                            PolyLine polyLine = insObj as PolyLine;
                            IList<XYZ> points = polyLine.GetCoordinates();


                            for (int i = 0; i < points.Count - 1; i++)
                            {
                                Line line = Line.CreateBound(points[i], points[i + 1]);
                                line = TransformLine(transform, line);
                                curveArray.Append(line);
                            }


                            XYZ normal = XYZ.BasisZ;
                            XYZ point = points.First();
                            point = transform.OfPoint(point);


                            CreateModelCurveArray(curveArray, normal, point);
                        }




                    }
                }


            }
            transGroup.Assimilate();




            return Result.Succeeded;
        }


        private void CreateModelCurveArray(CurveArray curveArray, XYZ normal, XYZ point)
        {
            if (curveArray.Size > 0)
            {
                Transaction transaction2 = new Transaction(doc);
                transaction2.Start("绘制模型线");
                try
                {
                    SketchPlane modelSketch = SketchPlane.Create(doc, app.Create.NewPlane(normal, point));
                    ModelCurveArray modelLine = doc.Create.NewModelCurveArray(curveArray, modelSketch);
                }
                catch
                {


                }
                transaction2.Commit();
                curveArray.Clear();
            }
        }


        private Line TransformLine(Transform transform, Line line)
        {
            XYZ startPoint = transform.OfPoint(line.GetEndPoint(0));
            XYZ endPoint = transform.OfPoint(line.GetEndPoint(1));
            Line newLine = Line.CreateBound(startPoint, endPoint);
            return newLine;
        }
    }
}
