using MonitorWebAPI.Helpers;
using MonitorWebAPI.Helpers.Query;
using MonitorWebAPI.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TestProjectNUnit
{
    class QueryBuilderEvalTest
    {
        QueryModel q;
        List<AllInfoForDevice> devices;


        [SetUp]
        public void Setup()
        {
            devices = new List<AllInfoForDevice>();

            AllInfoForDevice ad;
            Device d;
            Averages a;

            q = new QueryModel();

            // Adding devices
            #region
            ad = new AllInfoForDevice();
            d = new Device();
            a = new Averages();

            d.Name = "d1";
            d.Location = "l1";
            d.LocationLongitude = 5;
            d.LocationLatitude = 5;
            d.Status = true;

            a.CpuUsage = 5;
            a.RamUsage = 5;
            a.Hddusage = 5;
            a.Gpuusage = 5;

            ad.Device = d;
            ad.Averages = a;
            ad.GroupName = "g1";
            devices.Add(ad);
            #endregion

            #region
            ad = new AllInfoForDevice();
            d = new Device();
            a = new Averages();

            d.Name = "d2";
            d.Location = "l2";
            d.LocationLongitude = 10;
            d.LocationLatitude = 10;
            d.Status = false;

            a.CpuUsage = 10;
            a.RamUsage = 10;
            a.Hddusage = 10;
            a.Gpuusage = 10;

            ad.Device = d;
            ad.Averages = a;
            ad.GroupName = "g2";
            devices.Add(ad);
            #endregion

            #region
            ad = new AllInfoForDevice();
            d = new Device();
            a = new Averages();

            d.Name = "d3";
            d.Location = "l3";
            d.LocationLongitude = 15;
            d.LocationLatitude = 15;
            d.Status = true;

            a.CpuUsage = 15;
            a.RamUsage = 15;
            a.Hddusage = 15;
            a.Gpuusage = 15;

            ad.Device = d;
            ad.Averages = a;
            ad.GroupName = "g3";
            devices.Add(ad);
            #endregion
        }

        [Test]
        public void NoRule()
        {
            GroupModel g = new GroupModel();
            g.rules = new List<object>();

            q.query = g;
            Assert.IsTrue(devices.TrueForAll(x => q.query.Eval(x)));
        }

        [Test]
        public void SingleRuleFloat()
        {
            GroupModel g = new GroupModel();
            g.rules = new List<object>();
            g.rule_items = new List<QueryItem>();

            RuleModel r = new RuleModel();
            r.field = "latitude";
            r.operator_str = "=";
            r.value = "10";
            g.rule_items.Add(r);

            q.query = g;
            Assert.IsFalse( q.query.Eval( devices[0] ) ) ;
            Assert.IsTrue ( q.query.Eval( devices[1] ) ) ;
            Assert.IsFalse( q.query.Eval( devices[2] ) );

        }

        [Test]
        public void SingleRuleBool()
        {
            GroupModel g = new GroupModel();
            g.rules = new List<object>();
            g.rule_items = new List<QueryItem>();

            RuleModel r = new RuleModel();
            r.field = "status";
            r.operator_str = "=";
            r.value = "False";
            g.rule_items.Add(r);

            q.query = g;
            Assert.IsFalse(q.query.Eval(devices[0]));
            Assert.IsTrue(q.query.Eval(devices[1]));
            Assert.IsFalse(q.query.Eval(devices[2]));

        }

        [Test]
        public void SingleRuleString()
        {
            GroupModel g = new GroupModel();
            g.rules = new List<object>();
            g.rule_items = new List<QueryItem>();

            RuleModel r = new RuleModel();
            r.field = "name";
            r.operator_str = "=";
            r.value = "d2";
            g.rule_items.Add(r);

            q.query = g;
            Assert.IsFalse(q.query.Eval(devices[0]));
            Assert.IsTrue(q.query.Eval(devices[1]));
            Assert.IsFalse(q.query.Eval(devices[2]));

        }

        [Test]
        public void SingleRuleFloatLess()
        {
            GroupModel g = new GroupModel();
            g.rules = new List<object>();
            g.rule_items = new List<QueryItem>();

            RuleModel r = new RuleModel();
            r.field = "latitude";
            r.operator_str = "<";
            r.value = "10";
            g.rule_items.Add(r);

            q.query = g;
            Assert.IsTrue(q.query.Eval(devices[0]));
            Assert.IsFalse(q.query.Eval(devices[1]));
            Assert.IsFalse(q.query.Eval(devices[2]));

        }

        [Test]
        public void SingleRuleFloatLessEqual()
        {
            GroupModel g = new GroupModel();
            g.rules = new List<object>();
            g.rule_items = new List<QueryItem>();

            RuleModel r = new RuleModel();
            r.field = "latitude";
            r.operator_str = "<=";
            r.value = "10";
            g.rule_items.Add(r);

            q.query = g;
            Assert.IsTrue(q.query.Eval(devices[0]));
            Assert.IsTrue(q.query.Eval(devices[1]));
            Assert.IsFalse(q.query.Eval(devices[2]));

        }

        [Test]
        public void SingleRuleFloatGreater()
        {
            GroupModel g = new GroupModel();
            g.rules = new List<object>();
            g.rule_items = new List<QueryItem>();

            RuleModel r = new RuleModel();
            r.field = "latitude";
            r.operator_str = ">";
            r.value = "10";
            g.rule_items.Add(r);

            q.query = g;
            Assert.IsFalse(q.query.Eval(devices[0]));
            Assert.IsFalse(q.query.Eval(devices[1]));
            Assert.IsTrue(q.query.Eval(devices[2]));

        }

        [Test]
        public void SingleRuleFloatGreaterEqual()
        {
            GroupModel g = new GroupModel();
            g.rules = new List<object>();
            g.rule_items = new List<QueryItem>();

            RuleModel r = new RuleModel();
            r.field = "latitude";
            r.operator_str = ">=";
            r.value = "10";
            g.rule_items.Add(r);

            q.query = g;
            Assert.IsFalse(q.query.Eval(devices[0]));
            Assert.IsTrue(q.query.Eval(devices[1]));
            Assert.IsTrue(q.query.Eval(devices[2]));

        }

        [Test]
        public void SingleRuleFloatNotEqual()
        {
            GroupModel g = new GroupModel();
            g.rules = new List<object>();
            g.rule_items = new List<QueryItem>();

            RuleModel r = new RuleModel();
            r.field = "latitude";
            r.operator_str = "!=";
            r.value = "10";
            g.rule_items.Add(r);

            q.query = g;
            Assert.IsTrue(q.query.Eval(devices[0]));
            Assert.IsFalse(q.query.Eval(devices[1]));
            Assert.IsTrue(q.query.Eval(devices[2]));

        }

        [Test]
        public void SingleRuleStringBeginsWith()
        {
            GroupModel g = new GroupModel();
            g.rules = new List<object>();
            g.rule_items = new List<QueryItem>();

            RuleModel r = new RuleModel();
            r.field = "name";
            r.operator_str = "beginsWith";
            r.value = "d";
            g.rule_items.Add(r);

            q.query = g;
            Assert.IsTrue(q.query.Eval(devices[0]));
            Assert.IsTrue(q.query.Eval(devices[1]));
            Assert.IsTrue(q.query.Eval(devices[2]));

        }

        [Test]
        public void SingleRuleStringDoesNotBeginWith()
        {
            GroupModel g = new GroupModel();
            g.rules = new List<object>();
            g.rule_items = new List<QueryItem>();

            RuleModel r = new RuleModel();
            r.field = "name";
            r.operator_str = "doesNotBeginWith";
            r.value = "d";
            g.rule_items.Add(r);

            q.query = g;
            Assert.IsFalse(q.query.Eval(devices[0]));
            Assert.IsFalse(q.query.Eval(devices[1]));
            Assert.IsFalse(q.query.Eval(devices[2]));

        }


        [Test]
        public void SingleRuleStringEndsWith()
        {
            GroupModel g = new GroupModel();
            g.rules = new List<object>();
            g.rule_items = new List<QueryItem>();

            RuleModel r = new RuleModel();
            r.field = "name";
            r.operator_str = "endsWith";
            r.value = "2";
            g.rule_items.Add(r);

            q.query = g;
            Assert.IsFalse(q.query.Eval(devices[0]));
            Assert.IsTrue(q.query.Eval(devices[1]));
            Assert.IsFalse(q.query.Eval(devices[2]));

        }

        [Test]
        public void SingleRuleStringDoesNotEndWith()
        {
            GroupModel g = new GroupModel();
            g.rules = new List<object>();
            g.rule_items = new List<QueryItem>();

            RuleModel r = new RuleModel();
            r.field = "name";
            r.operator_str = "doesNotEndWith";
            r.value = "2";
            g.rule_items.Add(r);

            q.query = g;
            Assert.IsTrue(q.query.Eval(devices[0]));
            Assert.IsFalse(q.query.Eval(devices[1]));
            Assert.IsTrue(q.query.Eval(devices[2]));

        }

        [Test]
        public void SingleRuleStringContains()
        {
            GroupModel g = new GroupModel();
            g.rules = new List<object>();
            g.rule_items = new List<QueryItem>();

            RuleModel r = new RuleModel();
            r.field = "name";
            r.operator_str = "contains";
            r.value = "2";
            g.rule_items.Add(r);

            q.query = g;
            Assert.IsFalse(q.query.Eval(devices[0]));
            Assert.IsTrue(q.query.Eval(devices[1]));
            Assert.IsFalse(q.query.Eval(devices[2]));

        }

        [Test]
        public void SingleRuleStringDoesNotContainh()
        {
            GroupModel g = new GroupModel();
            g.rules = new List<object>();
            g.rule_items = new List<QueryItem>();

            RuleModel r = new RuleModel();
            r.field = "name";
            r.operator_str = "doesNotContain";
            r.value = "2";
            g.rule_items.Add(r);

            q.query = g;
            Assert.IsTrue(q.query.Eval(devices[0]));
            Assert.IsFalse(q.query.Eval(devices[1]));
            Assert.IsTrue(q.query.Eval(devices[2]));

        }

        [Test]
        public void MultipleRulesAnd()
        {
            GroupModel g = new GroupModel();
            g.rules = new List<object>();
            g.rule_items = new List<QueryItem>();
            g.combinator = "AND";

            RuleModel r;

            r = new RuleModel();
            r.field = "latitude";
            r.operator_str = ">=";
            r.value = "10";
            g.rule_items.Add(r);

            r = new RuleModel();
            r.field = "latitude";
            r.operator_str = "<=";
            r.value = "10";
            g.rule_items.Add(r);

            q.query = g;
            Assert.IsFalse(q.query.Eval(devices[0]));
            Assert.IsTrue(q.query.Eval(devices[1]));
            Assert.IsFalse(q.query.Eval(devices[2]));

        }

        [Test]
        public void MultipleRulesOr()
        {
            GroupModel g = new GroupModel();
            g.rules = new List<object>();
            g.rule_items = new List<QueryItem>();
            g.combinator = "OR";

            RuleModel r;

            r = new RuleModel();
            r.field = "latitude";
            r.operator_str = ">";
            r.value = "10";
            g.rule_items.Add(r);

            r = new RuleModel();
            r.field = "latitude";
            r.operator_str = "<";
            r.value = "10";
            g.rule_items.Add(r);

            q.query = g;
            Assert.IsTrue(q.query.Eval(devices[0]));
            Assert.IsFalse(q.query.Eval(devices[1]));
            Assert.IsTrue(q.query.Eval(devices[2]));

        }

        [Test]
        public void SingGroupInGroup()
        {
            GroupModel g = new GroupModel();
            g.rules = new List<object>();
            g.rule_items = new List<QueryItem>();

            GroupModel g1 = new GroupModel();
            g1.rules = new List<object>();
            g1.rule_items = new List<QueryItem>();
            g1.combinator = "OR";

            RuleModel r;

            r = new RuleModel();
            r.field = "latitude";
            r.operator_str = ">";
            r.value = "10";
            g1.rule_items.Add(r);

            r = new RuleModel();
            r.field = "latitude";
            r.operator_str = "<";
            r.value = "10";
            g1.rule_items.Add(r);

            g.rule_items.Add(g1);

            q.query = g;
            Assert.IsTrue(q.query.Eval(devices[0]));
            Assert.IsFalse(q.query.Eval(devices[1]));
            Assert.IsTrue(q.query.Eval(devices[2]));

        }

        [Test]
        public void MultipleSubGroups()
        {
            GroupModel g = new GroupModel();
            g.rules = new List<object>();
            g.rule_items = new List<QueryItem>();
            g.combinator = "OR";

            GroupModel g1 = new GroupModel();
            g1.rules = new List<object>();
            g1.rule_items = new List<QueryItem>();
            g1.combinator = "AND";

            GroupModel g2 = new GroupModel();
            g2.rules = new List<object>();
            g2.rule_items = new List<QueryItem>();
            g2.combinator = "AND";

            RuleModel r;

            r = new RuleModel();
            r.field = "latitude";
            r.operator_str = ">=";
            r.value = "10";
            g1.rule_items.Add(r);

            r = new RuleModel();
            r.field = "latitude";
            r.operator_str = "<=";
            r.value = "10";
            g1.rule_items.Add(r);

            r = new RuleModel();
            r.field = "latitude";
            r.operator_str = "!=";
            r.value = "10";
            g2.rule_items.Add(r);

            r = new RuleModel();
            r.field = "latitude";
            r.operator_str = "<";
            r.value = "10";
            g2.rule_items.Add(r);

            g.rule_items.Add(g1);
            g.rule_items.Add(g2);

            q.query = g;
            Assert.IsTrue(q.query.Eval(devices[0]));
            Assert.IsTrue(q.query.Eval(devices[1]));
            Assert.IsFalse(q.query.Eval(devices[2]));

        }

        [Test]
        public void GroupAndRule()
        {
            GroupModel g = new GroupModel();
            g.rules = new List<object>();
            g.rule_items = new List<QueryItem>();
            g.combinator = "OR";

            GroupModel g1 = new GroupModel();
            g1.rules = new List<object>();
            g1.rule_items = new List<QueryItem>();
            g1.combinator = "AND";

            RuleModel r;

            r = new RuleModel();
            r.field = "latitude";
            r.operator_str = ">=";
            r.value = "10";
            g1.rule_items.Add(r);

            r = new RuleModel();
            r.field = "latitude";
            r.operator_str = "<=";
            r.value = "10";
            g1.rule_items.Add(r);

            r = new RuleModel();
            r.field = "latitude";
            r.operator_str = "<";
            r.value = "10";
            g.rule_items.Add(r);

            g.rule_items.Add(g1);

            q.query = g;
            Assert.IsTrue(q.query.Eval(devices[0]));
            Assert.IsTrue(q.query.Eval(devices[1]));
            Assert.IsFalse(q.query.Eval(devices[2]));

        }
    }
}
