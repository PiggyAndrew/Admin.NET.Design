// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core.Service;

/// <summary>
/// 系统机构服务 🧩
/// </summary>
[ApiDescriptionSettings(Order = 470)]
public class SysCategoryService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<SysCategory> _sysCategoryRep;
    private readonly UserManager _userManager;
    private readonly SysCacheService _sysCacheService;
    private readonly SysFileCategoryService _sysFileCategoryService;

    public SysCategoryService(
        SqlSugarRepository<SysCategory> sysOrgRep,
        UserManager userManager,
         SysCacheService sysCacheService,
        SysFileCategoryService sysFileCategoryService
        )
    {
        _sysCategoryRep = sysOrgRep;
        _userManager = userManager;
        _sysCacheService = sysCacheService;
        _sysFileCategoryService = sysFileCategoryService;
    }

    /// <summary>
    /// 获取列表 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("获取资源类型列表")]
    public async Task<List<SysCategory>> GetList([FromQuery] CategoryInput input)
    {
        // 获取拥有的机构Id集合
        var fileCategoryIdList = await GetFileCategoryIdList();

        var queryable = _sysCategoryRep.AsQueryable()
            .WhereIF(_userManager.SuperAdmin && input.TenantId > 0, u => u.TenantId == input.TenantId)
            .OrderBy(u => new { u.OrderNo, u.Id });

        // 带条件筛选时返回列表数据
        if (!string.IsNullOrWhiteSpace(input.Name) || !string.IsNullOrWhiteSpace(input.Code) || !string.IsNullOrWhiteSpace(input.Type))
        {
            return await queryable.WhereIF(fileCategoryIdList.Count > 0, u => fileCategoryIdList.Contains(u.Id))
                .WhereIF(!string.IsNullOrWhiteSpace(input.Name), u => u.Name.Contains(input.Name))
                .WhereIF(!string.IsNullOrWhiteSpace(input.Code), u => u.Code == input.Code)
                .WhereIF(!string.IsNullOrWhiteSpace(input.Type), u => u.Type == input.Type)
                .ToListAsync();
        }

        List<SysCategory> categoryTree;
        if (_userManager.SuperAdmin)
        {
            categoryTree = await queryable.ToTreeAsync(u => u.Children, u => u.Pid, input.Id);
        }
        else
        {
            categoryTree = await queryable.ToTreeAsync(u => u.Children, u => u.Pid, input.Id, fileCategoryIdList.Select(d => (object)d).ToArray());
            // 递归禁用没权限的机构（防止用户修改或创建无权的机构和用户）
            HandlerCategoryTree(categoryTree, fileCategoryIdList);
        }

        var sysCategory = await _sysCategoryRep.GetSingleAsync(u => u.Id == input.Id);
        if (sysCategory == null) return categoryTree;

        sysCategory.Children = categoryTree;
        categoryTree = new List<SysCategory> { sysCategory };
        return categoryTree;
    }

    /// <summary>
    /// 递归禁用没权限的机构
    /// </summary>
    /// <param name="categoryTree"></param>
    /// <param name="fileCategoryIdList"></param>
    private static void HandlerCategoryTree(List<SysCategory> categoryTree, List<long> fileCategoryIdList)
    {
        foreach (var category in categoryTree)
        {
            category.Disabled = !fileCategoryIdList.Contains(category.Id); // 设置禁用/不可选择
            if (category.Children != null)
                HandlerCategoryTree(category.Children, fileCategoryIdList);
        }
    }

    /// <summary>
    /// 增加资源类型 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Add"), HttpPost]
    [DisplayName("增加资源类型")]
    public async Task<long> AddCategory(AddCategoryInput input)
    {
        //if (!_userManager.SuperAdmin && input.Pid == 0)
        //    throw Oops.Oh(ErrorCodeEnum.D2009);

        if (await _sysCategoryRep.IsAnyAsync(u => u.Name == input.Name && u.Code == input.Code))
            throw Oops.Oh(ErrorCodeEnum.D2002);

        //if (!_userManager.SuperAdmin && input.Pid != 0)
        //{
        //    // 新增机构父Id不是0，则进行权限校验
        //    var CategoryIds = await GetFileCategoryIdList();
        //    // 新增机构的父机构不在自己的数据范围内
        //    if (CategoryIds.Count < 1 || !CategoryIds.Contains(input.Pid))
        //        throw Oops.Oh(ErrorCodeEnum.D2003);
        //}

        // 删除与此父机构有关的用户机构缓存
        if (input.Pid == 0)
        {
            DeleteAllUserOrgCache(0, 0);
        }
        else
        {
            var pOrg = await _sysCategoryRep.GetFirstAsync(u => u.Id == input.Pid);
            if (pOrg != null)
                DeleteAllUserOrgCache(pOrg.Id, pOrg.Pid);
        }
        var category = input.Adapt<SysCategory>();
        category.Id = YitIdHelper.NextId();
        var newCateogory = await _sysCategoryRep.AsInsertable(category).ExecuteReturnEntityAsync();
        return newCateogory.Id;
    }

    /// <summary>
    /// 批量增加资源类型
    /// </summary>
    /// <param name="orgs"></param>
    /// <returns></returns>
    [NonAction]
    public async Task BatchAddOrgs(List<SysCategory> categories)
    {
        DeleteAllUserOrgCache(0, 0);
        await _sysCategoryRep.AsDeleteable().ExecuteCommandAsync();
        await _sysCategoryRep.AsInsertable(categories).ExecuteCommandAsync();
    }

    /// <summary>
    /// 更新资源类型 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [UnitOfWork]
    [ApiDescriptionSettings(Name = "Update"), HttpPost]
    [DisplayName("更新资源类型")]
    public async Task UpdateCategory(UpdateOrgInput input)
    {
        if (!_userManager.SuperAdmin && input.Pid == 0)
            throw Oops.Oh(ErrorCodeEnum.D2012);

        if (input.Pid != 0)
        {
            //var pOrg = await _sysOrgRep.GetFirstAsync(u => u.Id == input.Pid);
            //_ = pOrg ?? throw Oops.Oh(ErrorCodeEnum.D2000);

            // 若父机构发生变化则清空用户机构缓存
            var sysOrg = await _sysCategoryRep.GetFirstAsync(u => u.Id == input.Id);
            if (sysOrg != null && sysOrg.Pid != input.Pid)
            {
                // 删除与此机构、新父机构有关的用户机构缓存
                DeleteAllUserOrgCache(sysOrg.Id, input.Pid);
            }
        }
        if (input.Id == input.Pid)
            throw Oops.Oh(ErrorCodeEnum.D2001);

        if (await _sysCategoryRep.IsAnyAsync(u => u.Name == input.Name && u.Code == input.Code && u.Id != input.Id))
            throw Oops.Oh(ErrorCodeEnum.D2002);

        // 父Id不能为自己的子节点
        var childIdList = await GetChildIdListWithSelfById(input.Id);
        if (childIdList.Contains(input.Pid))
            throw Oops.Oh(ErrorCodeEnum.D2001);

        // 是否有权限操作此机构
        if (!_userManager.SuperAdmin)
        {
            var orgIdList = await GetFileCategoryIdList();
            if (orgIdList.Count < 1 || !orgIdList.Contains(input.Id))
                throw Oops.Oh(ErrorCodeEnum.D2003);
        }

        await _sysCategoryRep.AsUpdateable(input.Adapt<SysCategory>()).IgnoreColumns(true).ExecuteCommandAsync();
    }

    /// <summary>
    /// 删除资源类型 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [UnitOfWork]
    [ApiDescriptionSettings(Name = "Delete"), HttpPost]
    [DisplayName("删除资源类型")]
    public async Task DeleteOrg(DeleteOrgInput input)
    {
        var sysOrg = await _sysCategoryRep.GetFirstAsync(u => u.Id == input.Id) ?? throw Oops.Oh(ErrorCodeEnum.D1002);

        // 是否有权限操作此机构
        if (!_userManager.SuperAdmin)
        {
            var orgIdList = await GetFileCategoryIdList();
            if (orgIdList.Count < 1 || !orgIdList.Contains(sysOrg.Id))
                throw Oops.Oh(ErrorCodeEnum.D2003);
        }

        // 若机构为租户默认机构禁止删除
        var isTenantOrg = await _sysCategoryRep.ChangeRepository<SqlSugarRepository<SysTenant>>()
            .IsAnyAsync(u => u.OrgId == input.Id);
        if (isTenantOrg)
            throw Oops.Oh(ErrorCodeEnum.D2008);

        // 若机构有用户则禁止删除
        var orgHasEmp = await _sysCategoryRep.ChangeRepository<SqlSugarRepository<SysUser>>()
            .IsAnyAsync(u => u.OrgId == input.Id);
        if (orgHasEmp)
            throw Oops.Oh(ErrorCodeEnum.D2004);

        // 若扩展机构有用户则禁止删除
        var hasExtOrgEmp = await _sysFileCategoryService.HasFileCategory(sysOrg.Id);
        if (hasExtOrgEmp)
            throw Oops.Oh(ErrorCodeEnum.D2005);

        // 若子机构有用户则禁止删除
        var childOrgTreeList = await _sysCategoryRep.AsQueryable().ToChildListAsync(u => u.Pid, input.Id, true);
        var childOrgIdList = childOrgTreeList.Select(u => u.Id).ToList();

        // 若子机构有用户则禁止删除
        var cOrgHasEmp = await _sysCategoryRep.ChangeRepository<SqlSugarRepository<SysUser>>()
            .IsAnyAsync(u => childOrgIdList.Contains(u.OrgId));
        if (cOrgHasEmp) throw Oops.Oh(ErrorCodeEnum.D2007);

        // 若有绑定注册方案则禁止删除
        var hasUserRegWay = await _sysCategoryRep.Context.Queryable<SysUserRegWay>().AnyAsync(u => u.OrgId == input.Id);
        if (hasUserRegWay) throw Oops.Oh(ErrorCodeEnum.D2010);

        // 删除与此机构、父机构有关的用户机构缓存
        DeleteAllUserOrgCache(sysOrg.Id, sysOrg.Pid);

        // 级联删除机构子节点
        await _sysCategoryRep.DeleteAsync(u => childOrgIdList.Contains(u.Id));

        // 级联删除用户机构数据
        await _sysFileCategoryService.DeleteFileCategoryByCategoryIdList(childOrgIdList);
    }

    /// <summary>
    /// 删除与此资源类型、父资源类型有关的用户资源类型缓存
    /// </summary>
    /// <param name="orgId"></param>
    /// <param name="orgPid"></param>
    private void DeleteAllUserOrgCache(long orgId, long orgPid)
    {
        var userOrgKeyList = _sysCacheService.GetKeysByPrefixKey(CacheConst.KeyUserOrg);
        if (userOrgKeyList is not { Count: > 0 }) return;

        foreach (var userOrgKey in userOrgKeyList)
        {
            var userOrgList = _sysCacheService.Get<List<long>>(userOrgKey);
            var userId = long.Parse(userOrgKey.Substring(CacheConst.KeyUserOrg));
            if (userOrgList != null && (userOrgList.Contains(orgId) || userOrgList.Contains(orgPid)))
                SqlSugarFilter.DeleteUserOrgCache(userId, _sysCategoryRep.Context.CurrentConnectionConfig.ConfigId.ToString());

            if (orgPid != 0) continue;

            var dataScope = _sysCacheService.Get<int>($"{CacheConst.KeyRoleMaxDataScope}{userId}");
            if (dataScope == (int)DataScopeEnum.All)
                SqlSugarFilter.DeleteUserOrgCache(userId, _sysCategoryRep.Context.CurrentConnectionConfig.ConfigId.ToString());
        }
    }

    /// <summary>
    /// 获取当前用户机构Id集合
    /// </summary>
    /// <returns></returns>
    [NonAction]
    public async Task<List<long>> GetFileCategoryIdList()
    {
        if (_userManager.SuperAdmin) return new();
        return await GetFileCategoryIdList(_userManager.UserId);
    }

    /// <summary>
    /// 根据指定用户Id获取资源类型Id集合
    /// </summary>
    /// <returns></returns>
    [NonAction]
    public async Task<List<long>> GetFileCategoryIdList(long userId)
    {
        //var orgIdList = _sysCacheService.Get<List<long>>($"{CacheConst.KeyUserOrg}{userId}"); // 取缓存
        //if (orgIdList is { Count: >= 1 }) return orgIdList;

        // 本人创建机构集合
        var orgList0 = await _sysCategoryRep.AsQueryable().Where(u => u.CreateUserId == userId).Select(u => u.Id).ToListAsync();

        // 扩展机构集合
        var orgList1 = await _sysFileCategoryService.GetFileCategoryList(userId);


        // 机构并集
        var orgIdList = orgList1.Select(u => u.CategoryId).Union(orgList0).ToList();

        //_sysCacheService.Set($"{CacheConst.KeyUserOrg}{userId}", orgIdList, TimeSpan.FromDays(7)); // 存缓存
        return orgIdList;
    }

    /// <summary>
    /// 根据节点Id获取子节点Id集合(包含自己)
    /// </summary>
    /// <param name="pid"></param>
    /// <returns></returns>
    [NonAction]
    public async Task<List<long>> GetChildIdListWithSelfById(long pid)
    {
        var orgTreeList = await _sysCategoryRep.AsQueryable().ToChildListAsync(u => u.Pid, pid, true);
        return orgTreeList.Select(u => u.Id).ToList();
    }
}