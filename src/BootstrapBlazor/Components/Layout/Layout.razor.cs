﻿// Copyright (c) Argo Zhang (argo@163.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://www.blazor.zone or https://argozhang.github.io/

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace BootstrapBlazor.Components
{
    /// <summary>
    /// 
    /// </summary>
    public partial class Layout : IAsyncDisposable
    {
        private JSInterop<Layout>? Interop { get; set; }

        private bool IsAuthenticated { get; set; }

        /// <summary>
        /// 获得 组件样式
        /// </summary>
        private string? ClassString => CssBuilder.Default("layout")
            .AddClass("has-sidebar", Side != null && IsFullSide)
            .AddClass("is-page", IsPage)
            .AddClass("has-footer", ShowFooter)
            .AddClassFromAttributes(AdditionalAttributes)
            .Build();

        /// <summary>
        /// 获得 页脚样式
        /// </summary>
        private string? FooterClassString => CssBuilder.Default("layout-footer")
            .AddClass("is-fixed", IsFixedFooter)
            .AddClass("is-collapsed", IsCollapsed)
            .Build();

        /// <summary>
        /// 获得 页头样式
        /// </summary>
        private string? HeaderClassString => CssBuilder.Default("layout-header")
            .AddClass("is-fixed", IsFixedHeader)
            .Build();

        /// <summary>
        /// 获得 侧边栏样式
        /// </summary>
        private string? SideClassString => CssBuilder.Default("layout-side")
            .AddClass("is-collapsed", IsCollapsed)
            .AddClass("is-fixed-header", IsFixedHeader)
            .AddClass("is-fixed-footer", IsFixedFooter)
            .Build();

        /// <summary>
        /// 获得 侧边栏 Style 字符串
        /// </summary>
        private string? SideStyleString => CssBuilder.Default()
            .AddClass($"width: {SideWidth.ConvertToPercentString()}", !string.IsNullOrEmpty(SideWidth) && SideWidth != "0")
            .Build();

        /// <summary>
        /// 获得 Main 样式
        /// </summary>
        private string? MainClassString => CssBuilder.Default("layout-main")
            .AddClass("is-collapsed", IsCollapsed)
            .Build();

        /// <summary>
        /// 获得 展开收缩 Bar 样式
        /// </summary>
        private string? CollapseBarClassString => CssBuilder.Default("layout-header-bar")
            .AddClass("is-collapsed", IsCollapsed)
            .Build();

        /// <summary>
        /// 获得/设置 排除地址支持通配符
        /// </summary>
        [Parameter]
        public IEnumerable<string>? ExcludeUrls { get; set; }

        /// <summary>
        /// 获得/设置 Gets or sets a collection of additional assemblies that should be searched for components that can match URIs.
        /// </summary>
        [Parameter]
        [NotNull]
        public IEnumerable<Assembly>? AdditionalAssemblies { get; set; }

        /// <summary>
        /// 获得/设置 鼠标悬停提示文字信息
        /// </summary>
        [Parameter]
        [NotNull]
        public string? TooltipText { get; set; }

        [Inject]
        [NotNull]
        private IStringLocalizer<Layout>? Localizer { get; set; }

        /// <summary>
        /// 获得 登录授权信息
        /// </summary>
        [CascadingParameter]
        private Task<AuthenticationState>? AuthenticationStateTask { get; set; }

        [Inject]
        private IAuthorizationPolicyProvider? AuthorizationPolicyProvider { get; set; }

        [Inject]
        private IAuthorizationService? AuthorizationService { get; set; }

        [Inject]
        [NotNull]
        private NavigationManager? Navigator { get; set; }

        private bool IsInit { get; set; }

        /// <summary>
        /// OnInitialized 方法
        /// </summary>
        protected override void OnInitialized()
        {
            base.OnInitialized();

            TooltipText ??= Localizer[nameof(TooltipText)];

            if (!OperatingSystem.IsBrowser() && AdditionalAssemblies == null)
            {
                AdditionalAssemblies = new[] { Assembly.GetEntryAssembly()! };
            }

            AdditionalAssemblies ??= Enumerable.Empty<Assembly>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            // 需要认证并且未认证
            if (AuthenticationStateTask != null)
            {
                var url = Navigator.ToBaseRelativePath(Navigator.Uri);
                var context = RouteTableFactory.Create(AdditionalAssemblies, url);
                if (context.Handler != null)
                {
                    IsAuthenticated = await BootstrapBlazorAuthorizeView.IsAuthorizedAsync(context.Handler, AuthenticationStateTask, AuthorizationPolicyProvider, AuthorizationService);
                }
            }
            else
            {
                IsAuthenticated = true;
            }

            IsInit = true;
        }

        /// <summary>
        /// OnAfterRenderAsync 方法
        /// </summary>
        /// <param name="firstRender"></param>
        /// <returns></returns>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                Interop = new JSInterop<Layout>(JSRuntime);
                await Interop.InvokeVoidAsync(this, null, "bb_layout", nameof(SetCollapsed));
            }
        }

        /// <summary>
        /// 设置侧边栏收缩方法 客户端监控 window.onresize 事件回调此方法
        /// </summary>
        /// <returns></returns>
        [JSInvokable]
        public void SetCollapsed(int width)
        {
            IsSmallScreen = width < 768;
        }

        /// <summary>
        /// 获得/设置 组件是否已经 Render
        /// </summary>
        protected bool IsRendered { get; set; }

        /// <summary>
        /// DisposeAsyncCore 方法
        /// </summary>
        /// <param name="disposing"></param>
        /// <returns></returns>
        protected virtual async ValueTask DisposeAsyncCore(bool disposing)
        {
            if (disposing && IsRendered && Interop != null)
            {
                await Interop.InvokeVoidAsync(this, null, "bb_layout", "dispose");
                Interop.Dispose();
                Interop = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore(true);
            GC.SuppressFinalize(this);
        }
    }
}
