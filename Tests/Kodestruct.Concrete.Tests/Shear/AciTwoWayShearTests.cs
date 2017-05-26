//Sample license text.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Kodestruct.Concrete.ACI318_14;
using Kodestruct.Concrete.ACI;
using Kodestruct.Common.Section.Interfaces;
using Kodestruct.Concrete.ACI.ACI318_14.C22_SectionalStrength.Shear.TwoWay;
using Kodestruct.Common.Mathematics;

namespace Kodestruct.Concrete.ACI318_14.Tests.Shear
{
    [TestFixture]
    public partial class AciTwoWayShearTests : AciConcreteShearTestsBase
    {
        /// <summary>
        /// MacGregor, Wight. Reinforced concrete. 6th edition
        /// Example 13-11
        /// </summary>
        [Test]
        public void SlabPunchingStrengthReturnsValue()
        {
            IConcreteMaterial mat = this.GetConcreteMaterial(3000, false);
            PerimeterFactory f = new PerimeterFactory();
            double d = 4.75;
            double b_1 = 26.0;
            double b_2 = 12.0;
            Point2D ColumnCenter = new Point2D(0, 0);
            PunchingPerimeterData perimeter = f.GetPerimeterData(PunchingPerimeterConfiguration.Interior, b_1, b_2, d,0,0, ColumnCenter);
            //ConcreteSectionTwoWayShear sec = new ConcreteSectionTwoWayShear(mat, perimeter, d, b_1, b_2, true, PunchingPerimeterConfiguration.Interior);
            ConcreteSectionTwoWayShear sec = new ConcreteSectionTwoWayShear(mat, perimeter,d, b_1, b_2, PunchingPerimeterConfiguration.Interior);
            double phi_v_c = sec.GetTwoWayStrengthForUnreinforcedConcrete()/1000.0; //convert to kips
            
            double refValue = 71.3/d/95.0; //from example
            double actualTolerance = EvaluateActualTolerance(phi_v_c, refValue);

            Assert.LessOrEqual(actualTolerance, tolerance);

        
        }
        /// <summary>
        /// MacGregor, Wight. Reinforced concrete. 6th edition
        /// Example 13-13
        /// </summary>
        [Test]
        public void SlabPunchingMomentAndShearReturnsStressMG()
        {
            IConcreteMaterial mat = this.GetConcreteMaterial(3000, false);
            PerimeterFactory f = new PerimeterFactory();
            double d = 5.5;
            double cx = 12.0;
            double cy = 16.0;
            Point2D ColumnCenter = new Point2D(0, 0);
            PunchingPerimeterData data = f.GetPerimeterData(PunchingPerimeterConfiguration.EdgeLeft, cx, cy, d, 4.0, 0.0, ColumnCenter);
            ConcreteSectionTwoWayShear sec = new ConcreteSectionTwoWayShear(data, d, cx, cy, PunchingPerimeterConfiguration.EdgeLeft);
            double M_u = 6400.0 * 2.6; //MacGregor reduces unbalanced moment to 6.4 kip*ft. Kodestruct makes adjustment for gamma so 6.4 is multipled by 1 / gamma_v
            double phi_v_c = sec.GetCombinedShearStressDueToMomementAndShear(0, M_u, 35300, false).v_max / 1000.0; //note: moment is adjusted to be at column centroid

            double refValue = 0.144; //from example
            double actualTolerance = EvaluateActualTolerance(phi_v_c, refValue);

            Assert.LessOrEqual(actualTolerance, tolerance);


        }

