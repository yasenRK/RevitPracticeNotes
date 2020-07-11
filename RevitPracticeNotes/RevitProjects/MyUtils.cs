using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB.Structure;

/// <summary>
/// 这个静态类封装了一些常用的方法
/// </summary>
namespace RevitProjects
{

    public static class MyUtils
    {
        #region Geometrical Comparison
        public const double _eps = 1.0e-9;
        
        public static double Eps
        {
            get
            {
                return _eps;
            }
        }
        /// <summary>
        /// 最短的线长度
        /// </summary>
        public static double MinLength
        {
            get
            {
                return _eps;
            }
        }
        public static double TolPointOnPlane
        {
            get
            {
                return _eps;
            }
        }

        /// <summary>
        /// 判断一个只是否等于0
        /// </summary>
        /// <param name="a"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static bool IsZero(double a,double tolerance=_eps)
        {
            return tolerance > Math.Abs(a);
        }

        /// <summary>
        /// 判断两个值是否相等
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="tolearnce"></param>
        /// <returns></returns>
        public static bool IsEqual(double a,double b,double tolearnce=_eps)
        {
            return IsZero(b - a, tolearnce);
        }

        /// <summary>
        /// 判断两个数字的大小
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
       public static int Compare(double a,double b,double tolerance=_eps)
        {
            return IsEqual(a, b, tolerance) ? 0 : (a > b ? 1 : -1);
        }

        /// <summary>
        /// 比较两个点的大小
        /// </summary>
        /// <param name="pt1"></param>
        /// <param name="pt2"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static int Compare(XYZ pt1, XYZ pt2, double tolerance=_eps)
        {
            int d = Compare(pt1.X, pt2.X, tolerance);
            if(d==0)
            {
                d = Compare(pt1.Y, pt2.Y, tolerance);
                if(d==0)
                {
                    d = Compare(pt1.Z, pt2.Z, tolerance);
                }
            }
            return d;

        }
        /// <summary>
        /// 实现了一个线比较的运算符，在XY平面上对线进行排序的时候很有用
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static int Compare(Line a,Line b)
        {
            XYZ pa = a.GetEndPoint(0);
            XYZ qa = a.GetEndPoint(1);
            XYZ pb = b.GetEndPoint(0);
            XYZ qb = b.GetEndPoint(1);
            XYZ va = qa - pa;
            XYZ vb = qb - pb;

            // 在XY平面上比较夹角
            double ang_a = Math.Atan2(va.Y, va.X);
            double ang_b = Math.Atan2(vb.Y, vb.X);

            int d = Compare(ang_a, ang_b);
            if(d==0)// 两条线的角度是一样的
            {
                // Compare distance of undouned line to origin
                double da = (qa.X * pa.Y - qa.Y * pa.Y) / va.GetLength();
                double db = (qb.X * pb.Y - qb.Y * pb.Y) / vb.GetLength();
                d = Compare(da, db);
                if(0==d)
                {
                    // Compare distance of start point to origin
                    d = Compare(pa.GetLength(), pb.GetLength());
                    if(0==d)
                    {
                        // Compare distance of end point to origin
                        d = Compare(qa.GetLength(), qb.GetLength());
                    }
                }
            }
            return d;
        }


        /// <summary>
        /// 比较两个点在给定容差范围内能不能相等
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static bool IsEqual(XYZ p1,XYZ p2,double tolerance=_eps)
        {
            return 0 == Compare(p1, p2, tolerance);
        }

        /// <summary>
        /// 判断一个点是否在一个BoundingBox的里面
        /// </summary>
        /// <param name="bb"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public static bool BoundingBoxXyzContains(BoundingBoxXYZ bb,XYZ p)
        {
            return 0 < Compare(bb.Min, p) && 0 < Compare(p,bb.Max);
        }

        /// <summary>
        /// 判断两个非零向量是否垂直
        /// </summary>
        /// <param name="v"></param>
        /// <param name="w"></param>
        /// <returns></returns>
        public static bool IsPerpendicular(XYZ v ,XYZ w)
        {
            double a = v.GetLength();
            double b = w.GetLength();
            double c = Math.Abs(v.DotProduct(w));
            return _eps < a
                && _eps < b
                && _eps > c;
        }






















        #endregion

        /// <summary>
        /// 获取用户框选的元素的Id
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="reffs"></param>
        /// <returns></returns>
        public static List<ElementId> GetElementIds(Document doc, IList<Reference> reffs)
        {
            List<ElementId> elemIds = new List<ElementId>();
            foreach (Reference item in reffs)
            {
                ElementId elemId = doc.GetElement(item).Id;
                elemIds.Add(elemId);
            }
            return elemIds;
        }
        /// <summary>
        /// 获取用户框选的元素
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="reffs"></param>
        /// <returns></returns>
        public static List<Element> GetElements(Document doc, IList<Reference> reffs)
        {
            List<Element> elems = new List<Element>();
            foreach (Reference item in reffs)
            {
                Element elem = doc.GetElement(item);
                elems.Add(elem);
            }
            return elems;
        }



















    }
}
