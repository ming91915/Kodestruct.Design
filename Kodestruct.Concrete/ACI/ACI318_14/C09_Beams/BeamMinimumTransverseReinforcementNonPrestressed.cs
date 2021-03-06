#region Copyright
   /*Copyright (C) 2015 Konstantin Udilovich

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
   */
#endregion
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kodestruct.Common.CalculationLogger.Interfaces;
using Kodestruct.Concrete.ACI;


namespace Kodestruct.Concrete.ACI318_14
{
    public  partial class ReinforcedConcreteBeamNonprestressed
    {
        public double GetMinimumTransverseReinforcement(IConcreteSection Section,
           double f_yt, double s, ICalcLog log)
        {
            double f_c = Section.Material.SpecifiedCompressiveStrength;
            double b_w = Section.b_w;

            //Table 9.6.3.3�Required Av,min
            double A_vMin1 = 0.75 * Section.Material.Sqrt_f_c_prime * ((b_w) / (f_yt)) * s;
            double A_vMin2 = 50 * ((b_w) / (f_yt)) * s;
            return Math.Max(A_vMin1, A_vMin2);
        }
    }
}