        /// <summary>
        /// ACI 421.1R-19  SHEAR REINFORCEMENT FOR SLABS 
        /// Example D.1
        /// </summary>
        [Test]
        public void InteriorSlabReturnsPerimeterJ_yProperty()
        {
            IConcreteMaterial mat = this.GetConcreteMaterial(3000, false);
            PerimeterFactory f = new PerimeterFactory();
            double d = 5.62;
            double cx = 12.0;
            double cy = 20.0;
            Point2D ColumnCenter = new Point2D(0, 0);
            PunchingPerimeterData data = f.GetPerimeterData(PunchingPerimeterConfiguration.Interior, cx, cy, d, 0.0, 0.0, ColumnCenter);
            ConcreteSectionTwoWayShear sec = new ConcreteSectionTwoWayShear( data, d, cx, cy, PunchingPerimeterConfiguration.Interior);
            double J_y = sec.GetJy(sec.AdjustedSegments);

            double refValue = 27474; //from example  (page 19)
            double actualTolerance = EvaluateActualTolerance(J_y, refValue);

            Assert.LessOrEqual(actualTolerance, tolerance);
        }

        /// <summary>
        /// ACI 421.1R-19  SHEAR REINFORCEMENT FOR SLABS 
        /// Example D.2
        /// </summary>
        [Test]
        public void EdgeSlabReturnsPerimeterJ_yPropertyFor3SidedPerimeterEdgeLeft()
        {
            IConcreteMaterial mat = this.GetConcreteMaterial(3000, false);
            PerimeterFactory f = new PerimeterFactory();
            double d = 5.62;
            double cx = 18.0;
            double cy = 18.0;
            Point2D ColumnCenter = new Point2D(0, 0);
            PunchingPerimeterData data = f.GetPerimeterData(PunchingPerimeterConfiguration.EdgeLeft, cx, cy, d, 0.0, 0.0, ColumnCenter);
            ConcreteSectionTwoWayShear sec = new ConcreteSectionTwoWayShear( data, d, cx, cy,  PunchingPerimeterConfiguration.EdgeLeft);
            double J_y = sec.GetJy(sec.AdjustedSegments);

            double refValue = 17630; //from example  (page 19)
            double actualTolerance = EvaluateActualTolerance(J_y, refValue);

            Assert.LessOrEqual(actualTolerance, tolerance);
        }

        /// <summary>
        /// ACI 421.1R-19  SHEAR REINFORCEMENT FOR SLABS 
        /// Example D.2
        /// </summary>
        [Test]
        public void EdgeSlabReturnsPunchingShearStressFor3SidedPerimeterEdgeLeft()
        {
            IConcreteMaterial mat = this.GetConcreteMaterial(3000, false);
            PerimeterFactory f = new PerimeterFactory();
            double d = 5.62;
            double cx = 18.0;
            double cy = 18.0;
            Point2D ColumnCenter = new Point2D(0, 0);
            PunchingPerimeterData data = f.GetPerimeterData(PunchingPerimeterConfiguration.EdgeLeft, cx, cy, d, 0.0, 0.0, ColumnCenter);
            ConcreteSectionTwoWayShear sec = new ConcreteSectionTwoWayShear(data, d, cx, cy, PunchingPerimeterConfiguration.EdgeLeft);
            double v_u = sec.GetCombinedShearStressDueToMomementAndShear(0* 1000, 1720 * 1000, 36000, false).v_min;

            double refValue = -338; //from example  (page 19)
            double actualTolerance = EvaluateActualTolerance(v_u, refValue);

            Assert.LessOrEqual(actualTolerance, tolerance);
        }

        /// <summary>
        /// ACI 421.1R-19  SHEAR REINFORCEMENT FOR SLABS 
        /// Example D.2
        /// </summary>
        [Test]
        public void EdgeSlabReturnsPerimeterJ_yPropertyFor3SidedPerimeterEdgeRight()
        {
            IConcreteMaterial mat = this.GetConcreteMaterial(3000, false);
            PerimeterFactory f = new PerimeterFactory();
            double d = 5.62;
            double cx = 18.0;
            double cy = 18.0;
            Point2D ColumnCenter = new Point2D(0, 0);
            PunchingPerimeterData data = f.GetPerimeterData(PunchingPerimeterConfiguration.EdgeLeft, cx, cy, d, 0.0, 0.0, ColumnCenter);
            ConcreteSectionTwoWayShear sec = new ConcreteSectionTwoWayShear(data, d, cx, cy, PunchingPerimeterConfiguration.EdgeRight);
            double J_y = sec.GetJy(sec.AdjustedSegments);

            double refValue = 17630; //from example  (page 19)
            double actualTolerance = EvaluateActualTolerance(J_y, refValue);

            Assert.LessOrEqual(actualTolerance, tolerance);
        }

