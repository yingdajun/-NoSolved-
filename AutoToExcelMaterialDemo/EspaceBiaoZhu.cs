using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
namespace AutoToExcelMaterialDemo
{
    //标注文字避让  392  完成
    [Regeneration(RegenerationOption.Manual)]
    [Transaction(TransactionMode.Manual)]
    class EspaceBiaoZhu : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //throw new NotImplementedException();

            //生成标注，神器啊 //只需要生成标注即可，啊啊啊啊
            
            //ApplicationException app = uiapp.Application; 
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            //生成标注
            TagSelectFilter tagSelectFilter = new TagSelectFilter();
            IList<Reference> reDisList = uidoc.Selection.PickObjects(ObjectType.Element,tagSelectFilter
                , "请选择需要重新排序的标注");

            //获取所有的标注样式
            List<Dimension> dimensionsList = new List<Dimension>();

            //原来你的名字是这个 
            Dimension dsn = null;
            List<Line> lineList = new List<Line>();
            List<Line> newLineList = new List<Line>();

            Line line = null;
            foreach (Reference refer in reDisList)
            {
                dsn = doc.GetElement(refer) as Dimension;
                line = dsn.Curve as Line;
                lineList.Add(line);
                newLineList.Add(line);
                dimensionsList.Add(dsn);
                //擅长对应的XYZ
                //doc.Delete(dsn.Id);
            }
            Transaction ts = new Transaction(doc);
            
            ts.Start("1");
            //把标注标记好
            

            int tag = 0;

            //这是位置
            XYZ translater = null;
            //删去头
            lineList.Remove(lineList[0]);
            //成功封印
            foreach (Line li in lineList)
            {
                translater = newLineList[0].Project(newLineList[tag+1].Origin).XYZPoint -newLineList[tag+1].Origin;
                ElementTransformUtils.MoveElement(doc, dimensionsList[tag+1].Id, translater);
                tag = tag + 1;
            }



            ts.Commit();
            //doc.Delete();
            //Line li = dsn.Curve as Line;

