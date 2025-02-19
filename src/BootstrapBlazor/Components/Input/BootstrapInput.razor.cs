﻿// Copyright (c) Argo Zhang (argo@163.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://www.blazor.zone or https://argozhang.github.io/

using Microsoft.AspNetCore.Components;

namespace BootstrapBlazor.Components
{
    /// <summary>
    /// BootstrapInputTextBase 组件
    /// </summary>
    public partial class BootstrapInput<TValue>
    {
        /// <summary>
        /// 获得/设置 是否为 Input-Group 组合中的 input 组件 默认 false
        /// </summary>
        [Parameter]
        public bool IsGroup { get; set; }
    }
}
