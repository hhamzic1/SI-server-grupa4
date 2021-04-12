using MonitorWebAPI.Helpers.Query;
using MonitorWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MonitorWebAPI.Helpers
{
    public class RuleModel : QueryItem
    {
        public string field { get; set; }
        public string value { get; set; }
        public string operator_str { get; set; }

        public bool Eval(AllInfoForDevice d)
        {
            string field_value = GetFieldFromDevice(d);
            return EvalOperator(field_value);
        }

        private bool EvalOperator(string field_value)
        {
            switch (operator_str)
            {
                case "=": return field_value.Equals(value);
                case "!=": return !(field_value.Equals(value));
                case "<=": return Double.Parse(field_value) <= Double.Parse(value);
                case ">=": return Double.Parse(field_value) >= Double.Parse(value);
                case "<": return Double.Parse(field_value) < Double.Parse(value);
                case ">": return Double.Parse(field_value) > Double.Parse(value);
                case "contains": return field_value.Contains(value);
                case "beginsWith": return field_value.StartsWith(value);
                case "endsWith": return field_value.EndsWith(value);
                case "doesNotContain": return !(field_value.Contains(value));
                case "doesNotBeginWith": return !(field_value.StartsWith(value));
                case "doesNotEndWith": return !(field_value.EndsWith(value));
                case "isNull": return field_value == null;
                case "isNotNull": return !(field_value == null);
                case "in": throw new NotImplementedException();
                case "notIn": throw new NotImplementedException();
            }

            throw new ArgumentException("bad query operator");
        }

        public bool IsGroup()
        {
            return false;
        }

        public bool IsRule()
        {
            return true;
        }

        private string GetFieldFromDevice(AllInfoForDevice d)
        {
            switch (field)
            {
                case "avgRamUsage": return  d.Averages.RamUsage.ToString();
                case "avgGpuUsage": return d.Averages.Gpuusage.ToString();
                case "quarterlyCpuUsage": return d.Averages.CpuUsage.ToString();
                case "diskUtilization": return d.Averages.Hddusage.ToString();
                case "name": return d.Device.Name;
                case "location": return d.Device.Location;
                case "latitude": return d.Device.LocationLatitude.ToString();
                case "longitude": return d.Device.LocationLongitude.ToString();
                case "status": return d.Device.Status.ToString();
                case "groupName": throw new NotImplementedException();
            }

            throw new ArgumentException("bad query field");
        }

    }
}