            //GetTag(uiapp, uidoc);
            return Result.Succeeded;
        }



        /// <summary>
        /// 原始
        /// </summary>
        /// <param name="uiapp"></param>
        /// <param name="uidoc"></param>
        public void GetTag(UIApplication uiapp,UIDocument uidoc) {

            Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;
            Document doc = uidoc.Document;

            Autodesk.Revit.DB.View view = doc.ActiveView;
            ViewType vt = view.ViewType;
            if (vt == ViewType.FloorPlan || vt == ViewType.Elevation)
            {
                Reference eRef = uidoc.Selection.PickObject(ObjectType.Element, "Please pick a curve based element like wall.");
                Element element = doc.GetElement(eRef);
                if (eRef != null && element != null)
                {
                    XYZ dirVec = new XYZ();
                    XYZ viewNormal = view.ViewDirection;

                    LocationCurve locCurve = element.Location as LocationCurve;
                    if (locCurve == null || locCurve.Curve == null)
                    {
                        TaskDialog.Show("Prompt", "Selected element isn’t curve based!");
                        //  return Result.Cancelled;
                    }
                    XYZ dirCur = locCurve.Curve.GetEndPoint(0).Subtract(locCurve.Curve.GetEndPoint(1)).Normalize();
                    double d = dirCur.DotProduct(viewNormal);
                    if (d > -0.000000001 && d < 0.000000001)
                    {
                        dirVec = dirCur.CrossProduct(viewNormal);
                        XYZ p1 = locCurve.Curve.GetEndPoint(0);
                        XYZ p2 = locCurve.Curve.GetEndPoint(1);
                        XYZ dirLine = XYZ.Zero.Add(p1);
                        XYZ newVec = XYZ.Zero.Add(dirVec);
                        newVec = newVec.Normalize().Multiply(3);
                        dirLine = dirLine.Subtract(p2);
                        p1 = p1.Add(newVec);
                        p2 = p2.Add(newVec);
                        Line newLine = Line.CreateBound(p1, p2);
                        ReferenceArray arrRefs = new ReferenceArray();
                        Options options = app.Create.NewGeometryOptions();
                        options.ComputeReferences = true;
                        options.DetailLevel = ViewDetailLevel.Fine;
                        GeometryElement gelement = element.get_Geometry(options);
                        foreach (var geoObject in gelement)
                        {
                            Solid solid = geoObject as Solid;
                            if (solid == null)
                                continue;

                            FaceArrayIterator fIt = solid.Faces.ForwardIterator();
                            while (fIt.MoveNext())
                            {
                                PlanarFace p = fIt.Current as PlanarFace;
                                if (p == null)
                                    continue;

                                p2 = p.FaceNormal.CrossProduct(dirLine);
                                if (p2.IsZeroLength())
                                {
                                    arrRefs.Append(p.Reference);
                                }
                                if (2 == arrRefs.Size)
                                {
                                    break;
                                }
                            }
                            if (2 == arrRefs.Size)
                            {
                                break;
                            }
                        }
                        if (arrRefs.Size != 2)
                        {
                            TaskDialog.Show("Prompt", "Couldn’t find enough reference for creating dimension");
                            //return Result.Cancelled;
                        }

                        Transaction trans = new Transaction(doc, "create dimension");
                        trans.Start();
                        doc.Create.NewDimension(doc.ActiveView, newLine, arrRefs);
                        trans.Commit();
                    }
                    else
                    {
                        TaskDialog.Show("Prompt", "Selected element isn’t curve based!");
                        // return Result.Cancelled;
                    }
                }
            }
            else
            {
                TaskDialog.Show("Prompt", "Only support Plan View or Elevation View");
            }
        }

        /// <summary>
        /// 超量召唤 版本
        /// </summary>
        /// <param name="uiapp"></param>
        /// <param name="uidoc"></param>
        public void GetTagVer(UIApplication uiapp, UIDocument uidoc)
        {

            Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;
            Document doc = uidoc.Document;

            Autodesk.Revit.DB.View view = doc.ActiveView;
            ViewType vt = view.ViewType;
            if (vt == ViewType.FloorPlan || vt == ViewType.Elevation)
            {
                Reference eRef = uidoc.Selection.PickObject(ObjectType.Element, "Please pick a curve based element like wall.");
                Element element = doc.GetElement(eRef);
                if (eRef != null && element != null)
                {
                    XYZ dirVec = new XYZ();
                    XYZ viewNormal = view.ViewDirection;

                    LocationCurve locCurve = element.Location as LocationCurve;
                    if (locCurve == null || locCurve.Curve == null)
                    {
                        TaskDialog.Show("Prompt", "Selected element isn’t curve based!");
                        //  return Result.Cancelled;
                    }
                    XYZ dirCur = locCurve.Curve.GetEndPoint(0).Subtract(locCurve.Curve.GetEndPoint(1)).Normalize();
                    double d = dirCur.DotProduct(viewNormal);
                    if (d > -0.000000001 && d < 0.000000001)
                    {
                        dirVec = dirCur.CrossProduct(viewNormal);
                        XYZ p1 = locCurve.Curve.GetEndPoint(0);
                        XYZ p2 = locCurve.Curve.GetEndPoint(1);
                        
                        XYZ dirLine = XYZ.Zero.Add(p1);
                        XYZ newVec = XYZ.Zero.Add(dirVec);
                        newVec = newVec.Normalize().Multiply(3);
                        dirLine = dirLine.Subtract(p2);
                        p1 = p1.Add(newVec);
                        p2 = p2.Add(newVec);
                        Line newLine = Line.CreateBound(p1, p2);
                        ReferenceArray arrRefs = new ReferenceArray();
                        Options options = app.Create.NewGeometryOptions();
                        options.ComputeReferences = true;
                        options.DetailLevel = ViewDetailLevel.Fine;
                        GeometryElement gelement = element.get_Geometry(options);
                        foreach (var geoObject in gelement)
                        {
                            Solid solid = geoObject as Solid;
                            if (solid == null)
                                continue;

                            FaceArrayIterator fIt = solid.Faces.ForwardIterator();
                            while (fIt.MoveNext())
                            {
                                PlanarFace p = fIt.Current as PlanarFace;
                                if (p == null)
                                    continue;

                                p2 = p.FaceNormal.CrossProduct(dirLine);
                                if (p2.IsZeroLength())
                                {
                                    arrRefs.Append(p.Reference);
                                }
                                if (2 == arrRefs.Size)
                                {
                                    break;
                                }
                            }
                            if (2 == arrRefs.Size)
                            {
                                break;
                            }
                        }
                        if (arrRefs.Size != 2)
                        {
                            TaskDialog.Show("Prompt", "Couldn’t find enough reference for creating dimension");
                            //return Result.Cancelled;
                        }

                        Transaction trans = new Transaction(doc, "create dimension");
                        trans.Start();
                        doc.Create.NewDimension(doc.ActiveView, newLine, arrRefs);
                        trans.Commit();
                    }
                    else
                    {
                        TaskDialog.Show("Prompt", "Selected element isn’t curve based!");
                        // return Result.Cancelled;
                    }
                }
            }
            else
            {
                TaskDialog.Show("Prompt", "Only support Plan View or Elevation View");
            }
        }
    }

    class TagSelectFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            //throw new NotImplementedException();
            if (elem is Dimension) {
                return true;
            }
            return false;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            //throw new NotImplementedException();
            return false;
        }
    }
}
