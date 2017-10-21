using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jy.MVC.Infrastructure
{
    public static class API
    {
        public static class Auth
        {
            public static string CheckUser(string baseUri, string userName, string password)
            {
                return $"{baseUri}/{userName}";
            }

            public static string Index(string baseUri)
            {
                return $"{baseUri}/Index";
            }
            public static string GetToken(string baseUri)
            {
                return $"{baseUri}/GetToken";
            }
            public static string VerifyToken(string baseUri)
            {
                return $"{baseUri}/VerifyToken";
            }
            public static string LogOutToken(string baseUri)
            {
                return $"{baseUri}/LogOutToken";
            }
            public static string GetMenuForLeftMenu(string baseUri,string token)
            {
                return $"{baseUri}/GetMenuForLeftMenu?token={token}";
            }
            public static string GetTreeData(string baseUri)
            {
                return $"{baseUri}/GetTreeData";
            }
            public static string GetMenuTreeData(string baseUri)
            {
                return $"{baseUri}/GetMenuTreeData";
            }
            public static string GetMenuTreeData(string baseUri,Guid id)
            {
                return $"{baseUri}/GetMenuTreeData/{id}";
            }
            public static string GetChildrenByParent(string baseUri, Guid departmentId, int startPage, int pageSize)
            { //api/v1/[controller]/GetChildrenByParent/1[?pageSize=3&pageIndex=10]
                return $"{baseUri}/GetChildrenByParent/{departmentId}?startPage={startPage}&pageSize={pageSize}";
            }
            public static string GetListPaged(string baseUri, int startPage, int pageSize)
            {// GET api/v1/[controller]/GetListPaged[?pageSize=3&pageIndex=10]
                return $"{baseUri}/GetListPaged?startPage={startPage}&pageSize={pageSize}";
            }
            public static string GetMneusByParent(string baseUri, Guid parentId, int startPage, int pageSize)
            {// GET api/v1/[controller]/GetMneusByParent/1[?pageSize=3&pageIndex=10]
                return $"{baseUri}/GetMneusByParent/{parentId}?startPage={startPage}&pageSize={pageSize}";
            }
            public static string GetChildrenByParent2(string baseUri, Guid parentId, int startPage, int pageSize)
            {// GET api/v1/[controller]/GetChildrenByParent/1[?pageSize=3&pageIndex=10]
                return $"{baseUri}/GetChildrenByParent/{parentId}?startPage={startPage}&pageSize={pageSize}";
            }
            public static string Edit(string baseUri)
            {
                return $"{baseUri}";
            }
            public static string Create(string baseUri)
            {
                return $"{baseUri}";
            }
            public static string ResetPassword(string baseUri)
            {
                return $"{baseUri}/ResetPassword";
            }
            public static string DeleteMuti(string baseUri,string ids)
            {// DELETE api/User/DeleteMuti/5,6
                return $"{baseUri}/DeleteMuti/{ids}";
            }
            public static string Delete(string baseUri,Guid id)
            {
                return $"{baseUri}/{id}";
            }
            public static string Get(string baseUri, Guid id)
            {
                return $"{baseUri}/{id}";
            }
            public static string RoleMenu(string baseUri)
            {
                return $"{baseUri}/RoleMenu";
            }
            public static string BatchUserRole(string baseUri)
            {//api/Authority/Batch
                return $"{baseUri}/Batch";
            }
            public static string GetBatchRoleTreeData(string baseUri)
            {
                return $"{baseUri}/GetBatchRoleTreeData";
            }
            public static string GetRoleTreeData(string baseUri,Guid id)
            {
                return $"{baseUri}/GetRoleTreeData/{id}";
            }
            public static string UserRole(string baseUri)
            {
                return $"{baseUri}";
            }
            
        }
    }
}
