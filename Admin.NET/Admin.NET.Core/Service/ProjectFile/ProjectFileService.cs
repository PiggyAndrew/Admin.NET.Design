using Aliyun.OSS.Util;

namespace Admin.NET.Core.Service;

/// <summary>
/// 系统文件服务 🧩
/// </summary>
[ApiDescriptionSettings(Order = 500, Description = "工程文件")]
public class ProjectFileService : IDynamicApiController, ITransient
{
    private readonly UserManager _userManager;
    private readonly SqlSugarRepository<SysFile> _sysFileRep;
    private readonly OSSProviderOptions _OSSProviderOptions;
    private readonly UploadOptions _uploadOptions;
    private readonly string _imageType = ".jpeg.jpg.png.bmp.gif.tif";
    private readonly INamedServiceProvider<ICustomFileProvider> _namedServiceProvider;
    private readonly ICustomFileProvider _customFileProvider;

    public ProjectFileService(UserManager userManager,
        SqlSugarRepository<SysFile> sysFileRep,
        IOptions<OSSProviderOptions> oSSProviderOptions,
        IOptions<UploadOptions> uploadOptions, INamedServiceProvider<ICustomFileProvider> namedServiceProvider)
    {
        _namedServiceProvider = namedServiceProvider;
        _userManager = userManager;
        _sysFileRep = sysFileRep;
        _OSSProviderOptions = oSSProviderOptions.Value;
        _uploadOptions = uploadOptions.Value;
        if (_OSSProviderOptions.Enabled)
        {
            _customFileProvider = _namedServiceProvider.GetService<ITransient>(nameof(OSSFileProvider));
        }
        else if (App.Configuration["SSHProvider:Enabled"].ToBoolean())
        {
            _customFileProvider = _namedServiceProvider.GetService<ITransient>(nameof(SSHFileProvider));
        }
        else
        {
            _customFileProvider = _namedServiceProvider.GetService<ITransient>(nameof(DefaultFileProvider));
        }
    }


    /// <summary>
    /// 上传文件 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("上传文件")]
    public async Task<SysFile> UploadFile([FromForm] UploadFileInput input)
    {
        if (input.File == null) throw Oops.Oh(ErrorCodeEnum.D8000);

        // 判断是否重复上传的文件
        var sizeKb = input.File.Length / 1024; // 大小KB
        var fileMd5 = string.Empty;
        if (_uploadOptions.EnableMd5)
        {
            await using (var fileStream = input.File.OpenReadStream())
            {
                fileMd5 = OssUtils.ComputeContentMd5(fileStream, fileStream.Length);
            }
            // Mysql8 中如果使用了 utf8mb4_general_ci 之外的编码会出错，尽量避免在条件里使用.ToString()
            // 因为 Squsugar 并不是把变量转换为字符串来构造SQL语句，而是构造了CAST(123 AS CHAR)这样的语句，这样这个返回值是utf8mb4_general_ci，所以容易出错。
            var sysFile = await _sysFileRep.GetFirstAsync(u => u.FileMd5 == fileMd5 && u.SizeKb == sizeKb);
            if (sysFile != null) return sysFile;
        }

        // 验证文件类型
        if (!_uploadOptions.ContentType.Contains(input.File.ContentType)) throw Oops.Oh($"{ErrorCodeEnum.D8001}:{input.File.ContentType}");

        // 验证文件大小
        if (sizeKb > _uploadOptions.MaxSize) throw Oops.Oh($"{ErrorCodeEnum.D8002}，允许最大：{_uploadOptions.MaxSize}KB");

        // 获取文件后缀
        var suffix = Path.GetExtension(input.File.FileName).ToLower(); // 后缀
        if (string.IsNullOrWhiteSpace(suffix))
            suffix = string.Concat(".", input.File.ContentType.AsSpan(input.File.ContentType.LastIndexOf('/') + 1));
        
        if (string.IsNullOrWhiteSpace(suffix)) throw Oops.Oh(ErrorCodeEnum.D8003);

        // 防止客户端伪造文件类型
        if (!string.IsNullOrWhiteSpace(input.AllowSuffix) && !input.AllowSuffix.Contains(suffix)) throw Oops.Oh(ErrorCodeEnum.D8003);
        //if (!VerifyFileExtensionName.IsSameType(file.OpenReadStream(), suffix)) throw Oops.Oh(ErrorCodeEnum.D8001);

        // 文件存储位置
        var path = string.IsNullOrWhiteSpace(input.SavePath) ? _uploadOptions.Path : input.SavePath;
        path = path.ParseToDateTimeForRep();

        var newFile = input.Adapt<SysFile>();
        newFile.Id = YitIdHelper.NextId();
        newFile.BucketName = _OSSProviderOptions.Enabled ? _OSSProviderOptions.Bucket : "Local"; // 阿里云对bucket名称有要求，1.只能包括小写字母，数字，短横线（-）2.必须以小写字母或者数字开头  3.长度必须在3-63字节之间
        newFile.FileName = Path.GetFileNameWithoutExtension(input.File.FileName);
        newFile.Suffix = suffix;
        newFile.SizeKb = sizeKb;
        newFile.FilePath = path;
        newFile.FileMd5 = fileMd5;

        var finalName = newFile.Id + suffix; // 文件最终名称

        newFile = await _customFileProvider.UploadFileAsync(input.File, newFile, path, finalName);
        await _sysFileRep.AsInsertable(newFile).ExecuteCommandAsync();
        return newFile;
    }

}