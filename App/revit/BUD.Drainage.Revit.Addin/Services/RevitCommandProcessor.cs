//using Autodesk.Revit.DB;
//using Autodesk.Revit.UI;
//using BUD.Drainage.Revit.Addin.Models;
//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using System.Linq;

//namespace BUD.Drainage.Revit.Addin.Services
//{
//    /// <summary>
//    /// Revit命令处理器
//    /// </summary>
//    public class RevitCommandProcessor
//    {
//        private readonly UIApplication _uiApp;
//        private readonly Document _doc;
//        private readonly UIDocument _uiDoc;

//        public RevitCommandProcessor(UIApplication uiApp)
//        {
//            _uiApp = uiApp;
//            _uiDoc = uiApp.ActiveUIDocument;
//            _doc = _uiDoc?.Document;
//        }

//        /// <summary>
//        /// 处理从Web接收的命令
//        /// </summary>
//        /// <param name="jsonMessage">JSON格式的命令消息</param>
//        /// <returns>处理结果的JSON字符串</returns>
//        public string ProcessCommand(string jsonMessage)
//        {
//            try
//            {
//                // 解析JSON消息
//                var commandModel = JsonConvert.DeserializeObject<RevitCommandModel>(jsonMessage);
//                return CreateErrorResult($"不支持的操作类型");
//            }
//            catch (Exception ex)
//            {
//                return CreateErrorResult($"处理命令时出错: {ex.Message}");
//            }
//        }

//        #region 命令执行方法

//        /// <summary>
//        /// 隐藏元素
//        /// </summary>
//        private string HideElements(RevitActionCommandModel command)
//        {
//            try
//            {
//                if (_doc == null)
//                {
//                    return CreateErrorResult("没有打开的文档");
//                }

//                using (Transaction trans = new Transaction(_doc, "隐藏元素"))
//                {
//                    trans.Start();

//                    if (command.TargetType.ToLower() == "category")
//                    {
//                        // 处理类别隐藏
//                        var view = _doc.ActiveView;
//                        bool success = false;

//                        foreach (var targetName in command.Targets)
//                        {
//                            var categories = GetCategoriesByName(targetName);
//                            foreach (var categoryId in categories)
//                            {
//                                view.SetCategoryHidden(categoryId, true);
//                                success = true;
//                            }
//                        }

//                        trans.Commit();
//                        return success 
//                            ? CreateSuccessResult($"已隐藏类别: {string.Join(", ", command.Targets)}")
//                            : CreateErrorResult("未找到指定的类别");
//                    }
//                    else if (command.TargetType.ToLower() == "element")
//                    {
//                        // 处理元素隐藏
//                        var view = _doc.ActiveView;
//                        ICollection<ElementId> elementIds;

//                        if (command.Scope.ToLower() == "selected")
//                        {
//                            // 获取选中的元素
//                            elementIds = _uiDoc.Selection.GetElementIds();
                            
//                            // 过滤选中的元素，只保留目标类型
//                            if (command.Targets.Any())
//                            {
//                                elementIds = FilterElementsByCategory(elementIds, command.Targets);
//                            }
//                        }
//                        else
//                        {
//                            // 获取所有指定类型的元素
//                            elementIds = GetElementIdsByCategories(command.Targets);
//                        }

//                        if (elementIds.Any())
//                        {
//                            view.HideElements(elementIds);
//                            trans.Commit();
//                            return CreateSuccessResult($"已隐藏 {elementIds.Count} 个元素");
//                        }
//                        else
//                        {
//                            trans.RollBack();
//                            return CreateErrorResult("未找到符合条件的元素");
//                        }
//                    }
//                    else
//                    {
//                        trans.RollBack();
//                        return CreateErrorResult($"不支持的目标类型: {command.TargetType}");
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                return CreateErrorResult($"隐藏元素时出错: {ex.Message}");
//            }
//        }

//        /// <summary>
//        /// 显示元素
//        /// </summary>
//        private string ShowElements(RevitActionCommandModel command)
//        {
//            try
//            {
//                if (_doc == null)
//                {
//                    return CreateErrorResult("没有打开的文档");
//                }

//                using (Transaction trans = new Transaction(_doc, "显示元素"))
//                {
//                    trans.Start();

//                    if (command.TargetType.ToLower() == "category")
//                    {
//                        // 处理类别显示
//                        var view = _doc.ActiveView;
//                        bool success = false;

//                        foreach (var targetName in command.Targets)
//                        {
//                            var categories = GetCategoriesByName(targetName);
//                            foreach (var categoryId in categories)
//                            {
//                                view.SetCategoryHidden(categoryId, false);
//                                success = true;
//                            }
//                        }

