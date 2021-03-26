﻿using MonitorWebAPI.Models;
using System.Collections.Generic;
using System.Linq;
namespace MonitorWebAPI.Helpers
{
    public class HelperMethods
    {

        public GroupHierarchyModel FindHierarchyTree(Group g)
        {
            monitorContext mc = new monitorContext();
            GroupHierarchyModel ghm = new GroupHierarchyModel() { GroupId = g.GroupId, Name = g.Name, SubGroups = new List<GroupHierarchyModel>() };
            findSubgroups(ghm, mc);
            return ghm;
        }

        void findSubgroups(GroupHierarchyModel ghm, monitorContext mc)
        {
            var tempList = mc.Groups.Where(x => x.ParentGroup == ghm.GroupId);
            foreach(var group in tempList)
            {
                ghm.SubGroups.Add(new GroupHierarchyModel { GroupId = group.GroupId, Name = group.Name, SubGroups = new List<GroupHierarchyModel>() });
            }
            foreach (var tempGhm in ghm.SubGroups)
            {
                findSubgroups(tempGhm, mc);
            }
        }

        // ----------- da li uređaj pripada korisnikovom stablu
        public bool CheckIfDeviceBelongsToUsersTree(VerifyUserModel vu, int deviceId)
        {
            monitorContext mc = new monitorContext();
            string groupName = mc.Groups.Where(x => x.GroupId == vu.groupId).FirstOrDefault().Name;
            bool belongs = false;
            int? deviceGroupId = mc.DeviceGroups.Where(x => x.DeviceId == deviceId).FirstOrDefault().GroupId;
            GroupHierarchyModel ghm = new GroupHierarchyModel() { GroupId = vu.groupId, Name = groupName, SubGroups = new List<GroupHierarchyModel>() };
            checkSubgroup(ref belongs, ghm, mc, deviceGroupId);
            return belongs;
        }

        void checkSubgroup(ref bool belongs, GroupHierarchyModel ghm, monitorContext mc, int? deviceGroupId)
        {
            var tempList = mc.Groups.Where(x => x.ParentGroup == ghm.GroupId);
            foreach (var group in tempList)
            {
                if(deviceGroupId==group.GroupId)
                {
                    belongs = true;
                    return;
                }
                ghm.SubGroups.Add(new GroupHierarchyModel { GroupId = group.GroupId, Name = group.Name, SubGroups = new List<GroupHierarchyModel>() });
            }
            if(belongs!=true)
            {
                foreach (var tempGhm in ghm.SubGroups)
                {
                    checkSubgroup(ref belongs, tempGhm, mc, deviceGroupId);
                }
            }
        }
        // ------------


        //------------- Da li grupa pripada korisnikovom stablu
        public bool CheckIfGroupBelongsToUsersTree(VerifyUserModel vu, int? groupId)
        {
            monitorContext mc = new monitorContext();
            string groupName = mc.Groups.Where(x => x.GroupId == vu.groupId).FirstOrDefault().Name;
            bool belongs = vu.groupId==groupId;
            GroupHierarchyModel ghm = new GroupHierarchyModel() { GroupId = vu.groupId, Name = groupName, SubGroups = new List<GroupHierarchyModel>() };
            ifGroupBelongsToTree(ref belongs, ghm, mc, groupId);
            return belongs;
        }

        void ifGroupBelongsToTree(ref bool belongs, GroupHierarchyModel ghm, monitorContext mc, int? groupId)
        {
            if(ghm.GroupId==groupId)
            {
                belongs = true;
                return;
            }
            var tempList = mc.Groups.Where(x => x.ParentGroup == ghm.GroupId);
            foreach (var group in tempList)
            {
                if (groupId == group.GroupId)
                {
                    belongs = true;
                    return;
                }
                ghm.SubGroups.Add(new GroupHierarchyModel { GroupId = group.GroupId, Name = group.Name, SubGroups = new List<GroupHierarchyModel>() });
            }
            foreach (var tempGhm in ghm.SubGroups)
            {
                ifGroupBelongsToTree(ref belongs, tempGhm, mc, groupId);
            }
        }
        //-------------

    }
}