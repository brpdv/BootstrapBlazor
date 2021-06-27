﻿// Copyright (c) Argo Zhang (argo@163.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://www.blazor.zone or https://argozhang.github.io/

using BootstrapBlazor.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;


namespace BootstrapBlazor.Shared.Pages.Table
{
    /// <summary>
    /// 表格动态添加列示例组件
    /// </summary>
    public partial class TablesDynamicColumn
    {
        /// <summary>
        /// 数据服务
        /// </summary>
        public MyDataService DataService { get; set; } = new MyDataService();

        private Table<DynamicUser> table;
        static int propIndex = 1;

        private List<string> properties = new List<string>();
        /// <summary>
        /// 添加列
        /// </summary>
        public void AddColumn()
        {
            //动态属性名称
            var newPropertyName = $"动态属性-{propIndex}";

            //把属性名保存起来，便于后面动态移除属性
            properties.Add(newPropertyName);
            if (propIndex % 2 == 0)
            {
                DynamicUser.dynamicObjectBuilder.AddProperty(newPropertyName,
                    typeof(string),
                    new Attribute[] {
                        //必填
                        new RequiredAttribute(),
                        new AutoGenerateColumnAttribute()
                        { Order = propIndex + 10,Text = $"动态属性{propIndex}-字符串类型" }});

                propIndex++;
            }
            else
            {
                //将动态属性注册全局动态属性注册中心
                DynamicUser.dynamicObjectBuilder.AddProperty(newPropertyName, typeof(int),
                    new Attribute[] { new AutoGenerateColumnAttribute() {
                        Order = propIndex + 10,
                        Text = $"动态属性{propIndex}-int类型" }});
                propIndex++;
            }
            //手动通知Table，更新列信息
            table.ReGenerateColumn();

            //重新渲染组件
            StateHasChanged();
        }
        /// <summary>
        /// 删除最后一列
        /// </summary>
        public void DeleteLastColumn()
        {
            if (properties.Count > 1)
            {
                var propName = properties.Last();

                DynamicUser.dynamicObjectBuilder.RemoveProperty(propName);
                //将动态属性注册全局动态属性注册中心
            }

            //手动通知Table，更新列信息
            table.ReGenerateColumn();

            //重新渲染组件
            StateHasChanged();
        }
    }

    /// <summary>
    /// 示例Model，动态属性的User
    /// </summary>
    public class DynamicUser : IDynamicType
    {
        public static string userTypeKey { get; }
        public static Type type { get; }
        public static DynamicObjectBuilder dynamicObjectBuilder { get; }



        static DynamicUser()
        {
            type = typeof(DynamicUser);
            userTypeKey = type.FullName!;

            //定义动态对象拥有的属性
            dynamicObjectBuilder =
                new DynamicObjectBuilder(type, userTypeKey)
                .AddClassAttribute(new AutoGenerateClassAttribute
                {
                    Filterable = true,
                    Searchable = true
                })
                .AddProperty("Name", typeof(string), new Attribute[] {
                    new AutoGenerateColumnAttribute() {
                        Order = 1,
                        Text = "名称" },
                    new RequiredAttribute(),
                    new StringLengthAttribute(5)
                })
                .AddProperty("Age", typeof(int), new Attribute[] {
                    new AutoGenerateColumnAttribute() {
                        Order = 2,
                        Searchable = false,
                        Text = "年龄" }
                });
        }
        private ConcurrentDictionary<string, object?> propDic = new();

        /// <summary>
        /// 动态类型实体
        /// </summary>
        /// <param name="propDic"></param>
        public DynamicUser(ConcurrentDictionary<string, object?> propDic)
        {
            this.propDic = propDic;
        }

        static int i = 1;

        /// <summary>
        /// 生成一个默认动态用户，用户点击添加按钮时，使用此构造函数，创建默认对象
        /// </summary>
        public DynamicUser()
        {
            dynamicObjectBuilder.SetDefaultValues(this);
        }

        /// <summary>
        /// Id属性
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 根据属性名，获取属性值
        /// </summary>
        /// <param name="propName"></param>
        /// <returns></returns>
        public object? GetValue(string propName)
        {
            //如果属性不存在，则添加属性默认值
            //当在运行时动态添加属性时，就会出现属性不存在的情况
            if (!propDic.ContainsKey(propName))
            {
                propDic[propName] = dynamicObjectBuilder.GetPropertyDefaultValue(propName);
            }
            return propDic[propName];
        }
        /// <summary>
        /// 当前对象是否是创建时的临时对象
        /// </summary>
        public bool IsTemp { get; set; } = false;
        /// <summary>
        /// 设置名称属性
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public DynamicUser SetName(string n)
        {
            propDic["Name"] = n;
            return this;
        }
        /// <summary>
        /// 设置指定属性为指定值
        /// </summary>
        /// <param name="propName"></param>
        /// <param name="value"></param>
        public void SetValue(string propName, object value)
        {
            //这里需要类型转换
            propDic[propName] = value;
        }

        /// <summary>
        /// 编辑时 调用Clone
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            ConcurrentDictionary<string, object?> newPropDic = new();
            foreach (var item in propDic)
            {
                newPropDic[item.Key] = item.Value;
            }
            return new DynamicUser(newPropDic) { Id = this.Id };
        }

        /// <summary>
        /// 获取类型Key
        /// </summary>
        /// <returns></returns>
        public string GetTypeKey()
        {
            return userTypeKey;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class MyDataService : IDataService<DynamicUser>
    {
        static MyDataService()
        {
            for (int i = 0; i < 10; i++)
            {
                AllUser.Add(new DynamicUser().SetName(DateTime.Now.ToString()));
            }
        }

        public static List<DynamicUser> AllUser = new List<DynamicUser>();
        /// <summary>
        /// 点击新建按钮时触发
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Task<bool> AddAsync(DynamicUser model)
        {
            //设置对象为临时对象，在保存时，会使用此标记判断对象是新增还是更新
            model.IsTemp = true;
            return Task.FromResult(true);
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        public Task<bool> DeleteAsync(IEnumerable<DynamicUser> models)
        {
            foreach (var item in models)
            {
                AllUser.Remove(item);
            }
            return Task.FromResult(true);
        }
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        public Task<QueryData<DynamicUser>> QueryAsync(QueryPageOptions option)
        {
            return Task.FromResult(new QueryData<DynamicUser> { Items = AllUser });
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Task<bool> SaveAsync(DynamicUser model)
        {
            //临时对象 保存 就是新增操作
            if (model.IsTemp)
            {
                //新增
                model.IsTemp = false;
                AllUser.Add(model);
            }
            else
            {
                //编辑 直接替换对象
                var index = AllUser.FindIndex(p => p.Id == model.Id);
                AllUser[index] = model;
            }
            return Task.FromResult(true);
        }
    }
}