//                        trans.Commit();
//                        return success 
//                            ? CreateSuccessResult($"已显示类别: {string.Join(", ", command.Targets)}")
//                            : CreateErrorResult("未找到指定的类别");
//                    }
//                    else if (command.TargetType.ToLower() == "element")
//                    {
//                        // 处理元素显示
//                        var view = _doc.ActiveView;
                        
//                        // 获取当前视图中隐藏的元素
//                        //var hiddenElementIds = view.GetHiddenElementIds();
                        
//                        // 如果指定了目标类别，则过滤隐藏的元素
//                        if (command.Targets.Any())
//                        {
//                            //hiddenElementIds = FilterElementsByCategory(hiddenElementIds, command.Targets);
//                        }

//                        //if (hiddenElementIds.Any())
//                        //{
//                        //    view.UnhideElements(hiddenElementIds);
//                        //    trans.Commit();
//                           return CreateSuccessResult($"已显示 N 个元素");
//                        //}
//                        //else
//                        //{
//                        //    trans.RollBack();
//                        //    return CreateErrorResult("未找到符合条件的隐藏元素");
//                        //}
//                    }
//                    else
//                    {
//                        trans.RollBack();
//                        return CreateErrorResult($"不支持的目标类型: {command.TargetType}");
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                return CreateErrorResult($"显示元素时出错: {ex.Message}");
//            }
//        }

//        /// <summary>
//        /// 隔离元素
//        /// </summary>
//        private string IsolateElements(RevitActionCommandModel command)
//        {
//            try
//            {
//                if (_doc == null)
//                {
//                    return CreateErrorResult("没有打开的文档");
//                }

//                using (Transaction trans = new Transaction(_doc, "隔离元素"))
//                {
//                    trans.Start();

//                    if (command.TargetType.ToLower() == "category")
//                    {
//                        // 处理类别隔离
//                        var view = _doc.ActiveView;
//                        var categoriesToIsolate = new List<ElementId>();

//                        foreach (var targetName in command.Targets)
//                        {
//                            var categories = GetCategoriesByName(targetName);
//                            categoriesToIsolate.AddRange(categories);
//                        }

//                        if (categoriesToIsolate.Any())
//                        {
//                            view.IsolateCategoriesTemporary(new List<ElementId>(categoriesToIsolate));
//                            trans.Commit();
//                            return CreateSuccessResult($"已隔离类别: {string.Join(", ", command.Targets)}");
//                        }
//                        else
//                        {
//                            trans.RollBack();
//                            return CreateErrorResult("未找到指定的类别");
//                        }
//                    }
//                    else if (command.TargetType.ToLower() == "element")
//                    {
//                        // 处理元素隔离
//                        ICollection<ElementId> elementIds;

//                        if (command.Scope.ToLower() == "selected")
//                        {
//                            // 获取选中的元素
//                            elementIds = _uiDoc.Selection.GetElementIds();
                            
//                            // 过滤选中的元素，只保留目标类型
//                            if (command.Targets.Any())
//                            {
//                                elementIds = FilterElementsByCategory(elementIds, command.Targets);
//                            }
//                        }
//                        else
//                        {
//                            // 获取所有指定类型的元素
//                            elementIds = GetElementIdsByCategories(command.Targets);
//                        }

//                        if (elementIds.Any())
//                        {
//                            _doc.ActiveView.IsolateElementsTemporary(elementIds);
//                            trans.Commit();
//                            return CreateSuccessResult($"已隔离 {elementIds.Count} 个元素");
//                        }
//                        else
//                        {
//                            trans.RollBack();
//                            return CreateErrorResult("未找到符合条件的元素");
//                        }
//                    }
//                    else
//                    {
//                        trans.RollBack();
//                        return CreateErrorResult($"不支持的目标类型: {command.TargetType}");
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                return CreateErrorResult($"隔离元素时出错: {ex.Message}");
//            }
//        }

//        /// <summary>
//        /// 选择元素
//        /// </summary>
//        private string SelectElements(RevitActionCommandModel command)
//        {
//            try
//            {
//                if (_doc == null || _uiDoc == null)
//                {
//                    return CreateErrorResult("没有打开的文档");
//                }

//                ICollection<ElementId> elementIds;

//                if (command.TargetType.ToLower() == "category")
//                {
//                    // 获取所有指定类别的元素
//                    elementIds = GetElementIdsByCategories(command.Targets);
//                }
//                else if (command.TargetType.ToLower() == "element")
//                {
//                    // 如果是选择当前选中的元素中的特定类型
//                    if (command.Scope.ToLower() == "selected")
//                    {
//                        var selectedIds = _uiDoc.Selection.GetElementIds();
//                        elementIds = FilterElementsByCategory(selectedIds, command.Targets);
//                    }
//                    else
//                    {
//                        // 获取所有指定类型的元素
//                        elementIds = GetElementIdsByCategories(command.Targets);
//                    }
//                }
//                            else
//                {
//                    return CreateErrorResult($"不支持的目标类型: {command.TargetType}");
//                }

