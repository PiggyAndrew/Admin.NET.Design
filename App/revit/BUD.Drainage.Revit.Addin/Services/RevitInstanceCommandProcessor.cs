//using Autodesk.Revit.DB;
//using Autodesk.Revit.DB.Structure;
//using BUD.Drainage.Revit.Addin.Models;
//using BUD.Revit.Framework;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
//using System;
//using System.Collections.Generic;
//using System.Linq;

//namespace BUD.Revit.Framework.Services
//{
//    /// <summary>
//    /// 处理创建Revit实例的命令
//    /// </summary>
//    public class RevitInstanceCommandProcessor
//    {
//        private readonly CommandContext _commandContext;
//        private readonly RevitInstanceCreationService _creationService;

//        /// <summary>
//        /// 初始化RevitInstanceCommandProcessor的新实例
//        /// </summary>
//        /// <param name="commandContext">命令上下文</param>
//        public RevitInstanceCommandProcessor(CommandContext commandContext)
//        {
//            _commandContext = commandContext ?? throw new ArgumentNullException(nameof(commandContext));
//            _creationService = new RevitInstanceCreationService(commandContext);
//        }

//        /// <summary>
//        /// 处理创建实例的命令
//        /// </summary>
//        /// <param name="jsonCommand">JSON格式的命令</param>
//        /// <returns>处理结果</returns>
//        public string ProcessCreateInstanceCommand(string jsonCommand)
//        {
//            try
//            {
//                if (_commandContext.Document == null)
//                {
//                    return CreateErrorResult("没有打开的文档");
//                }

//                // 解析JSON命令为RevitCreateCommandModel
//                RevitCommandModel message = JsonConvert.DeserializeObject<RevitCommandModel>(jsonCommand);
//                var commandModel = message.RevitCommand as RevitCreateCommandModel;
//                commandModel.FillContextInfo(_commandContext.Document);
//                if (commandModel == null)
//                {
//                    return CreateErrorResult("无效的命令格式");
//                }

//                // 获取命令参数
//                string elementType = commandModel.ElementType;

//                if (string.IsNullOrEmpty(elementType))
//                {
//                    return CreateErrorResult("缺少元素类型");
//                }

//                // 使用事务创建元素
//                using (Transaction trans = new Transaction(_commandContext.Document, $"创建{elementType}"))
//                {
//                    trans.Start();

//                    try
//                    {
//                        // 根据元素类型选择创建方法
//                        List<Element> createdElement = new List<Element>() { };

//                        switch (elementType.ToLower())
//                        {
//                            case "wall":
//                            case "墙":
//                                createdElement.Add(CreateWall(commandModel));
//                                break;
//                            case "door":
//                            case "门":
//                            case "window":
//                            case "窗":
//                            case "column":
//                            case "柱":
//                            case "furniture":
//                            case "家具":
//                            case "beam":
//                            case "梁":
//                            case "generic":
//                            case "通用":
//                                createdElement = CreateFamilyInstance(commandModel).Select(x => x as Element).ToList();
//                                break;
//                            default:
//                                return CreateErrorResult($"不支持的元素类型: {elementType}");
//                        }

//                        if (createdElement != null && createdElement.Count > 0)
//                        {
//                            // 应用参数
//                            foreach (Element element in createdElement)
//                            {
//                                //ApplyParameters(element, commandModel.Parameters);
//                            }

//                            trans.Commit();

//                            // 返回创建的元素ID列表
//                            string elementIds = string.Join(",", createdElement.Select(e => e.Id.IntegerValue.ToString()));
//                            return CreateSuccessResult(elementIds, "创建成功");
//                        }
//                        else
//                        {
//                            trans.RollBack();
//                            return CreateErrorResult("创建元素失败");
//                        }
//                    }
//                    catch (Exception ex)
//                    {
//                        trans.RollBack();
//                        return CreateErrorResult($"创建元素时发生错误: {ex.Message}");
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                return CreateErrorResult($"处理命令时发生错误: {ex.Message}");
//            }
//        }

//        /// <summary>
//        /// 创建墙体
//        /// </summary>
//        private Autodesk.Revit.DB.Wall CreateWall(RevitCreateCommandModel createModel)
//        {
//            // 创建墙体曲线
//            Autodesk.Revit.DB.Line wallLine = Autodesk.Revit.DB.Line.CreateBound(createModel.Placement.Points[0], createModel.Placement.Points[1]);

//            // 获取墙高
//            double height = 3.0; // 默认高度
//            var wallType = createModel.GetElementTypeFromDocument(_commandContext.Document) as WallType;

//            var level = _commandContext.Document.GetElement(createModel.Placement.LevelId) as Level;

//            // 创建墙体
//            return _creationService.CreateWall(wallType, wallLine, level, height);
//        }

//        /// <summary>
//        /// 创建族实例
//        /// </summary>
//        private List<FamilyInstance> CreateFamilyInstance(RevitCreateCommandModel commandModel)
//        {
//            // 获取族和类型名称
//            string elementType = commandModel.ElementType;
//            string familyName = commandModel.FamilyName;
//            string typeName = commandModel.TypeName;

//            // 查找族类型
//            ElementType symbol = commandModel.GetElementTypeFromDocument(_commandContext.Document);



//            // 获取放置信息
//            if (commandModel.Placement == null)
//            {
//                throw new ArgumentException("缺少放置信息");
//            }


//            // 创建族实例
//            return _creationService.CreateFamilyInstance(symbol as FamilySymbol, commandModel.Placement);
//        }


//        /// <summary>
//        /// 应用参数
//        /// </summary>
//        private void ApplyParameters(Autodesk.Revit.DB.Element element, JObject commandObj)
//        {
//            JObject parametersObj = commandObj["parameters"] as JObject;
//            if (parametersObj == null)
//            {
//                return;
//            }

//            foreach (var param in parametersObj.Properties())
//            {
//                string paramName = param.Name;
//                JToken paramValue = param.Value;

//                // 查找参数
//                Autodesk.Revit.DB.Parameter parameter = element.LookupParameter(paramName);
//                if (parameter != null && !parameter.IsReadOnly)
//                {
//                    // 根据参数类型设置值
//                    switch (parameter.StorageType)
//                    {
//                        case Autodesk.Revit.DB.StorageType.Double:
//                            parameter.Set((double)paramValue);
//                            break;
//                        case Autodesk.Revit.DB.StorageType.Integer:
//                            parameter.Set((int)paramValue);
//                            break;
//                        case Autodesk.Revit.DB.StorageType.String:
//                            parameter.Set(paramValue.ToString());
//                            break;
//                        case Autodesk.Revit.DB.StorageType.ElementId:
//                            if (int.TryParse(paramValue.ToString(), out int id))
//                            {
//                                parameter.Set(new Autodesk.Revit.DB.ElementId(id));
//                            }
//                            break;
//                    }
//                }
//            }
//        }

//        /// <summary>
//        /// 创建成功结果
//        /// </summary>
//        private string CreateSuccessResult(string elementId, string message)
//        {
//            var result = new
//            {
//                success = true,
//                element_id = elementId,
//                message = message
//            };

//            return JsonConvert.SerializeObject(result);
//        }

//        /// <summary>
//        /// 创建错误结果
//        /// </summary>
//        private string CreateErrorResult(string errorMessage)
//        {
//            var result = new
//            {
//                success = false,
//                message = errorMessage
//            };

//            return JsonConvert.SerializeObject(result);
//        }
//    }
//}