        /// <summary>
        /// ACI 421.1R-19  SHEAR REINFORCEMENT FOR SLABS 
        /// Example D.2
        /// </summary>
        [Test]
        public void EdgeSlabReturnsPerimeterJ_yPropertyFor3SidedPerimeterEdgeTop()
        {
            IConcreteMaterial mat = this.GetConcreteMaterial(3000, false);
            PerimeterFactory f = new PerimeterFactory();
            double d = 5.62;
            double cx = 18.0;
            double cy = 18.0;
            Point2D ColumnCenter = new Point2D(0, 0);
            PunchingPerimeterData data = f.GetPerimeterData(PunchingPerimeterConfiguration.EdgeLeft, cx, cy, d, 0.0, 0.0, ColumnCenter);
            ConcreteSectionTwoWayShear sec = new ConcreteSectionTwoWayShear(data, d, cx, cy, PunchingPerimeterConfiguration.EdgeTop);
            double J_y = sec.GetJy(sec.AdjustedSegments);

            double refValue = 17630; //from example  (page 19)
            double actualTolerance = EvaluateActualTolerance(J_y, refValue);

            Assert.LessOrEqual(actualTolerance, tolerance);
        }

        /// <summary>
        /// ACI 421.1R-19  SHEAR REINFORCEMENT FOR SLABS 
        /// Example D.2
        /// </summary>
        [Test]
        public void EdgeSlabReturnsPerimeterJ_yPropertyFor3SidedPerimeterEdgeBottom()
        {
            IConcreteMaterial mat = this.GetConcreteMaterial(3000, false);
            PerimeterFactory f = new PerimeterFactory();
            double d = 5.62;
            double cx = 18.0;
            double cy = 18.0;
            Point2D ColumnCenter = new Point2D(0, 0);
            PunchingPerimeterData data = f.GetPerimeterData(PunchingPerimeterConfiguration.EdgeLeft, cx, cy, d, 0.0, 0.0, ColumnCenter);
            ConcreteSectionTwoWayShear sec = new ConcreteSectionTwoWayShear(data, d, cx, cy, PunchingPerimeterConfiguration.EdgeBottom);
            double J_y = sec.GetJy(sec.AdjustedSegments);

            double refValue = 17630; //from example  (page 19)
            double actualTolerance = EvaluateActualTolerance(J_y, refValue);

            Assert.LessOrEqual(actualTolerance, tolerance);
        }

        /// <summary>
        /// MacGregor
        /// Example 13-13
        /// </summary>
        [Test]
        public void EdgeSlabReturnsPerimeterJ_yPropertyFor3SidedPerimeterMG()
        {
            IConcreteMaterial mat = this.GetConcreteMaterial(3500, false);
            PerimeterFactory f = new PerimeterFactory();
            double d = 5.5;
            double cx = 12.0;
            double cy = 16.0;
            Point2D ColumnCenter = new Point2D(0, 0);
            PunchingPerimeterData data = f.GetPerimeterData(PunchingPerimeterConfiguration.EdgeLeft, cx, cy, d, 4.0, 4.0, ColumnCenter);
            ConcreteSectionTwoWayShear sec = new ConcreteSectionTwoWayShear( data, d, cx, cy,  PunchingPerimeterConfiguration.EdgeLeft);
            double J_y = sec.GetJy(sec.AdjustedSegments);

            double refValue = 13200.0; 
            double actualTolerance = EvaluateActualTolerance(J_y, refValue);

            Assert.LessOrEqual(actualTolerance, tolerance);
        }