//                if (elementIds.Any())
//                {
//                    _uiDoc.Selection.SetElementIds(elementIds);
//                    return CreateSuccessResult($"已选择 {elementIds.Count} 个元素");
//                }
//                else
//                {
//                    return CreateErrorResult("未找到符合条件的元素");
//                }
//            }
//            catch (Exception ex)
//            {
//                return CreateErrorResult($"选择元素时出错: {ex.Message}");
//            }
//        }

//        #endregion

//        #region 辅助方法

//        /// <summary>
//        /// 根据类别名称获取类别ID
//        /// </summary>
//        private List<ElementId> GetCategoriesByName(string categoryName)
//        {
//            var result = new List<ElementId>();
            
//            // 获取所有可见的类别
//            var categories = _doc.Settings.Categories;
            
//            // 尝试精确匹配
//            foreach (Category category in categories)
//            {
//                if (category.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase))
//                {
//                    result.Add(category.Id);
//                    return result;
//                }
//            }
            
//            // 如果没有精确匹配，尝试模糊匹配
//            foreach (Category category in categories)
//            {
//                if (category.Name.Contains(categoryName, StringComparison.OrdinalIgnoreCase))
//                {
//                    result.Add(category.Id);
//                }
//            }
            
//            return result;
//        }

//        /// <summary>
//        /// 根据类别名称获取元素ID
//        /// </summary>
//        private ICollection<ElementId> GetElementIdsByCategories(List<string> categoryNames)
//        {
//            var result = new List<ElementId>();
            
//            if (categoryNames == null || !categoryNames.Any())
//            {
//                return result;
//            }
            
//            // 获取所有类别ID
//            var categoryIds = new List<ElementId>();
//            foreach (var categoryName in categoryNames)
//            {
//                categoryIds.AddRange(GetCategoriesByName(categoryName));
//            }
            
//            if (!categoryIds.Any())
//            {
//                return result;
//            }
            
//            // 获取所有元素
//            foreach (var categoryId in categoryIds)
//            {
//                var collector = new FilteredElementCollector(_doc, _doc.ActiveView.Id)
//                    .WhereElementIsNotElementType()
//                    .OfCategoryId(categoryId);
                
//                result.AddRange(collector.ToElementIds());
//            }
            
//            return result;
//        }

//        /// <summary>
//        /// 根据类别过滤元素
//        /// </summary>
//        private ICollection<ElementId> FilterElementsByCategory(ICollection<ElementId> elementIds, List<string> categoryNames)
//        {
//            if (elementIds == null || !elementIds.Any() || categoryNames == null || !categoryNames.Any())
//            {
//                return new List<ElementId>();
//            }
            
//            // 获取所有类别ID
//            var categoryIds = new List<ElementId>();
//            foreach (var categoryName in categoryNames)
//            {
//                categoryIds.AddRange(GetCategoriesByName(categoryName));
//            }
            
//            if (!categoryIds.Any())
//            {
//                return new List<ElementId>();
//            }
            
//            // 过滤元素
//            var result = new List<ElementId>();
//            foreach (var elementId in elementIds)
//            {
//                var element = _doc.GetElement(elementId);
//                if (element != null && categoryIds.Contains(element.Category?.Id))
//                {
//                    result.Add(elementId);
//                }
//            }
            
//            return result;
//        }

//        /// <summary>
//        /// 创建成功结果
//        /// </summary>
//        private string CreateSuccessResult(string message, object data = null)
//        {
//            var result = new RevitSendMessage
//            {
//                Success = true,
//                Message = message,
//                Data = data
//            };
            
//            return JsonConvert.SerializeObject(result);
//        }

//        /// <summary>
//        /// 创建错误结果
//        /// </summary>
//        private string CreateErrorResult(string message)
//        {
//            var result = new RevitSendMessage
//            {
//                Success = false,
//                Message = message
//            };
            
//            return JsonConvert.SerializeObject(result);
//        }

//        #endregion
//    }

//    /// <summary>
//    /// 字符串扩展方法
//    /// </summary>
//    public static class StringExtensions
//    {
//        /// <summary>
//        /// 判断字符串是否包含指定字符串（忽略大小写）
//        /// </summary>
//        public static bool Contains(this string source, string value, StringComparison comparisonType)
//        {
//            return source?.IndexOf(value, comparisonType) >= 0;
//        }
//    }
//}