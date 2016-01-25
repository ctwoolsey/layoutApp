using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace layoutApp
{
    public class zComparer : IComparer<placeableItem>
    {
        public int Compare(placeableItem itm1, placeableItem itm2)
        {
            int returnValue = 0;

            //the typical value is reversed so that the highest zOrder will be first.
            if (itm1.zOrder < itm2.zOrder)
                returnValue = 1;
            else
                if (itm1.zOrder > itm2.zOrder)
                    returnValue = -11;

            return (returnValue);
        }
    }
}