        /// <summary>
        /// ACI 421.1R-19  SHEAR REINFORCEMENT FOR SLABS 
        /// Example D.1
        /// </summary>
        [Test]
        public void InteriorSlabReturnsPerimeter_gamma_vy_Property()
        {
            IConcreteMaterial mat = this.GetConcreteMaterial(3000, false);
            PerimeterFactory f = new PerimeterFactory();
            double d = 5.62;
            double cx = 12.0;
            double cy = 20.0;
            Point2D ColumnCenter = new Point2D(0, 0);
            PunchingPerimeterData data = f.GetPerimeterData(PunchingPerimeterConfiguration.Interior, cx, cy, d, 0.0, 0.0, ColumnCenter);
            ConcreteSectionTwoWayShear sec = new ConcreteSectionTwoWayShear( data, d, cx, cy,  PunchingPerimeterConfiguration.Interior);
            double gamma_vy = sec.Get_gamma_vy(cx + d, cy + d);

            double refValue = 0.36; //from example 
            double actualTolerance = EvaluateActualTolerance(gamma_vy, refValue);

            Assert.LessOrEqual(actualTolerance, tolerance);
        }

        /// <summary>
        /// ACI 421.1R-19  SHEAR REINFORCEMENT FOR SLABS 
        /// Example D.3
        /// </summary>
        [Test]
        public void CornerSlabReturnsPunchingShearStress()
        {
            IConcreteMaterial mat = this.GetConcreteMaterial(3000, false);
            PerimeterFactory f = new PerimeterFactory();
            double d = 5.62;
            double cx = 20.00;
            double cy = 20.0;
            Point2D ColumnCenter = new Point2D(0, 0);
            PunchingPerimeterData data = f.GetPerimeterData(PunchingPerimeterConfiguration.CornerLeftTop, cx, cy, d, 0.0, 0.0, ColumnCenter);
            ConcreteSectionTwoWayShear sec = new ConcreteSectionTwoWayShear(data, d, cx, cy, PunchingPerimeterConfiguration.CornerLeftTop);
            double v_u = sec.GetCombinedShearStressDueToMomementAndShear(-238 * 1000, 338 * 1000,6000.0, false).v_max;

            double refValue = 192.0; //from example 
            double actualTolerance = EvaluateActualTolerance(v_u, refValue);

            Assert.LessOrEqual(actualTolerance, tolerance);
        }

        /// <summary>
        /// ACI 421.1R-19  SHEAR REINFORCEMENT FOR SLABS 
        /// Example D.1
        /// </summary>
        [Test]
        public void InteriorSlabReturnsPunchingShearStress()
        {
            IConcreteMaterial mat = this.GetConcreteMaterial(3000, false);
            PerimeterFactory f = new PerimeterFactory();
            double d = 5.62;
            double cx = 12.0;
            double cy = 20.0;
            Point2D ColumnCenter = new Point2D(0, 0);
            PunchingPerimeterData data = f.GetPerimeterData(PunchingPerimeterConfiguration.Interior, cx, cy, d, 0.0, 0.0, ColumnCenter);
            ConcreteSectionTwoWayShear sec = new ConcreteSectionTwoWayShear( data, d, cx, cy, PunchingPerimeterConfiguration.Interior);
            double v_u = sec.GetCombinedShearStressDueToMomementAndShear(0, 600*1000, 110*1000,false).v_max;

            double refValue = 294; //from example 
            double actualTolerance = EvaluateActualTolerance(v_u, refValue);

            Assert.LessOrEqual(actualTolerance, tolerance);
        }


