using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com2Com.Model
{
    class SlaveModelsComparer : IEqualityComparer<SlaveModel>
    {
        public bool Equals(SlaveModel x, SlaveModel y)
        {
            if (ReferenceEquals(x, y))
                return true;

            return (x.SlaveId == y.SlaveId && x.DigitalValue == y.DigitalValue && x.AnalogValue == y.AnalogValue) && (x != null && y != null);

        }


        public int GetHashCode(SlaveModel obj)
        {
            var idHashCode = obj?.SlaveId.GetHashCode() ?? 0;
            var digitalValueHashCode = obj?.DigitalValue.GetHashCode() ?? 0;
            var analogValueHashCode = obj?.AnalogValue.GetHashCode() ?? 0;

            return idHashCode ^ digitalValueHashCode ^ analogValueHashCode;
        }
    }
}
