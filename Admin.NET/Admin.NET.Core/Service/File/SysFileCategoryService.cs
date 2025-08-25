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

    public SysFileCategoryService(SqlSugarRepository<SysFileCategory> sysUserExtOrgRep)
    {
        _sysFileCategoryRep = sysUserExtOrgRep;
    }

    /// <summary>
    /// 获取用户扩展机构集合
    /// </summary>
    /// <param name="fileId"></param>
    /// <returns></returns>
    public async Task<List<SysFileCategory>> GetUserExtOrgList(long fileId)
    {
        return await _sysFileCategoryRep.GetListAsync(u => u.FileId == fileId);
    }

    /// <summary>
    /// 更新用户扩展机构
    /// </summary>
    /// <param name="fileId"></param>
    /// <param name="fileCategories"></param>
    /// <returns></returns>
    public async Task UpdateFileCategory(long fileId, List<SysFileCategory> fileCategories)
    {
        await _sysFileCategoryRep.DeleteAsync(u => u.FileId == fileId);

        if (fileCategories == null || fileCategories.Count < 1) return;
        fileCategories.ForEach(u =>
        {
            u.FileId = fileId;
        });
        await _sysFileCategoryRep.InsertRangeAsync(fileCategories);
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