        [Test]
        public void CornerSlabReturnsPunchingShearStress_UpperLeft()
        {
            IConcreteMaterial mat = this.GetConcreteMaterial(4000, false);
            PerimeterFactory f = new PerimeterFactory();
            double d = 5.62;
            double cx = 20.00;
            double cy = 14.0;
            Point2D ColumnCenter = new Point2D(0, 0);
            PunchingPerimeterData data = f.GetPerimeterData(PunchingPerimeterConfiguration.CornerLeftTop, cx, cy, d, 0.0, 0.0, ColumnCenter);
            ConcreteSectionTwoWayShear sec = new ConcreteSectionTwoWayShear(data, d, cx, cy, PunchingPerimeterConfiguration.CornerLeftTop);
            double v_u = sec.GetCombinedShearStressDueToMomementAndShear(0 * 1000, 1326 * 1000, 0, false).v_max;

            double refValue = 935; 
            double actualTolerance = EvaluateActualTolerance(v_u, refValue);

            Assert.LessOrEqual(actualTolerance, tolerance);
        }

        [Test]
        public void CornerSlabReturnsPunchingShearStress_UpperRight()
        {
            IConcreteMaterial mat = this.GetConcreteMaterial(4000, false);
            PerimeterFactory f = new PerimeterFactory();
            double d = 5.62;
            double cx = 20.00;
            double cy = 14.0;
            Point2D ColumnCenter = new Point2D(0, 0);
            PunchingPerimeterData data = f.GetPerimeterData(PunchingPerimeterConfiguration.CornerLeftTop, cx, cy, d, 0.0, 0.0, ColumnCenter);
            ConcreteSectionTwoWayShear sec = new ConcreteSectionTwoWayShear(data, d, cx, cy, PunchingPerimeterConfiguration.CornerRightTop);
            double v_u = sec.GetCombinedShearStressDueToMomementAndShear(0 * 1000, 1326 * 1000, 0, false).v_max;

            double refValue = 935;
            double actualTolerance = EvaluateActualTolerance(v_u, refValue);

            Assert.LessOrEqual(actualTolerance, tolerance);
        }

        [Test]
        public void CornerSlabReturnsPunchingShearStress_LowerLeft()
        {
            IConcreteMaterial mat = this.GetConcreteMaterial(4000, false);
            PerimeterFactory f = new PerimeterFactory();
            double d = 5.62;
            double cx = 20.00;
            double cy = 14.0;
            Point2D ColumnCenter = new Point2D(0, 0);
            PunchingPerimeterData data = f.GetPerimeterData(PunchingPerimeterConfiguration.CornerLeftTop, cx, cy, d, 0.0, 0.0, ColumnCenter);
            ConcreteSectionTwoWayShear sec = new ConcreteSectionTwoWayShear(data, d, cx, cy, PunchingPerimeterConfiguration.CornerLeftBottom);
            double v_u = sec.GetCombinedShearStressDueToMomementAndShear(0 * 1000, 1326 * 1000, 0, false).v_max;

            double refValue = 935;
            double actualTolerance = EvaluateActualTolerance(v_u, refValue);

            Assert.LessOrEqual(actualTolerance, tolerance);
        }


        [Test]
        public void CornerSlabReturnsPunchingShearStress_LowerRight()
        {
            IConcreteMaterial mat = this.GetConcreteMaterial(4000, false);
            PerimeterFactory f = new PerimeterFactory();
            double d = 5.62;
            double cx = 20.00;
            double cy = 14.0;
            Point2D ColumnCenter = new Point2D(0, 0);
            PunchingPerimeterData data = f.GetPerimeterData(PunchingPerimeterConfiguration.CornerLeftTop, cx, cy, d, 0.0, 0.0, ColumnCenter);
            ConcreteSectionTwoWayShear sec = new ConcreteSectionTwoWayShear(data, d, cx, cy, PunchingPerimeterConfiguration.CornerRightBottom);
            double v_u = sec.GetCombinedShearStressDueToMomementAndShear(0 * 1000, 1326 * 1000, 0, false).v_max;

            double refValue = 935;
            double actualTolerance = EvaluateActualTolerance(v_u, refValue);

            Assert.LessOrEqual(actualTolerance, tolerance);
        }
    }
}
