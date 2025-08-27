using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using BUD.Revit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using static BUD.Tuna.Revit.Extensions.Constants.BuiltInParameters;
using Element = Autodesk.Revit.DB.Element;
using View = Autodesk.Revit.DB.View;
using Level = Autodesk.Revit.DB.Level;
using BUD.Drainage.Revit.Addin.Models.Arguments.Create;

namespace BUD.Revit.Framework.Services
{
    /// <summary>
    /// 提供创建Revit实例的服务
    /// </summary>
    public class RevitInstanceCreationService
    {
        private readonly CommandContext _commandContext;

        /// <summary>
        /// 初始化RevitInstanceCreationService的新实例
        /// </summary>
        /// <param name="commandContext">命令上下文</param>
        public RevitInstanceCreationService(CommandContext commandContext)
        {
            _commandContext = commandContext ?? throw new ArgumentNullException(nameof(commandContext));
        }

        #region 公共方法

        /// <summary>
        /// 创建族实例
        /// </summary>
        /// <param name="familySymbol">族类型</param>
        /// <param name="placementData">放置数据</param>
        /// <returns>创建的族实例</returns>
        public List<FamilyInstance> CreateFamilyInstance(FamilySymbol familySymbol, PlacementData placementData)
        {
            if (familySymbol == null)
                throw new ArgumentNullException(nameof(familySymbol));

            if (placementData == null)
                throw new ArgumentNullException(nameof(placementData));

            if (_commandContext.Document == null)
                throw new InvalidOperationException("当前没有活动文档");

            // 确保族类型已激活
            if (!familySymbol.IsActive)
                familySymbol.Activate();

            // 根据放置方法选择合适的创建方式
            switch (placementData.Method)
            {
                case PlacementMethod.Point:
                    return CreatePointBasedInstance(familySymbol, placementData);
                //case PlacementMethod.Line:
                //    return CreateLineBasedInstance(familySymbol, placementData);
                //case PlacementMethod.Face:
                //    return CreateFaceBasedInstance(familySymbol, placementData);
                default:
                    throw new ArgumentException($"不支持的放置方法: {placementData.Method}");
            }
        }

        /// <summary>
        /// 创建墙体
        /// </summary>
        /// <param name="wallType">墙类型</param>
        /// <param name="curve">墙体基线</param>
        /// <param name="level">标高</param>
        /// <param name="height">墙高</param>
        /// <param name="offset">基线偏移</param>
        /// <param name="flip">是否翻转</param>
        /// <param name="structural">是否结构墙</param>
        /// <returns>创建的墙体</returns>
        public Wall CreateWall(WallType wallType, Curve curve, Autodesk.Revit.DB.Level level, double height,
            double offset = 0, bool flip = false, bool structural = false)
        {
            if (wallType == null)
                throw new ArgumentNullException(nameof(wallType));

            if (curve == null)
                throw new ArgumentNullException(nameof(curve));

            if (level == null)
                throw new ArgumentNullException(nameof(level));

            if (_commandContext.Document == null)
                throw new InvalidOperationException("当前没有活动文档");

            return Wall.Create(
                _commandContext.Document,
                curve,
                wallType.Id,
                level.Id,
                height,
                offset,
                flip,
                structural
            );
        }




        #endregion

        #region 私有方法

        /// <summary>
        /// 创建基于点的族实例
        /// </summary>
        private List<FamilyInstance> CreatePointBasedInstance(FamilySymbol symbol, PlacementData data)
        {
            Document doc = _commandContext.Document;

            if (data.Points == null || data.Points.Count == 0)
            {
                throw new ArgumentException("放置数据中缺少位置点");
            }

            List<FamilyInstance> instances = new List<FamilyInstance>();
            foreach (XYZ location in data.Points)
            {
                FamilyInstance instance = null;
               // 规避ID为空的情况
                Autodesk.Revit.DB.Element host = null;
                Autodesk.Revit.DB.Level level = null;
                Autodesk.Revit.DB.View view = null;
                
                // 只有在ID有效时才尝试获取元素
                if (data.HostId != null && data.HostId.IntegerValue > 0)
                {
                    host = _commandContext.Document.GetElement(data.HostId) as Element;
                }
                
                if (data.LevelId != null && data.LevelId.IntegerValue > 0)
                {
                    level = _commandContext.Document.GetElement(data.LevelId) as Level;
                }
                
                if (data.ViewId != null && data.ViewId.IntegerValue > 0)
                {
                    view = _commandContext.Document.GetElement(data.ViewId) as View;
                }
                // 根据不同的放置条件创建族实例
                if (host != null && level != null)
                {
                    // 使用宿主和标高
                    instance = doc.Create.NewFamilyInstance(
                        location,
                        symbol,
                        host,
                        level,
                        data.StructuralType
                    );
                }
                else if (host != null)
                {
                    // 仅使用宿主
                    instance = doc.Create.NewFamilyInstance(
                        location,
                        symbol,
                        host,
                        data.StructuralType
                    );
                }
                else if (level != null)
                {
                    // 基于标高的点放置
                    instance = doc.Create.NewFamilyInstance(
                        location,
                        symbol,
                        level,
                        data.StructuralType
                    );
                }
                else if (view != null)
                {
                    // 基于视图的点放置（详图元素）
                    instance = doc.Create.NewFamilyInstance(
                        location,
                        symbol,
                        view
                    );
                }
                else
                {
                    // 简单的点放置
                    instance = doc.Create.NewFamilyInstance(
                        location,
                        symbol,
                        data.StructuralType
                    );
                }

                if (instance != null)
                {
                    instances.Add(instance);
                }
            }
            return instances;
        }



        #endregion
    }


}