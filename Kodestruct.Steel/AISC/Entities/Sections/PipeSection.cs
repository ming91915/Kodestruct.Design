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
using Kodestruct.Common.Entities; 
using Kodestruct.Common.Section.Interfaces; 
using Kodestruct.Steel.AISC.Interfaces;
using Kodestruct.Common.Section.Interfaces;
using Kodestruct.Steel.AISC.Interfaces;
using Kodestruct.Steel.AISC.Steel.Entities.Sections;
//using SectionDesigner;


namespace Kodestruct.Steel.AISC.SteelEntities.Sections
{
    public class SteelPipeSection: SteelSectionBase
    {
        private ISectionPipe section;

        public ISectionPipe Section
        {
            get { return section; }
        }

        public override ISection Shape
        {
            get { return section as ISection; }
        }


        public SteelPipeSection(ISectionPipe Section, ISteelMaterial Material)
            :base(Material)
        {
            this.section = Section;
        }

        //public override ISection Clone()
        //{
        //    return section.Clone();
        //}
    }
}
