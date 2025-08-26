// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core.Service;

/// <summary>
/// 系统用户扩展机构服务
/// </summary>
public class SysFileCategoryService : ITransient
{
    private readonly SqlSugarRepository<SysFileCategory> _sysFileCategoryRep;
    private readonly SqlSugarRepository<SysCategory> _sysCategoryRep;

    public SysFileCategoryService(
        SqlSugarRepository<SysFileCategory> sysFileCategoryRep,
        SqlSugarRepository<SysCategory> sysCategoryRep)
    {
        _sysFileCategoryRep = sysFileCategoryRep;
        _sysCategoryRep = sysCategoryRep;
    }

    /// <summary>
    /// 获取用户扩展机构集合
    /// </summary>
    /// <param name="fileId"></param>
    /// <returns></returns>
    public async Task<List<SysFileCategory>> GetFileCategoryList(long fileId)
    {
        return await _sysFileCategoryRep.GetListAsync(u => u.FileId == fileId);
    }

    /// <summary>
    /// 更新文件分类关联
    /// </summary>
    /// <param name="fileId"></param>
    /// <param name="categoryId"></param>
    /// <returns></returns>
    public async Task UpdateFileCategory(long fileId, long categoryId)
    {
        // 删除该文件的所有分类关联
        await _sysFileCategoryRep.DeleteAsync(u => u.FileId == fileId);

        // 如果categoryId为0或负数，表示不关联任何分类，直接返回
        //if (categoryId <= 0) return;

        // 获取分类及其所有祖先分类的ID列表
        var categoryIdList = await GetCategoryAndAncestorIds(categoryId);

        // 创建文件分类关联列表
        var fileCategories = categoryIdList.Select(id => new SysFileCategory
        {
            FileId = fileId,
            CategoryId = id
        }).ToList();

        // 批量插入关联关系
        await _sysFileCategoryRep.InsertRangeAsync(fileCategories);
    }

    /// <summary>
    /// 递归获取分类及其所有祖先分类的ID列表
    /// </summary>
    /// <param name="categoryId"></param>
    /// <returns></returns>
    [NonAction]
    public async Task<List<long>> GetCategoryAndAncestorIds(long categoryId)
    {
        var categoryIds = new List<long>();
        var currentId = categoryId;

        while (currentId > 0)
        {
            categoryIds.Add(currentId);

            // 查找父分类
            var category = await _sysCategoryRep.GetFirstAsync(u => u.Id == currentId);
            if (category == null || category.Pid == 0)
                break;

            currentId = category.Pid;
        }

        return categoryIds;
    }

    /// <summary>
    /// 根据机构Id集合删除扩展机构
    /// </summary>
    /// <param name="categoryIdList"></param>
    /// <returns></returns>
    public async Task DeleteFileCategoryByCategoryIdList(List<long> categoryIdList)
    {
        await _sysFileCategoryRep.DeleteAsync(u => categoryIdList.Contains(u.CategoryId));
    }

    /// <summary>
    /// 根据用户Id删除扩展机构
    /// </summary>
    /// <param name="fileId"></param>
    /// <returns></returns>
    public async Task DeleteFileCategoryByFileId(long fileId)
    {
        await _sysFileCategoryRep.DeleteAsync(u => u.FileId == fileId);
    }

    /// <summary>
    /// 根据机构Id判断是否有用户
    /// </summary>
    /// <param name="categoryId"></param>
    /// <returns></returns>
    public async Task<bool> HasFileCategory(long categoryId)
    {
        return await _sysFileCategoryRep.IsAnyAsync(u => u.CategoryId == categoryId);
    }

}