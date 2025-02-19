﻿// Copyright (c) Argo Zhang (argo@163.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://www.blazor.zone or https://argozhang.github.io/

using Microsoft.Extensions.Localization;
using System.Threading.Tasks;

namespace BootstrapBlazor.Components
{
    /// <summary>
    /// Popover 服务类
    /// </summary>
    public class PopoverService : BootstrapServiceBase<PopoverConfirmOption>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="localizer"></param>
        public PopoverService(IStringLocalizer<PopoverService> localizer) : base(localizer)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        public Task Show(PopoverConfirmOption option) => Invoke(option);
    }
}
