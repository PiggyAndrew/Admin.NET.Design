namespace Admin.NET.Core.Service;

/// <summary>
/// 文件分页查询
/// </summary>
public class ProjectPageFileInput : PageFileInput
{
}

public class ProjectFileInput : FileInput
{
}

public class ProjectDeleteFileInput : DeleteFileInput
{
}

/// <summary>
/// 上传文件
/// </summary>
public class ProjectUploadFileInput: UploadFileInput
{
  
}

/// <summary>
/// 上传文件Base64
/// </summary>
public class ProjectUploadFileFromBase64Input:UploadFileFromBase64Input
{
   
}

/// <summary>
/// 查询关联查询输入
/// </summary>
public class ProjectRelationQueryInput: RelationQueryInput
{
    
}