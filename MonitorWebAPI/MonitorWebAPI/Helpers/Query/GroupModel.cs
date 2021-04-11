using MonitorWebAPI.Helpers.Query;
using MonitorWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MonitorWebAPI.Helpers
{
    public class GroupModel : QueryItem
    {
        List<QueryItem> rule_items { get; set; } = null;
        public List<object> rules { get; set; } = null;
        public string combinator { get; set; } = "AND";
        public bool not { get; set; } = false;

        public void GenerateItems()
        {
            rule_items = new List<QueryItem>();
            foreach(object rule in rules)
            {
                if(rule.ToString()[2] == 'r')
                {
                    rule_items.Add(JsonSerializer.Deserialize<GroupModel>(rule.ToString()));
                }
                else if(rule.ToString()[2] == 'f')
                {
                    rule_items.Add(JsonSerializer.Deserialize<RuleModel>(rule.ToString()));
                }
                else
                {
                    throw new ArgumentException("bad query");
                }
            }
        }

        public bool Eval(Device d)
        {
            //return (rules == null);
            if (rule_items == null) GenerateItems();
            bool return_val = combinator.ToLower().Equals("and") ? true : false;

            foreach(QueryItem item in rule_items)
            {
                bool ev = item.Eval(d);
                return_val = combinator.ToLower().Equals("and") ? ev && return_val : ev || return_val;

                /*
                if (item.IsRule())
                {
                    bool ev = item.Eval(d);
                    return_val = combinator.ToLower().Equals("and") ? ev && return_val : ev || return_val;
                }
                else
                {
                    bool ev = item.Eval(d);
                    return_val = combinator.ToLower().Equals("and") ? ev && return_val : ev || return_val;
                }
                */
            }


            return not ? !return_val : return_val;
        }

        public bool IsGroup()
        {
            return true;
        }

        public bool IsRule()
        {
            return false;
        }

    }
}
